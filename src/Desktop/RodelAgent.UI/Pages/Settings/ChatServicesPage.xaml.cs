// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Controls.Settings;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.Pages.Settings;

/// <summary>
/// 对话模型设置.
/// </summary>
public sealed partial class ChatServicesPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServicesPage"/> class.
    /// </summary>
    public ChatServicesPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        await ViewModel.InitializeOnlineChatServicesCommand.ExecuteAsync(default);
        foreach (var item in ViewModel.OnlineChatServices)
        {
            var section = item.ProviderType switch
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
                or ProviderType.SiliconFlow
                or ProviderType.DashScope => CreateForm<ModelClientConfigSettingSection>(item),
                ProviderType.Anthropic
                or ProviderType.Gemini
                or ProviderType.Ollama => CreateForm<ModelClientEndpointConfigSettingSection>(item),
                ProviderType.OpenAI => CreateForm<OpenAIChatConfigSettingSection>(item),
                ProviderType.AzureOpenAI => CreateForm<AzureOpenAIChatConfigSettingSection>(item),
                ProviderType.QianFan => CreateForm<QianFanChatConfigSettingSection>(item),
                ProviderType.HunYuan => CreateForm<HunYuanChatConfigSettingSection>(item),
                ProviderType.SparkDesk => CreateForm<SparkDeskChatConfigSettingSection>(item),
                _ => throw new NotImplementedException(),
            };

            if (section is not null)
            {
                RootContainer.Children.Add(section);
            }
        }
    }

    private static FrameworkElement CreateForm<TControl>(ChatServiceItemViewModel vm)
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is ChatServiceConfigControlBase form)
        {
            form.ViewModel = vm;
        }

        return control as FrameworkElement;
    }
}
