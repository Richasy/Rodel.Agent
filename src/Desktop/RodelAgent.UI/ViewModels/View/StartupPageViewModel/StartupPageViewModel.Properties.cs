// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.View;

public sealed partial class StartupPageViewModel
{
    [ObservableProperty]
    public partial string Version { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; }

    [ObservableProperty]
    public partial bool IsMigrating { get; set; }
}
