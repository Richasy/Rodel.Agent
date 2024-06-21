using System.ComponentModel.DataAnnotations;

namespace Migration.V1.Models;

/// <summary>
/// 会话负载.
/// </summary>
internal sealed class ChatSession
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [Key]
    public string? Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 历史消息记录.
    /// </summary>
    public List<ChatMessage>? Messages { get; set; }

    /// <summary>
    /// 会话设置.
    /// </summary>
    public SessionOptions? Options { get; set; }

    /// <summary>
    /// 助理标识符列表.
    /// </summary>
    public List<string>? Assistants { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatSession payload && Id == payload.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
