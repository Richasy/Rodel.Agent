// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels;

/// <summary>
/// 提示词测试会话项视图模型.
/// </summary>
public sealed partial class PromptTestSessionItemViewModel : ViewModelBase
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<PromptTestSessionItemViewModel> _logger;
    private readonly string _sourceInput;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private InfoType _state;

    [ObservableProperty]
    private string _input;

    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private string _error;

    [ObservableProperty]
    private bool _isSuccess;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestSessionItemViewModel"/> class.
    /// </summary>
    public PromptTestSessionItemViewModel(
        IChatClient chatClient,
        int index,
        ChatSessionPreset preset,
        string input,
        string sourceInput)
    {
        _chatClient = chatClient;
        _logger = this.Get<ILogger<PromptTestSessionItemViewModel>>();
        State = InfoType.Information;
        Index = index;
        Input = input;
        Title = string.Format(ResourceToolkit.GetLocalizedString(StringNames.TestName), index + 1);
        Session = _chatClient.CreateSession(preset);
        _sourceInput = sourceInput;
    }

    /// <summary>
    /// 获取或设置会话.
    /// </summary>
    public ChatSession Session { get; private set; }

    /// <summary>
    /// 获取条目 JSON 字符串.
    /// </summary>
    /// <returns>JSON.</returns>
    public string GetItemJson(string? context = default)
    {
        var question = _sourceInput;
        var answer = Result;
        if (answer.Contains("|R1|"))
        {
            var regex = new Regex(@"\|R1\|(.*?)\|R1\|", RegexOptions.Singleline);

            var match = regex.Match(Result);
            if (match.Success)
            {
                answer = match.Groups[1].Value.Trim();
            }
        }

        var obj = new
        {
            question,
            answer,
            context = context?.ToLower(),
        };

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PromptTestSessionItemViewModel model && Index == model.Index;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Index);

    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            State = InfoType.Loading;
            var message = ChatMessage.CreateUserMessage(Input);
            _cancellationTokenSource = new CancellationTokenSource();
            var response = await _chatClient.SendMessageAsync(Session.Id, message, cancellationToken: _cancellationTokenSource.Token);
            if (response != null)
            {
                Result = response.GetFirstTextContent();
                State = InfoType.Success;
                IsSuccess = true;
            }
            else
            {
                State = InfoType.Warning;
                IsSuccess = false;
            }
        }
        catch (TaskCanceledException)
        {
            State = InfoType.Warning;
            Error = ResourceToolkit.GetLocalizedString(StringNames.UserCancelGenerate);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{Title} Failed.\n {Input}\n");
            State = InfoType.Error;
            Error = ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        State = InfoType.Warning;
    }
}
