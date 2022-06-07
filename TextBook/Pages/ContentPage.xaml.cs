using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TextBook.Data;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для ContentPage.xaml
    /// </summary>
    public partial class ContentPage : Page
    {
        string titleTest;
        int idTheme;
        int currentImage = 1;
        int countInImage = 1;
        int maxImage;
        public ContentPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
            var theme = ConnectionClass.connection.Theme.FirstOrDefault(x => x.Title == Properties.Settings.Default.TitleTheme);
            var imageTheme = ConnectionClass.connection.ImageTheme.FirstOrDefault(x => x.IdTheme == theme.idTheme);
            var test = ConnectionClass.connection.TopicTest.FirstOrDefault(x => x.Theme == theme.idTheme);
            idTheme = theme.idTheme;
            string texttheme = Encoding.UTF8.GetString(theme.TextTheme);
            prTitle.Inlines.Add(theme.Title);
            prTheme.Inlines.Add(texttheme);
            if (test == null) { btnTestTheme.Opacity = 0.3; btnTestTheme.IsEnabled = false; }
            else { btnTestTheme.Opacity = 1; btnTestTheme.IsEnabled = true; var title = ConnectionClass.connection.Test.FirstOrDefault(x => x.IdTest == test.Test); titleTest = title.Title; }
            if (imageTheme == null) { stplImage.Visibility = Visibility.Hidden; } 
            else { ListImage(); stplImage.Visibility = Visibility.Visible; }
            
        }

        private void btnTestTheme_Click(object sender, RoutedEventArgs e)
        {
            FrameClass.mainFrame.Navigate(new RegistrationPage());
            Properties.Settings.Default.TitleTest = titleTest;
        }

        private void txbSearchWord_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbVisibleText, txbSearchText); }


        private void txbSearchWord_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbVisibleText); }


        private void txbSearchWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbTheme.Document.ContentStart, rtbTheme.Document.ContentEnd);
            textRange.ClearAllProperties();
            lbl_Status.Content = "";
            string textBoxText = textRange.Text;
            string searchText = txbSearchText.Text;

            if (string.IsNullOrWhiteSpace(textBoxText) || string.IsNullOrWhiteSpace(searchText))
            {
                lbl_Status.Content = "Введите текст для поиска";
            }
            else
            {
                Regex regex = new Regex(searchText);
                int count_MatchFound = Regex.Matches(textBoxText, regex.ToString()).Count;

                for (TextPointer startPointer = rtbTheme.Document.ContentStart;
                            startPointer.CompareTo(rtbTheme.Document.ContentEnd) <= 0;
                                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward))
                {
                    if (startPointer.CompareTo(rtbTheme.Document.ContentEnd) == 0)
                    {
                        break;
                    }
                    string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);
                    int indexOfParseString = parsedString.IndexOf(searchText);

                    if (indexOfParseString >= 0) 
                    {
                        startPointer = startPointer.GetPositionAtOffset(indexOfParseString);

                        if (startPointer != null)
                        {
                            TextPointer nextPointer = startPointer.GetPositionAtOffset(searchText.Length);

                            TextRange searchedTextRange = new TextRange(startPointer, nextPointer);
                            searchedTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.LightGray));
                        }
                    }
                }

                if (count_MatchFound > 0) { lbl_Status.Content = "Всего найдено совпадений: " + count_MatchFound; }
                else { lbl_Status.Content = "Совпадений не найдено!"; }
            }
        }

        private void GotFocusAnimation(TextBlock textblock)
        {
            TranslateTransform transform = new TranslateTransform();
            textblock.RenderTransform = transform;
            DoubleAnimation animationY = new DoubleAnimation(0, -20, TimeSpan.FromSeconds(0.3));
            transform.BeginAnimation(TranslateTransform.YProperty, animationY);
            textblock.FontSize = 14;
        }

        private void LostFocusAnimation(TextBlock textVisible, TextBox textBox)
        {
            if (String.IsNullOrWhiteSpace(textBox.Text))
            {
                TranslateTransform transform = new TranslateTransform();
                textVisible.RenderTransform = transform;
                DoubleAnimation animationY = new DoubleAnimation(-20, 0, TimeSpan.FromSeconds(0.3));
                transform.BeginAnimation(TranslateTransform.YProperty, animationY);
                textVisible.FontSize = 18;
                textBox.Text = null;
            }
        }

        private void btnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage < maxImage)
            {
                currentImage++;
                ListImage();
            }
            else { currentImage = 1; ListImage(); }
        }

        private void lbImage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                imageVisible.Width = 700; imageVisible.Height = 700;
                currentImageTheme.Width = 630; currentImageTheme.Height = 630;
                txbNameImage.FontSize = 24;
            }
            else
            {
                imageVisible.Width = 370; imageVisible.Height = 370;
                currentImageTheme.Width = 310; currentImageTheme.Height = 310;
                txbNameImage.FontSize = 18;
            }
            int id = Convert.ToInt32(txbIdImage.Text);
            var image = ConnectionClass.connection.ImageTheme.FirstOrDefault(x => x.IdImage == id);
            currentImageTheme.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(image.Image);
            txbNameImage.Text = image.Name;
            rtbTheme.Opacity = 0.4;
            imageVisible.Visibility = Visibility.Visible;
        }

        private void ListImage()
        {
            ConnectionClass.connection = new DBTextBookEntities();
            List<ImageTheme> themes = ConnectionClass.connection.ImageTheme.Where(x => x.IdTheme == idTheme).ToList();
            maxImage = themes.Count;
            lbImage.ItemsSource = themes.Skip((currentImage - 1) * countInImage).Take(countInImage).ToList();
        }

        private void btnCloseVisibleImage_Click(object sender, RoutedEventArgs e) { imageVisible.Visibility = Visibility.Hidden; rtbTheme.Opacity = 1; }
    }
}
