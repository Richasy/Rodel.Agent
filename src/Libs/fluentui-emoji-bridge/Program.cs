// 向上查找目录，直到找到名为 Libs 的目录
using System.Text;
using System.Text.Json;

var current = AppDomain.CurrentDomain.BaseDirectory;
while (current != null)
{
    var path = Path.Combine(current, "Libs");
    if (Directory.Exists(path))
    {
        current = path;
        Console.WriteLine(path);
        break;
    }
    current = Directory.GetParent(current)?.FullName;
}

if (string.IsNullOrEmpty(current))
{
    Console.WriteLine("未找到 Libs 文件夹");
    return;
}

// 在当前目录下查找 fluentui-emoji/assets 目录
var emojiFolder = Path.Combine(current!, "fluentui-emoji", "assets");

// 将所有子文件夹名称汇总
var folders = Directory.GetDirectories(emojiFolder)
    .Select(x => Path.GetFileName(x))
    .ToList();

// 遍历所有子文件夹，获取文件夹内的 metadata.json 文件，然后反序列化为 EmojiMetadata 对象
var emojis = folders.Select(folder =>
    {
        var metadataFile = Path.Combine(emojiFolder, folder, "metadata.json");
        if (!File.Exists(metadataFile))
        {
            return null;
        }

        var metadata = JsonSerializer.Deserialize<EmojiMetadata>(File.ReadAllText(metadataFile));
        Console.WriteLine($"已获取 {metadata!.CommonName} | {metadata.Glyph}");
        metadata.FolderName = folder;
        return metadata;
    })
    .Where(p => p is not null)
    .ToList();

var staticFolder = Path.Combine(Path.GetDirectoryName(current)!, "Core", "RodelAgent.Statics");
if (!Directory.Exists(staticFolder))
{
    Console.WriteLine("未找到 Core\\RodelAgent.Statics 文件夹");
    return;
}

var sb = new StringBuilder();

// 遍历 emojis，在对应的文件夹中查找是否有 3D 子文件夹，有的话把其中的 png 图片提取出来
foreach (var emoji in emojis)
{
    var emojiFolderName = Path.Combine(emojiFolder, emoji!.FolderName!);
    if (!Directory.Exists(emojiFolderName))
    {
        Console.WriteLine($"未找到 {emojiFolderName} 文件夹");
        continue;
    }

    var threeD = Path.Combine(emojiFolderName, "3D");
    if (!Directory.Exists(threeD))
    {
        Console.WriteLine($"未找到 {emojiFolderName}\\3D 文件夹");
        continue;
    }

    var pngFiles = Directory.GetFiles(threeD, "*.png");
    var pngFile = pngFiles.FirstOrDefault();
    if (pngFile == null)
    {
        Console.WriteLine($"未找到 {emojiFolderName}\\3D 文件夹下的 png 文件");
        continue;
    }

    sb.AppendLine($"            new(\"{emoji.FolderName}\", \"{emoji.Unicode}\", \"{emoji.Group}\"),");
}

var code = """
    // Copyright (c) Rodel. All rights reserved.

    using System.Collections.Generic;

    namespace RodelAgent.Statics;

    /// <summary>
    /// Emoji 静态类.
    /// </summary>
    public static class EmojiStatics
    {
        /// <summary>
        /// 获取 Emoji 列表.
        /// </summary>
        /// <returns>Emoji 列表.</returns>
        public static List<EmojiItem> GetEmojis()
        {
            return
            [
    {HOLDER}
            ];
        }
    }

    /// <summary>
    /// Emoji 项.
    /// </summary>
    public sealed class EmojiItem(string name, string unicode, string group)
    {
        /// <summary>
        /// Emoji 名称.
        /// </summary>
        public string? Name { get; } = name;

        /// <summary>
        /// Unicode.
        /// </summary>
        public string? Unicode { get; } = unicode;

        /// <summary>
        /// 分组.
        /// </summary>
        public string? Group { get; } = group;

        /// <summary>
        /// 转换为 Emoji.
        /// </summary>
        /// <returns>Emoji 表示.</returns>
        public string ToEmoji()
        {
            var unicodePoints = Unicode!.Split(' ');
            var result = string.Empty;

            foreach (var point in unicodePoints)
            {
                var code = int.Parse(point, System.Globalization.NumberStyles.HexNumber);
                if (code <= 0xFFFF)
                {
                    result += (char)code;
                }
                else
                {
                    result += char.ConvertFromUtf32(code);
                }
            }

            return result;
        }
    }
    
    """;
code = code.Replace("{HOLDER}", sb.ToString().TrimEnd());

// 将代码写入 EmojiStatics.cs 文件，保存到 `staticFolder` 中
var staticFile = Path.Combine(staticFolder, "EmojiStatics.cs");
File.WriteAllText(staticFile, code, Encoding.UTF8);
Console.WriteLine($"已生成 {staticFile}");
