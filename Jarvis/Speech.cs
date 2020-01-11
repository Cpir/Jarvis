using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Jarvis
{
    public class Speech
    {
        private GCHandle handle;
        private CultureInfo cultureInfo;
        private SpeechRecognitionEngine speechRecognitionEngine;

        public Speech()
        {
            handle = GCHandle.Alloc(this);
            cultureInfo = new CultureInfo("ru-RU");
        }

        public void Speak(string FilePath)
        {



        }



        private void SpeechEngineConfigure()
        {

        }
    }
}
