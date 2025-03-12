// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Converters;

internal sealed partial class TokenCountConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var count = System.Convert.ToInt32(value);
        return count < 0 ? "--" : count.ToString("N0");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
