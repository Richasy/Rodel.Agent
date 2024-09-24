// Copyright (c) Rodel. All rights reserved.

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
    private bool _isSuccess;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestSessionItemViewModel"/> class.
    /// </summary>
    public PromptTestSessionItemViewModel(
        IChatClient chatClient,
        int index,
        ChatSessionPreset preset,
        string input)
    {
        _chatClient = chatClient;
        _logger = this.Get<ILogger<PromptTestSessionItemViewModel>>();
        State = InfoType.Information;
        Index = index;
        Input = input;
        Title = string.Format(ResourceToolkit.GetLocalizedString(StringNames.TestName), index + 1);
        Session = _chatClient.CreateSession(preset);
    }

    /// <summary>
    /// 获取或设置会话.
    /// </summary>
    public ChatSession Session { get; private set; }

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
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{Title} Failed.\n {Input}\n");
            State = InfoType.Error;
        }
    }

    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        State = InfoType.Warning;
    }
}
