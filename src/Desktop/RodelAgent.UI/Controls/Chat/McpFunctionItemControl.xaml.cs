// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class McpFunctionItemControl : McpFunctionItemControlBase
{
    public McpFunctionItemControl()
    {
        this.InitializeComponent();
    }
}

public abstract class McpFunctionItemControlBase : LayoutUserControlBase<AIFunctionItemViewModel>;