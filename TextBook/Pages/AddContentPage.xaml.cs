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

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddContentPage.xaml
    /// </summary>
    public partial class AddContentPage : Page
    {
        bool Existing;
        int id;
        public AddContentPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
            btnBindTest.Opacity = 0.3;
            var existing = ConnectionClass.connection.Theme.FirstOrDefault(x => x.idTheme == Properties.Settings.Default.IdExistingTheme);
            if (existing == null)
            {
                prgTitleTheme.Inlines.Add("Введите наименование темы");
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
                Existing = true;
                LoadTheme();
            }
        }
        string path;
        private void btnLoadTheme_Click(object sender, RoutedEventArgs e)
        {

            TextRange doc = new TextRange(prgTextTheme.ContentStart, prgTextTheme.ContentEnd);
            doc.Text = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "RichText Files (*.rtf)|*.rtf|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {

                
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                        doc.Load(fs, DataFormats.Rtf); 
                    else if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                        doc.Load(fs, DataFormats.Text);
                    else if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".docx")
                    {
                        this.ReadDocx(ofd.FileName);
                    }
                    path = doc.Text;
                    btnSaveTheme.IsEnabled = true; btnSaveTheme.Opacity = 1;
                }
            }
        }

        private void btnSaveTheme_Click(object sender, RoutedEventArgs e)
        {
            TextRange title = new TextRange(prgTitleTheme.ContentStart, prgTitleTheme.ContentEnd);
            TextRange description = new TextRange(prgDescriptionTheme.ContentStart, prgDescriptionTheme.ContentEnd);
            if (Existing == false)
            {
                if (!String.IsNullOrWhiteSpace(title.Text))
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
                    rtbTheme.Document.Blocks.Clear();
                    title.Text = "";
                    btnSaveTheme.Opacity = 0.3; btnSaveTheme.IsEnabled = false;
                    btnListTest.Opacity = 1;btnListTest.IsEnabled = true;
                }
                else { MessageBox.Show("Введите наименование темы"); }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(title.Text))
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
                    rtbTheme.Document.Blocks.Clear();
                    title.Text = "";
                    btnSaveTheme.Opacity = 0.3;
                    btnSaveTheme.IsEnabled = false;
                }
                else { MessageBox.Show("Введите наименование темы"); }
            }
           
        }

        private void btnDeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            var topicTest = ConnectionClass.connection.TopicTest.FirstOrDefault(x=>x.Theme == id);
            var theme = ConnectionClass.connection.Theme.FirstOrDefault(x=> x.idTheme == id);
            if (topicTest == null)
            {
                ConnectionClass.connection.Theme.Remove(theme);
            }
            else
            {
                ConnectionClass.connection.TopicTest.Remove(topicTest);
                ConnectionClass.connection.Theme.Remove(theme);
            }
            ConnectionClass.connection.SaveChanges();
            btnSaveTheme.Opacity = 0.3; btnSaveTheme.IsEnabled = false;

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
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = 450;
                anim.To = 0;
                anim.Duration = TimeSpan.FromSeconds(1);
                brdListTest.BeginAnimation(WidthProperty, anim);
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation();
                anim.From = 0;
                anim.To = 450;
                anim.Duration = TimeSpan.FromSeconds(1);
                brdListTest.BeginAnimation(WidthProperty, anim);
            }
        }
    }
}
