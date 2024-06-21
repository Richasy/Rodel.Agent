// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAudio.Console;
using RodelAudio.Core;
using RodelAudio.Interfaces.Client;

ConfigureConsole();

var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Properties";
});

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton(GetAudioProviderFactory);
builder.Services.AddSingleton<IStringResourceToolkit, StringResourceToolkit>();
builder.Services.AddSingleton<IAudioParametersFactory, AudioParametersFactory>();
builder.Services.AddSingleton<IAudioClient, AudioClient>();

builder.Services.AddHostedService<AudioService>();

using var host = builder.Build();
GlobalStatics.SetServiceProvider(host.Services);
await host.RunAsync();
