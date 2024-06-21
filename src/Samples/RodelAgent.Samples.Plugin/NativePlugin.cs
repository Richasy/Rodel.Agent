// Copyright (c) Rodel. All rights reserved.

using System.ComponentModel;
using System.Management;
using System.Text;
using Microsoft.SemanticKernel;

namespace RodelAgent.Samples.Plugin;

/// <summary>
/// Native plugin.
/// </summary>
[DisplayName("本机插件")]
[Description("该插件可以获取当前设备相关的信息")]
public sealed class NativePlugin
{
    /// <summary>
    /// 获取当前设备参数信息.
    /// </summary>
    /// <returns>设备参数信息.</returns>
    [KernelFunction]
    [Description("获取当前设备的参数信息.")]
#pragma warning disable CA1822 // 将成员标记为 static
    public Task<string> GetDeviceInfoAsync(CancellationToken cancellationToken = default)
#pragma warning restore CA1822 // 将成员标记为 static
    {
        var processor = GetProcessorInfo();
        var memory = GetMemoryInfo();
        var videoController = GetVideoControllerInfo();
        var operatingSystem = GetOperatingSystemInfo();
        var text = processor + memory + videoController + operatingSystem;
        return Task.FromResult(text);
    }

    private static string GetProcessorInfo()
    {
        using var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
        var sb = new StringBuilder();
        sb.AppendLine("Processor Information:");
        foreach (ManagementObject obj in searcher.Get())
        {
            sb.AppendLine($"Name: {obj["Name"]}");
            sb.AppendLine($"Manufacturer: {obj["Manufacturer"]}");
            sb.AppendLine($"NumberOfCores: {obj["NumberOfCores"]}");
            sb.AppendLine($"NumberOfLogicalProcessors: {obj["NumberOfLogicalProcessors"]}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetMemoryInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Memory Information:");
        using var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
        foreach (ManagementObject obj in searcher.Get())
        {
            sb.AppendLine($"Capacity: {Convert.ToUInt64(obj["Capacity"]) / (1024 * 1024 * 1024)} GB");
            sb.AppendLine($"Speed: {obj["Speed"]} MHz");
            sb.AppendLine($"Manufacturer: {obj["Manufacturer"]}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetVideoControllerInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Video Controller Information:");
        using var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
        foreach (ManagementObject obj in searcher.Get())
        {
            sb.AppendLine($"Name: {obj["Name"]}");
            sb.AppendLine($"Description: {obj["Description"]}");
            sb.AppendLine($"DeviceID: {obj["DeviceID"]}");
            sb.AppendLine($"DriverVersion: {obj["DriverVersion"]}");
            sb.AppendLine($"VideoProcessor: {obj["VideoProcessor"]}");
            sb.AppendLine($"VideoModeDescription: {obj["VideoModeDescription"]}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetOperatingSystemInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Operating System Information:");
        using var searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        foreach (ManagementObject obj in searcher.Get())
        {
            sb.AppendLine($"Name: {obj["Caption"]}");
            sb.AppendLine($"Version: {obj["Version"]}");
            sb.AppendLine($"Manufacturer: {obj["Manufacturer"]}");
            sb.AppendLine($"OS Architecture: {obj["OSArchitecture"]}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
