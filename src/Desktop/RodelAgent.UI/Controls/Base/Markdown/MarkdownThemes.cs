﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Microsoft.UI.Xaml.Media;
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Microsoft.UI.Text.FontWeights;

namespace RodelAgent.UI.Controls.Markdown;

public sealed class MarkdownThemes : DependencyObject
{
    internal static MarkdownThemes Default { get; } = new();

    public Thickness Padding { get; set; } = new(8);

    public Thickness InternalMargin { get; set; } = new(4);

    public CornerRadius CornerRadius { get; set; } = new(4);

    public double H1FontSize { get; set; } = 22;

    public double H2FontSize { get; set; } = 20;

    public double H3FontSize { get; set; } = 18;

    public double H4FontSize { get; set; } = 16;

    public double H5FontSize { get; set; } = 14;

    public double H6FontSize { get; set; } = 12;

    public Brush HeadingForeground { get; set; } = Extensions.GetAccentColorBrush();

    public FontWeight H1FontWeight { get; set; } = FontWeights.Black;

    public FontWeight H2FontWeight { get; set; } = FontWeights.Bold;

    public FontWeight H3FontWeight { get; set; } = FontWeights.Bold;

    public FontWeight H4FontWeight { get; set; } = FontWeights.Normal;

    public FontWeight H5FontWeight { get; set; } = FontWeights.Normal;

    public FontWeight H6FontWeight { get; set; } = FontWeights.Normal;

    public Brush InlineCodeBackground { get; set; } = (Brush)Application.Current.Resources["ExpanderHeaderBackground"];

    public Brush InlineCodeBorderBrush { get; set; } = (Brush)Application.Current.Resources["ControlStrokeColorDefaultBrush"];

    public Brush CodeBlockForeground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

    public Brush CodeBlockBackground { get; set; } = (Brush)Application.Current.Resources["SolidBackgroundFillColorBaseAltBrush"];

    public Brush CodeHeaderBackground { get; set; } = (Brush)Application.Current.Resources["ControlSolidFillColorDefaultBrush"];

    public Thickness InlineCodeBorderThickness { get; set; } = new(1);

    public CornerRadius InlineCodeCornerRadius { get; set; } = new(2);

    public Thickness InlineCodePadding { get; set; } = new(2);

    public double InlineCodeFontSize { get; set; } = 12;

    public FontWeight InlineCodeFontWeight { get; set; } = FontWeights.Normal;
}
