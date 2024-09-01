// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Startup;
using RodelAgent.UI.Toolkits;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天服务项视图模型.
/// </summary>
public sealed partial class ChatServiceItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ProviderType _providerType;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private ClientConfigBase _config;

    [ObservableProperty]
    private bool _isServerModelVisible;

    [ObservableProperty]
    private bool _isCustomModelsEmpty;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServiceItemViewModel"/> class.
    /// </summary>
    public ChatServiceItemViewModel(
        ProviderType providerType,
        string name)
    {
        ProviderType = providerType;
        Name = name;

        var serverModels = this.Get<IChatClient>()
            .GetPredefinedModels(ProviderType);
        ServerModels.Clear();
        serverModels.ForEach(p => ServerModels.Add(new ChatModelItemViewModel(p)));
        IsServerModelVisible = ServerModels.Count > 0;
        CheckCustomModelsCount();
    }

    /// <summary>
    /// 自定义模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> CustomModels { get; } = new();

    /// <summary>
    /// 服务模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> ServerModels { get; } = new();

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    public void SetConfig(ClientConfigBase config)
    {
        Config = config;
        if (config?.IsCustomModelNotEmpty() ?? false)
        {
            CustomModels.Clear();
            config.CustomModels.ForEach(p => CustomModels.Add(new ChatModelItemViewModel(p, DeleteCustomModel)));
            CheckCustomModelsCount();
        }

        CheckCurrentConfig();
    }

    /// <summary>
    /// 检查当前配置是否有效.
    /// </summary>
    public void CheckCurrentConfig()
        => IsCompleted = Config?.IsValid() ?? false;

    /// <summary>
    /// 模型是否已存在于列表之中.
    /// </summary>
    /// <param name="model">模型.</param>
    /// <returns>是否存在.</returns>
    public bool IsModelExist(ChatModel model)
        => CustomModels.Any(p => p.Id == model.Id) || ServerModels.Any(p => p.Id == model.Id);

    /// <summary>
    /// 添加自定义模型.
    /// </summary>
    /// <param name="model">模型.</param>
    public void AddCustomModel(ChatModel model)
    {
        if (IsModelExist(model))
        {
            return;
        }

        CustomModels.Add(new ChatModelItemViewModel(model, DeleteCustomModel));
        Config.CustomModels ??= new();
        Config.CustomModels.Add(model);
        CheckCustomModelsCount();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);

    [RelayCommand]
    private async Task CreateCustomModelAsync()
    {
        var dialog = new CustomChatModelDialog();
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            var model = dialog.Model;
            if (model == null)
            {
                return;
            }

            if (IsModelExist(model))
            {
                this.Get<AppViewModel>()
                    .ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ModelAlreadyExist), Models.Constants.InfoType.Error));
                return;
            }

            AddCustomModel(model);
        }
    }

    private void DeleteCustomModel(ChatModelItemViewModel model)
    {
        CustomModels.Remove(model);
        Config.CustomModels?.Remove(model.Data);
        CheckCustomModelsCount();
    }

    private void CheckCustomModelsCount()
        => IsCustomModelsEmpty = CustomModels.Count == 0;
}
