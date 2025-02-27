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
        ViewModel.InjectGetOptionsFunc(GetOptions);
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
                ReloadOpenAIOptionsUI();
                break;
            default:
                break;
        }
    }

    private ChatOptions GetOptions()
    {
        var options = new ChatOptions();
        options.AdditionalProperties ??= [];
        options.AdditionalProperties["stream"] = StreamOutputSwitch.IsOn;
        options.AdditionalProperties["max_rounds"] = Convert.ToInt32(MaxRoundsSlider.Value);
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
