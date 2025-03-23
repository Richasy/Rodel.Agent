// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatGroupConfigDialog : AppDialog
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ChatGroupConfigViewModel), typeof(ChatGroupConfigDialog), new PropertyMetadata(default));

    public ChatGroupConfigDialog()
    {
        this.InitializeComponent(); ViewModel = this.Get<ChatGroupConfigViewModel>();
        Closed += (_, _) => ViewModel.CloseRequested -= OnCloseRequested;
        ViewModel.CloseRequested += OnCloseRequested;
    }

    public ChatGroupConfigViewModel ViewModel
    {
        get => (ChatGroupConfigViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnCloseRequested(object? sender, EventArgs e)
        => Hide();

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsManualClose = true;
        Hide();
    }

    private async void OnSaveClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        btn.IsEnabled = false;
        if (!GroupPanel.IsValid())
        {
            this.Get<AppViewModel>()
                .ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.MustFillRequireFields), InfoType.Warning));
            btn.IsEnabled = true;
            return;
        }

        try
        {
            await GroupPanel.SaveAvatarAsync();
            ViewModel.SaveCommand.Execute(default);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        btn.IsEnabled = true;
    }
}
