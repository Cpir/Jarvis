using System;
using System.Globalization;
using System.Threading;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace PRMover
{
    class Program
    {
        static void Main(string[] args)
        {
            Jarvis jarvis = new Jarvis();
            while (true)
            {
                //Console.WriteLine("Что скажем?");
                //jarvis.Voice.Speak(Console.ReadLine());
                jarvis.Voice.Listen($"{AppDomain.CurrentDomain.BaseDirectory}1.wav");
                jarvis.Voice.Speak(Console.ReadLine());
                
            }
        }
    }
}
