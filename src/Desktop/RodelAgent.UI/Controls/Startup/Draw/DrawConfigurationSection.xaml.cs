// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Constants;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 聊天配置部分.
/// </summary>
public sealed partial class DrawConfigurationSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawConfigurationSection"/> class.
    /// </summary>
    public DrawConfigurationSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as DrawServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        var formControl = newVM.ProviderType switch
        {
            ProviderType.OpenAI => CreateForm<OpenAIDrawConfigSection>(),
            ProviderType.AzureOpenAI => CreateForm<AzureOpenAIDrawConfigSection>(),
            ProviderType.QianFan => CreateForm<QianFanDrawConfigSection>(),
            ProviderType.HunYuan => CreateForm<HunYuanDrawConfigSection>(),
            ProviderType.SparkDesk => CreateForm<SparkDeskDrawConfigSection>(),
            _ => throw new NotImplementedException(),
        };

        FormPresenter.Content = formControl;
    }

    private DependencyObject CreateForm<TControl>()
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is DrawServiceConfigControlBase form)
        {
            form.ViewModel = ViewModel;
        }

        return control as DependencyObject;
    }

    private void OnPredefinedModelsClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
