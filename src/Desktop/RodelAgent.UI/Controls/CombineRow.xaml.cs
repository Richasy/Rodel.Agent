// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls;

public sealed partial class CombineRow : LayoutUserControlBase
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CombineRow), new PropertyMetadata(default));

    public static readonly DependencyProperty ElementProperty =
        DependencyProperty.Register(nameof(Element), typeof(object), typeof(CombineRow), new PropertyMetadata(default));

    public static readonly DependencyProperty BottomBorderVisibilityProperty =
        DependencyProperty.Register(nameof(BottomBorderVisibility), typeof(Visibility), typeof(CombineRow), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty TipProperty =
        DependencyProperty.Register(nameof(Tip), typeof(string), typeof(CombineRow), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CombineRow"/> class.
    /// </summary>
    public CombineRow() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public object Element
    {
        get => (object)GetValue(ElementProperty);
        set => SetValue(ElementProperty, value);
    }

    public Visibility BottomBorderVisibility
    {
        get => (Visibility)GetValue(BottomBorderVisibilityProperty);
        set => SetValue(BottomBorderVisibilityProperty, value);
    }

    public string Tip
    {
        get => (string)GetValue(TipProperty);
        set => SetValue(TipProperty, value);
    }
}
