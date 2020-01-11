using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace PRMover
{
    public class Jarvis : IDisposable
    {
        private GCHandle handle;
        private CultureInfo cultureInfo = new CultureInfo("ru-RU");
        private Speech speech;

        public Speech Speech
        {
            get { return speech; }
            set { speech = value; }
        }

        public Jarvis()
        {
            handle = GCHandle.Alloc(this);
            Speech = new Speech();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                if (disposing)
                {
                    handle.Free();
                    Speech.Dispose();
                }
            disposedValue = true;
        }

        ~Jarvis() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class Speech : IDisposable
    {
        private GCHandle handle;
        private CultureInfo cultureInfo = new CultureInfo("ru-RU");
        private SpeechRecognitionEngine recognition;
        private SpeechSynthesizer synthesizer;
        private ReadOnlyCollection<InstalledVoice> voices;
        private Thread mainWorker;
        private Thread supportWorker;
        private bool recognitionReady = false, synthesizerReady = false;

        public Speech()
        {
            handle = GCHandle.Alloc(this);
            mainWorker = new Thread(() => RecognitionConfigure()) { Priority = ThreadPriority.Highest };
            supportWorker = new Thread(() => SynthesizerConfigure()) { Priority = ThreadPriority.Highest };
            mainWorker.Start(); mainWorker.Join();
            supportWorker.Start(); supportWorker.Join();
        }

        public ReadOnlyCollection<InstalledVoice> Voices
        {
            get { return voices; }
            set { voices = value; }
        }

        public CultureInfo CultureInfo
        {
            get { return cultureInfo; }
            set { cultureInfo = value; }
        }

        public void Speak(string Text)
        {
            while (true)
                synthesizer.SpeakAsync(Text);
        }

        private void RecognitionConfigure()
        {

        }

        private void SynthesizerConfigure()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            Voices = synthesizer.GetInstalledVoices();

            if (!synthesizerReady)
            {
                synthesizer.SpeakStarted +=
                    new EventHandler<SpeakStartedEventArgs>(Synthesizer_SpeakStarted);
                synthesizer.SpeakProgress +=
                    new EventHandler<SpeakProgressEventArgs>(Synthesizer_SpeakProgress);
                synthesizer.BookmarkReached +=
                     new EventHandler<BookmarkReachedEventArgs>(Synthesizer_BookmarkReached);
                synthesizer.SpeakCompleted +=
                    new EventHandler<SpeakCompletedEventArgs>(Synthesizer_SpeakCompleted);
            }

            synthesizerReady = true;
        }

        #region События синтезатора речи
        private void Synthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Synthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                if (disposing)
                    handle.Free();
            disposedValue = true;
        }

        ~Speech() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
