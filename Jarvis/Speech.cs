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

        public Speech Voice { get; set; } = new Speech();

        public Jarvis()
        {
            handle = GCHandle.Alloc(this);
        }

        public class Speech : IDisposable
        {
            private GCHandle handle;
            public CultureInfo CultureInfo { get; set; } = new CultureInfo("ru-RU");
            private SpeechRecognitionEngine recognition { get; set; }
            private SpeechSynthesizer synthesizer;
            private Thread mainWorker, supportWorker;
            private bool recognitionReady = false, synthesizerReady = false;

            private string innerText, outerText;

            //public delegate void JarvisSpeechEventDelegate(object sender, JarvisSpeechEventArgs e);
            //public event JarvisSpeechEventDelegate JarvisSpeechEvent;

            public string InnerText
            {
                get { return innerText; }
                set { innerText = value; }
            }

            public string OuterText
            {
                get { return outerText; }
                set { outerText = value; }
            }

            public Speech()
            {
                handle = GCHandle.Alloc(this);
                mainWorker = new Thread(() => RecognitionConfigure()) { Priority = ThreadPriority.Highest };
                supportWorker = new Thread(() => SynthesizerConfigure()) { Priority = ThreadPriority.Highest };
                mainWorker.Start(); mainWorker.Join();
                supportWorker.Start(); supportWorker.Join();
            }

            public void Speak(string Text)
            {
                mainWorker = new Thread(() => { synthesizer.SpeakAsync(Text); }) { Priority = ThreadPriority.Highest };
                mainWorker.Start();
            }

            private void RecognitionConfigure()
            {

            }

            private void SynthesizerConfigure()
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.SetOutputToDefaultAudioDevice();
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

            //public class JarvisSpeechEventArgs : EventArgs
            //{
            //    BookmarkReachedEventArgs bookmarkReached { get; set; }
            //    SpeakCompletedEventArgs completedEventArgs { get; set; }
            //    SpeakProgressEventArgs progressEventArgs { get; set; }
            //    SpeakStartedEventArgs startedEventArgs { get; set; }

            //    JarvisSpeechEventArgs(BookmarkReachedEventArgs BookmarkReached,
            //        SpeakCompletedEventArgs CompletedEventArgs,
            //        SpeakProgressEventArgs ProgressEventArgs,
            //        SpeakStartedEventArgs StartedEventArgs)
            //    {
            //        bookmarkReached = BookmarkReached;
            //        completedEventArgs = CompletedEventArgs;
            //        progressEventArgs = ProgressEventArgs;
            //        startedEventArgs = StartedEventArgs;
            //    }
            //}

            #region События синтезатора речи
            private void Synthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
            {

            }

            private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
            {

            }

            private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
            {

            }

            private void Synthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
            {

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

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
                if (disposing)
                {
                    handle.Free();
                    Voice.Dispose();
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

    //public class Speech : IDisposable
    //{
    //    private GCHandle handle;
    //    public CultureInfo CultureInfo { get; set; } = new CultureInfo("ru-RU");
    //    private SpeechRecognitionEngine recognition { get; set; }
    //    private SpeechSynthesizer synthesizer;
    //    private ReadOnlyCollection<InstalledVoice> voices;
    //    private Thread mainWorker, supportWorker;
    //    private bool recognitionReady = false, synthesizerReady = false;

    //    public delegate void JarvisSpeechEventDelegate(object sender, JarvisSpeechEventArgs e);
    //    public event JarvisSpeechEventDelegate JarvisSpeechEvent;

    //    public Speech()
    //    {
    //        handle = GCHandle.Alloc(this);
    //        mainWorker = new Thread(() => RecognitionConfigure()) { Priority = ThreadPriority.Highest };
    //        supportWorker = new Thread(() => SynthesizerConfigure()) { Priority = ThreadPriority.Highest };
    //        mainWorker.Start(); mainWorker.Join();
    //        supportWorker.Start(); supportWorker.Join();
    //    }

    //    public ReadOnlyCollection<InstalledVoice> Voices
    //    {
    //        get { return voices; }
    //        set { voices = value; }
    //    }

    //    public void Speak(string Text)
    //    {
    //        mainWorker = new Thread(() => { while (true) synthesizer.SpeakAsync(Text); }) { Priority = ThreadPriority.Highest };
    //        mainWorker.Start(); mainWorker.Join();
    //    }

    //    private void RecognitionConfigure()
    //    {

    //    }

    //    private void SynthesizerConfigure()
    //    {
    //        synthesizer = new SpeechSynthesizer();
    //        synthesizer.SetOutputToDefaultAudioDevice();
    //        try { Voices = synthesizer.GetInstalledVoices(); } catch { }

    //        if (!synthesizerReady)
    //        {
    //            synthesizer.SpeakStarted +=
    //                new EventHandler<SpeakStartedEventArgs>(Synthesizer_SpeakStarted);
    //            synthesizer.SpeakProgress +=
    //                new EventHandler<SpeakProgressEventArgs>(Synthesizer_SpeakProgress);
    //            synthesizer.BookmarkReached +=
    //                 new EventHandler<BookmarkReachedEventArgs>(Synthesizer_BookmarkReached);
    //            synthesizer.SpeakCompleted +=
    //                new EventHandler<SpeakCompletedEventArgs>(Synthesizer_SpeakCompleted);
    //        }

    //        synthesizerReady = true;
    //    }

    //    public class JarvisSpeechEventArgs : EventArgs
    //    {
    //        BookmarkReachedEventArgs bookmarkReached { get; set; }
    //        SpeakCompletedEventArgs completedEventArgs { get; set; }
    //        SpeakProgressEventArgs progressEventArgs { get; set; }
    //        SpeakStartedEventArgs startedEventArgs { get; set; }

    //        JarvisSpeechEventArgs(BookmarkReachedEventArgs BookmarkReached,
    //            SpeakCompletedEventArgs CompletedEventArgs,
    //            SpeakProgressEventArgs ProgressEventArgs,
    //            SpeakStartedEventArgs StartedEventArgs)
    //        {
    //            bookmarkReached = BookmarkReached;
    //            completedEventArgs = CompletedEventArgs;
    //            progressEventArgs = ProgressEventArgs;
    //            startedEventArgs = StartedEventArgs;
    //        }
    //    }

    //    #region События синтезатора речи
    //    private void Synthesizer_BookmarkReached(object sender, BookmarkReachedEventArgs e)
    //    {

    //    }

    //    private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
    //    {

    //    }

    //    private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e)
    //    {

    //    }

    //    private void Synthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
    //    {

    //    }
    //    #endregion

    //    #region IDisposable Support
    //    private bool disposedValue = false;

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //            if (disposing)
    //                handle.Free();
    //        disposedValue = true;
    //    }

    //    ~Speech() { Dispose(false); }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }
    //    #endregion
    //}
}
