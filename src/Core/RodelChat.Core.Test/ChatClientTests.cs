// Copyright (c) Rodel. All rights reserved.

using Moq;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Constants;

namespace RodelChat.Core.Test;

/// <summary>
/// 聊天客户端测试.
/// </summary>
public class ChatClientTests
{
    private readonly ChatClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClientTests"/> class.
    /// </summary>
    public ChatClientTests()
    {
        var mockFactory = new Mock<IChatProviderFactory>();
        mockFactory.Setup(x => x.GetOrCreateProvider(It.IsAny<ProviderType>())).Returns(Mock.Of<IProvider>());
        _client = new ChatClient(mockFactory.Object);
    }

    /// <summary>
    /// 测试创建会话.
    /// </summary>
    [Fact]
    public void Test_CreateSession()
    {
        var session = _client.CreateSession(ProviderType.OpenAI);
        Assert.NotNull(session);
    }

    /// <summary>
    /// 测试从 DLL 中加载插件.
    /// </summary>
    /// <param name="dllPath">DLL 路径.</param>
    [Theory]
    [InlineData("Microsoft.SemanticKernel.Plugins.Core.dll")]
    public void Test_GetPluginInstanceFromDll(string dllPath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestPlugins", dllPath);
        var plugins = _client.RetrievePluginsFromDll(path);
        Assert.NotEmpty(plugins);
    }

    /// <summary>
    /// 测试导入插件.
    /// </summary>
    /// <param name="dllPath">DLL 路径.</param>
    [Theory]
    [InlineData("Microsoft.SemanticKernel.Plugins.Core.dll")]
    public void Test_ImportPlugins(string dllPath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestPlugins", dllPath);
        var plugins = _client.RetrievePluginsFromDll(path);
        _client.InjectPluginsToKernel(plugins);

        var kernelPlugins = _client.GetKernelPlugins();
        Assert.NotEmpty(kernelPlugins);
    }
}
