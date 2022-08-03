using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Collections;
using EdSofta.ViewModels.Utility;
using EdSofta.Views.Pages;
using Button = System.Windows.Controls.Button;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading;
using EdSofta.Constants;
using EdSofta.Data;
using EdSofta.Migrations;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.DataAccess;
using EdSofta.Services;
using EdSofta.Views.Windows;
using Path = System.Windows.Shapes.Path;

namespace EdSofta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModels.Utility.App.UpdaterModulePath = System.IO.Path.Combine(FileParser.GetExecutingDirectoryName(), "updater.exe");
            //Thread thread = new Thread(StartSilent);
            //thread.Start();
            var splashScreen = new SplashPage(DisplayFrame);
            DisplayFrame.Navigate(splashScreen);
            //setTheme();

        }

        private static void StartSilent()
        {
            Thread.Sleep(10000);

            Process process = Process.Start(ViewModels.Utility.App.UpdaterModulePath, "/silent");
            process?.Close();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeApp();
            var userService = new UserService();
            var user = await userService.getCurrentUser();

            Page page;
            if (user == null)
            {
                page = new WelcomePage(DisplayFrame);
            }
            else
            {
                page = new LandingPage(DisplayFrame);
            }

            DisplayFrame.Navigate(page);
        }


        private async Task InitializeApp()
        {
            ViewModels.Utility.App.getConfigurations();
            var setup = new AppSetup();
            await setup.PerformCheck();
            using (var upgradeContextMigration = new ApplicationDbContextMigration())
            {
                upgradeContextMigration.Initialize();
            }

            await setup.SetTheme();
            var activationKey = SavedResourceUtility.getActivationKey();
            if (string.IsNullOrEmpty(activationKey)) return;
            var activationService = new ActivationService();
            ViewModels.Utility.App.IsActivated = await activationService.activateByKeyAsync(activationKey);
        }
        

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           
            //seed();
            
            var page = new WelcomePage(DisplayFrame);
            //var page = new LandingPage(DisplayFrame);


            DisplayFrame.Navigate(page);

            //var calculator = new CalculatorWindow();
            //calculator.Owner = this;
           // calculator.Show();

        }

        //private int TestDb()
        //{
        //    using (var db = new ApplicationDbContext())
        //    {


        //        var user = new User
        //        {
        //            Email = @"hollytesting@gmail.com",
        //            FirstName = "Boluwatife",
        //            LastName = "Adewusi",
        //            Username = "BoluSarz",
        //            PhoneNumber = "0901234567",
        //            Id = Guid.NewGuid(),
        //            UserRole = UserType.Administrator
        //        };

        //        var question = new Question
        //        {
        //            QuestionNumber = 5,
        //            SubjectName = "Mathematics",
        //            TopicName = "Surds",
        //            Failed = true,
        //            Saved = false,
        //            Reported = true,
        //            QuestionYear = "2016",
        //            UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //        };

        //        var list = db.Users.ToList();
        //        db.Users.Add(user);
        //        db.Questions.Add(question);
        //        db.SaveChanges();
        //        return db.Users.Count();
        //    }

        //}

        //private async void seed()
        //{

        //    var user1 = new User
        //    {
        //        Email = @"hollytesting@gmail.com",
        //        FirstName = "Boluwatife",
        //        LastName = "Adewusi",
        //        Username = "BoluSarz",
        //        PhoneNumber = "0901234567",
        //        Id = Guid.NewGuid(),
        //        UserRole = UserType.Administrator,
        //        IsCurrent = true
        //    };

        //    var user2 = new User
        //    {
        //        Email = @"hollytesting@gmail.com",
        //        FirstName = "Oladele",
        //        LastName = "Samson",
        //        Username = "Oaken",
        //        PhoneNumber = "0901234567",
        //        Id = Guid.NewGuid(),
        //        UserRole = UserType.Guest,
        //        IsCurrent = false
        //    };

        //    var question1 = new Question
        //    {
        //        QuestionNumber = 1,
        //        SubjectName = "Mathematics",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question2 = new Question
        //    {
        //        QuestionNumber = 2,
        //        SubjectName = "Mathematics",
        //        TopicName = "Surds",
        //        Failed = false,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question3 = new Question
        //    {
        //        QuestionNumber = 3,
        //        SubjectName = "Mathematics",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question4 = new Question
        //    {
        //        QuestionNumber = 1,
        //        SubjectName = "English",
        //        TopicName = "Phrases and their structures",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question5 = new Question
        //    {
        //        QuestionNumber = 4,
        //        SubjectName = "Mathematics",
        //        TopicName = "vector",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = Guid.NewGuid().ToString()
        //    };

        //    var question6 = new Question
        //    {
        //        QuestionNumber = 5,
        //        SubjectName = "Biology",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2016",
        //        UserId = Guid.NewGuid().ToString()
        //    };

        //    var question7 = new Question
        //    {
        //        QuestionNumber = 2,
        //        SubjectName = "English",
        //        TopicName = "Phrases and their structures",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question8 = new Question
        //    {
        //        QuestionNumber = 3,
        //        SubjectName = "English",
        //        TopicName = "Summaries of essays",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question9 = new Question
        //    {
        //        QuestionNumber = 7,
        //        SubjectName = "English",
        //        TopicName = "Summaries of essays",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question10 = new Question
        //    {
        //        QuestionNumber = 8,
        //        SubjectName = "English",
        //        TopicName = "Phrases and their structures",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question11 = new Question
        //    {
        //        QuestionNumber = 9,
        //        SubjectName = "English",
        //        TopicName = "Phrases and their structures",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question12 = new Question
        //    {
        //        QuestionNumber = 20,
        //        SubjectName = "English",
        //        TopicName = "Use of figures of speech",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question13 = new Question
        //    {
        //        QuestionNumber = 5,
        //        SubjectName = "Mathematics",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question14 = new Question
        //    {
        //        QuestionNumber = 6,
        //        SubjectName = "Mathematics",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question15 = new Question
        //    {
        //        QuestionNumber = 10,
        //        SubjectName = "Biology",
        //        TopicName = "Surds",
        //        Failed = true,
        //        Saved = false,
        //        Reported = true,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var question16 = new Question
        //    {
        //        QuestionNumber = 12,
        //        SubjectName = "English",
        //        TopicName = "Understanding and writing essays",
        //        Failed = true,
        //        Saved = false,
        //        Reported = false,
        //        QuestionYear = "2015",
        //        UserId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString()
        //    };

        //    var list = new List<Question>
        //    {
        //        question1, question2, question3, question4, question5, question6, question7, question8, question9,
        //        question10, question11, question12, question13, question14, question15, question16
        //    };

        //    using (var dal = new UnitOfWork())
        //    {
        //        dal.QuestionRepository.AddRange(list);
        //        dal.UserRepository.AddRange(new List<User>{user1, user2});
        //        await dal.SaveChangesAsync();
        //    }

        //}
    }
}
