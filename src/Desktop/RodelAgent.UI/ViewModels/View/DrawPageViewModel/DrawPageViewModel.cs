// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Draw;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;

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
