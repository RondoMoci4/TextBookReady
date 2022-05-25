using System;
using System.Windows;
using System.Windows.Input;
using TextBook.Data;
using TextBook.Pages;

namespace TextBook
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            FrameClass.mainFrame = frmMain;
            frmMain.Navigate(new MainPage());
            var start = TimeSpan.Parse("8:30");
            var end = TimeSpan.Parse("20:00");
            var now = DateTime.Now.TimeOfDay;

            bool isInRange = start <= now && now <= end;
            if (isInRange == true)
            {
                var uri = new Uri(@"/StyleElement/LightTheme.xaml", UriKind.Relative); // Локальная переменная с ссылкой на словарь ресурсов
                ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                theme = false;
            }
            else
            {
                var uri = new Uri(@"/StyleElement/DarkTheme.xaml", UriKind.Relative); // Локальная переменная с ссылкой на словарь ресурсов
                ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                theme = true;
            }
        }

        private void borderMain_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); } // Перетаскивание окна мышью
        private void btnCollapse_Click(object sender, RoutedEventArgs e) { Application.Current.MainWindow.WindowState = WindowState.Minimized; } // Кнопка для сворачивания окна
        private void btnClose_Click(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
        private void btnExpand_Click(object sender, RoutedEventArgs e) // Кнопка для развертывания на весь экран либо в окно 
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal) { Application.Current.MainWindow.WindowState = WindowState.Maximized; }
            else { Application.Current.MainWindow.WindowState = WindowState.Normal; }
        }

        bool theme = false;

        private void btnThemeLightDark_Click(object sender, RoutedEventArgs e) // Кнопка загрузки определенного стиля элементов Светой/Темной темы
        {
            if (theme == false)
            {
                var uri = new Uri(@"/StyleElement/DarkTheme.xaml", UriKind.Relative); // Локальная переменная с ссылкой на словарь ресурсов
                ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                theme = true;
            }
            else
            {
                var uri = new Uri(@"/StyleElement/LightTheme.xaml", UriKind.Relative); // Локальная переменная с ссылкой на словарь ресурсов
                ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                theme = false;
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e) // Кнопка загрузки страницы для авторизации администратора
        {
            Properties.Settings.Default.AdminStatus = false;
            if (Properties.Settings.Default.AdminStatus == false) { FrameClass.mainFrame.Navigate(new EnterPage()); }
            else { FrameClass.mainFrame.Navigate(new AdminPage()); Properties.Settings.Default.IdExistingTest = 0; Properties.Settings.Default.IdExistingTheme = 0; }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            FrameClass.mainFrame.Navigate(new AdminPage());
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e) { FrameClass.mainFrame.Navigate(new MainPage()); Properties.Settings.Default.AdminStatus = false;
            Properties.Settings.Default.IdExistingTest = 0; Properties.Settings.Default.IdExistingTheme = 0;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try { FrameClass.mainFrame.GoBack(); }
            catch { MessageBox.Show("Вы уже на главной странице"); }
        }
    }
}
