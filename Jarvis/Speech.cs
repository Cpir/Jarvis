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

        public CultureInfo CultureInfo { get; set; } = new CultureInfo("ru-RU");
        public Speech Voice { get; set; } = new Speech();

        public Jarvis()
        {
            handle = GCHandle.Alloc(this);
        }

        public class Speech : IDisposable
        {
            private GCHandle handle;
            private SpeechSynthesizer synthesizer { get; set; } = new SpeechSynthesizer();
            private SpeechRecognitionEngine recognition { get; set; }
            public Grammar DictationGrammar { get; set; }

            private Thread mainWorker, supportWorker;
            private bool recognitionReady = false, synthesizerReady = false;
            private string innerText, outerText;

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
                GetDictationGrammar();
                RecognitionConfigure();
                SynthesizerConfigure();
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
                recognition.SetInputToNull();
                recognition.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(Recognition_LoadGrammarCompleted);
                recognition.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(Recognition_SpeechDetected);
                recognition.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(Recognition_SpeechHypothesized);
                recognition.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(Recognition_SpeechRecognitionRejected);
                recognition.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Recognition_SpeechRecognized);
                recognition.AudioSignalProblemOccurred += new EventHandler<AudioSignalProblemOccurredEventArgs>(Recognition_AudioSignalProblemOccurred);
                recognitionReady = true;
            }

            private void SynthesizerConfigure()
            {
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

            private void GetDictationGrammar()
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

                var _g = new Grammar(GrammarBuilder.Add(startStop, spellingGB));
                _g.Enabled = true;
                _g.Name = "Free-Text and Spelling Dictation";

                SrgsDocument srgs = new SrgsDocument(GrammarBuilder.Add(startStop, spellingGB));
                var _write = XmlWriter.Create($"{AppDomain.CurrentDomain.BaseDirectory}dict.xml");
                srgs.WriteSrgs(_write);
                _write.Close();


                DictationGrammar = _g;
            }

            public void SetDictationMode()
            {
                if (recognition == null)
                    throw new NullReferenceException(nameof(recognition));
                if (this.DictationGrammar == null)
                    throw new NullReferenceException(nameof(DictationGrammar));
                DictationGrammar.Enabled = true;
                recognition.LoadGrammar(DictationGrammar);
            }

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

            #region События распозновальщика речи
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
