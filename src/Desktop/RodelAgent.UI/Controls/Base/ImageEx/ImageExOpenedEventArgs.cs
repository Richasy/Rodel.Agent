// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// A delegate for <see cref="ImageEx"/> opened.
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="e">The event arguments.</param>
public delegate void ImageExOpenedEventHandler(object sender, ImageExOpenedEventArgs e);

/// <summary>
/// Provides data for the <see cref="ImageEx"/> ImageOpened event.
/// </summary>
public class ImageExOpenedEventArgs : EventArgs
{
}
