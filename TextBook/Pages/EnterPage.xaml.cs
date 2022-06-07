using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TextBook.Data;

namespace TextBook.Pages
{
    public partial class EnterPage : Page
    {
        public EnterPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 0;
            anim.To = 50;
            anim.Duration = TimeSpan.FromSeconds(1);
            brdInfo.BeginAnimation(HeightProperty, anim);
            if (String.IsNullOrWhiteSpace(txbLogin.Text) && String.IsNullOrWhiteSpace(psbPassword.Password))
            { txbInfoEnter.Text = "Введите логин и пароль"; }
            else if (String.IsNullOrWhiteSpace(psbPassword.Password))
            { txbInfoEnter.Text = "Введите пароль"; }
            else if (String.IsNullOrWhiteSpace(txbLogin.Text))
            { txbInfoEnter.Text = "Введите логин"; }
            else
            {
                var enter = ConnectionClass.connection.Autorization.FirstOrDefault(x => x.Password == 
                psbPassword.Password && x.Login == txbLogin.Text);
                if (enter == null)
                { txbInfoEnter.Text = "Неверный логин или пароль"; }
                else
                {
                    Properties.Settings.Default.AdminStatus = true;
                    Properties.Settings.Default.NameAdmin = enter.Name;
                    Properties.Settings.Default.SurnameAdmin = enter.Surname;
                    Properties.Settings.Default.AdminId = enter.IdUser;
                    FrameClass.mainFrame.Navigate(new AdminPage());
                }
            }
        }

        private void txbLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLogin.Visibility = Visibility.Hidden;
            if (txbLogin.Text != "")
            {
                txtLogin.Visibility = Visibility.Hidden;
            }
        }

        private void txbLogin_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbLogin.Text == "")
            {
                txtLogin.Visibility = Visibility.Visible;
            }
        }

        private void psbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Visibility = Visibility.Hidden;
            if (psbPassword.Password != "")
            {
                txtPassword.Visibility = Visibility.Hidden;
            }
        }

        private void psbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (psbPassword.Password == "")
            {
                txtPassword.Visibility = Visibility.Visible;
            }
        }

        private void txbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Visibility = Visibility.Hidden;
            if (txbPassword.Text != "")
            {
                txtPassword.Visibility = Visibility.Hidden;
            }
        }

        private void txbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbPassword.Text == "")
            {
                txtPassword.Visibility = Visibility.Visible;
            }
        }
        private void chkbPassword_Checked(object sender, RoutedEventArgs e)
        {
            chkbPassword.Content = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye,
                Width = 25,
                Height = 25,
            };
            chkbPassword.ToolTip = new ToolTip
            {
                Content = "Скрыть пароль",
                Style = (Style)FindResource("tltpElement")
            };
            txbPassword.Text = psbPassword.Password;
            psbPassword.Visibility = Visibility.Hidden;
            txbPassword.Visibility = Visibility.Visible;
        }

        private void chkbPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            chkbPassword.Content = new MaterialDesignThemes.Wpf.PackIcon
            {
                Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff,
                Width = 25,
                Height = 25,
            };
            chkbPassword.ToolTip = new ToolTip
            {
                Content = "Показать пароль",
                Style = (Style)FindResource("tltpElement")
            };
            psbPassword.Password = txbPassword.Text;
            txbPassword.Visibility = Visibility.Hidden;
            psbPassword.Visibility = Visibility.Visible;
        }

    }
}
