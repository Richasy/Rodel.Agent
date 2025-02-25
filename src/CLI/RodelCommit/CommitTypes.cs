// Copyright (c) Richasy. All rights reserved.

namespace RodelCommit;

internal static class CommitTypes
{
    public static CommitTypeItem[] Items { get; } =
    [
        new CommitTypeItem
        {
            Code = ":sparkles:",
            Description = "Introducing new features.",
            Emoji = "✨",
            Name = "sparkles",
            Type = "feat"
        },
        new CommitTypeItem
        {
            Code = ":bug:",
            Description = "Fixing a bug.",
            Emoji = "🐛",
            Name = "bug",
            Type = "fix"
        },
        new CommitTypeItem
        {
            Code = ":bookmark:",
            Description = "There is no clear attribute, it is a phased submission.",
            Emoji = "🔖",
            Name = "bookmark",
            Type = "submission"
        },
        new CommitTypeItem
        {
            Code = ":memo:",
            Description = "Writing docs.",
            Emoji = "📝",
            Name = "memo",
            Type = "docs"
        },
        new CommitTypeItem
        {
            Code = ":art:",
            Description = "Improving structure / format of the code.",
            Emoji = "🎨",
            Name = "art",
            Type = "style"
        },
        new CommitTypeItem
        {
            Code = ":zap:",
            Description = "Improving performance.",
            Emoji = "⚡",
            Name = "zap",
            Type = "perf"
        },
        new CommitTypeItem
        {
            Code = ":tada:",
            Description = "Initial commit.",
            Emoji = "🎉",
            Name = "tada",
            Type = "init"
        },
        new CommitTypeItem
        {
            Code = ":rocket:",
            Description = "Deploying stuff.",
            Emoji = "🚀",
            Name = "rocket",
            Type = "deploy"
        },
        new CommitTypeItem
        {
            Code = ":white_check_mark:",
            Description = "Adding tests.",
            Emoji = "✅",
            Name = "white_check_mark",
            Type = "test"
        }
    ];
}

internal sealed class CommitTypeItem
{
    public string Code { get; set; }

    public string Description { get; set; }

    public string Emoji { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }
}