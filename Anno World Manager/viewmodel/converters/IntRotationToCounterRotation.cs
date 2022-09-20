using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Anno_World_Manager.viewmodel.converters
{
    internal class IntRotationToCounterRotation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int currentrotation = (int)value;
                int counterrotation = currentrotation * -1;
                return counterrotation;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int currentrotation = (int)value;
                int counterrotation = currentrotation * -1;
                return counterrotation;
            }

            return value;
        }
    }
}
