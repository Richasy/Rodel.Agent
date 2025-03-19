// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Core.Mcp.Client;
using Richasy.AgentKernel.Core.Mcp.Configuration;
using Richasy.AgentKernel.Core.Mcp.Protocol.Transport;
using Richasy.AgentKernel.Core.Mcp.Shared;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class McpServerItemViewModel : ViewModelBase<McpServerDefinition>
{
    private readonly string _sourceId;
    private IMcpClient? _client;
    private CancellationTokenSource? _runCts;

    public McpServerItemViewModel(string id, McpServerDefinition data) : base(data)
    {
        _sourceId = id;
        Name = id;
        State = McpServerState.Stopped;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id => _sourceId;

    /// <summary>
    /// 服务名称.
    /// </summary>
    [ObservableProperty]
    public partial string Name { get; set; }

    /// <summary>
    /// 服务器状态.
    /// </summary>
    [ObservableProperty]
    public partial McpServerState State { get; set; }

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
    public async Task TryConnectAsync()
    {
        // 正在连接或者运行时，不进行操作.
        if (_client != null && (State == McpServerState.Running || State == McpServerState.Connecting))
        {
            return;
        }

        if (_client != null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        _runCts = new CancellationTokenSource();
        State = McpServerState.Connecting;

        var clientOptions = new McpClientOptions
        {
            ClientInfo = new() { Name = "RodelAgent", Version = this.Get<IAppToolkit>().GetPackageVersion() },
        };

        var serverConfig = new McpServerConfig
        {
            Id = _sourceId,
            Name = _sourceId,
            TransportType = TransportTypes.StdIo,
            Location = Data.WorkingDirectory,
            TransportOptions = new()
            {
                ["command"] = Data.Command!,
            }
        };

        if (Data.Arguments?.Length > 0)
        {
            serverConfig.TransportOptions["arguments"] = string.Join(" ", Data.Arguments);
        }

        if (Data.Environments?.Count > 0)
        {
            foreach (var item in Data.Environments)
            {
                serverConfig.TransportOptions[$"env:{item.Key}"] = item.Value;
            }
        }

        var loggerFactory = this.Get<ILoggerFactory>();
        try
        {
            _client = await McpClientFactory.CreateAsync(serverConfig, clientOptions, default, loggerFactory, _runCts.Token);
            Name = _client.ServerInfo?.Name ?? _sourceId;
            await _client.PingAsync(_runCts.Token);
            var functions = await _client.GetAIFunctionsAsync(_runCts.Token);
            IsFunctionEmpty = functions == null || functions.Count == 0;
            FunctionCount = functions?.Count ?? 0;
            Functions.Clear();
            functions?.ToList().ForEach(item => Functions.Add(new AIFunctionItemViewModel(item)));

            State = McpServerState.Running;
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
}
