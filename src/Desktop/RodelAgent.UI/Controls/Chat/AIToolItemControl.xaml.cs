// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class AIToolItemControl : AIToolItemControlBase
{
    public AIToolItemControl() => InitializeComponent();

    public event EventHandler? ItemClick;

    private void OnItemClick(object sender, RoutedEventArgs e)
        => ItemClick?.Invoke(this, EventArgs.Empty);
}

public abstract class AIToolItemControlBase : LayoutUserControlBase<AIToolsetItemViewModel>;