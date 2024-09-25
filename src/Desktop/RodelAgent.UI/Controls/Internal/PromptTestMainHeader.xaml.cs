// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Internal;

/// <summary>
/// 提示词测试页面主标题.
/// </summary>
public sealed partial class PromptTestMainHeader : PromptTestPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestMainHeader"/> class.
    /// </summary>
    public PromptTestMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        InitializePromptSelector();
        ViewModel.SystemPromptInitialzied += OnSystemPromptInitialized;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.SystemPromptInitialzied -= OnSystemPromptInitialized;

    private void OnSystemPromptInitialized(object? sender, EventArgs e)
        => InitializePromptSelector();

    private void OnPromptSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is null)
        {
            return;
        }

        var item = sender.SelectedItem.Tag as PromptTestSystemPromptItemViewModel;
        if (ViewModel.CurrentSystemPrompt == item)
        {
            return;
        }

        ViewModel.CurrentSystemPrompt = item;
    }

    private void InitializePromptSelector()
    {
        if (ViewModel.SystemPrompts?.Count == 0 || ViewModel.CurrentSystemPrompt is null)
        {
            return;
        }

        PromptSelector.Items.Clear();
        foreach (var item in ViewModel.SystemPrompts)
        {
            PromptSelector.Items.Add(new SelectorBarItem { Text = (item.Index + 1).ToString(), Tag = item });
        }

        PromptSelector.SelectedItem = PromptSelector.Items.FirstOrDefault(p => p.Tag == ViewModel.CurrentSystemPrompt);
    }
}
