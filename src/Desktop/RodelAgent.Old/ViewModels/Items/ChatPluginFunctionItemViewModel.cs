// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天插件功能项视图模型.
/// </summary>
public sealed partial class ChatPluginFunctionItemViewModel : ViewModelBase<KernelFunction>
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPluginFunctionItemViewModel"/> class.
    /// </summary>
    public ChatPluginFunctionItemViewModel(KernelFunction function)
        : base(function)
    {
        Name = function.Name;
        Description = function.Description;
    }
}
