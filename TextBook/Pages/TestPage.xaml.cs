using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TextBook.Data;
using TextBook.Pages;

namespace TextBook.Pages
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        TimeSpan zeroTime = new TimeSpan(0,0,10);
        TimeSpan timeStart;
        TimeSpan timeEnd;
        Random random = new Random();
        List<int> idQuestion = new List<int>();
        List<int> idBackQuestion = new List<int>();
        string textAnswer;
        int resultRating;
        int countQuestion;
        int ranQuestion;
        int numberQuestion;
        double countCorrentQuestion;
        double currentQuestion;
        double twoRating = 0;
        double threeRating = 0.3;
        double fourRating = 0.6;
        double fiveRating = 0.9;
        double currentRating;
        bool backQuestionTrue;
        public TestPage()
        {
            InitializeComponent();
            ConnectionClass.connection = new DBTextBookEntities();
            var test = ConnectionClass.connection.Test.Where(x => x.Title == Properties.Settings.Default.TitleTest).ToList();
            int idtest = ConnectionClass.connection.Test.FirstOrDefault(x => x.Title == Properties.Settings.Default.TitleTest).IdTest;
            var count = ConnectionClass.connection.TestQuestion.Where(x => x.IdTest == idtest).Count();
            var time = test.Select(x => x.Time).FirstOrDefault(); countQuestion = count;
            var create = test.Join(ConnectionClass.connection.Autorization, x => x.CreatorTest, p => p.IdUser, (x, p) => new
            {
                FIO = p.Name + " " + p.Surname + " " + p.Patronymic,
            }).FirstOrDefault();
            txbCreateInfo.Text = create.FIO.ToString(); txbTimeInfo.Text = test.Select(x => x.Time).FirstOrDefault().ToString();
            txbQuestion.Text = count.ToString();
            txbAllQuestion.Text = count.ToString(); txbTestInfo.Text = test.Select(x => x.Title).FirstOrDefault().ToString();
            Timer(time);
            var question = test.Join(ConnectionClass.connection.TestQuestion, x => x.IdTest, p => p.IdTest, (x, p) => new { p.IdQuestion }).ToList(); 
            idQuestion = new List<int>(question.Select(x => x.IdQuestion)); 
            ranQuestion = random.Next(idQuestion.Count); numberQuestion = idQuestion[ranQuestion]; 
            LoadAnswer();
            currentQuestion++;
            txbCurrentQuestion.Text = currentQuestion.ToString();
            txbTextQuestion.Text = ConnectionClass.connection.TestQuestion.Where(x => x.IdQuestion == numberQuestion).Select(x => x.TitleQuestion).FirstOrDefault();
            ImageQuestion(txbTextQuestion.Text);
            timeStart = time;
            btnBack.IsEnabled = false; btnBack.Opacity = 0.3;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (rbOneAnswer.IsChecked == false && rbTwoAnswer.IsChecked == false && rbThreeAnswer.IsChecked == false && rbFourAnswer.IsChecked == false)
            {
                MessageBox.Show("Выберите ответ");
            }
            else
            {
                if (idQuestion.Count == 1)
                {
                    string correctAnswer = ConnectionClass.connection.TestAnswer.Where(x => x.Answer == textAnswer).Select(x => x.Correct).FirstOrDefault().ToString();
                    if (correctAnswer == "True") { countCorrentQuestion++; backQuestionTrue = true; } else { backQuestionTrue = false; }
                    rbOneAnswer.IsChecked = false; rbTwoAnswer.IsChecked = false; rbThreeAnswer.IsChecked = false; rbFourAnswer.IsChecked = false;
                    grdTest.Visibility = Visibility.Hidden;
                    grdResult.Visibility = Visibility.Visible;
                }
                else
                {
                    
                    btnBack.IsEnabled = true; btnBack.Opacity = 1;
                    string correctAnswer = ConnectionClass.connection.TestAnswer.Where(x => x.Answer == textAnswer).Select(x => x.Correct).FirstOrDefault().ToString();
                    if (correctAnswer == "True") { countCorrentQuestion++; backQuestionTrue = true; } else { backQuestionTrue = false; }
                    idBackQuestion.Add(numberQuestion); idQuestion.Remove(numberQuestion);
                    ranQuestion = random.Next(idQuestion.Count); numberQuestion = idQuestion[ranQuestion];
                    currentQuestion++;
                    txbCurrentQuestion.Text = currentQuestion.ToString();
                    LoadAnswer();
                    rbOneAnswer.IsChecked = false; rbTwoAnswer.IsChecked = false; rbThreeAnswer.IsChecked = false; rbFourAnswer.IsChecked = false;
                    
                    txbTextQuestion.Text = ConnectionClass.connection.TestQuestion.Where(x => x.IdQuestion == numberQuestion).Select(x => x.TitleQuestion).FirstOrDefault();
                    ImageQuestion(txbTextQuestion.Text);
                    MessageBox.Show(txbTextQuestion.Text);
                    
                }
                
            }

            //удаляет из списка текущий вопрос
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion == 1)
            {
                btnBack.IsEnabled = true; btnBack.Opacity = 0.3;
            }
            else
            {
                ImageQuestion(txbTextQuestion.Text);
                numberQuestion = idBackQuestion.Last();
                idQuestion.Add(numberQuestion); idBackQuestion.Remove(numberQuestion);
                currentQuestion--;
                if (backQuestionTrue == true) { countCorrentQuestion--; }
                LoadAnswer();
                txbCurrentQuestion.Text = currentQuestion.ToString();
                rbOneAnswer.IsChecked = false; rbTwoAnswer.IsChecked = false; rbThreeAnswer.IsChecked = false; rbFourAnswer.IsChecked = false;
                txbTextQuestion.Text = ConnectionClass.connection.TestQuestion.Where(x => x.IdQuestion == numberQuestion).Select(x => x.TitleQuestion).FirstOrDefault();
                
            }

            //добавляет в список прошлый вопрос
        }

        private void btnOpenInfo_Click(object sender, RoutedEventArgs e)
        {
            if (brdInfoTest.Width == 450)
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 450,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdInfoTest.BeginAnimation(WidthProperty, anim);
            }
            else
            {
                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 0,
                    To = 450,
                    Duration = TimeSpan.FromSeconds(1)
                };
                brdInfoTest.BeginAnimation(WidthProperty, anim);
            }
        }

        private void btnExitTest_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan resultTime = timeStart - timeEnd;
            TestResult result = new TestResult()
            {
                IdTest = 1,
                Name = Properties.Settings.Default.NameStudent,
                Surname = Properties.Settings.Default.SurnameStudent,
                Time = resultTime,
                CorrectAnswers = resultRating,
                DateOfPassage = Convert.ToDateTime(txbDateTest.Text)
            };
            ConnectionClass.connection.TestResult.Add(result);
            ConnectionClass.connection.SaveChanges();
            FrameClass.mainFrame.Navigate(new ListTestPage());
        }

        private void grdResult_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            currentRating = countCorrentQuestion / countQuestion;
            if (twoRating <= currentRating && currentRating <= threeRating) { txbSmileyAngry.Opacity = 1; resultRating = 2; txbRating.Text = $"{resultRating} (Очень плохой результат.)";}
            else if (threeRating <= currentRating && currentRating <= fourRating) { txbSmileyCry.Opacity = 1; resultRating = 3; txbRating.Text = $"{resultRating} (Уже лучше.)";}
            else if (fourRating <= currentRating && currentRating <= fiveRating) { txbSmiley.Opacity = 1; resultRating = 4; txbRating.Text = $"{resultRating} (Хороший результат.)";}
            else if (fourRating <= currentRating && currentRating <= 1) { txbSmileyCool.Opacity = 1; resultRating = 5; txbRating.Text = $"{resultRating} (Вы великолепны!)";}
            timeEnd = TimeSpan.Parse(txbTime.Text);
            txbCountCurrentQuestion.Text = $"{countCorrentQuestion} из {countQuestion}";
            txbUserTest.Text = $"{Properties.Settings.Default.NameStudent} {Properties.Settings.Default.SurnameStudent}";
            txbDateTest.Text = DateTime.Now.ToString();
            txbTitleTest.Text = $"{Properties.Settings.Default.TitleTest}";
        }
        private void rbAll_Checked(object sender, RoutedEventArgs e) { RadioButton rb = (RadioButton)sender; textAnswer = rb.Content.ToString(); }

        public void LoadQuestion(List<Test> test)
        {
            var question = test.Join(ConnectionClass.connection.TestQuestion, x => x.IdTest, p => p.IdTest, (x, p) => new { p.IdQuestion }).ToList(); //список вопросов определенного теста
            idQuestion = new List<int>(question.Select(x => x.IdQuestion)); // список id вопросов
            ranQuestion = random.Next(idQuestion.Count); numberQuestion = idQuestion[ranQuestion]; // случайный выбор вопроса из списка
            idQuestion.Remove(numberQuestion); idBackQuestion.Add(numberQuestion);
        }

        public void LoadAnswer()
        {
            var answerAll = ConnectionClass.connection.TestAnswer.Where(x => x.IdQuestion == numberQuestion).ToList(); // список ответов на выбранный вопрос
            List<string> answer = new List<string>(answerAll.Select(x => x.Answer)); // список ответов на вопрос
            List<int> numAnswer = new List<int>();
            for (int i = 0; i < answer.Count; i++)
            {
                int j = random.Next(answer.Count);
                while (numAnswer.Contains(j))
                    j = random.Next(answer.Count);
                numAnswer.Add(j);
            }
            rbOneAnswer.Content = answer[numAnswer[0]];
            rbTwoAnswer.Content = answer[numAnswer[1]];
            rbThreeAnswer.Content = answer[numAnswer[2]];
            rbFourAnswer.Content = answer[numAnswer[3]];
        }

        public void Timer(TimeSpan time)
        {
            DispatcherTimer _timer = new DispatcherTimer();
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                txbTime.Text = time.ToString("c");
                if (time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    grdTest.Visibility = Visibility.Hidden;
                    grdResult.Visibility = Visibility.Visible;
                }

                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

            _timer.Start();
        }

        private void ImageQuestion(string question)
        {
            imageQuestion.Visibility = Visibility.Hidden;
            txbTextQuestion.Visibility = Visibility.Visible;
            var image = ConnectionClass.connection.TestQuestion.FirstOrDefault(x => x.TitleQuestion == question);
            if (image.ImageQuestion != null)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized) { imageQuestion.Width = 450; imageQuestion.Height = 450; }
                else { imageQuestion.Width = 250; imageQuestion.Height = 250; }
                txbTextQuestion.Visibility = Visibility.Hidden;
                imageQuestion.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(image.ImageQuestion);
                imageQuestion.Visibility = Visibility.Visible;
            }
            
        }
    }
}
