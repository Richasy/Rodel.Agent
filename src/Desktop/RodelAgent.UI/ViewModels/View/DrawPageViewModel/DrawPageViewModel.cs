// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Draw;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(DrawPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadDrawServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.DrawServicePageIsEnterSend, true);
            await ReloadAvailableServicesAsync();
        }
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<DrawProviderType>();
        var services = new List<DrawServiceItemViewModel>();
        var drawConfigManager = this.Get<IDrawConfigManager>();
        foreach (var p in providers)
        {
            var config = await drawConfigManager.GetDrawConfigAsync(p);
            if (config?.IsValid() == true)
            {
                services.Add(new DrawServiceItemViewModel(p));
            }
        }

        Services = services;
        IsNoService = services.Count == 0;

        if (IsNoService)
        {
            return;
        }

        var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedDrawService, DrawProviderType.OpenAI);
        var service = Services.Find(p => p.ProviderType == lastSelectedService) ?? Services[0];
        SelectServiceCommand.Execute(service);
    }

    [RelayCommand]
    private async Task SelectServiceAsync(DrawServiceItemViewModel service)
    {
        if (IsNoService)
        {
            return;
        }

        Models.Clear();
        SelectedModel = null;
        foreach (var item in Services)
        {
            item.IsSelected = item.ProviderType == service.ProviderType;
        }

        SelectedService = service;
        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedDrawService, service.ProviderType);

        _drawService = this.Get<IDrawService>(service.ProviderType.ToString());
        var sourceConfig = await this.Get<IDrawConfigManager>().GetDrawConfigAsync(service.ProviderType);
        var serviceModels = _drawService.GetPredefinedModels();
        var customModels = sourceConfig?.CustomModels ?? [];
        var models = serviceModels.ToList().Concat(customModels).ToList().ConvertAll(p => new DrawModelItemViewModel(p));
        models.ForEach(Models.Add);
        var lastSelectedModel = this.Get<ISettingsToolkit>().ReadLocalSetting($"{service.ProviderType}LastSelectedDrawModel", string.Empty);
        var model = Models.FirstOrDefault(p => p.Id == lastSelectedModel) ?? Models.FirstOrDefault();
        SelectModelCommand.Execute(model);
        ReloadHistoryCommand.Execute(default);
    }

    [RelayCommand]
    private void SelectModel(DrawModelItemViewModel model)
    {
        if (SelectedModel == model)
        {
            return;
        }

        Sizes.Clear();
        SelectedSize = null;
        foreach (var item in Models)
        {
            item.IsSelected = item.Id == model.Id;
        }

        SelectedModel = model;
        this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService.ProviderType}LastSelectedDrawModel", model.Id);
        var sizes = model.Data.SupportSizes.ToList().ConvertAll(p => new DrawSizeItemViewModel(p));
        sizes.ForEach(Sizes.Add);
        var lastSelectedSize = this.Get<ISettingsToolkit>().ReadLocalSetting($"{SelectedService.ProviderType}_{model.Id}_DrawSize", string.Empty);
        SelectedSize = string.IsNullOrEmpty(lastSelectedSize) || !sizes.Any(p => p.ToString() == lastSelectedSize)
            ? sizes.FirstOrDefault()
            : sizes.Find(p => p.ToString() == lastSelectedSize);
    }

    [RelayCommand]
    private static async Task OpenDrawFolderAsync()
    {
        if (!Directory.Exists(AppToolkit.GetDrawFolderPath()))
        {
            Directory.CreateDirectory(AppToolkit.GetDrawFolderPath());
        }

        var folder = await StorageFolder.GetFolderFromPathAsync(AppToolkit.GetDrawFolderPath());
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ReloadHistoryAsync()
    {
        if (SelectedService is null)
        {
            return;
        }

        var history = await this.Get<IStorageService>().GetDrawSessionsAsync();
        SyncDrawHistory(history ?? []);
    }

    private void SyncDrawHistory(List<DrawRecord> list)
    {
        // 边界情况处理
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(History);

        list = [.. list.OrderByDescending(item => item.Time)];
        var listDict = list.ToDictionary(item => item.Id);

        for (var i = History.Count - 1; i >= 0; i--)
        {
            var item = History[i];
            if (!listDict.ContainsKey(item.Data.Id))
            {
                History.RemoveAt(i);
            }
        }

        for (var i = 0; i < list.Count; i++)
        {
            var listItem = list[i];
            if (i < History.Count)
            {
                var collectionItem = History[i];
                if (!Equals(listItem.Id, collectionItem.Data.Id))
                {
                    History.Insert(i, new(listItem));
                }
            }
            else
            {
                History.Add(new(listItem));
            }
        }

        CheckHistoryCount();
    }

    private void CheckHistoryCount()
    {
        HistoryCount = History.Count;
        IsHistoryEmpty = History.Count == 0;
    }

    partial void OnSelectedSizeChanged(DrawSizeItemViewModel? value)
    {
        if (SelectedModel is null || value is null)
        {
            return;
        }

        this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService.ProviderType}_{SelectedModel.Id}_DrawSize", value.ToString());
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DrawServicePageIsEnterSend, value);
}
