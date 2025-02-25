// Copyright (c) Richasy. All rights reserved.

namespace RodelCommit;

internal static class Gitmojis
{
    public static GitmojiItem[] Items { get; } =
    [
        new GitmojiItem
        {
            Code = ":sparkles:",
            Description = "Introducing new features.",
            Emoji = "✨",
            Name = "sparkles",
            Type = "feat"
        },
        new GitmojiItem
        {
            Code = ":bug:",
            Description = "Fixing a bug.",
            Emoji = "🐛",
            Name = "bug",
            Type = "fix"
        },
        new GitmojiItem
        {
            Code = ":bookmark:",
            Description = "There is no clear attribute, it is a phased submission.",
            Emoji = "🔖",
            Name = "bookmark",
            Type = "submission"
        },
        new GitmojiItem
        {
            Code = ":memo:",
            Description = "Writing docs.",
            Emoji = "📝",
            Name = "memo",
            Type = "docs"
        },
        new GitmojiItem
        {
            Code = ":art:",
            Description = "Improving structure / format of the code.",
            Emoji = "🎨",
            Name = "art",
            Type = "style"
        },
        new GitmojiItem
        {
            Code = ":zap:",
            Description = "Improving performance.",
            Emoji = "⚡",
            Name = "zap",
            Type = "perf"
        },
        new GitmojiItem
        {
            Code = ":tada:",
            Description = "Initial commit.",
            Emoji = "🎉",
            Name = "tada",
            Type = "init"
        },
        new GitmojiItem
        {
            Code = ":rocket:",
            Description = "Deploying stuff.",
            Emoji = "🚀",
            Name = "rocket",
            Type = "deploy"
        },
        new GitmojiItem
        {
            Code = ":white_check_mark:",
            Description = "Adding tests.",
            Emoji = "✅",
            Name = "white_check_mark",
            Type = "test"
        }
    ];
}

internal sealed class GitmojiItem
{
    public string Code { get; set; }

    public string Description { get; set; }

    public string Emoji { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }
}