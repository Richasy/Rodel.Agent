// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 绘图会话视图模型.
/// </summary>
public sealed partial class DrawSessionViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSessionViewModel"/> class.
    /// </summary>
    public DrawSessionViewModel(
        IDrawClient drawClient,
        ILogger<DrawSessionViewModel> logger,
        IStorageService storageService)
    {
        _drawClient = drawClient;
        _logger = logger;
        _storageService = storageService;
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.DrawServicePageIsEnterSend, true);

        AttachIsRunningToAsyncCommand(p => IsDrawing = p, DrawCommand);
        AttachExceptionHandlerToAsyncCommand(HandleDrawException, DrawCommand);
    }

    private static bool IsImageValid(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return false;
        }

        if (!File.Exists(imagePath))
        {
            var appVM = GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>();
            appVM.ShowTip(StringNames.ImageNotFound, InfoType.Error);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void LoadSession(DrawSession session)
    {
        if (IsDrawing || (ImagePath?.Contains(session.Id) ?? false))
        {
            return;
        }

        if (DrawService.ProviderType != session.Provider)
        {
            Clear();
            Initialize(session.Provider);
        }

        Prompt = session.Request.Prompt;
        NegativePrompt = session.Request.NegativePrompt;
        var model = Models.FirstOrDefault(p => p.Id == session.Model);
        if (model != null)
        {
            ChangeModel(model);
        }

        Size = session.Request.Size;
        ImagePath = AppToolkit.GetDrawPicturePath(session.Id);
        LastGenerateTime = session.Time?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
        DataChanged?.Invoke(this, session);
    }

    [RelayCommand]
    private void Initialize(ProviderType providerType)
    {
        var pageVM = GlobalDependencies.ServiceProvider.GetRequiredService<DrawServicePageViewModel>();
        var serviceVM = pageVM.AvailableServices.FirstOrDefault(p => p.ProviderType == providerType);
        DrawService = serviceVM;
        ImagePath = string.Empty;
        Models.Clear();
        var models = _drawClient.GetModels(providerType);
        foreach (var item in models)
        {
            Models.Add(new DrawModelItemViewModel(item));
        }

        var localModelId = SettingsToolkit.ReadLocalSetting($"{providerType}LastSelectedDrawModel", string.Empty);
        var model = Models.FirstOrDefault(p => p.Id == localModelId) ?? Models.FirstOrDefault();
        ChangeModel(model);
    }

    [RelayCommand]
    private void Clear()
    {
        Prompt = string.Empty;
        NegativePrompt = string.Empty;
        ImagePath = string.Empty;
        LastGenerateTime = string.Empty;
        Models.Clear();
        Sizes.Clear();
        Size = default;
    }

    [RelayCommand]
    private void ChangeModel(DrawModelItemViewModel model)
    {
        if (model.IsSelected)
        {
            return;
        }

        foreach (var item in Models)
        {
            item.IsSelected = item.Id == model.Id;
        }

        IsNegativePromptVisible = model.Data.IsNegativeSupport;
        var settingName = $"{DrawService.ProviderType}LastSelectedDrawModel";
        SettingsToolkit.WriteLocalSetting(settingName, model.Id);
        Sizes.Clear();
        Size = default;
        var sizes = _drawClient.GetModels(DrawService.ProviderType).FirstOrDefault(p => p.Id == model.Id)?.SupportSizes
            ?? throw new InvalidDataException("Model sizes not found");
        sizes.ToList().ForEach(Sizes.Add);
        var localSize = SettingsToolkit.ReadLocalSetting($"{DrawService.ProviderType}_{model.Id}_DrawSize", string.Empty);
        Size = string.IsNullOrEmpty(localSize)
            || !sizes.Contains(localSize)
                ? sizes.FirstOrDefault()
                : localSize;
    }

    [RelayCommand]
    private async Task DrawAsync()
    {
        if (string.IsNullOrEmpty(Prompt))
        {
            return;
        }

        ErrorText = string.Empty;
        var sessionData = new DrawSession
        {
            Id = Guid.NewGuid().ToString("N"),
            Model = Models.FirstOrDefault(p => p.IsSelected)?.Id,
            Provider = DrawService.ProviderType,
            Request = new DrawRequest
            {
                Size = Size,
                NegativePrompt = NegativePrompt,
                Prompt = Prompt,
            },
        };

        CancelDraw();
        ImagePath = string.Empty;
        LastGenerateTime = string.Empty;
        _cancellationTokenSource = new CancellationTokenSource();
        var dispatcherQueue = GlobalDependencies.ServiceProvider.GetRequiredService<Microsoft.UI.Dispatching.DispatcherQueue>();
        var result = await _drawClient.DrawAsync(sessionData, _cancellationTokenSource.Token).ConfigureAwait(false);
        dispatcherQueue.TryEnqueue(async () =>
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var bytes = Convert.FromBase64String(result);
            await _storageService.AddOrUpdateDrawSessionAsync(sessionData, bytes);
            var pageVM = GlobalDependencies.ServiceProvider.GetRequiredService<DrawServicePageViewModel>();
            pageVM.UpdateHistoryCommand.Execute(default);
            LastGenerateTime = sessionData.Time.Value.ToString("yyyy-MM-dd HH:mm:ss");
            ImagePath = AppToolkit.GetDrawPicturePath(sessionData.Id);
            DataChanged?.Invoke(this, sessionData);
        });
    }

    [RelayCommand]
    private void CancelDraw()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private async Task OpenImageAsync(string imagePath = default)
    {
        imagePath ??= ImagePath;
        if (!IsImageValid(imagePath))
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(imagePath);
        await Launcher.LaunchFileAsync(file);
    }

    [RelayCommand]
    private async Task CopyImageAsync(string imagePath = default)
    {
        imagePath ??= ImagePath;
        if (!IsImageValid(imagePath))
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(imagePath);
        var dataPackage = new DataPackage();
        dataPackage.SetStorageItems(new[] { file });
        Clipboard.SetContent(dataPackage);
        var appVM = GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>();
        appVM.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private async Task SaveImageAsync(string imagePath = default)
    {
        imagePath ??= ImagePath;
        if (!IsImageValid(imagePath))
        {
            return;
        }

        var appVM = GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>();
        var targetImage = await FileToolkit.SaveFileAsync(".png", appVM.ActivatedWindow);
        if (targetImage == null)
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(imagePath);
        await file.CopyAndReplaceAsync(targetImage);
        appVM.ShowTip(StringNames.Saved, InfoType.Success);
    }

    private void HandleDrawException(Exception ex)
    {
        ErrorText = ex.Message;
        _logger.LogError(ex, "Draw failed.");
    }

    partial void OnSizeChanged(string value)
    {
        var selectedModel = Models.FirstOrDefault(p => p.IsSelected);
        if (selectedModel == null)
        {
            return;
        }

        var settingName = $"{DrawService.ProviderType}_{Models.FirstOrDefault(p => p.IsSelected)?.Id}_DrawSize";
        SettingsToolkit.WriteLocalSetting(settingName, value);
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DrawServicePageIsEnterSend, value);
}
