﻿// Copyright (c) Richasy. All rights reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Security;
using System.Text;
using System.Xml;

namespace RodelAgent.UI.ResourceGenerator;

/// <summary>
/// 编译期间获取资源文件并生成对应的枚举类型.
/// </summary>
[Generator]
public class ResourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 如果需要调试，取消下面的注释
        // if (!Debugger.IsAttached) Debugger.Launch();
        var additionalFiles = context.AdditionalTextsProvider.Where(f => f.Path.EndsWith(".resw", StringComparison.OrdinalIgnoreCase));
        var contents = additionalFiles.Select((file, cancellationToken) => file!.GetText(cancellationToken)!.ToString());
        context.RegisterSourceOutput(contents, (spc, content) => ProcessResourceFile(content, spc));
    }

    private static void ProcessResourceFile(string content, SourceProductionContext context)
    {
        var xml = new XmlDocument();
        try
        {
            xml.LoadXml(content);
        }
        catch (Exception)
        {
            // Show the warning if no resource files are found
            var desc = new DiagnosticDescriptor(
                        "RESW0002",
                        "Failed to load .resw file",
                        "An error was encountered while trying to parse the resw file, please check the resw document structure.",
                        "Problem",
                        DiagnosticSeverity.Error,
                        true);

            context.ReportDiagnostic(Diagnostic.Create(desc, Location.None));
            return;
        }

        var sb = new StringBuilder();
        var dataNodes = xml.SelectNodes("//data");

        _ = sb.AppendLine("// <auto-generated />");
        _ = sb.AppendLine();
        _ = sb.AppendLine("namespace RodelAgent.UI.Models.Constants;");
        _ = sb.AppendLine();
        _ = sb.AppendLine("#pragma warning disable CS1591");
        _ = sb.AppendLine("public enum StringNames");
        _ = sb.AppendLine("{");

        foreach (XmlNode dataNode in dataNodes)
        {
            var name = dataNode.Attributes["name"].Value;
            var value = dataNode.SelectSingleNode("value").InnerText.Replace("\r", "\n");
            _ = sb.AppendLine("    /// <summary>");
            if (value.Contains("\n"))
            {
                var sp = value.Split('\n');
                foreach (var spt in sp)
                {
                    if (string.IsNullOrEmpty(spt.Trim()))
                    {
                        continue;
                    }

                    _ = sb.AppendLine($"    /// {SecurityElement.Escape(spt)}");
                }
            }
            else
            {
                _ = sb.AppendLine($"    /// {SecurityElement.Escape(value)}");
            }

            _ = sb.AppendLine("    /// </summary>");
            _ = sb.AppendLine($"    {name},");
            _ = sb.AppendLine();
        }

        _ = sb.AppendLine("}");
        context.AddSource("StringNames.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}
