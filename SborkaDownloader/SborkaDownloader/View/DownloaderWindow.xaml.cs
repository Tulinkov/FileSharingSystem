using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SborkaDownloader.View
{
    /// <summary>
    /// Interaction logic for DowloaderWindow.xaml
    /// </summary>
    public partial class DowloaderWindow : Window
    {
        public DowloaderWindow()
        {
            InitializeComponent();
        }
    }

#region IValueConverters
    public class LongToFilesSizeConverter : IValueConverter
    {

        // Define the Convert method to change a file size in bytes to 
        // a human-readable file size
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long bytes = (long)value;

            string[] suf = { "B", "KB", "MB", "GB" };
            int place = System.Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return String.Format("{0:0.##} {1}", num, suf[place]);
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#endregion
}
