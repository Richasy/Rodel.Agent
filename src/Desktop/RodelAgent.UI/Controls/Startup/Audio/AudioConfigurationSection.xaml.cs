// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Constants;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 聊天配置部分.
/// </summary>
public sealed partial class AudioConfigurationSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioConfigurationSection"/> class.
    /// </summary>
    public AudioConfigurationSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as AudioServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        var formControl = newVM.ProviderType switch
        {
            ProviderType.OpenAI => CreateForm<OpenAIAudioConfigSection>(),
            ProviderType.AzureOpenAI => CreateForm<AzureOpenAIAudioConfigSection>(),
            ProviderType.AzureSpeech => CreateForm<AzureSpeechAudioConfigSection>(),
            _ => throw new NotImplementedException(),
        };

        FormPresenter.Content = formControl;
    }

    private DependencyObject CreateForm<TControl>()
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is AudioServiceConfigControlBase form)
        {
            form.ViewModel = ViewModel;
        }

        return control as DependencyObject;
    }

    private void OnPredefinedModelsClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
