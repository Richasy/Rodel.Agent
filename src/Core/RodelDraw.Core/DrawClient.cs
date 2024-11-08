// Copyright (c) Rodel. All rights reserved.

using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.TextToImage;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelDraw.Core;

/// <summary>
/// 绘图客户端.
/// </summary>
public sealed partial class DrawClient : IDrawClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawClient"/> class.
    /// </summary>
    public DrawClient(
        IDrawProviderFactory providerFactory,
        IDrawParametersFactory parameterFactory,
        ILogger<DrawClient> logger)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _parameterFactory = parameterFactory;
    }

    /// <inheritdoc/>
    public async Task<string> DrawAsync(DrawSession session, CancellationToken cancellationToken = default)
    {
        var kernel = FindKernelProvider(session.Provider, session.Model)
            ?? throw new ArgumentException("Kernel not found.");

        try
        {
            session.Parameters ??= GetDrawParameters(session.Provider);
            var settings = GetExecutionSettings(session);
            var drawService = kernel.GetRequiredService<ITextToImageService>();
            var result = await drawService.GenerateImageAsync(session.Request.Prompt, settings.Width, settings.Height, cancellationToken: cancellationToken).ConfigureAwait(false);
            session.Time = DateTimeOffset.Now;
            if (result.StartsWith("http"))
            {
                // Download image and convert to base64.
                var imageBytes = await new HttpClient().GetByteArrayAsync(result, cancellationToken).ConfigureAwait(false);
                return Convert.ToBase64String(imageBytes);
            }

            return result;
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Image generation task was canceled.");
        }
        catch (Exception)
        {
            throw;
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public List<DrawModel> GetModels(ProviderType type)
        => GetProvider(type).GetModelList();

    /// <inheritdoc/>
    public List<DrawModel> GetPredefinedModels(ProviderType type)
    {
        var preType = typeof(PredefinedModels);
        var properties = preType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var prop in properties)
        {
            if (prop.Name.StartsWith(type.ToString()))
            {
                return prop.GetValue(default) as List<DrawModel>
                    ?? throw new ArgumentException("Predefined models not found.");
            }
        }

        return new List<DrawModel>();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
