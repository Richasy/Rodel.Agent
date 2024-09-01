// Copyright (c) Rodel. All rights reserved.

using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图服务项视图模型.
/// </summary>
public sealed partial class AudioServiceItemViewModel : ViewModelBase
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
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceItemViewModel"/> class.
    /// </summary>
    public AudioServiceItemViewModel(
        ProviderType providerType,
        string name)
    {
        ProviderType = providerType;
        Name = name;

        var serverModels = this.Get<IAudioClient>()
            .GetPredefinedModels(ProviderType);
        ServerModels.Clear();
        serverModels.ForEach(p => ServerModels.Add(new AudioModelItemViewModel(p)));
        IsServerModelVisible = ServerModels.Count > 0;
    }

    /// <summary>
    /// 服务模型.
    /// </summary>
    public ObservableCollection<AudioModelItemViewModel> ServerModels { get; } = new();

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置内容.</param>
    public void SetConfig(ClientConfigBase config)
    {
        Config = config;
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
    public bool IsModelExist(AudioModel model)
        => ServerModels.Any(p => p.Id == model.Id);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AudioServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
