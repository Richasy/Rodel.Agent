// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class GroupOptionsPanel : LayoutUserControlBase
{
    public GroupOptionsPanel() => InitializeComponent();

    public void ReloadOptionsUI(int maxRounds)
        => MaxRoundsSlider.Value = maxRounds;

    public int GetMaxRounds()
        => Convert.ToInt32(MaxRoundsSlider.Value);
}
