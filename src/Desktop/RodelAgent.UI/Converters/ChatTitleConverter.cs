﻿// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.Converters;

internal sealed partial class ChatTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string title)
        {
            return string.IsNullOrEmpty(title) ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.RandomChat) : title;
        }

        return ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.RandomChat);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
