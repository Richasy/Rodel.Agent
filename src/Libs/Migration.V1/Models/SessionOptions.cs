using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Migration.V1.Models;

/// <summary>
/// Session options.
/// </summary>
internal class SessionOptions
{
    /// <summary>
    /// Controls randomness: lowering temperature results in less random completions. At zero the model becomes deterministic.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    /// <summary>
    /// The maximum number of tokens to generate in the output. One token is roughly 4 characters.
    /// </summary>
    [JsonPropertyName("max_response_tokens")]
    public int MaxResponseTokens { get; set; }

    /// <summary>
    /// Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopP { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on their existing frequency in the text so far.
    /// Decreases the model's likelihood to repeat the same line verbatim.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// How much to penalize new tokens based on whether they appear in the text so far.
    /// Increases the model's likelihood to talk about new topics.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; }

    /// <summary>
    /// Session id.
    /// </summary>
    [Key]
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }
}
