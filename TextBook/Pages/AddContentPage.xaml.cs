using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Xceed.Words.NET;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddContentPage.xaml
    /// </summary>
    public partial class AddContentPage : Page
    {
        bool Existing;
        int id;
        int currentImage = 1;
        int countInImage = 1;
        int maxImage;
        public AddContentPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
            var existing = ConnectionClass.connection.Theme.FirstOrDefault(x => x.idTheme == Properties.Settings.Default.IdExistingTheme);
            if (existing == null)
            {
                prgTitleTheme.Inlines.Add("Введите наименование темы");
                btnListImage.Opacity = 0.3;
                btnDeleteTheme.Opacity = 0.3;
                btnSaveTheme.Opacity = 0.3;
                btnBindTest.Opacity = 0.3;
                btnListTest.Opacity = 0.3;
                Existing = false;
            }
            else
            {
                id = Properties.Settings.Default.IdExistingTheme;
                btnDeleteTheme.Opacity = 1; btnDeleteTheme.IsEnabled = true;
                btnSaveTheme.Opacity = 1; btnSaveTheme.IsEnabled = true;
                btnListTest.Opacity = 1; btnListTest.IsEnabled = true;
                btnListImage.IsEnabled = true; btnListImage.Opacity = 1;
                btnBindTest.IsEnabled = false; btnBindTest.Opacity = 0.3;
                Existing = true;
                LoadTheme();
            }
        }
        string path;
        private void btnLoadTheme_Click(object sender, RoutedEventArgs e)
        {

            TextRange doc = new TextRange(prgTextTheme.ContentStart, prgTextTheme.ContentEnd)
            {
                Text = ""
            };
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "RichText Files (*.rtf)|*.rtf|Word FIles (*.docx*)|*.docx*|Text File (*.txt)|*.txt"
            };

            if (ofd.ShowDialog() == true)
            {
                
                if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".docx")
                {
                    string filename = ofd.FileName;
                    doc.Text = filename;

                    var document = DocX.Load(filename);

                    string contents = document.Text;
                    doc.Text = contents;
                }
                else
                {
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                            doc.Load(fs, DataFormats.Rtf);
                        else if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                            doc.Load(fs, DataFormats.Text);
                    }
                }
                path = doc.Text;
                btnSaveTheme.IsEnabled = true; btnSaveTheme.Opacity = 1;
            }
        }

        private void btnSaveTheme_Click(object sender, RoutedEventArgs e)
        {
            TextRange title = new TextRange(prgTitleTheme.ContentStart, prgTitleTheme.ContentEnd);
            TextRange description = new TextRange(prgDescriptionTheme.ContentStart, prgDescriptionTheme.ContentEnd);
            if (Existing == false)
            {
                if (title.Text != "Введите наименование темы")
                {
                    byte[] data = Encoding.UTF8.GetBytes(path);
                    Theme theme = new Theme()
                    {
                        Title = title.Text,
                        TextTheme = data,
                        Description = description.Text,
                    };
                    ConnectionClass.connection.Theme.Add(theme);
                    ConnectionClass.connection.SaveChanges();
                    var idTheme = ConnectionClass.connection.Theme.FirstOrDefault(x => x.Title == title.Text);
                    id = idTheme.idTheme;
                    btnDeleteTheme.IsEnabled = true; btnDeleteTheme.Opacity = 1;
                    btnListImage.IsEnabled = true; btnListImage.Opacity = 1;
                    btnSaveTheme.Opacity = 0.3; btnSaveTheme.IsEnabled = false;
                    btnListTest.Opacity = 1;btnListTest.IsEnabled = true;
                }
                else { MessageBox.Show("Введите наименование темы"); }
            }
            else
            {
                if (title.Text != "Введите наименование темы")
                {
                    TextRange text = new TextRange(prgTextTheme.ContentStart,prgTextTheme.ContentEnd);
                    byte[] data = Encoding.UTF8.GetBytes(text.Text);
                    var theme = ConnectionClass.connection.Theme.FirstOrDefault(x => x.idTheme == Properties.Settings.Default.IdExistingTheme);
                    theme.Title = title.Text;
                    theme.TextTheme = data;
                    theme.Description = description.Text;
                    ConnectionClass.connection.SaveChanges();
                    btnDeleteTheme.IsEnabled = true;
                    btnDeleteTheme.Opacity = 1;
                    btnSaveTheme.Opacity = 0.3; btnSaveTheme.IsEnabled = false;
                }
                else { MessageBox.Show("Введите наименование темы"); }
            }
           
        }

        private void btnDeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            var topicTest = ConnectionClass.connection.TopicTest.Where(x => x.Theme == id).ToList() ;
            var theme = ConnectionClass.connection.Theme.FirstOrDefault(x=> x.idTheme == id);
            var image = ConnectionClass.connection.ImageTheme.Where(x => x.IdTheme == id).ToList();
            if (topicTest == null)
            {
                ConnectionClass.connection.Theme.Remove(theme);
            }
            else
            {
                ConnectionClass.connection.ImageTheme.RemoveRange(image);
                ConnectionClass.connection.TopicTest.RemoveRange(topicTest);
                ConnectionClass.connection.Theme.Remove(theme);
            }
            ConnectionClass.connection.SaveChanges();
            btnSaveTheme.Opacity = 0.3; btnSaveTheme.IsEnabled = false;
            btnDeleteTheme.Opacity = 0.3; btnDeleteTheme.IsEnabled = false;
            FrameClass.mainFrame.Navigate(new ListThemePage());
        }
        private void btnBindTest_Click(object sender, RoutedEventArgs e)
        {
           btnBindTest.IsEnabled = false; btnBindTest.Opacity = 0.3;
            var existing = ConnectionClass.connection.TopicTest.FirstOrDefault(x=> x.Theme == id);
            if (existing == null)
            {
                TopicTest topic = new TopicTest()
                {
                    Theme = id,
                    Test = Properties.Settings.Default.IdTestLink,
                };
                ConnectionClass.connection.TopicTest.Add(topic);
            }
            else
            {
                existing.Theme = id;
                existing.Test = Properties.Settings.Default.IdTestLink;
            }
            ConnectionClass.connection.SaveChanges();
            btnBindTest.IsEnabled = false; btnBindTest.Opacity = 0.3;
        }

        private void LoadTheme()
        {
            var theme = ConnectionClass.connection.Theme.FirstOrDefault(x => x.Title == Properties.Settings.Default.TitleTheme);
            if (theme.Description != null) { prgDescriptionTheme.Inlines.Add(theme.Description);}
            string texttheme = Encoding.UTF8.GetString(theme.TextTheme);
            prgTitleTheme.Inlines.Add(theme.Title);
            prgTextTheme.Inlines.Add(texttheme);
        }

        public void ReadDocx(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var flowDocumentConverter = new DocxToFlowDocunetConvert(stream);
                flowDocumentConverter.Read();
                this.rtbTheme.Document = flowDocumentConverter.Document;
                this.Title = System.IO.Path.GetFileName(path);
            }
        }

        private void prgTitleTheme_GotFocus(object sender, RoutedEventArgs e)
        {
            TextRange title = new TextRange(prgTitleTheme.ContentStart, prgTitleTheme.ContentEnd);
            if (title.Text == "Введите наименование темы" || String.IsNullOrWhiteSpace(title.Text))
            {
                prgTitleTheme.Inlines.Clear();
            }
        }

        private void prgTitleTheme_LostFocus(object sender, RoutedEventArgs e)
        {
            TextRange title = new TextRange(prgTitleTheme.ContentStart, prgTitleTheme.ContentEnd);
            if (title.Text == "Введите наименование темы" || String.IsNullOrWhiteSpace(title.Text))
            {
                prgTitleTheme.Inlines.Clear();
                prgTitleTheme.Inlines.Add("Введите наименование темы");
            }
        }

        private void lbListTest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.IdTestLink = Convert.ToInt32(txbTestListBox.Text);
            btnBindTest.Opacity = 1; btnBindTest.IsEnabled = true;
        }

        private void btnListTest_Click(object sender, RoutedEventArgs e)
        {
            lbListTest.ItemsSource = ConnectionClass.connection.Test.ToList();
            if (brdListTest.Width == 450)
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 450,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdListTest.BeginAnimation(WidthProperty, anim);
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 0,
                    To = 450,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdListTest.BeginAnimation(WidthProperty, anim);
            }
        }

        private void btnListImage_Click(object sender, RoutedEventArgs e)
        {
            if (brdListImage.Width == 450)
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 450,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdListImage.BeginAnimation(WidthProperty, anim);
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 0,
                    To = 450,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdListImage.BeginAnimation(WidthProperty, anim);
                btnAddImage.IsEnabled = true;
                ListImage();
            }
        }

        private void txbTitleImage_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbVisibleImage,txbTitleImage); }

        private void txbTitleImage_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbVisibleImage); }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG"
            };
            if (txbIdImage.Text == "")
            {
                if (!String.IsNullOrWhiteSpace(txbTitleImage.Text))
                {
                    if (open.ShowDialog() == true)
                    {
                        byte[] imageData = File.ReadAllBytes(open.FileName);
                        ImageTheme theme = new ImageTheme()
                        {
                            IdTheme = id,
                            Image = imageData,
                            Name = txbTitleImage.Text
                        };
                        ConnectionClass.connection.ImageTheme.Add(theme);
                        ConnectionClass.connection.SaveChanges();
                        txbTitleImage.Clear();
                    }
                }
                else { MessageBox.Show("Введите название изображения"); }
            }
            else
            {
                int id = Convert.ToInt32(txbIdImage.Text);
                var idImage = ConnectionClass.connection.ImageTheme.FirstOrDefault(x => x.IdImage == id);
                if (!String.IsNullOrWhiteSpace(txbTitleImage.Text))
                {
                    MessageBox.Show(Properties.Settings.Default.IdExistingTheme.ToString());
                    if (open.ShowDialog() == true)
                    {
                        byte[] imageData = File.ReadAllBytes(open.FileName);
                        idImage.Image = imageData;
                        idImage.Name = txbTitleImage.Text;
                        ConnectionClass.connection.SaveChanges();
                        txbTitleImage.Clear();
                    }
                }
                else { MessageBox.Show("Введите название изображения"); }
            }
            ListImage(); 
        }

        private void ListImage()
        {
            ConnectionClass.connection = new DBTextBookEntities();
            List<ImageTheme> themes = ConnectionClass.connection.ImageTheme.Where(x => x.IdTheme == id).ToList();
            maxImage = themes.Count;
            lbImage.ItemsSource = themes.Skip((currentImage - 1) * countInImage).Take(countInImage).ToList();
        }

        private void btnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(txbIdImage.Text);
            var image = ConnectionClass.connection.ImageTheme.FirstOrDefault(x => x.IdImage == id);
            ConnectionClass.connection.ImageTheme.Remove(image);
            ConnectionClass.connection.SaveChanges();
            ListImage();
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
            btnDeleteImage.Opacity = 1; btnDeleteImage.IsEnabled = true;
            int id = Convert.ToInt32(txbIdImage.Text);
            var image = ConnectionClass.connection.ImageTheme.FirstOrDefault(x => x.IdImage == id);
            GotFocusAnimation(txbVisibleImage);
            txbTitleImage.Text = image.Name;
        }

        private void btnBackToList_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.GoBack(); }

    }
}
