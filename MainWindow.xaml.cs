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
using Omnis.Scripts.Buttons;

namespace Omnis
{
    // EMPTY LOADED ICONS FOLDER ON CLOSE

    public partial class MainWindow : Window
    {
        AppSearch appSearch = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AppSearch_Directory(object sender, RoutedEventArgs e)
        {
            //UPDATE STATUS ON MAIN WINDOW -> Running

            appSearch.DirectorySearch();

            //UPDATE STATUS ON MAIN WINDOW -> Idle/Ready
        }

        private void UpdateAppStatus()
        {

        }
    }
}