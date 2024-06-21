// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelDraw.Console;
using RodelDraw.Core;
using RodelDraw.Interfaces.Client;

ConfigureConsole();

var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Properties";
});

builder.Services.AddSingleton(GetChatProviderFactory);
builder.Services.AddSingleton<IStringResourceToolkit, StringResourceToolkit>();
builder.Services.AddSingleton<IDrawParametersFactory, DrawParametersFactory>();
builder.Services.AddSingleton<IDrawClient, DrawClient>();

builder.Services.AddHostedService<DrawService>();

using var host = builder.Build();
GlobalStatics.SetServiceProvider(host.Services);
await host.RunAsync();
