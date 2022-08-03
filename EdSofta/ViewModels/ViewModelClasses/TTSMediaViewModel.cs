using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpeechLib;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class TTSMediaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //private SpeechSynthesizer _speechSynthesizer;

        private bool canPlay { get; set; }

        public bool CanPlay
        {
            get { return canPlay; }
            set
            {
                canPlay = value;
                OnPropertyChanged("CanPlay");
            }
        }

        private bool canPause { get; set; }

        public bool CanPause
        {
            get { return canPause; }
            set
            {
                canPause = value;
                OnPropertyChanged("CanPause");
            }
        }

        private bool canStop { get; set; }

        public bool CanStop
        {
            get { return canStop; }
            set
            {
                canStop = value;
                OnPropertyChanged("CanStop");
            }
        }

        private bool canRestart { get; set; }

        public bool CanRestart
        {
            get { return canRestart; }
            set
            {
                canRestart = value;
                OnPropertyChanged("CanRestart");
            }
        }

        private readonly string _text;
        private SpVoice _speechSynthesizer;
        private bool isSpeaking;

        public TTSMediaViewModel(string text)
        {
            //_speechSynthesizer = new SpeechSynthesizer();
            _text = text;
            //_speechSynthesizer.SpeakCompleted += SynthesizerSpeakCompleted;
            //_speechSynthesizer.SpeakStarted += SynthesizerSpeakStarted;
            //_synth = new SpVoice();
        }


        public void Play()
        {
            if (_speechSynthesizer == null)
            {
                Speak();
                return;
            }

            Resume();
        }

        private void Resume()
        {
            if (_speechSynthesizer == null) return;

            //if (_speechSynthesizer.State != SynthesizerState.Paused) return;
            isSpeaking = true;
            _speechSynthesizer.Resume();
            CanPlay = false;
            CanPause = true;
            //can speak is enabled
        }

        public void Pause()
        {
            if (_speechSynthesizer == null) return;
            isSpeaking = false;
            //if (!isSpeaking) return;
            //if (_speechSynthesizer.State != SynthesizerState.Speaking) return;
            _speechSynthesizer.Pause();
            CanPlay = true;
            CanPause = false;
            //can speak is disabled
        }

        public void Restart()
        {
            Stop();
            Speak();
        }

        public void Stop()
        {
            if (_speechSynthesizer == null) return;

            //_speechSynthesizer.Dispose();
            if (!isSpeaking) return;
            _speechSynthesizer.Skip("Sentence", int.MaxValue);
            _speechSynthesizer = null;
            //can speak is enabled
            CanPlay = false;
            CanPause = false;
            CanStop = false;

        }

        private void Speak()
        {
            //_speechSynthesizer?.Dispose();

            if (string.IsNullOrWhiteSpace(_text)) return;
            _speechSynthesizer = new SpVoice();
            //_speechSynthesizer = new SpeechSynthesizer();
            //_speechSynthesizer.SetOutputToDefaultAudioDevice();
            //_speechSynthesizer.SpeakCompleted += SynthesizerSpeakCompleted;
            //_speechSynthesizer.SpeakStarted += SynthesizerSpeakStarted;
            //_speechSynthesizer.SpeakAsync(_text);

            _speechSynthesizer.Speak(_text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            isSpeaking = true;
            CanPause = true;
            CanStop = true;
            CanPlay = false;
        }

        public void CancelAll()
        {
            Stop();
            //_speechSynthesizer?.Dispose();
            //GC.Collect();

        }

    }
}
