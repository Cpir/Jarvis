using System;
using System.Globalization;
using System.Threading;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using Plugin.TextToSpeech;

namespace PRMover
{
    class Program
    {
        static void Main(string[] args)
        {
            CrossTextToSpeech.Current.Speak("Text to speak");
            
            Console.ReadLine();
            Jarvis jarvis = new Jarvis();
            while (true)
            {
                //Console.WriteLine("Что скажем?");
                //jarvis.Voice.Speak(Console.ReadLine());
                //jarvis.Voice.Listen($"{AppDomain.CurrentDomain.BaseDirectory}1.wav");
                
                jarvis.Voice.Speak(Console.ReadLine());
            }
           
        }
    }
}
