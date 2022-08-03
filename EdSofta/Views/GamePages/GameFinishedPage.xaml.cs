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

namespace EdSofta.Views.GamePages
{
    /// <summary>
    /// Interaction logic for GameFinishedPage.xaml
    /// </summary>
    public partial class GameFinishedPage : Page
    {
        private Frame _parentFrame;
        private Window _parentWindow;
        public GameFinishedPage(Frame parentFrame, Window parentWindow, string profession)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _parentWindow = parentWindow;
            DataContext = profession;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new GameHomePage(_parentFrame, _parentWindow);
            _parentFrame.Navigate(page);
        }
    }
}
