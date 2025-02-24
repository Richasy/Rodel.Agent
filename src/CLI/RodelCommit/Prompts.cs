// Copyright (c) Richasy. All rights reserved.

using System.Globalization;

namespace RodelCommit;

internal static class Prompts
{
    public static string GetCommitSummaryPrompt(string diff, int maxLength = 20, string? locale = "")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

        var typeList = string.Join('\n', Gitmojis.Items.Select(p=>$"    - {p.Type}: {p.Description}"));

        return $"""
            You are to act as the author of a commit message in git. Your mission is to create clean and comprehensive commit messages in the conventional commit convention and explain WHAT were the changes and WHY the changes were done.
            I'll enter a git diff summary, and your job is to convert it into a useful commit message.
            Add a short description of the changes are done after the commit message. Don't start it with "This commit", just describe the changes.
            Use the present tense. Lines must not be longer than {maxLength} characters.
            --------
            {diff}
            --------
            Add a short description of the changes are done after the commit message. Don't start it with "This commit", just return only 1 type commit message describes the git diff summary.

            ## Rules
            - Choose only 1 type from the type-to-description below:
            {typeList}
            - Commit message must be a maximum of {maxLength} characters.
            - Commit message language: {locale}.
            - First line need add the type, like "feat: add new feature".
            """;
    }

    public static string GetSegmentSummaryPrompt(string diffPart, string? locale="")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

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

            Return only the code-focused summary without markdown formatting.
            """;
    }

    public static string GetCommitSummaryPrompt(List<string> summarizes, int maxLength = 20, string? locale = "")
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = CultureInfo.CurrentUICulture.Name;
        }

        var typeList = string.Join('\n', Gitmojis.Items.Select(p => $"    - {p.Type}: {p.Description}"));
        var allSummaries = string.Join('\n', summarizes.Select((p, i) => $"    {i + 1}. {p}"));

        return $"""
            You are a senior git engineer synthesizing segmented summaries into a conventional commit message. Follow these steps:

            1. Analyze all technical summaries below
            2. Select ONE appropriate commit type from:
            {typeList}
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
