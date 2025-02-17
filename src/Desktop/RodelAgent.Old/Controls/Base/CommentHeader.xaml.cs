// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// 带注释的头部控件.
/// </summary>
public sealed partial class CommentHeader : UserControl
{
    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CommentHeader), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Comment"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommentProperty =
        DependencyProperty.Register(nameof(Comment), typeof(string), typeof(CommentHeader), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentHeader"/> class.
    /// </summary>
    public CommentHeader() => InitializeComponent();

    /// <summary>
    /// 获取或设置标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 获取或设置注释.
    /// </summary>
    public string Comment
    {
        get => (string)GetValue(CommentProperty);
        set => SetValue(CommentProperty, value);
    }
}
