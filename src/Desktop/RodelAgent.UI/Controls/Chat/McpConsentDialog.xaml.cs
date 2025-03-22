// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Documents;
using Richasy.AgentKernel.Core.Mcp.Protocol.Messages;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class McpConsentDialog : AppDialog
{
    public McpConsentDialog(string clientId, string method, JsonRpcRequest request)
    {
        InitializeComponent();
        var callHeader = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.McpRequestHeaderTemplate), clientId, method);
        foreach (var item in callHeader.Split("<!"))
        {
            if (item.Contains("!>", StringComparison.Ordinal))
            {
                var split = item.Split("!>", StringSplitOptions.RemoveEmptyEntries);
                var run = new Run { Text = split[0], Foreground = this.Get<IResourceToolkit>().GetThemeBrush("AccentTextFillColorPrimaryBrush") };
                CallHeader.Inlines.Add(run);
                run = new Run { Text = split[1] };
                CallHeader.Inlines.Add(run);
            }
            else
            {
                var run = new Run { Text = item };
                CallHeader.Inlines.Add(run);
            }
        }

        if (request.Params != null)
        {
            CallDetail.Text = request.Params.ToString();
            CallDetailContainer.Visibility = Visibility.Visible;
        }
        else
        {
            CallDetailContainer.Visibility = Visibility.Collapsed;
        }
    }
}
