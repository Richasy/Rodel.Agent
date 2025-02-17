// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using System.Globalization;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型的函数调用部分.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private static KernelFunction GetTitleGenerationFunction()
    {
        var functionDef = """
            You are a title generator. You will get the first chat message in the chat history, please generate a title for this conversation,
            which must be concise and express the core meaning of the first chat message.
            
            Need response with {{$LANGUAGE}}, Try to keep it as simple as possible and don't require punctuation.

            Here is the first chat message:

            ```
            {{$INPUT}}
            ```
            """;

        var function = KernelFunctionFactory.CreateFromPrompt(functionDef, functionName: "GenerateTitle", description: "Generate a title for the conversation.");
        return function;
    }

    [RelayCommand]
    private async Task GenerateTitleAsync()
    {
        var input = Messages.FirstOrDefault(p => p.IsUser)?.Content ?? string.Empty;
        if (string.IsNullOrEmpty(input))
        {
            return;
        }

#pragma warning disable SKEXP0050 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        var firstMessage = TextChunker.SplitPlainTextLines(input, 1024).FirstOrDefault();
#pragma warning restore SKEXP0050 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        var currentLan = CultureInfo.CurrentCulture.EnglishName;
        var arguments = new KernelArguments
        {
            { "LANGUAGE", currentLan },
            { "INPUT", firstMessage },
        };

        var function = GetTitleGenerationFunction();
        var response = string.Empty;
        try
        {
            response = await _chatClient.InvokeFunctionAsync(Data.Provider, Data.Model, function, arguments);
            response = response.Trim('"').Trim('<').Trim('>').Trim('《').Trim('》');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate title.");
        }

        if (string.IsNullOrEmpty(response))
        {
            this.Get<AppViewModel>()
                .ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.GenerateTitleFailed), InfoType.Error));
            return;
        }

        await ChangeTitleAsync(response);
    }
}
