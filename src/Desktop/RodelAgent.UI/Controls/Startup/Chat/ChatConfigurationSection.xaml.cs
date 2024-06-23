// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 聊天配置部分.
/// </summary>
public sealed partial class ChatConfigurationSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatConfigurationSection"/> class.
    /// </summary>
    public ChatConfigurationSection()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as ChatServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        var formControl = newVM.ProviderType switch
        {
            ProviderType.Moonshot
            or ProviderType.ZhiPu
            or ProviderType.LingYi
            or ProviderType.DeepSeek
            or ProviderType.OpenRouter
            or ProviderType.Groq
            or ProviderType.MistralAI
            or ProviderType.TogetherAI
            or ProviderType.Perplexity
            or ProviderType.DashScope => CreateForm<ChatClientConfigSection>(),
            ProviderType.Anthropic
            or ProviderType.Gemini
            or ProviderType.Ollama => CreateForm<ChatClientEndpointConfigSection>(),
            ProviderType.OpenAI => CreateForm<OpenAIChatConfigSection>(),
            ProviderType.AzureOpenAI => CreateForm<AzureOpenAIChatConfigSection>(),
            ProviderType.QianFan => CreateForm<QianFanChatConfigSection>(),
            ProviderType.HunYuan => CreateForm<HunYuanChatConfigSection>(),
            ProviderType.SparkDesk => CreateForm<SparkDeskChatConfigSection>(),
            _ => throw new NotImplementedException(),
        };

        FormPresenter.Content = formControl;
    }

    private DependencyObject CreateForm<TControl>()
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is ChatServiceConfigControlBase form)
        {
            form.ViewModel = ViewModel;
        }

        return control as DependencyObject;
    }

    private void OnPredefinedModelsClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
