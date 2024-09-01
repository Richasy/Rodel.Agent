// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 自定义绘图模型对话框.
/// </summary>
public sealed partial class CustomDrawModelDialog : AppContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDrawModelDialog"/> class.
    /// </summary>
    public CustomDrawModelDialog()
    {
        InitializeComponent();
        Title = ResourceToolkit.GetLocalizedString(StringNames.CreateCustomModel);
        WidthBox.Value = 0;
        HeightBox.Value = 0;
        CheckSize();
        CheckSizeCount();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomDrawModelDialog"/> class.
    /// </summary>
    public CustomDrawModelDialog(DrawModel model, bool isIdEnabled = true)
        : this()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.ModifyCustomModel);
        ModelNameBox.Text = model.DisplayName;
        ModelIdBox.Text = model.Id;
        ModelIdBox.IsEnabled = isIdEnabled;
        foreach (var size in model.SupportSizes)
        {
            Sizes.Add(size);
        }

        CheckSizeCount();
    }

    /// <summary>
    /// 获取或设置模型.
    /// </summary>
    public DrawModel Model { get; private set; }

    private ObservableCollection<string> Sizes { get; } = new();

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var modelName = ModelNameBox.Text?.Trim() ?? string.Empty;
        var modelId = ModelIdBox.Text?.Trim() ?? string.Empty;
        var appVM = this.Get<AppViewModel>();
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelId))
        {
            args.Cancel = true;
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ModelNameOrIdCanNotBeEmpty), InfoType.Error));
            return;
        }

        if (Sizes.Count == 0)
        {
            args.Cancel = true;
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.DrawSizeCanNotBeEmpty), InfoType.Error));
            return;
        }

        var model = new DrawModel
        {
            DisplayName = modelName,
            Id = modelId,
            SupportSizes = Sizes.ToArray(),
            IsCustomModel = true,
        };

        Model = model;
    }

    private void OnSizeRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is string element)
        {
            Sizes.Remove(element);
            CheckSizeCount();
        }
    }

    private void OnAddSizeButtonClick(object sender, RoutedEventArgs e)
    {
        var width = Convert.ToInt32(WidthBox.Value);
        var height = Convert.ToInt32(HeightBox.Value);
        if (width > 0 && height > 0)
        {
            var size = $"{width}x{height}";
            if (!Sizes.Contains(size))
            {
                Sizes.Add(size);
                NewSizeFlyout.Hide();
                CheckSizeCount();
            }

            WidthBox.Value = 0;
            HeightBox.Value = 0;
            CheckSize();
        }
    }

    private void OnWidthChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        => CheckSize();

    private void OnHeightChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        => CheckSize();

    private void CheckSize()
    {
        if (double.IsNaN(WidthBox.Value) || double.IsNaN(HeightBox.Value))
        {
            return;
        }

        var width = Convert.ToInt32(WidthBox.Value);
        var height = Convert.ToInt32(HeightBox.Value);
        AddSizeButton.IsEnabled = width > 0 && height > 0;
    }

    private void CheckSizeCount()
    {
        var isEmpty = Sizes.Count == 0;
        NoSizeContainer.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        SizesRepeater.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
    }
}
