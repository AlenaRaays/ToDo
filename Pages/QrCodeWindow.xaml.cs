using System;
using System.Collections.Generic;
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

namespace ToDo.Pages
{
    /// <summary>
    /// Логика взаимодействия для QrCodeWindow.xaml
    /// </summary>
    public partial class QrCodeWindow : Window
    {
        public QrCodeWindow(ImageSource qrSource)
        {
            InitializeComponent();
            QrImage.Source = qrSource;
        }
    }
}
