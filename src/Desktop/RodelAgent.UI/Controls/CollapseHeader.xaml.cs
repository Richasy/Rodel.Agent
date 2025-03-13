// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// Header of the collapse panel.
/// </summary>
public sealed partial class CollapseHeader : LayoutUserControlBase
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CollapseHeader), new PropertyMetadata(default));

    public static readonly DependencyProperty IsCollapsedProperty =
        DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(CollapseHeader), new PropertyMetadata(default));

    public static readonly DependencyProperty ElementProperty =
        DependencyProperty.Register(nameof(Element), typeof(object), typeof(CollapseHeader), new PropertyMetadata(default));

    public CollapseHeader() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    public object Element
    {
        get => (object)GetValue(ElementProperty);
        set => SetValue(ElementProperty, value);
    }

    private void OnCollapseButtonClick(object sender, RoutedEventArgs e)
        => IsCollapsed = !IsCollapsed;
}
