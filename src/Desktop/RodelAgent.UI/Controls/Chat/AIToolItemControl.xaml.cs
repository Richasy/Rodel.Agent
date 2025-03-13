// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class AIToolItemControl : AIToolItemControlBase
{
    public AIToolItemControl() => InitializeComponent();
}

public abstract class AIToolItemControlBase : LayoutUserControlBase<AIToolsetItemViewModel>;