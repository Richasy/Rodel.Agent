namespace Migration.V1.Models;

/// <summary>
/// 内核类型.
/// </summary>
internal enum KernelType
{
    /// <summary>
    /// Azure Open AI.
    /// </summary>
    AzureOpenAI,

    /// <summary>
    /// Open AI.
    /// </summary>
    OpenAI,

    /// <summary>
    /// 自定义.
    /// </summary>
    Custom,
}
