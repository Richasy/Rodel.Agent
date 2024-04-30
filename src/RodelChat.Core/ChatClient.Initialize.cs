// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using RodelChat.Core.Models.Providers;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的初始化部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 初始化 OpenAI 服务.
    /// </summary>
    public void InitializeOpenAI(string apiKey, string proxyUrl = "", string organizationId = "", List<ChatModel> customModels = null)
    {
        _openAIProvider ??= new OpenAIProvider();
        _openAIProvider.AccessKey = apiKey;
        _openAIProvider.OrganizationId = organizationId;

        if (!string.IsNullOrEmpty(proxyUrl))
        {
            _openAIProvider.BaseUrl = proxyUrl;
        }

        if (customModels != null)
        {
            _openAIProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化 Azure OpenAI 服务.
    /// </summary>
    public void InitializeAzureOpenAI(string apiKey, string endpoint, AzureOpenAIVersion apiVersion = AzureOpenAIVersion.V2024_02_01, List<ChatModel> customModels = null)
    {
        _azureOpenAIProvider ??= new AzureOpenAIProvider();
        _azureOpenAIProvider.AccessKey = apiKey;
        _azureOpenAIProvider.BaseUrl = endpoint;
        _azureOpenAIProvider.Version = apiVersion;

        if (customModels != null)
        {
            _azureOpenAIProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化智谱AI服务.
    /// </summary>
    public void InitializeZhipu(string apiKey, List<ChatModel> customModels = null)
    {
        _zhipuProvider ??= new ZhipuProvider();
        _zhipuProvider.AccessKey = apiKey;

        if (customModels != null)
        {
            _zhipuProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化零一万物服务.
    /// </summary>
    public void InitializeLingYi(string apiKey, List<ChatModel> customModels = null)
    {
        _lingYiProvider ??= new LingYiProvider();
        _lingYiProvider.AccessKey = apiKey;

        if (customModels != null)
        {
            _lingYiProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化月之暗面服务.
    /// </summary>
    public void InitializeMoonshot(string apiKey, List<ChatModel> customModels = null)
    {
        _moonshotProvider ??= new MoonshotProvider();
        _moonshotProvider.AccessKey = apiKey;

        if (customModels != null)
        {
            _moonshotProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化Gemini服务.
    /// </summary>
    public void InitializeGemini(string apiKey, List<ChatModel> customModels = null)
    {
        _geminiProvider ??= new GeminiProvider();
        _geminiProvider.AccessKey = apiKey;

        if (customModels != null)
        {
            _geminiProvider.CustomModels = customModels;
        }
    }

    /// <summary>
    /// 初始化阿里灵积服务.
    /// </summary>
    /// <param name="apiKey">访问密钥.</param>
    /// <param name="customModels">自定义模型.</param>
    public void InitializeDashScope(string apiKey, List<ChatModel> customModels = null)
    {
        _dashScopeProvider ??= new DashScopeProvider();
        _dashScopeProvider.AccessKey = apiKey;

        if (customModels != null)
        {
            _dashScopeProvider.CustomModels = customModels;
        }

        _dashScopeClient = new Sdcb.DashScope.DashScopeClient(apiKey);
    }

    /// <summary>
    /// 初始化百度千帆服务.
    /// </summary>
    /// <param name="apiKey">密钥.</param>
    /// <param name="secret">密匙.</param>
    /// <param name="customModels">自定义模型列表.</param>
    public void InitializeQianFan(string apiKey, string secret, List<ChatModel> customModels = null)
    {
        _qianFanProvider ??= new QianFanProvider();
        _qianFanProvider.AccessKey = apiKey;
        _qianFanProvider.Secret = secret;

        if (customModels != null)
        {
            _qianFanProvider.CustomModels = customModels;
        }

        _qianFanClient ??= new Sdcb.WenXinQianFan.QianFanClient(apiKey, secret);
    }

    /// <summary>
    /// 初始化Spark服务.
    /// </summary>
    public void InitializeSparkDesk(string apiKey, string secret, string appId, List<ChatModel> customModels = null)
    {
        _sparkDeskProvider ??= new SparkDeskProvider();
        _sparkDeskProvider.AccessKey = apiKey;
        _sparkDeskProvider.Secret = secret;
        _sparkDeskProvider.AppId = appId;

        if (customModels != null)
        {
            _sparkDeskProvider.CustomModels = customModels;
        }
    }
}
