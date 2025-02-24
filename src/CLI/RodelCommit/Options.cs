// Copyright (c) Richasy. All rights reserved.

using CommandLine;

namespace RodelCommit;

internal sealed class Options
{
    [Option('c', "config", Required = false, HelpText = "Open configuration file.")]
    public bool OpenConfig { get; set; }
}
