// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages.Internal;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestPageViewModel"/> class.
    /// </summary>
    public PromptTestPageViewModel(
        ILogger<PromptTestPageViewModel> logger,
        IStorageService storageService,
        IChatParametersFactory chatParametersFactory,
        IChatClient chatClient)
    {
        _logger = logger;
        _storageService = storageService;
        _chatParametersFactory = chatParametersFactory;
        _chatClient = chatClient;
        ExtraColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestPageExtraColumnWidth, 280d);
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(PromptTestPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var chatPageVM = this.Get<ChatServicePageViewModel>();
        if (chatPageVM.AvailableServices.Count == 0)
        {
            await chatPageVM.ResetAvailableChatServicesCommand.ExecuteAsync(default);
        }

        var chatServices = await PageViewModelShare.GetChatServicesAsync(_storageService);
        AvailableServices = chatServices.Where(p => p.IsCompleted).ToList();
        var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestLastSelectedChatService, string.Empty);
        SelectedService = default;
        IsAvailableServicesEmpty = !AvailableServices.Any();
        if (!string.IsNullOrEmpty(lastSelectedService))
        {
            var lastSelectedServiceVM = AvailableServices.FirstOrDefault(p => p.ProviderType.ToString() == lastSelectedService);
            ChangeService(lastSelectedServiceVM ?? AvailableServices.FirstOrDefault());
        }
        else
        {
            ChangeService(AvailableServices.FirstOrDefault());
        }

        if (string.IsNullOrEmpty(InputFilePath))
        {
            InputFilePath = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestInputFilePath, string.Empty);
            if (!string.IsNullOrEmpty(InputFilePath))
            {
                await ParseInputAsync(InputFilePath, false);
            }
        }

        if (string.IsNullOrEmpty(PresetVariablesFilePath))
        {
            PresetVariablesFilePath = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestPresetVariablesFilePath, string.Empty);
            if (!string.IsNullOrEmpty(PresetVariablesFilePath))
            {
                await ParsePresetVariablesAsync(PresetVariablesFilePath);
            }
        }

        if (string.IsNullOrEmpty(MessageJsonFilePath))
        {
            MessageJsonFilePath = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestMessageJsonFilePath, string.Empty);
            if (!string.IsNullOrEmpty(MessageJsonFilePath))
            {
                await ParseHistoryAsync(MessageJsonFilePath, false);
            }
        }

        CheckSessionCount();
    }

    [RelayCommand]
    private void ChangeService(ChatServiceItemViewModel service)
    {
        if (SelectedService == service)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestLastSelectedChatService, service?.ProviderType.ToString() ?? string.Empty);
        SelectedService = service;

        if (SelectedService is not null)
        {
            var models = _chatClient.GetModels(SelectedService.ProviderType);
            AvailableModels = models.Select(p => new ChatModelItemViewModel(p)).ToList();
            var lastSelectedModel = SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestLastSelectedModel, string.Empty);
            SelectedModel = default;
            if (!string.IsNullOrEmpty(lastSelectedModel))
            {
                var lastSelectedModelVM = AvailableModels.FirstOrDefault(p => p.Data.Id.ToString() == lastSelectedModel);
                ChangeModel(lastSelectedModelVM ?? AvailableModels.FirstOrDefault());
            }
            else
            {
                ChangeModel(AvailableModels.FirstOrDefault());
            }
        }
    }

    [RelayCommand]
    private void ChangeModel(ChatModelItemViewModel model)
    {
        if (SelectedModel == model)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestLastSelectedModel, model?.Data.Id.ToString() ?? string.Empty);
        SelectedModel = model;

        var preset = new ChatSessionPreset
        {
            Provider = SelectedService.ProviderType,
            Name = model?.Name,
            Model = model?.Data.Id,
            Id = DateTimeOffset.Now.ToString("yyyy_MM_dd_HH_mm_ss"),
            Parameters = _chatParametersFactory.CreateChatParameters(SelectedService.ProviderType),
        };

        Preset = new ChatPresetItemViewModel(preset);
    }

    private void CheckSessionCount()
        => IsSessionsEmpty = Sessions is null || Sessions.Count == 0;

    partial void OnExtraColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.ReadLocalSetting(SettingNames.PromptTestPageExtraColumnWidth, value);
        }
    }
}
