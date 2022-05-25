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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextBook.Data;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
            
            dgInfoResult.ItemsSource = ConnectionClass.connection.TestResult.ToList();
        }

        private void btnCreateTest_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListTestPage()); }

        private void btnAddContent_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListThemePage()); }

        private void txbSearchSurname_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbSearchSurnameVisible); }

        private void txbSearchSurname_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbSearchSurnameVisible); }

        private void txbSearchName_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbSearchNameVisible); }

        private void txbSearchName_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbSearchNameVisible); }

        private void GotFocusAnimation(TextBlock textblock)
        {
            TranslateTransform transform = new TranslateTransform();
            textblock.RenderTransform = transform;
            DoubleAnimation animationY = new DoubleAnimation(0, -20, TimeSpan.FromSeconds(0.3));
            transform.BeginAnimation(TranslateTransform.YProperty, animationY);
            textblock.FontSize = 14;
        }

        private void LostFocusAnimation(TextBlock textblock)
        {
            TranslateTransform transform = new TranslateTransform();
            textblock.RenderTransform = transform;
            DoubleAnimation animationY = new DoubleAnimation(-20, 0, TimeSpan.FromSeconds(0.3));
            transform.BeginAnimation(TranslateTransform.YProperty, animationY);
            textblock.FontSize = 18;
        }

        private void txbSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txbSearchSurname_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
