// Copyright (c) Rodel. All rights reserved.

using System.Drawing;

internal static class Toolkits
{
    public static string ConvertToBase64(string path, bool containPrefix = true)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        using var img = Image.FromFile(path);
        using var ms = new MemoryStream();
        img.Save(ms, img.RawFormat);
        var imageBytes = ms.ToArray();
        var base64String = Convert.ToBase64String(imageBytes);
        return containPrefix
            ? $"data:image/jpeg;base64,{base64String}"
            : base64String;
    }
}
