using System.ComponentModel.DataAnnotations;

namespace Migration.V1.Models;

/// <summary>
/// AI图片.
/// </summary>
internal sealed class AiImage
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string? Id { get; set; }

    /// <summary>
    /// 生成提示词.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// 图片链接.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// 生成时间.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 图片宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 图片高度.
    /// </summary>
    public int Height { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AiImage image && Id == image.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
