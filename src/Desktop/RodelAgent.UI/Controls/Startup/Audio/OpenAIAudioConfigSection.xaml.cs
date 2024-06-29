﻿// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// OpenAI 配置部分.
/// </summary>
public sealed partial class OpenAIAudioConfigSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAudioConfigSection"/> class.
    /// </summary>
    public OpenAIAudioConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as AudioServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        if (newVM.Config is OpenAIClientConfig config && string.IsNullOrEmpty(config.Endpoint))
        {
            config.Endpoint = ProviderConstants.OpenAIApi;
        }

        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private static OpenAIClientConfig CreateCurrentConfig()
    {
        var config = new OpenAIClientConfig
        {
            Endpoint = ProviderConstants.OpenAIApi,
            OrganizationId = string.Empty,
        };
        return config;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OrganizationBox.Text = ((OpenAIClientConfig)ViewModel.Config).OrganizationId;

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIClientConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
