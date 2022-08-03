using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class LicensePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<string> licenseList { get; set; }

        private List<string> LicenseList
        {
            get { return licenseList; }
            set
            {
                licenseList = value;
                OnPropertyChanged("LicenseList");
            }
        }

        private NotifyTaskCompletion<string> licenses { get; set; }

        public NotifyTaskCompletion<string> Licenses
        {
            get { return licenses; }
            set
            {
                licenses = value;
                OnPropertyChanged("Licenses");
            }
        }

        public LicensePageViewModel()
        {
           Licenses = new NotifyTaskCompletion<string>(GetLicenses());
        }

        private async Task<string> GetLicenses()
        {
            var path = Path.Combine(FileParser.GetExecutingDirectoryName(), "Licenses.html");
            return await FileParser.readFileAsync(path);
        }
    }
}
