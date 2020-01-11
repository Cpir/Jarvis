using System;
using System.Globalization;
using System.Threading;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace Jarvis
{
    class Program
    {
        static bool completed;

        static void Main(string[] args)
        {
            CultureInfo culture = new CultureInfo("ru-RU");

            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

            Grammar testGrammar =
             new Grammar(new GrammarBuilder("ЕЕЕЕИИБААААТЬ КОПАТЬ")) { Name = "Test Grammar" };
            recognizer.LoadGrammar(testGrammar);

            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.EmulateRecognizeCompleted += Recognizer_EmulateRecognizeCompleted;

            recognizer.SetInputToWaveFile($"{AppDomain.CurrentDomain.BaseDirectory}1.wav");

            recognizer.Recognize();
            completed = false;

            // Start asynchronous emulated recognition.   
            // This matches the grammar and generates a SpeechRecognized event.  

            // Wait for the asynchronous operation to complete.  



            static void Recognizer_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
            {
                throw new NotImplementedException();
            }

            static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
            {
                throw new NotImplementedException();
            }
            while (true)
            {
                Thread.Sleep(0);
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();



            // Handle the SpeechRecognized event.  
            //static void SpeechRecognizedHandler(
            //  object sender, SpeechRecognizedEventArgs e)
            //{
            //    if (e.Result != null)
            //    {
            //        Console.WriteLine("Recognition result = {0}",
            //          e.Result.Text ?? "<no text>");
            //    }
            //    else
            //    {
            //        Console.WriteLine("No recognition result");
            //    }
            //}

            //// Handle the SpeechRecognizeCompleted event.  
            //static void EmulateRecognizeCompletedHandler(
            //  object sender, EmulateRecognizeCompletedEventArgs e)
            //{
            //    if (e.Result == null)
            //    {
            //        Console.WriteLine("No result generated.");
            //    }

            //    // Indicate the asynchronous operation is complete.  
            //    completed = true;
            //}


        }
    }
}
