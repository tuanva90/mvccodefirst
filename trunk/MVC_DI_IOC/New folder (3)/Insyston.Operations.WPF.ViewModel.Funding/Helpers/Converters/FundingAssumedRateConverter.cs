using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Insyston.Operations.WPF.ViewModels.Funding.Helpers.Converters
{
    public class FundingAssumedRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != "")
            {
                return value;
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != "")
            {
                return value;
            }
            return "0";
        }
    }
}
