// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;
using System.Runtime.InteropServices;
using Windows.System;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Settings page.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        SectionSelector.SelectedItem = SectionSelector.Items[0];
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 || !this.Get<AppViewModel>().IsTraySupport)
        {
            HideWindowSetting.Visibility = Visibility.Collapsed;
        }
    }

    /// <inheritdoc/>
    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => await ViewModel.CheckSaveServicesAsync();

    private void OnJoinGroupButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private async void OnSectionSelectorChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var index = Convert.ToInt32(sender.SelectedItem.Tag);
        if (index == 0)
        {
            GenericContainer.Visibility = Visibility.Visible;
            TranslateContainer.Visibility = Visibility.Collapsed;
            SpeechContainer.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Collapsed;
            DrawContainer.Visibility = Visibility.Collapsed;
        }
        else if (index == 1)
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            TranslateContainer.Visibility = Visibility.Collapsed;
            SpeechContainer.Visibility = Visibility.Collapsed;
            DrawContainer.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Visible;
            await ViewModel.InitializeChatServicesAsync();
            if (ChatPanel.Children.Count <= 1)
            {
                await LoadChatControlsAsync();
            }
        }
        else if (index == 2)
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            TranslateContainer.Visibility = Visibility.Collapsed;
            SpeechContainer.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Collapsed;
            DrawContainer.Visibility = Visibility.Visible;
            await ViewModel.InitializeDrawServicesAsync();
            if (DrawPanel.Children.Count <= 1)
            {
                await LoadDrawControlsAsync();
            }
        }
        else if (index == 3)
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            TranslateContainer.Visibility = Visibility.Collapsed;
            SpeechContainer.Visibility = Visibility.Visible;
            DrawContainer.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Collapsed;
            await ViewModel.InitializeAudioServicesAsync();
            if (SpeechPanel.Children.Count <= 1)
            {
                await LoadAudioControlsAsync();
            }
        }
        else
        {
            GenericContainer.Visibility = Visibility.Collapsed;
            TranslateContainer.Visibility = Visibility.Visible;
            SpeechContainer.Visibility = Visibility.Collapsed;
            DrawContainer.Visibility = Visibility.Collapsed;
            ChatContainer.Visibility = Visibility.Collapsed;
            await ViewModel.InitializeTranslateServicesAsync();
            if (TranslatePanel.Children.Count <= 1)
            {
                await LoadTranslateControlsAsync();
            }
        }
    }

    private async void OnChatDetailButtonClick(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new(AppToolkit.GetDocumentLink("chat-config")));

    private async void OnDrawDetailButtonClick(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new(AppToolkit.GetDocumentLink("image-config")));

    private async void OnSpeechDetailButtonClick(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new(AppToolkit.GetDocumentLink("tts-config")));

    private async void OnTranslateDetailButtonClick(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new(AppToolkit.GetDocumentLink("translate-config")));

    private async Task LoadChatControlsAsync()
    {
        foreach (var vm in ViewModel.ChatServices)
        {
            var control = vm.GetSettingControl();

            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                ChatPanel.Children.Add(control);
            }
        }
    }

    private async Task LoadDrawControlsAsync()
    {
        foreach (var vm in ViewModel.DrawServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                DrawPanel.Children.Add(control);
            }
        }
    }

    private async Task LoadAudioControlsAsync()
    {
        foreach (var vm in ViewModel.AudioServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                SpeechPanel.Children.Add(control);
            }
        }
    }

    private async Task LoadTranslateControlsAsync()
    {
        foreach (var vm in ViewModel.TranslateServices)
        {
            var control = vm.GetSettingControl();
            if (control != null)
            {
                await vm.InitializeCommand.ExecuteAsync(default);
                TranslatePanel.Children.Add(control);
            }
        }
    }
}

/// <summary>
/// Settings page base.
/// </summary>
public abstract class SettingsPageBase : LayoutPageBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageBase"/> class.
    /// </summary>
    protected SettingsPageBase() => ViewModel = this.Get<SettingsPageViewModel>();
}