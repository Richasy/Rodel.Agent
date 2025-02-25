// Copyright (c) Richasy. All rights reserved.

using System.Globalization;

namespace RodelCommit;

internal static class Prompts
{
    public static string GetCommitSummaryPrompt(string diff, GitmojiItem commitType, int maxLength = 20, string? locale = "")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

        var typeList = commitType.Type == "auto"
            ? string.Join('\n', Gitmojis.Items.Select(p=>$"    - {p.Type}: {p.Description}"))
            : $"    - {commitType.Type}: {commitType.Description}";

        var typeRule = commitType.Type == "auto"
            ? $"""
            - Choose only 1 type from the type-to-description below:
                {typeList}
            """
            : $"The specified commit type is:\n{typeList}";

        return $"""
            You are a senior git engineer synthesizing git diff information into a conventional commit message. Follow these steps:
            
            1. Analyze all git diff below
            {typeRule}
            3. Create format-compliant header: "<type>: <concise subject>" (max {maxLength} chars)
            4. Body content:
               - Explain WHAT changed technically (key modifications)
               - Specify WHY changes were made (without mentioning "this commit")
               - Reference specific components/files where relevant
            5. Format constraints:
               - Language: {locale}
               - Wrap body at {maxLength} characters
               - Use imperative present tense
               - Disable markdown code block tags
            
            Summarized Inputs:
            --------
            {diff}
            --------
            
            Return only the formatted commit message without commentary.
            """;
    }

    public static string GetSegmentSummaryPrompt(string diffPart, GitmojiItem commitType, string? locale="")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

        var typeRule = commitType.Type == "auto"
            ? ""
            : $"The specified commit type is:\n- {commitType.Type}: {commitType.Description}";

        return $"""
            You are a git commit assistant analyzing segmented code changes. For each git diff segment:
            1. Provide a brief technical summary of modified files and key changes
            2. Highlight specific code-level modifications (methods added, parameters changed, etc.)
            3. Keep explanations concise but technically specific
            4. Use present tense and bullet points
            5. Avoid conventional commit formatting or type selection
            6. Maintain original file/line references where relevant

            Segment Input:
            --------
            {diffPart}
            --------

            {typeRule}
            Return only the code-focused summary without markdown formatting.
            """;
    }

    public static string GetCommitSummaryPrompt(List<string> summarizes, GitmojiItem commitType, int maxLength = 20, string? locale = "")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

        var allSummaries = string.Join('\n', summarizes.Select((p, i) => $"    {i + 1}. {p}"));
        var typeList = commitType.Type == "auto"
            ? string.Join('\n', Gitmojis.Items.Select(p => $"    - {p.Type}: {p.Description}"))
            : $"    - {commitType.Type}: {commitType.Description}";

        var typeRule = commitType.Type == "auto"
            ? $"""
            2. Choose only 1 type from the type-to-description below:
                {typeList}
            """
            : $"2. The specified commit type is:\n{typeList}";

        return $"""
            You are a senior git engineer synthesizing segmented summaries into a conventional commit message. Follow these steps:

            1. Analyze all technical summaries below
            {typeRule}
            3. Create format-compliant header: "<type>: <concise subject>" (max {maxLength} chars)
            4. Body content:
               - Explain WHAT changed technically (key modifications)
               - Specify WHY changes were made (without mentioning "this commit")
               - Reference specific components/files where relevant
            5. Format constraints:
               - Language: {locale}
               - Wrap body at {maxLength} characters
               - Use imperative present tense
               - Disable markdown code block tags

            Summarized Inputs:
            --------
            {allSummaries}
            --------

            Return only the formatted commit message without commentary.
            """;
    }
}
