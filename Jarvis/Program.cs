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
            jarvis.Voice.SetDictationMode();
            Console.ReadLine();
        }
    }
}
