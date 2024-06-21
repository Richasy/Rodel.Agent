// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelChat.Console;
using RodelChat.Core;
using RodelChat.Interfaces.Client;

ConfigureConsole();

var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Properties";
});

builder.Services.AddSingleton(GetChatProviderFactory);
builder.Services.AddSingleton<IStringResourceToolkit, StringResourceToolkit>();
builder.Services.AddSingleton<IChatParametersFactory, ChatParametersFactory>();
builder.Services.AddSingleton<IChatClient, ChatClient>();

builder.Services.AddHostedService<ChatService>();

using var host = builder.Build();
GlobalStatics.SetServiceProvider(host.Services);
await host.RunAsync();
