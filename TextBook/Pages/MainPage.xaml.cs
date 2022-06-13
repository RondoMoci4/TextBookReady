using System.Windows;
using System.Windows.Controls;
using TextBook.Data;
using TextBook.Pages;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage() { InitializeComponent();}

        private void btnListTest_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListTestPage()); }

        private void btnListTheme_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListThemePage()); }

    }
}
