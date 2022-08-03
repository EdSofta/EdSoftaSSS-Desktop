using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class OnboardingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string activeSlide { get; set; }

        public string ActiveSlide
        {
            get { return activeSlide; }
            set
            {
                activeSlide = value;
                OnPropertyChanged("ActiveSlide");
            }
        }

        private string headerText { get; set; }

        public string HeaderText
        {
            get { return headerText; }
            set
            {
                headerText = value;
                OnPropertyChanged("HeaderText");
            }
        }

        private string bodyText { get; set; }

        public string BodyText
        {
            get { return bodyText; }
            set
            {
                bodyText = value;
                OnPropertyChanged("BodyText");
            }
        }

        private readonly DispatcherTimer timer = new DispatcherTimer();
        public OnboardingViewModel()
        {
            changeSlide("Slide1");
            timer.Tick += (sender, e) =>
            {
                setNextSlide();
            };
            timer.Interval = new TimeSpan(0, 0, 8);
            timer.Start();
        }


        private void setNextSlide()
        {
            switch (ActiveSlide)
            {
                case "Slide1":
                    changeSlide("Slide2");
                    break;
                case "Slide2":
                    changeSlide("Slide3");
                    break;
                case "Slide3":
                    changeSlide("Slide1");
                    break;
                default:
                    changeSlide("Slide1");
                    break;
            }
        }

        public void changeSlide(string name)
        {
            var header = Application.Current.FindResource($"{name}.Header");
            var body = Application.Current.FindResource($"{name}.Body");

            ActiveSlide = name;
            HeaderText = (string)header ?? "Welcome to EdSofta";
            BodyText = (string) body ?? "";
        }

    }
}
