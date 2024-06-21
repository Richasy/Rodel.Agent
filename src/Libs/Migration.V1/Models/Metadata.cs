using System.ComponentModel.DataAnnotations;

namespace Migration.V1.Models;

internal sealed class Metadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    public Metadata()
    {
        Id = string.Empty;
        Value = string.Empty;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 值.
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Metadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
