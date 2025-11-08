using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace DocumentationGenerator.Helpers
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null && value.Equals(parameter);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value == null || parameter == null) { return BindingOperations.DoNothing; }
            return (bool)value ? parameter : BindingOperations.DoNothing;
        }
    }
}