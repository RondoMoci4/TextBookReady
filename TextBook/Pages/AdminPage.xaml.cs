using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Word = Microsoft.Office.Interop.Word;
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
            SearchResult();
            var test = ConnectionClass.connection.Test.ToList();
            test.Insert(0, new Test
            {
                Title = "Все тесты"
            });
            cmbTest.ItemsSource = test;
            cmbTest.SelectedIndex = 0;
        }

        private void btnCreateTest_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListTestPage()); }

        private void btnAddContent_Click(object sender, RoutedEventArgs e) { FrameClass.mainFrame.Navigate(new ListThemePage()); }

        private void txbSearchSurname_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbSearchSurnameVisible); }

        private void txbSearchSurname_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbSearchSurnameVisible,txbSearchSurname); }

        private void txbSearchName_LostFocus(object sender, RoutedEventArgs e) { LostFocusAnimation(txbSearchNameVisible,txbSearchName); }

        private void txbSearchName_GotFocus(object sender, RoutedEventArgs e) { GotFocusAnimation(txbSearchNameVisible); }

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
        private void txbSearchName_TextChanged(object sender, TextChangedEventArgs e) { SearchResult(); }

        private void txbSearchSurname_TextChanged(object sender, TextChangedEventArgs e) { SearchResult(); }

        private void btnWordFile_Click(object sender, RoutedEventArgs e)
        {
            var allTest = ConnectionClass.connection.TestResult.ToList();
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();
            Word.Paragraph paragraph = document.Paragraphs.Add();
            Word.Range range = paragraph.Range;
            range.Text = "Результат тестирования";
            range.InsertParagraphAfter();
            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table table = document.Tables.Add(tableRange, allTest.Count() + 1, 5);
            table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle
                    = Word.WdLineStyle.wdLineStyleSingle;
            table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            Word.Range cellRange;
            cellRange = table.Cell(1, 1).Range; cellRange.Text = "Имя и фамилия";
            cellRange = table.Cell(1, 2).Range; cellRange.Text = "Наименование теста";
            cellRange = table.Cell(1, 3).Range; cellRange.Text = "Время прохождения";
            cellRange = table.Cell(1, 4).Range; cellRange.Text = "Оценка";
            cellRange = table.Cell(1, 5).Range; cellRange.Text = "Дата прохождения";
            if (cmbTest.SelectedIndex > 0)
            {
                allTest = allTest.Where(x => x.Test.Equals(cmbTest.SelectedItem as Test)).ToList();
                if (allTest.Count != 0)
                {
                    for (int i = 0; i < allTest.Count; i++)
                    {
                        var currentResult = allTest[i];
                        cellRange = table.Cell(i + 2, 1).Range; cellRange.Text = currentResult.NameSurname;
                        cellRange = table.Cell(i + 2, 2).Range; cellRange.Text = currentResult.Test.Title;
                        cellRange = table.Cell(i + 2, 3).Range; cellRange.Text = currentResult.Time.ToString();
                        cellRange = table.Cell(i + 2, 4).Range; cellRange.Text = currentResult.CorrectAnswers.ToString();
                        cellRange = table.Cell(i + 2, 5).Range; cellRange.Text = currentResult.DateOfPassage.ToString();
                    }
                }
                else { MessageBox.Show("Данные о результатах теста отсутствуют"); }
                

            }
            else
            {
                for (int i = 0; i < allTest.Count; i++)
                {
                    var currentResult = allTest[i];
                    cellRange = table.Cell(i + 2, 1).Range; cellRange.Text = currentResult.NameSurname;
                    cellRange = table.Cell(i + 2, 2).Range; cellRange.Text = currentResult.Test.Title;
                    cellRange = table.Cell(i + 2, 3).Range; cellRange.Text = currentResult.Time.ToString();
                    cellRange = table.Cell(i + 2, 4).Range; cellRange.Text = currentResult.CorrectAnswers.ToString();
                    cellRange = table.Cell(i + 2, 5).Range; cellRange.Text = currentResult.DateOfPassage.ToString();
                }
            }
            application.Visible = true;
        }

        private void SearchResult()
        {
            var result = ConnectionClass.connection.TestResult.ToList();
            if (cmbTest.SelectedIndex > 0)
                result = result.Where(x => x.Test.Equals(cmbTest.SelectedItem as Test)).ToList();
            result = result.Where(x => x.Surname.ToLower().Contains(txbSearchSurname.Text.ToLower())).ToList();
            result = result.Where(x => x.Name.ToLower().Contains(txbSearchName.Text.ToLower())).ToList();
            dgInfoResult.ItemsSource = result;
        }

        private void cmbTest_SelectionChanged(object sender, SelectionChangedEventArgs e) { SearchResult(); }
    }
}
