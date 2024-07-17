// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天预设模块视图模型.
/// </summary>
public sealed partial class ChatPresetModuleViewModel : ViewModelBase<ChatPresetItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPresetModuleViewModel"/> class.
    /// </summary>
    public ChatPresetModuleViewModel(
        IStorageService storageService,
        IChatParametersFactory chatParametersFactory)
        : base(default)
    {
        _storageService = storageService;
        _chatParametersFactory = chatParametersFactory;
        StepCount = 2;
        Messages.CollectionChanged += (sender, e) => CheckMessageCount();
        CheckMessageCount();
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    /// <param name="data">数据.</param>
    /// <param name="type">预设类型.</param>
    public void SetData(ChatPresetItemViewModel data, ChatSessionPresetType type)
    {
        IsManualClose = false;
        Data = data;
        Name = data.Name;
        PresetType = type;
        Instruction = data.Data.SystemInstruction;
        StopSequences.Clear();
        FilterCharacters.Clear();
        Messages.Clear();
        if (data.Data.StopSequences != null)
        {
            foreach (var item in data.Data.StopSequences)
            {
                StopSequences.Add(item);
            }
        }

        if (data.Data.FilterCharacters != null)
        {
            foreach (var item in data.Data.FilterCharacters)
            {
                FilterCharacters.Add(item);
            }
        }

        CurrentStep = 0;
        CheckStep();
        CheckPresetType();

        if (data.Data.Messages != null)
        {
            foreach (var item in data.Data.Messages)
            {
                AddMessage(item);
            }
        }

        AvailableServices.Clear();
        var services = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>().AvailableServices;
        foreach (var item in services)
        {
            AvailableServices.Add(new ChatServiceItemViewModel(item.ProviderType, item.Name));
        }

        CheckMaxTurnEnabled();
        SelectedService = AvailableServices.FirstOrDefault(x => x.ProviderType == data.Data.Provider);
    }

    [RelayCommand]
    private void GoNext()
    {
        if (!IsLastStep)
        {
            CurrentStep++;
        }
    }

    [RelayCommand]
    private void GoPrev()
        => CurrentStep--;

    [RelayCommand]
    private Task SaveAgentPresetAsync()
        => SavePresetInternalAsync(ChatSessionPresetType.Agent);

    [RelayCommand]
    private Task SaveSessionPresetAsync()
        => SavePresetInternalAsync(ChatSessionPresetType.Session);

    [RelayCommand]
    private void AddMessage(ChatMessage message)
        => Messages.Add(new ChatMessageItemViewModel(message, default, DeleteMessage));

    private async Task SavePresetInternalAsync(ChatSessionPresetType type)
    {
        UpdatePresetData();
        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        switch (type)
        {
            case ChatSessionPresetType.Agent:
                await _storageService.AddOrUpdateChatAgentAsync(Data.Data);
                pageVM.ResetAgentsCommand.Execute(default);
                pageVM.ReloadGroupAgentsCommand.Execute(Data.Data.Id);
                break;
            case ChatSessionPresetType.Session:
                await _storageService.AddOrUpdateChatSessionPresetAsync(Data.Data);
                pageVM.ResetSessionPresetsCommand.Execute(default);
                break;
        }

        GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().ForceUpdatePresetAvatar(Data.Data.Id);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void CheckStep()
    {
        IsModelSelectionStep = CurrentStep == 0;
        IsPresetDetailStep = CurrentStep == 1;
        IsLastStep = CurrentStep == StepCount - 1;
        IsPreviousStepShown = CurrentStep > 0;
        CheckNextStepEnabled();
    }

    private void CheckPresetType()
    {
        IsSessionPreset = PresetType == ChatSessionPresetType.Session;
        IsAgentPreset = PresetType == ChatSessionPresetType.Agent;
    }

    private void CheckNextStepEnabled()
        => IsNextButtonEnabled = (IsModelSelectionStep && !string.IsNullOrEmpty(Data.Name)) || !IsModelSelectionStep;

    private void UpdatePresetData()
    {
        if (!IsMaxRoundEnabled)
        {
            Data.Data.MaxRounds = 0;
        }

        Data.Data.Name = Name;
        Data.Data.SystemInstruction = Instruction;
        Data.Data.Provider = SelectedService.ProviderType;
        Data.Data.Messages = Messages.Count > 0 ? Messages.Select(p => p.Data).ToList() : default;
        if (StopSequences.Count > 0)
        {
            Data.Data.StopSequences = StopSequences.ToList();
        }

        if (FilterCharacters.Count > 0)
        {
            Data.Data.FilterCharacters = FilterCharacters.ToList();
        }
    }

    private Task DeleteMessage(ChatMessage message)
    {
        var source = Messages.FirstOrDefault(p => p.Data.Equals(message));
        Messages.Remove(source);
        return Task.CompletedTask;
    }

    private void CheckMessageCount()
    {
        IsMessageEmpty = Messages.Count == 0;
        CheckMaxTurnEnabled();
    }

    private void CheckMaxTurnEnabled()
    {
        if (!IsMessageEmpty && IsMaxRoundEnabled)
        {
            IsMaxRoundEnabled = false;
        }
        else if (IsMessageEmpty && !IsMaxRoundEnabled)
        {
            IsMaxRoundEnabled = true;
        }
    }

    private void ReloadModels()
    {
        Models.Clear();
        if (SelectedService == null)
        {
            IsModelsEmpty = true;
            return;
        }

        var chatClient = GlobalDependencies.ServiceProvider.GetService<IChatClient>();
        var totalModels = chatClient.GetModels(SelectedService.ProviderType);
        foreach (var item in totalModels)
        {
            Models.Add(new ChatModelItemViewModel(item));
        }

        IsModelsEmpty = Models.Count == 0;

        if (!IsModelsEmpty)
        {
            SelectedModel = Models.FirstOrDefault(x => x.Data.Id == Data.Data.Model) ?? Models.First();
        }
        else
        {
            Data.Data.Model = default;
        }
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();

    partial void OnNameChanged(string value)
    {
        Data.Name = value;
        CheckNextStepEnabled();
    }

    partial void OnSelectedServiceChanged(ChatServiceItemViewModel value)
    {
        if (value == null)
        {
            return;
        }

        Data.Data.Provider = value.ProviderType;
        Data.Data.Parameters = _chatParametersFactory.CreateChatParameters(value.ProviderType);
        ReloadModels();
        CheckNextStepEnabled();
    }

    partial void OnSelectedModelChanged(ChatModelItemViewModel value)
    {
        if (value == null)
        {
            return;
        }

        Data.Data.Model = value.Data.Id;
    }

    partial void OnPresetTypeChanged(ChatSessionPresetType value)
        => CheckPresetType();
}
