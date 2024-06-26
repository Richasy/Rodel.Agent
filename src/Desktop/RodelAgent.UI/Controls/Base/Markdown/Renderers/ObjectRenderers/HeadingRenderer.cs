﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using RodelAgent.UI.Controls.Markdown.TextElements;

namespace RodelAgent.UI.Controls.Markdown.Renderers.ObjectRenderers;

internal class HeadingRenderer : WinUIObjectRenderer<HeadingBlock>
{
    protected override void Write(WinUIRenderer renderer, HeadingBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var paragraph = new MyHeading(obj, renderer.Config);
        renderer.Push(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.Pop();
    }
}
