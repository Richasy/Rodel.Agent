// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat options panel.
/// </summary>
public sealed partial class ChatOptionsPanel : ChatSessionControlBase
{
    public ChatOptionsPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.RequestReloadOptionsUI += OnRequestReloadOptionsUI;
        ViewModel.InjectFunc(GetOptions, GetStreamOutput, GetMaxRounds);
    }

    protected override void OnControlUnloaded()
    {
        ViewModel.RequestReloadOptionsUI -= OnRequestReloadOptionsUI;
    }

    private void OnRequestReloadOptionsUI(object? sender, EventArgs e)
        => ReloadOptionsUI();

    private void ReloadOptionsUI()
    {
        StreamOutputSwitch.IsOn = ViewModel.GetCurrentConversation()?.UseStreamOutput ?? true;
        MaxRoundsSlider.Value = ViewModel.GetCurrentConversation()?.MaxRounds ?? 0;
        switch (ViewModel.CurrentProvider)
        {
            case ChatProviderType.OpenAI:
            case ChatProviderType.AzureOpenAI:
            case ChatProviderType.ZhiPu:
                ReloadOpenAIOptionsUI();
                break;
            case ChatProviderType.Gemini:
            case ChatProviderType.LingYi:
            case ChatProviderType.DeepSeek:
            case ChatProviderType.Qwen:
            case ChatProviderType.Ernie:
            case ChatProviderType.Moonshot:
            case ChatProviderType.AzureAI:
            case ChatProviderType.Hunyuan:
            case ChatProviderType.Doubao:
            case ChatProviderType.Spark:
            case ChatProviderType.OpenRouter:
            case ChatProviderType.TogetherAI:
            case ChatProviderType.Groq:
            case ChatProviderType.Perplexity:
            case ChatProviderType.Mistral:
            case ChatProviderType.SiliconFlow:
            case ChatProviderType.Ollama:
            case ChatProviderType.XAI:
                ReloadCommonOptionsUI();
                break;
            case ChatProviderType.Anthropic:
                ReloadAnthropicOptionsUI();
                break;
            default:
                ReloadUnspecifiedOptionsUI();
                break;
        }
    }

    private bool GetStreamOutput()
        => StreamOutputSwitch.IsOn;

    private int GetMaxRounds()
        => Convert.ToInt32(MaxRoundsSlider.Value);

    private ChatOptions GetOptions()
    {
        var options = new ChatOptions();
        options.AdditionalProperties ??= [];
        if (FrequencyPenaltyContainer.Visibility == Visibility.Visible)
        {
            options.FrequencyPenalty = (float)FrequencyPenaltySlider.Value;
        }

        if (PresencePenaltyContainer.Visibility == Visibility.Visible)
        {
            options.PresencePenalty = (float)PresencePenaltySlider.Value;
        }

        if (MaxOutputTokenContainer.Visibility == Visibility.Visible)
        {
            if (double.IsNaN(MaxOutputTokenBox.Value))
            {
                MaxOutputTokenBox.Value = 0;
            }

            var v = Convert.ToInt32(MaxOutputTokenBox.Value);
            options.MaxOutputTokens = v > 0 ? v : null;
        }

        if (TemperatureContainer.Visibility == Visibility.Visible)
        {
            options.Temperature = (float)TemperatureSlider.Value;
        }

        if (TopPContainer.Visibility == Visibility.Visible)
        {
            var v = TopPSlider.Value;
            options.TopP = Math.Abs(v - 1) < 0.1 ? null : (float)v;
        }

        if (TopKContainer.Visibility == Visibility.Visible)
        {
            var v = TopKSlider.Value;
            options.TopK = Convert.ToInt32(v) == 0 ? null : Convert.ToInt32(v);
        }

        if (ResponseFormatContainer.Visibility == Visibility.Visible)
        {
            var item = (ResponseFormatComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
            options.ResponseFormat = item == "json" ? ChatResponseFormat.Json : ChatResponseFormat.Text;
        }

        return options;
    }

    private void ReloadOpenAIOptionsUI()
    {
        FrequencyPenaltyContainer.Visibility = Visibility.Visible;
        PresencePenaltyContainer.Visibility = Visibility.Visible;
        MaxOutputTokenContainer.Visibility = Visibility.Visible;
        TemperatureContainer.Visibility = Visibility.Visible;
        TopPContainer.Visibility = Visibility.Visible;
        TopKContainer.Visibility = Visibility.Collapsed;
        ResponseFormatContainer.Visibility = Visibility.Visible;

        FrequencyPenaltySlider.Maximum = 2d;
        PresencePenaltySlider.Maximum = 2d;
        FrequencyPenaltySlider.Minimum = -2d;
        PresencePenaltySlider.Minimum = -2d;
        TemperatureSlider.Minimum = 0;
        TemperatureSlider.Maximum = 2;

        FrequencyPenaltySlider.Value = ViewModel.CurrentOptions?.FrequencyPenalty ?? 0d;
        PresencePenaltySlider.Value = ViewModel.CurrentOptions?.PresencePenalty ?? 0d;
        MaxOutputTokenBox.Value = ViewModel.CurrentOptions?.MaxOutputTokens ?? 0;
        TemperatureSlider.Value = ViewModel.CurrentOptions?.Temperature ?? 1d;
        TopPSlider.Value = ViewModel.CurrentOptions?.TopP ?? 1d;
        ResponseFormatComboBox.SelectedIndex = ViewModel.CurrentOptions?.ResponseFormat == ChatResponseFormat.Json ? 1 : 0;
    }

    private void ReloadCommonOptionsUI()
    {
        FrequencyPenaltyContainer.Visibility = Visibility.Visible;
        PresencePenaltyContainer.Visibility = Visibility.Visible;
        MaxOutputTokenContainer.Visibility = Visibility.Visible;
        TemperatureContainer.Visibility = Visibility.Visible;
        TopPContainer.Visibility = Visibility.Visible;
        TopKContainer.Visibility = Visibility.Collapsed;
        ResponseFormatContainer.Visibility = Visibility.Collapsed;

        FrequencyPenaltySlider.Maximum = 2d;
        PresencePenaltySlider.Maximum = 2d;
        FrequencyPenaltySlider.Minimum = -2d;
        PresencePenaltySlider.Minimum = -2d;
        TemperatureSlider.Minimum = 0;
        TemperatureSlider.Maximum = 2;

        FrequencyPenaltySlider.Value = ViewModel.CurrentOptions?.FrequencyPenalty ?? 0d;
        PresencePenaltySlider.Value = ViewModel.CurrentOptions?.PresencePenalty ?? 0d;
        MaxOutputTokenBox.Value = ViewModel.CurrentOptions?.MaxOutputTokens ?? 0;
        TemperatureSlider.Value = ViewModel.CurrentOptions?.Temperature ?? 1d;
        TopPSlider.Value = ViewModel.CurrentOptions?.TopP ?? 1d;
    }

    private void ReloadAnthropicOptionsUI()
    {
        FrequencyPenaltyContainer.Visibility = Visibility.Visible;
        PresencePenaltyContainer.Visibility = Visibility.Visible;
        MaxOutputTokenContainer.Visibility = Visibility.Visible;
        TemperatureContainer.Visibility = Visibility.Visible;
        TopPContainer.Visibility = Visibility.Visible;
        TopKContainer.Visibility = Visibility.Visible;
        ResponseFormatContainer.Visibility = Visibility.Visible;

        FrequencyPenaltySlider.Maximum = 2d;
        PresencePenaltySlider.Maximum = 2d;
        FrequencyPenaltySlider.Minimum = -2d;
        PresencePenaltySlider.Minimum = -2d;
        TemperatureSlider.Minimum = 0;
        TemperatureSlider.Maximum = 2;

        FrequencyPenaltySlider.Value = ViewModel.CurrentOptions?.FrequencyPenalty ?? 0d;
        PresencePenaltySlider.Value = ViewModel.CurrentOptions?.PresencePenalty ?? 0d;
        MaxOutputTokenBox.Value = ViewModel.CurrentOptions?.MaxOutputTokens ?? 0;
        TemperatureSlider.Value = ViewModel.CurrentOptions?.Temperature ?? 1d;
        TopPSlider.Value = ViewModel.CurrentOptions?.TopP ?? 1d;
        TopKSlider.Value = ViewModel.CurrentOptions?.TopK ?? 0;
        ResponseFormatComboBox.SelectedIndex = ViewModel.CurrentOptions?.ResponseFormat == ChatResponseFormat.Json ? 1 : 0;
    }

    private void ReloadUnspecifiedOptionsUI()
    {
        FrequencyPenaltyContainer.Visibility = Visibility.Collapsed;
        PresencePenaltyContainer.Visibility = Visibility.Collapsed;
        MaxOutputTokenContainer.Visibility = Visibility.Collapsed;
        TemperatureContainer.Visibility = Visibility.Collapsed;
        TopPContainer.Visibility = Visibility.Collapsed;
        TopKContainer.Visibility = Visibility.Collapsed;
        ResponseFormatContainer.Visibility = Visibility.Collapsed;
    }

    private void OnMaxOutputTokenBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (double.IsNaN(MaxOutputTokenBox.Value))
        {
            MaxOutputTokenBox.Value = 0;
        }

        var value = Convert.ToInt32(MaxOutputTokenBox.Value);
        MaxOutputTokenBox.Value = Math.Max(0, value);
    }
}
