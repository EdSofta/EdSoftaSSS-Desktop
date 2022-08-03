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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for LicensesPage.xaml
    /// </summary>
    public partial class LicensesPage : Page
    {
        public LicensesPage()
        {
            InitializeComponent();
            DataContext = new LicensePageViewModel();
        }

        private void LicenseList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
