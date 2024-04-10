using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //Add view
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Close();
        }
    }
}