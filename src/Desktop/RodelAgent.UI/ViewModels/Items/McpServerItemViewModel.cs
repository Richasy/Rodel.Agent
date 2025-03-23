// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Core.Mcp.Client;
using Richasy.AgentKernel.Core.Mcp.Protocol.Transport;
using Richasy.AgentKernel.Core.Mcp.Shared;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.ViewModels.View;
using System.Text.Json.Serialization;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class McpServerItemViewModel(string id, McpAgentConfig data, Func<Task> saveFunc) : ViewModelBase<McpAgentConfig>(data)
{
    private CancellationTokenSource? _runCts;

    /// <summary>
    /// MCP 服务列表.
    /// </summary>
    public static Dictionary<string, IMcpClient?> McpServers { get; } = [];

    /// <summary>
    /// 标识符.
    /// </summary>
    [ObservableProperty]
    public partial string Id { get; set; } = id;

    /// <summary>
    /// 是否启用.
    /// </summary>
    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = data.IsEnabled ?? true;

    /// <summary>
    /// 服务器状态.
    /// </summary>
    [ObservableProperty]
    public partial McpServerState State { get; set; } = McpServerState.Stopped;

    /// <summary>
    /// 初始化时的错误信息.
    /// </summary>
    [ObservableProperty]
    public partial string? InitErrorMessage { get; set; }

    /// <summary>
    /// 函数列表是否为空.
    /// </summary>
    [ObservableProperty]
    public partial bool IsFunctionEmpty { get; set; }

    /// <summary>
    /// 函数数量.
    /// </summary>
    [ObservableProperty]
    public partial int FunctionCount { get; set; }

    /// <summary>
    /// 是否被选中.
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    /// <summary>
    /// 函数列表.
    /// </summary>
    public ObservableCollection<AIFunctionItemViewModel> Functions { get; } = [];

    [RelayCommand]
    private async Task TryConnectAsync()
    {
        var client = GetClient();

        // 正在连接或者运行时，不进行操作.
        if (client != null && (State == McpServerState.Running || State == McpServerState.Connecting))
        {
            return;
        }

        if (client != null)
        {
            await client.DisposeAsync();
            McpServers.Remove(Id);
        }

        _runCts = new CancellationTokenSource();
        State = McpServerState.Connecting;

        var clientOptions = new McpClientOptions
        {
            ClientInfo = new() { Name = "RodelAgent", Version = this.Get<IAppToolkit>().GetPackageVersion() },
        };

        var loggerFactory = this.Get<ILoggerFactory>();
        try
        {
            client = await McpClientFactory.CreateAsync(Id, TransportTypes.StdIo, Data, clientOptions, default, loggerFactory, _runCts.Token);
            await client.PingAsync(_runCts.Token);
            var timeOutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var functions = await client.ListToolsAsync(cancellationToken: timeOutCts.Token).ToListAsync(timeOutCts.Token);
            IsFunctionEmpty = functions == null || functions.Count == 0;
            FunctionCount = functions?.Count ?? 0;
            Functions.Clear();
            functions?.ToList().ForEach(item => Functions.Add(new AIFunctionItemViewModel(item)));

            State = McpServerState.Running;
            McpServers[Id] = client;
        }
        catch (Exception ex)
        {
            State = McpServerState.Error;
            FunctionCount = 0;
            Functions.Clear();
            IsFunctionEmpty = true;
            InitErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private Task SaveAsync()
        => saveFunc();

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var client = GetClient();
        if (client != null)
        {
            await client.DisposeAsync();
            McpServers.Remove(Id);
        }

        var pageVM = this.Get<ChatPageViewModel>();
        pageVM.Servers.Remove(this);
        await saveFunc();
    }

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var dialog = new McpConfigDialog(this);
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        var client = GetClient();
        if (_runCts != null)
        {
            await _runCts.CancelAsync();
            _runCts.Dispose();
            _runCts = null;
        }

        if (client != null)
        {
            await client.DisposeAsync();
            McpServers.Remove(Id);
        }

        State = McpServerState.Stopped;
    }

    private IMcpClient? GetClient()
        => McpServers.ContainsKey(Id) ? McpServers.First(p => p.Key == Id).Value : default;
}

public sealed class McpAgentConfig : McpServerDefinition
{
    /// <summary>
    /// 是否启用.
    /// </summary>
    [JsonPropertyName("isEnabled")]
    public bool? IsEnabled { get; set; }
}

public sealed partial class McpAgentConfigCollection : Dictionary<string, McpAgentConfig>;