// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 自定义模型对话框.
/// </summary>
public sealed partial class CustomChatModelDialog : AppContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog()
    {
        InitializeComponent();
        Title = ResourceToolkit.GetLocalizedString(StringNames.CreateCustomModel);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomChatModelDialog"/> class.
    /// </summary>
    public CustomChatModelDialog(ChatModel model, bool isIdEnabled = true)
        : this()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.ModifyCustomModel);
        ModelNameBox.Text = model.DisplayName;
        ModelIdBox.Text = model.Id;
        ModelIdBox.IsEnabled = isIdEnabled;
        FileUploadButton.IsChecked = model.IsSupportFileUpload;
        ToolButton.IsChecked = model.IsSupportTool;
        VisionButton.IsChecked = model.IsSupportVision;
        ContextLengthSelection.SelectedIndex = GetSelectIndexFromContextLength(model.ContextLength);
    }

    /// <summary>
    /// 获取或设置模型.
    /// </summary>
    public ChatModel Model { get; private set; }

    private int GetSelectIndexFromContextLength(long contextLength)
    {
        var index = 0;
        var nums = ContextLengthSelection.Items.OfType<ComboBoxItem>().Select(p => Convert.ToInt64(p.Tag)).OrderBy(p => p).ToList();
        for (var i = 0; i < nums.Count; i++)
        {
            if (contextLength <= nums[i])
            {
                index = i;
                break;
            }
        }

        return index;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var modelName = ModelNameBox.Text?.Trim() ?? string.Empty;
        var modelId = ModelIdBox.Text?.Trim() ?? string.Empty;
        var contextLength = Convert.ToInt64(((ComboBoxItem)ContextLengthSelection.SelectedItem).Tag);
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelId))
        {
            args.Cancel = true;
            var appVM = this.Get<AppViewModel>();
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ModelNameOrIdCanNotBeEmpty), InfoType.Error));
            return;
        }

        var model = new ChatModel
        {
            DisplayName = modelName,
            Id = modelId,
            IsSupportFileUpload = FileUploadButton.IsChecked ?? false,
            IsSupportTool = ToolButton.IsChecked ?? false,
            IsSupportVision = VisionButton.IsChecked ?? false,
            ContextLength = contextLength,
            IsCustomModel = true,
        };

        Model = model;
    }
}
