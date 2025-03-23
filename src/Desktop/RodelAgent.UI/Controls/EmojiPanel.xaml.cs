// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Statics;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 表情面板.
/// </summary>
public sealed partial class EmojiPanel : LayoutUserControlBase
{
    private readonly ObservableCollection<EmojiItem> _items = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmojiPanel"/> class.
    /// </summary>
    public EmojiPanel() => InitializeComponent();

    /// <summary>
    /// 表情点击事件.
    /// </summary>
#pragma warning disable CA1003 // 使用泛型事件处理程序实例
    public event EventHandler<EmojiItem> EmojiClick;
#pragma warning restore CA1003 // 使用泛型事件处理程序实例

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (_items.Count == 0)
        {
            var emojis = EmojiStatics.GetEmojis().OrderBy(p => p.Group).ThenBy(p => p.Unicode);
            foreach (var emoji in emojis)
            {
                _items.Add(emoji);
            }
        }
    }

    private void OnEmojiButtonClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as Button)?.DataContext as EmojiItem;
        EmojiClick?.Invoke(this, context!);
    }
}
