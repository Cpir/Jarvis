using Microsoft.Speech.Recognition;
using Microsoft.Speech;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
            private Microsoft.Speech.Recognition.Grammar grammar { get; set; }
            private Microsoft.Speech.Synthesis.SpeechSynthesizer synthesizer;
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
                mainWorker = new Thread(() => { synthesizer.SetOutputToWaveFile($"{AppDomain.CurrentDomain.BaseDirectory}1.wav"); synthesizer.SpeakAsync(Text); }) { Priority = ThreadPriority.Highest };
                mainWorker.Start(); mainWorker.Join();
            }

            public void Listen(string FilePath)
            {
                recognition.RecognizeAsync();
            }

            private void RecognitionConfigure()
            {
                
                recognition = new SpeechRecognitionEngine();
                GrammarBuilder startStop = new GrammarBuilder();
                GrammarBuilder dictation = new GrammarBuilder();
                dictation.AppendWildcard();
                //dictation.AppendDictation();

                //startStop.Append(new SemanticResultKey("StartDictation", new SemanticResultValue("Start Dictation", true)));
                //startStop.Append(new SemanticResultKey("DictationInput", dictation));
                //startStop.Append(new SemanticResultKey("StopDictation", new SemanticResultValue("Stop Dictation", false)));
                Grammar grammar = new Grammar(dictation);
                grammar.Enabled = true;
                grammar.Name = "Free-Text Dictation";


                if (!recognitionReady)
                {
                    recognition.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(Recognition_LoadGrammarCompleted);
                    recognition.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(Recognition_SpeechDetected);
                    recognition.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(Recognition_SpeechHypothesized);
                    recognition.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(Recognition_SpeechRecognitionRejected);
                    recognition.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Recognition_SpeechRecognized);
                    recognition.AudioSignalProblemOccurred += new EventHandler<AudioSignalProblemOccurredEventArgs>(Recognition_AudioSignalProblemOccurred);
                }
                //GrammarBuilder builder = new GrammarBuilder();
                
                //SrgsDocument srgsDocument = new SrgsDocument($"{AppDomain.CurrentDomain.BaseDirectory}123.xml");
                //grammar = new Microsoft.Speech.Recognition.Grammar(srgsDocument);
                
                recognition.LoadGrammar(grammar);
              
                recognitionReady = true;
            }

            private void SynthesizerConfigure()
            {
                synthesizer = new Microsoft.Speech.Synthesis.SpeechSynthesizer();
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

            private void Recognition_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
            {

            }

            private void Recognition_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
            {

            }

            private void Recognition_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
            {

            }

            private void Recognition_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
            {

            }

            private void Recognition_SpeechDetected(object sender, SpeechDetectedEventArgs e)
            {

            }

            private void Recognition_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
            {

            }

            private void GetTextDictationSrgs()
            {
                GrammarBuilder startStop = new GrammarBuilder();
                GrammarBuilder dictation = new GrammarBuilder();
                dictation.AppendDictation();

                startStop.Append(new SemanticResultKey("StartDictation", new SemanticResultValue("Start Dictation", true)));
                startStop.Append(new SemanticResultKey("DictationInput", dictation));
                startStop.Append(new SemanticResultKey("EndDictation", new SemanticResultValue("Stop Dictation", false)));

                GrammarBuilder spelling = new GrammarBuilder();
                spelling.AppendDictation("spelling");
                GrammarBuilder spellingGB = new GrammarBuilder();
                spellingGB.Append(new SemanticResultKey("StartSpelling", new SemanticResultValue("Start Spelling", true)));
                spellingGB.Append(new SemanticResultKey("spellingInput", spelling));
                spellingGB.Append(new SemanticResultKey("StopSpelling", new SemanticResultValue("Stop Spelling", true)));

                GrammarBuilder both = GrammarBuilder.Add(startStop, spellingGB);

                SrgsDocument srgs = new SrgsDocument(both);

                var _write = XmlWriter.Create($"{AppDomain.CurrentDomain.BaseDirectory}dict.xml");
                srgs.WriteSrgs(_write);
                _write.Close();


                grammar = new Grammar(both);
                grammar.Enabled = true;
                grammar.Name = "Free-Text and Spelling Dictation";

                
                recognition.SetInputToDefaultAudioDevice();
                recognition.LoadGrammar(grammar);
            }

            //static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
            //{
            //    if (e.Result.Confidence > 0.7) l.Text = e.Result.Text;
            //}

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

  
}
