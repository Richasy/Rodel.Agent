// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Converters;

internal sealed partial class GridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var v = (double)value;
        return v <= 0 ? GridLength.Auto : new GridLength(v);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
