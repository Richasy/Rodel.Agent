// Copyright (c) Rodel. All rights reserved.

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端.
/// </summary>
public sealed partial class ChatClient : IChatClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    public ChatClient(
        IChatProviderFactory providerFactory,
        IChatParametersFactory parameterFactory,
        ILogger<ChatClient> logger)
    {
        Sessions = new List<ChatSession>();
        _logger = logger;
        _providerFactory = providerFactory;
        _parameterFactory = parameterFactory;
    }

    /// <inheritdoc/>
    public void LoadSessions(List<ChatSession> sessions)
    {
        Sessions.Clear();
        Sessions.AddRange(sessions);
    }

    /// <inheritdoc/>
    public List<ChatModel> GetPredefinedModels(ProviderType type)
    {
        var preType = typeof(PredefinedModels);
        var properties = preType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var prop in properties)
        {
            if (prop.Name.StartsWith(type.ToString()))
            {
                return prop.GetValue(default) as List<ChatModel>
                    ?? throw new ArgumentException("Predefined models not found.");
            }
        }

        return new List<ChatModel>();
    }

    /// <inheritdoc/>
    public ChatSession CreateSession(ProviderType providerType, BaseFieldParameters parameters = null, string? modelId = null)
    {
        var id = Guid.NewGuid().ToString("N");
        parameters = GetChatParameters(providerType, parameters);
        var session = ChatSession.CreateSession(id, parameters);
        session.Provider = providerType;
        if (!string.IsNullOrEmpty(modelId))
        {
            var model = FindModelInProvider(providerType, modelId);
            session.Model = model?.Id
                ?? throw new ArgumentException("Model not found.");
        }

        Sessions.Add(session);
        return session;
    }

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="preset">预设会话.</param>
    /// <returns>会话对象.</returns>
    /// <exception cref="ArgumentException">预设不合规.</exception>
    public ChatSession CreateSession(ChatSessionPreset preset)
    {
        var id = Guid.NewGuid().ToString("N");

        // 需要重新序列化以创建新的对象.
        var presetJson = JsonSerializer.Serialize(preset);
        var newPreset = JsonSerializer.Deserialize<ChatSessionPreset>(presetJson);
        newPreset.Parameters = GetChatParameters(preset.Provider, preset.Parameters);
        var session = ChatSession.CreateSession(id, newPreset);

        session.Parameters = GetChatParameters(preset.Provider, preset.Parameters);
        var model = string.IsNullOrEmpty(preset.Model)
            ? GetModels(preset.Provider).FirstOrDefault()
            : FindModelInProvider(preset.Provider, preset.Model);

        session.Model = model != null
            ? model.Id
            : throw new ArgumentException("Model not found.");

        Sessions.Add(session);
        return session;
    }

    /// <inheritdoc/>
    public List<ChatModel> GetModels(ProviderType type)
        => GetProvider(type).GetModelList();

    /// <inheritdoc/>
    public async Task<ChatMessage> SendMessageAsync(
        string sessionId,
        ChatMessage? message = null,
        string? modelId = null,
        Action<string> streamingAction = default,
        List<KernelPlugin>? plugins = default,
        CancellationToken cancellationToken = default)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException("Session not found.");

        ChatMessage response = default;
        ResetSessionModel(session, modelId);
        var model = FindModelInProvider(session.Provider, session.Model);
        if (message.Content.Any(p => p.Type == ChatContentType.ImageUrl) && !model.IsSupportVision)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.ModelNotSupportImage, string.Empty);
        }

        var kernel = FindKernelProvider(session.Provider, session.Model);
        if (kernel == null)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.ProviderNotSupported, string.Empty);
        }

        try
        {
            kernel.Plugins.Clear();
            if (plugins != null && plugins.Count > 0 && model.IsSupportTool)
            {
                kernel.Plugins.AddRange(plugins);
            }

            response = await KernelSendMessageAsync(kernel, session, message, streamingAction, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return ChatMessage.CreateClientMessage(ClientMessageType.GenerateCancelled, string.Empty);
            }

            response.Time = DateTimeOffset.Now;
            session.Messages.Add(response);

            return response;
        }
        catch (TaskCanceledException)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.GenerateCancelled, string.Empty);
        }
        catch
        {
            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async Task<List<KernelPlugin>> RetrievePluginsFromDllAsync(string dllPath)
    {
        var result = new List<KernelPlugin>();
        _tempPluginPath = dllPath;
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

        try
        {
            await Task.Run(() =>
            {
                var assembly = Assembly.LoadFile(dllPath);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name.EndsWith("Plugin", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(type);
                            var displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name.Replace("Plugin", string.Empty);
                            var kernelPlugin = KernelPluginFactory.CreateFromObject(instance, displayName)
                                ?? throw new Exception("Plugin not created.");

                            kernelPlugin.ExternalId = type.Name;
                            result.Add(kernelPlugin);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create plugin from type {Type}.", type);
                            _logger.LogDebug(ex.Message);
                            continue;
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            _tempPluginPath = string.Empty;
        }

        return result;
    }

    /// <inheritdoc/>
    public IChatCompletionService? GetKernelChatCompletionService(ProviderType type)
    {
        var provider = GetProvider(type);
        var kernel = provider.GetCurrentKernel();
        return kernel?.GetRequiredService<IChatCompletionService>();
    }

    /// <inheritdoc/>
    public async Task<string?> InvokeFunctionAsync(ProviderType type, string model, KernelFunction function, KernelArguments arguments)
    {
        var kernel = FindKernelProvider(type, model);
        var result = await function.InvokeAsync(kernel, arguments).ConfigureAwait(false);
        return result.GetValue<string>();
    }

    private Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        // 当导入插件需要依赖其他程序集时，尝试从插件目录加载.
        var pluginFolder = Path.GetDirectoryName(_tempPluginPath);
        var assemblyPath = Path.Combine(pluginFolder, new AssemblyName(args.Name).Name + ".dll");
        return File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : default;
    }
}
