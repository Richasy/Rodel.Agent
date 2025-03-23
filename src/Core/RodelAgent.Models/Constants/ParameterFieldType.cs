// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.Models.Constants;

/// <summary>
/// 参数字段类型.
/// </summary>
public enum ParameterFieldType
{
    /// <summary>
    /// 浮点数，具备数值范围.
    /// </summary>
    RangeFloat,

    /// <summary>
    /// 整数，具备数值范围.
    /// </summary>
    RangeInt,

    /// <summary>
    /// 长整数，具备数值范围.
    /// </summary>
    RangeLong,

    /// <summary>
    /// 文本.
    /// </summary>
    Text,

    /// <summary>
    /// 一组关键字，值为字符串.
    /// </summary>
    Keywords,

    /// <summary>
    /// 单选内容，值为字符串.
    /// </summary>
    Selection,

    /// <summary>
    /// 布尔值.
    /// </summary>
    Boolean,
}
