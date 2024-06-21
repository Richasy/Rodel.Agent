// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI;

internal static class GlobalFeatureSwitcher
{
    /// <summary>
    /// 本地模型尚处于实验阶段，仅供调试使用.
    /// </summary>
#if DEBUG
    public const bool IsLocalModelSupported = false;
    public const bool IsChatShareEnabled = false;
    public const bool IsChatImageSupported = false;
#else
    public const bool IsLocalModelSupported = false;
    public const bool IsChatShareEnabled = false;
    public const bool IsChatImageSupported = false;
#endif

    /// <summary>
    /// 是否启用 RAG 功能.
    /// </summary>
    public const bool IsRagFeatureEnabled = false;

    /// <summary>
    /// 是否启用文生图功能.
    /// </summary>
    public const bool IsT2IFeatureEnabled = true;

    /// <summary>
    /// 是否启用文本转语音功能.
    /// </summary>
    public const bool IsT2SFeatureEnabled = true;

    /// <summary>
    /// 是否启用全局搜索功能.
    /// </summary>
    public const bool IsGlobalSearchEnabled = false;
}
