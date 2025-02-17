// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Converters;

internal sealed class SecondsToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var seconds = (double)value;
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.ToString(@"mm\:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
