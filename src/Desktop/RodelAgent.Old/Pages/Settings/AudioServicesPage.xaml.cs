// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Controls.Settings;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Constants;

namespace RodelAgent.UI.Pages.Settings;

/// <summary>
/// 音频服务页面.
/// </summary>
public sealed partial class AudioServicesPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServicesPage"/> class.
    /// </summary>
    public AudioServicesPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        await ViewModel.InitializeOnlineAudioServicesCommand.ExecuteAsync(default);
        foreach (var item in ViewModel.OnlineAudioServices)
        {
            var section = item.ProviderType switch
            {
                ProviderType.OpenAI => CreateForm<OpenAIAudioConfigSettingSection>(item),
                ProviderType.AzureOpenAI => CreateForm<AzureOpenAIAudioConfigSettingSection>(item),
                ProviderType.AzureSpeech => CreateForm<AzureSpeechConfigSettingSection>(item),
                _ => default
            };

            if (section is not null)
            {
                RootContainer.Children.Add(section);
            }
        }
    }

    private static FrameworkElement CreateForm<TControl>(AudioServiceItemViewModel vm)
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is AudioServiceConfigControlBase form)
        {
            form.ViewModel = vm;
        }

        return control as FrameworkElement;
    }
}
