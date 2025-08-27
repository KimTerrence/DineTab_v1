using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace DineTab_v1.Converters
{


    //display active or inactive based on boolean value to the switch in ui
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isActive = (bool)value;
            return isActive ? "Active" : "Inactive";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            return status == "Active";
        }
    }
}
