using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class ContentUpdateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private int progressValue { get; set; }
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value >= 0 ? value : 0;
                OnPropertyChanged("ProgressValue");
            }
        }

        private string progressText { get; set; }
        public string ProgressText
        {
            get { return progressText; }
            set
            {
                progressText = !string.IsNullOrWhiteSpace(value) ? value : "Processing...";
                OnPropertyChanged("ProgressText");
            }
        }

        private string processText { get; set; }
        public string ProcessText
        {
            get { return processText; }
            set
            {
                processText = !string.IsNullOrWhiteSpace(value) ? value : "Processing...";
                OnPropertyChanged("ProcessText");
            }
        }

        private bool isExtracting { get; set; }
        public bool IsExtracting
        {
            get { return isExtracting; }
            set
            {
                isExtracting = value;
                OnPropertyChanged("IsExtracting");
            }
        }

        private bool isProcessStarted { get; set; }

        public bool IsProcessStarted
        {
            get { return isProcessStarted; }
            set
            {
                isProcessStarted = value;
                OnPropertyChanged("IsProcessStarted");
            }
        }

        
        private bool isDownloadAvailable { get; set; }

        public bool IsDownloadAvailable
        {
            get { return isDownloadAvailable; }
            set
            {
                isDownloadAvailable = value;
                OnPropertyChanged("IsDownloadAvailable");
            }
        }
        
        private bool isCheckUpdateVisible { get; set; }

        public bool IsCheckUpdateVisible
        {
            get { return isCheckUpdateVisible; }
            set
            {
                isCheckUpdateVisible = value;
                OnPropertyChanged("IsCheckUpdateVisible");
            }
        }

        private bool isLicenseExpired { get; set; }

        public bool IsLicenseExpired
        {
            get { return isLicenseExpired; }
            set
            {
                isLicenseExpired = value;
                OnPropertyChanged("IsLicenseExpired");
            }
        }

        private bool isProgressVisible { get; set; }

        public bool IsProgressVisible
        {
            get { return isProgressVisible; }
            set
            {
                isProgressVisible = value;
                OnPropertyChanged("IsProgressVisible");
            }
        }

        private string message { get; set; }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public ContentUpdateViewModel()
        {
            IsProgressVisible = true;
        }
    }
}
