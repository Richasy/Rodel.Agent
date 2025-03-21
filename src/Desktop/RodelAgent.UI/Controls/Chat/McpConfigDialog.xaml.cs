// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class McpConfigDialog : AppDialog
{
    private readonly McpServerItemViewModel? _source;
    internal ObservableCollection<VariableItemViewModel> Variables { get; } = [];

    public McpConfigDialog() => InitializeComponent();

    public McpConfigDialog(McpServerItemViewModel vm)
        : this()
    {
        _source = vm;
        NameBox.Text = vm.Id;
        vm.Data.Environments?.Select(p => new VariableItemViewModel { Name = p.Key, Value = p.Value }).ToList().ForEach(Variables.Add);
        EnableSwitch.IsOn = vm.IsEnabled;
        CommandBox.Text = vm.Data.Command + " " + string.Join(' ', vm.Data.Arguments ?? []);
        DirectoryBox.Text = vm.Data.WorkingDirectory;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var name = NameBox.Text;
        var commandStr = CommandBox.Text.Trim();
        args.Cancel = true;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(commandStr))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.MustFillRequireFields), InfoType.Warning));
            return;
        }

        var commandSplit = commandStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = commandSplit[0];
        var arguments = commandSplit.Skip(1).ToArray();

        var variables = Variables.ToDictionary(p => p.Name, p => p.Value);
        var config = new McpAgentConfig
        {
            IsEnabled = EnableSwitch.IsOn,
            Arguments = arguments,
            Command = command,
            Environments = variables,
            WorkingDirectory = DirectoryBox.Text,
        };

        var pageVM = this.Get<ChatPageViewModel>();
        var nameExist = pageVM.Servers.Any(p => p.Id == name);
        if (nameExist && (_source == null || _source.Id != name))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.McpServerNameExist), InfoType.Warning));
            return;
        }

        IsPrimaryButtonEnabled = false;

        if (_source != null)
        {
            _source.Data = config;
            _source.Name = name;
            _source.IsEnabled = config.IsEnabled ?? true;
            _source.SaveCommand.Execute(default);
            _source.DisconnectCommand.Execute(default);
        }
        else
        {
            pageVM.Servers.Add(new McpServerItemViewModel(name, config, () => pageVM.SaveMcpServersCommand.ExecuteAsync(default)));
            pageVM.SaveMcpServersCommand.Execute(default);
        }

        Hide();
    }

    private async void OnFolderButtonClick(object sender, RoutedEventArgs e)
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
        if (folder != null)
        {
            DirectoryBox.Text = folder.Path;
        }
    }

    private void OnAddVariableButtonClick(object sender, RoutedEventArgs e)
    {
        var name = VariableNameBox.Text;
        var value = VariableValueBox.Text;
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.MustFillRequireFields), InfoType.Warning));
            return;
        }

        if (Variables.Any(p => p.Name == name))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.VariableNameExist), InfoType.Warning));
            return;
        }

        Variables.Add(new VariableItemViewModel { Name = name, Value = value });
        VariableNameBox.Text = string.Empty;
        VariableValueBox.Text = string.Empty;
        VariableFlyout.Hide();
        CheckVariablesVisibility();
    }

    private void OnDeleteVariableButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: VariableItemViewModel item })
        {
            Variables.Remove(item);
        }

        CheckVariablesVisibility();
    }

    private void CheckVariablesVisibility()
    {
        var hasVariables = Variables.Any();
        VariablesContainer.Visibility = hasVariables ? Visibility.Visible : Visibility.Collapsed;
        NoVariablesContainer.Visibility = hasVariables ? Visibility.Collapsed : Visibility.Visible;
    }
}

internal sealed partial class VariableItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string Value { get; set; }
}