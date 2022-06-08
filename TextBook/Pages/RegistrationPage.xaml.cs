using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TextBook.Data;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 50;
            anim.Duration = TimeSpan.FromSeconds(1);
            brdInfo.BeginAnimation(HeightProperty, anim);
            if (txbName.Text == "Введите имя" && txbSurname.Text == "Введите фамилию")
            {
                txbInfoRegistration.Text = "Введите имя и фамилию";
            }
            else if (txbSurname.Text == "Введите фамилию")
            {
                txbInfoRegistration.Text = "Введите фамилию";
            }
            else if (txbName.Text == "Введите имя")
            {
                txbInfoRegistration.Text = "Введите имя";
            }
            else
            {
                Properties.Settings.Default.NameStudent = txbName.Text;
                Properties.Settings.Default.SurnameStudent = txbSurname.Text;
                FrameClass.mainFrame.Navigate(new TestPage());
            } 
        }

        private void txbSurname_GotFocus(object sender, RoutedEventArgs e)
        {
            if(txbSurname.Text == "Введите фамилию")
                txbSurname.Clear();
        }

        private void txbSurname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbSurname.Text == "" || txbSurname.Text == " ")
                txbSurname.Text = "Введите фамилию";
        }

        private void txbName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbName.Text == "Введите имя")
                txbName.Clear();
        }

        private void txbName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbName.Text == "" || txbName.Text == " ")
                txbName.Text = "Введите имя";
        }

        private void btnBackToList_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.GoBack(); }
    }
}
