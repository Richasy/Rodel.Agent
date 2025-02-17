// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Azure OpenAI 配置部分.
/// </summary>
public sealed partial class AzureOpenAIChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIChatConfigSection"/> class.
    /// </summary>
    public AzureOpenAIChatConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new AzureOpenAIClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }
}
