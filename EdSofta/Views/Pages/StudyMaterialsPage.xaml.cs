using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using HtmlAgilityPack;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for StudyMaterialsPage.xaml
    /// </summary>
    public partial class StudyMaterialsPage : Page
    {
        private readonly List<StudyMaterialsViewModel> _allStudyMaterials = new List<StudyMaterialsViewModel>();
        //SpeechSynthesizer _synth = new SpeechSynthesizer();
        public StudyMaterialsPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
            //_synth.SetOutputToDefaultAudioDevice();


            
        }

        private async void PageLoaded(object sender, EventArgs e)
        {
            //var studyMaterials = StudyResourceUtility.getStudyMaterialsSubjects()?.Select(subject => new StudyMaterialsViewModel
            //{
            //    Name = subject,
            //    Topics = StudyResourceUtility.getStudyMaterialTopics(subject).Select(topic => new TopicViewModel { TopicName = topic }).ToObservableCollection()
            //});

            var studyMaterials = new List<StudyMaterialsViewModel>();
            var studySubjects = await StudyResourceUtility.getStudyMaterialsSubjectsAsync();
            //foreach (var subject in studySubjects)
            //{
            //    var studyMaterialsViewModel = new StudyMaterialsViewModel{ Name = subject };
            //    var allTopics = await StudyResourceUtility.getStudyMaterialTopicsAsync(subject);
            //    studyMaterialsViewModel.Topics = allTopics.Select(topic => new TopicViewModel { TopicName = topic }).ToObservableCollection();
            //    studyMaterials.Add(studyMaterialsViewModel);
            //}

            _allStudyMaterials.AddRange(studyMaterials);

            DataContext = _allStudyMaterials.ToObservableCollection();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(StudyResourceUtility.getStudyMaterial("Mathematics", "test"));
            //mainDoc.LoadHtml(text);
            //mainDoc.DocumentNode.SelectNodes("//img").
            //var nodes = mainDoc.DocumentNode.SelectNodes("//body//text()[(normalize-space(.) != '') and not(parent::script) and not(*)]");
            
            foreach (var htmlNode in mainDoc.DocumentNode.Descendants().ToList())
            {
                if (htmlNode.Name == "p")
                {
                    htmlNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(htmlNode.InnerText + "."), htmlNode);
                }

                if (htmlNode.Name != "img") continue;
                var alt = htmlNode.GetAttributeValue("alt", "");
                var nodeForReplace = HtmlTextNode.CreateNode(alt);
                htmlNode.ParentNode.ReplaceChild(nodeForReplace, htmlNode);

            }
            var cleanText = mainDoc.DocumentNode.InnerText;
            //MessageBox.Show(cleanText.Replace("&nbsp;", " "), "Html");
            readText(cleanText.Replace("&nbsp;", " "));
            //readText(AppResources.getStudyMaterial("Mathematics", "test1"));
        }

        private void ButtonBaseq1_OnClick(object sender, RoutedEventArgs e)
        {
            //_synth.Pause();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            //_synth.Resume();
        }

        public void readText(string text)
        {
            //SpeechSynthesizer synth = new SpeechSynthesizer();

            //// Configure the audio output.   
            //synth.SetOutputToDefaultAudioDevice();

            // Speak a string.  
            //_synth.SpeakAsync(text);
        }

    }
}
