// Copyright (c) Richasy. All rights reserved.

using LangChain.Splitters.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using RichasyKernel;
using Spectre.Console;
using Spectre.Console.Extensions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tiktoken;

namespace RodelCommit;

#pragma warning disable VSTHRD111 // Use ConfigureAwait(bool)
internal sealed class CommitService(Kernel kernel, IChatConfigManager configManager, IHostApplicationLifetime lifetime) : IHostedService
{
    private readonly CancellationTokenSource _stopCts = new();
    private Task? _commitTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _commitTask = Task.Run(() => RunAsync(_stopCts.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_commitTask != null)
        {
            await _stopCts.CancelAsync();
        }
    }

    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            DisplayCurrentDirectory();

            // Get diff.
            var diff = await GetDiffAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(diff))
            {
                AnsiConsole.MarkupLine("[yellow]No changes found.[/]");
                lifetime.StopApplication();
                return;
            }

            if (ChatConfigManager.ShouldManual)
            {
                await GenerateWithManualAsync();
            }
            else
            {
                await GenerateWithAIAsync(diff);
            }

            lifetime.StopApplication();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            lifetime.StopApplication();
        }
    }

    private static async Task GenerateWithManualAsync()
    {
        await ChatConfigManager.InitializeAsync();
        var commitType = AskCommitType();
        AnsiConsole.MarkupLine($"Selected commit type: [green]{commitType.Type}[/]");
        var commitHeader = AnsiConsole.Prompt(new TextPrompt<string>("Please enter the commit header:"));
        commitHeader = $"{commitType.Type}: {commitHeader}";
        commitHeader = AddEmojiToMessage(commitHeader);
        var nextAction = AnsiConsole.Prompt(new SelectionPrompt<int>()
            .Title("Please select the operation:")
            .PageSize(10)
            .AddChoices([1, 2, 3])
            .UseConverter(p => p switch
            {
                1 => "Commit",
                2 => "Edit commit message",
                3 => "Cancel",
                _ => throw new NotSupportedException()
            }));
        if (nextAction == 1)
        {
            await GenerateCommitAsync(commitHeader);
        }
        else if (nextAction == 2)
        {
            await GenerateCommitAsync(commitHeader, true);
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Commit canceled.[/]");
        }
    }

    private async Task GenerateWithAIAsync(string diff)
    {
        // Handle edge cases.
        var availableServices = await GetAvailableServicesAsync().ConfigureAwait(false);
        var defaultService = ChatConfigManager.AppConfiguration?.App?.DefaultService;
        if (availableServices.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No available services found.[/]\n[grey]You can run [green]rodel-commit --config[/] to config your services.[/]");
            lifetime.StopApplication();
            return;
        }

        if (!string.IsNullOrEmpty(defaultService)
            && !availableServices.Any(p => JsonSerializer.Serialize(p, JsonGenContext.Default.ChatProviderType).Contains(defaultService)))
        {
            AnsiConsole.MarkupLine("[red]The default service is not available.[/]\n[grey]You can run [green]rodel-commit --config[/] to config your services.[/]");
            lifetime.StopApplication();
            return;
        }

        var (repoName, repoDesc) = await GetCurrentDescriptorAsync();
        if (!string.IsNullOrEmpty(repoName))
        {
            AnsiConsole.MarkupLine($"[grey]Found repository descriptor: [blue]{repoName}[/][/]");
        }

        var commitType = AskCommitType();

        // Initialize ai service.
        var provider = !string.IsNullOrEmpty(defaultService)
            ? JsonSerializer.Deserialize($"\"{defaultService}\"", JsonGenContext.Default.ChatProviderType)
            : AskProvider(availableServices);
        var aiService = kernel.GetRequiredService<IChatService>(provider.ToString());
        var model = ChatConfigManager.GetModel(provider);
        var serviceConfig = await configManager.GetServiceConfigAsync(provider, model);
        aiService.Initialize(serviceConfig!);

        // Split diff if needed.
        List<string> documents = [];
        if (NeedSplit(diff))
        {
            var splitter = new RecursiveCharacterTextSplitter(
                chunkSize: ChatConfigManager.AppConfiguration?.App?.DiffChunkSize ?? 4000,
                chunkOverlap: 0);
            documents = splitter.SplitText(diff).ToList();
            AnsiConsole.MarkupLine($"Split diff info to [yellow]({documents.Count})[/] by {ChatConfigManager.AppConfiguration?.App.DiffChunkSize ?? 4000} chunk size");
        }
        else
        {
            documents.Add(diff);
        }

        // Generate summary.
        var summaryList = await GenerateSummaryWithDocumentsAsync(aiService, model.Id, commitType, documents, repoDesc);
        var summary = string.Empty;
        if (summaryList.Count > 1)
        {
            AnsiConsole.MarkupLine($"[green]Already generated {summaryList.Count} summaries, continue to generate final summary...[/]");
            summary = await GenerateSummaryWithSegmentsAsync(aiService, model.Id, commitType, summaryList, repoDesc);
        }
        else
        {
            summary = summaryList[0];
        }

        // Add emoji to message.
        summary = Regex.Replace(summary, @"\((.*?)\):", match =>
        {
            string p1 = match.Groups[1].Value;
            return $"({p1.ToLower()}):";
        });
        summary = RemoveMarkdownCodeBlockDelimiters(summary);
        var commitMessage = AddEmojiToMessage(summary);

        var panel = new Panel(Markup.Escape(commitMessage))
        {
            Header = new PanelHeader("Commit Message"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1, 2, 1),
        };
        AnsiConsole.Write(panel);

        // Confirm to commit.
        var commitOperation = AnsiConsole.Prompt(new SelectionPrompt<int>()
            .Title("Please select the operation:")
            .PageSize(10)
            .AddChoices([1, 2, 3, 4])
            .UseConverter(p => p switch
            {
                1 => "Commit and push",
                2 => "Commit only",
                3 => "Edit commit message",
                4 => "Cancel",
                _ => "Unknown"
            }));
        if (commitOperation == 4)
        {
            AnsiConsole.MarkupLine("[yellow]Commit canceled.[/]");
        }
        else if (commitOperation == 3)
        {
            await GenerateCommitAsync(commitMessage, true);
        }
        else if (commitOperation == 2)
        {
            await GenerateCommitAsync(commitMessage);
        }
        else if (commitOperation == 1)
        {
            await GenerateCommitAsync(commitMessage);
            await PushCommitAsync(commitMessage);
        }
    }

    private async Task<List<string>> GenerateSummaryWithDocumentsAsync(IChatService chatService, string model, CommitTypeItem commitType, List<string> documents, string? repoDesc)
    {
        var result = new List<string>();
        var index = 0;
        // 使用并发限制器生成多个文档的摘要，最多一次运行 5 个任务.
        var semaphore = new SemaphoreSlim(5);
        var useSegments = documents.Count > 1;
        await AnsiConsole.Status()
            .StartAsync("Generating summaries...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots2);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                var tasks = documents.Select(async doc =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var summarizePrompt = useSegments
                            ? Prompts.GetSegmentSummaryPrompt(doc, commitType, ChatConfigManager.AppConfiguration.App.Locale, repoDesc)
                            : Prompts.GetCommitSummaryPrompt(
                                doc,
                                commitType,
                                ChatConfigManager.AppConfiguration.App.MaxCommitLength,
                                ChatConfigManager.AppConfiguration.App.Locale,
                                repoDesc);
                        var chatMsg = new ChatMessage(ChatRole.User, summarizePrompt);
                        var options = new ChatOptions
                        {
                            ModelId = model,
                            Temperature = 0.5f,
                        };
                        var summary = await chatService.Client!.CompleteAsync([chatMsg], options, cancellationToken: _stopCts.Token);
                        var text = summary.Message.Text!;
                        result.Add(text);
                        index++;
                        ctx.Status($"Generating summaries...({index}/{documents.Count})");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                return Task.WhenAll(tasks);
            });

        return result;
    }

    private async Task<string> GenerateSummaryWithSegmentsAsync(IChatService chatService, string model, CommitTypeItem commitType, List<string> segments, string? repoDesc)
    {
        var summarizePrompt = Prompts.GetCommitSummaryPrompt(segments, commitType, ChatConfigManager.AppConfiguration.App.MaxCommitLength, ChatConfigManager.AppConfiguration.App.Locale, repoDesc);
        var chatMsg = new ChatMessage(ChatRole.User, summarizePrompt);
        var options = new ChatOptions
        {
            ModelId = model,
            Temperature = 0.5f,
        };

        var summary = await chatService.Client!.CompleteAsync([chatMsg], options, cancellationToken: _stopCts.Token);
        return summary.Message.Text!;
    }

    private static ChatProviderType AskProvider(List<ChatProviderType> availableServices)
    {
        var provider = availableServices[0];
        if (availableServices.Count > 1)
        {
            provider = AnsiConsole.Prompt(new SelectionPrompt<ChatProviderType>()
                .Title("Please select an ai service:")
                .PageSize(10)
                .MoreChoicesText("More")
                .AddChoices(availableServices)
                .UseConverter(ProviderToName));
        }

        return provider;
    }

    private static CommitTypeItem AskCommitType()
    {
        var backupItmes = CommitTypes.Items.ToList();

        if (!ChatConfigManager.ShouldManual)
        {
            backupItmes.Insert(0, new CommitTypeItem
            {
                Code = ":bulb:",
                Name = "Auto generate",
                Description = "Let AI determine the commit type",
                Emoji = "💡",
                Type = "auto",
            });
        }

        return AnsiConsole.Prompt(new SelectionPrompt<CommitTypeItem>()
                .Title("Please select a change type:")
                .PageSize(20)
                .MoreChoicesText("More")
                .AddChoices(backupItmes)
                .UseConverter(item => $"{item.Emoji} {item.Type}: {item.Description}"));
    }

    private static void DisplayCurrentDirectory()
    {
        AnsiConsole.Clear();
        var currentDirectory = Environment.CurrentDirectory;
        var path = new TextPath(currentDirectory)
        {
            RootStyle = new Style(foreground: Color.Blue),
            SeparatorStyle = new Style(foreground: Color.Grey),
            LeafStyle = new Style(foreground: Color.Yellow),
            StemStyle = new Style(foreground: Color.Green),
        };
        var panel = new Panel(path)
        {
            Header = new PanelHeader("Current Directory"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1, 2, 1),
        };

        AnsiConsole.Write(panel);
    }

    private static int CalcTokenCount(string text)
    {
        var encoder = ModelToEncoder.For("gpt-4o");
        var tokens = encoder.Encode(text);
        return tokens.Count;
    }

    private static bool NeedSplit(string text)
    {
        var tokens = CalcTokenCount(text);
        return tokens > 8000;
    }

    private static string ProviderToName(ChatProviderType provider)
    {
        return provider switch
        {
            ChatProviderType.OpenAI => "OpenAI",
            ChatProviderType.AzureOpenAI => "Azure OpenAI",
            ChatProviderType.AzureAI => "Azure AI",
            ChatProviderType.XAI => "xAI",
            ChatProviderType.ZhiPu => "智谱",
            ChatProviderType.LingYi => "零一万物",
            ChatProviderType.Anthropic => "Anthropic",
            ChatProviderType.Moonshot => "月之暗面",
            ChatProviderType.Gemini => "Gemini",
            ChatProviderType.DeepSeek => "DeepSeek",
            ChatProviderType.Qwen => "通义千问",
            ChatProviderType.Ernie => "文心一言",
            ChatProviderType.Hunyuan => "混元",
            ChatProviderType.Spark => "讯飞星火",
            ChatProviderType.Doubao => "豆包",
            ChatProviderType.SiliconFlow => "硅基流动",
            ChatProviderType.OpenRouter => "OpenRouter",
            ChatProviderType.TogetherAI => "Together.AI",
            ChatProviderType.Groq => "Groq",
            ChatProviderType.Mistral => "Mistral",
            ChatProviderType.Ollama => "Ollama",
            ChatProviderType.Perplexity => "Perplexity",
            _ => throw new NotSupportedException(),
        };
    }

    public static string AddEmojiToMessage(string message)
    {
        // 将消息按 ": " 分割
        var parts = message.Split([": "], 2, StringSplitOptions.None);
        if (parts.Length < 2)
        {
            return message; // 如果格式不符合预期，直接返回原消息
        }

        var type = parts[0];
        var rest = parts[1];

        // 默认表情符号
        var emoji = "🔧";

        // 遍历 gitmojis 查找匹配的类型
        foreach (var item in CommitTypes.Items)
        {
            if (type.Contains(item.Type, StringComparison.OrdinalIgnoreCase))
            {
                emoji = item.Emoji;
                break;
            }
        }

        // 返回格式化后的消息
        return ChatConfigManager.AppConfiguration.App.UseCommitType ? $"{emoji} {type}: {rest}" : $"{type}: {rest}";
    }

    public static string RemoveMarkdownCodeBlockDelimiters(string input)
    {
        // 移除代码块的起始和结束标记，保留代码块内的内容
        var pattern = @"```[a-zA-Z]*\s*|```";
        var result = Regex.Replace(input, pattern, string.Empty, RegexOptions.Multiline);

        // 移除首行的 # 标记
        var lines = result.Split(['\n'], StringSplitOptions.None);
        if (lines.Length > 0)
        {
            lines[0] = lines[0].TrimStart('#').Trim() + "\n";
        }

        // 移除空行.
        lines = [.. lines.Where(p => !string.IsNullOrWhiteSpace(p))];

        // 重新组合文本
        return string.Join(Environment.NewLine, lines).Trim();
    }

    private static async Task<(string? name, string? desc)> GetCurrentDescriptorAsync()
    {
        var baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".rodel-commit");
        // 查找所有后缀为 .descriptor 的文件.
        var files = Directory.GetFiles(baseDirectory, "*.txt", SearchOption.TopDirectoryOnly);
        if (files.Length == 0)
        {
            return default;
        }

        var currentDirectory = Environment.CurrentDirectory;
        foreach (var file in files)
        {
            var lines = await File.ReadAllLinesAsync(file);
            var firstLine = lines.FirstOrDefault();
            if (string.IsNullOrEmpty(firstLine) || !Path.Exists(firstLine))
            {
                continue;
            }

            if (currentDirectory.Contains(firstLine!) || firstLine!.Contains(currentDirectory))
            {
                var otherContent = string.Join('\n', lines.Skip(1).Where(p => !string.IsNullOrWhiteSpace(p)).ToList());
                return (Path.GetFileNameWithoutExtension(file), otherContent.Trim());
            }
        }

        return default;
    }

    private static async Task<string> GetDiffAsync()
    {
        // 运行 `git diff --staged --ignore-all-space --diff-algorithm=minimal --function-context --no-ext-diff --no-color` 命令，获取当前 Environment.CurrentDirectory 下的文件变更。
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "diff --staged --ignore-all-space --diff-algorithm=minimal --function-context --no-ext-diff --no-color",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await process.WaitForExitAsync();
        if (!string.IsNullOrEmpty(error))
        {
            throw new InvalidOperationException(error);
        }

        return output;
    }

    private static async Task GenerateCommitAsync(string commitMessage, bool needEdit = false)
    {
        var command = needEdit ? "commit --edit -m" : "commit -m";
        var commitProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"{command} \"{commitMessage}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        commitProcess.Start();
        _ = await commitProcess.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var commitError = await commitProcess.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await commitProcess.WaitForExitAsync();
        AnsiConsole.MarkupLine("[green]Commit successfully.[/]");
        if (!string.IsNullOrEmpty(commitError))
        {
            AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(commitError)}[/]");
        }
    }

    private static async Task PushCommitAsync(string commitMessage)
    {
        // 运行 `git push` 命令，推送变更。
        AnsiConsole.MarkupLine("Start to push...");
        var pushProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "push",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        pushProcess.Start();

        _ = await pushProcess.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
        var pushError = await pushProcess.StandardError.ReadToEndAsync().ConfigureAwait(false);
        await pushProcess.WaitForExitAsync();
        AnsiConsole.MarkupLine("[green]Push successfully.[/]");
        if (!string.IsNullOrEmpty(pushError))
        {
            AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(pushError)}[/]");
        }
    }

    private async Task<List<ChatProviderType>> GetAvailableServicesAsync()
    {
        var totalServices = Enum.GetValues<ChatProviderType>().ToList();
        var result = new List<ChatProviderType>();
        foreach (var provider in totalServices)
        {
            var config = await configManager.GetChatConfigAsync(provider).ConfigureAwait(false);
            if (config?.IsValid() == true)
            {
                result.Add(provider);
            }
        }

        return result;
    }
}
#pragma warning restore VSTHRD111 // Use ConfigureAwait(bool)
