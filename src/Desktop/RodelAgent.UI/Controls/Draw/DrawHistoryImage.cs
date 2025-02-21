// Copyright (c) Richasy. All rights reserved.

using Microsoft.Graphics.Canvas;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 书籍封面图片.
/// </summary>
public sealed partial class DrawHistoryImage : ImageExBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawHistoryImage"/> class.
    /// </summary>
    public DrawHistoryImage()
    {
        DecodeWidth = 500;
        DecodeHeight = 500;
        UpdateHolderImage();
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
        {
            var width = e.NewSize.Width;
            var height = e.NewSize.Height;
            var perferHeight = width / (DecodeWidth / DecodeHeight);
            if (Math.Abs(perferHeight - height) > 2)
            {
                Height = perferHeight;
            }
        }
    }

    /// <inheritdoc/>
    protected override void DrawImage(CanvasBitmap canvasBitmap)
    {
        var width = canvasBitmap.Size.Width;
        var height = canvasBitmap.Size.Height;
        var destRect = new Rect(0, 0, DecodeWidth, DecodeHeight);
        var sourceRect = new Rect(0, 0, width, height);
        DrawImage(canvasBitmap, destRect, sourceRect);
    }

    /// <inheritdoc/>
    protected override void UpdateHolderImage()
    {
        var theme = this.Get<IAppToolkit>().GetCurrentTheme().ToString();
        var holderImagePath = Path.Combine(Package.Current.InstalledPath, "Assets", $"coverholder-{theme}.png");
        HolderImage = new Uri($"file://{holderImagePath}");
    }

    private void DrawImage(
        CanvasBitmap canvasBitmap,
        Rect destinationRect,
        Rect sourceRect)
    {
        using var ds = CanvasImageSource!.CreateDrawingSession(ClearColor);
        ds.DrawImage(canvasBitmap, destinationRect, destinationRect);
    }
}
