using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FNMES.Utility.Other
{
    public static class TTSUtils
    {
        private static bool canSpeak = true;
        public static void Speak(this string str)
        {
            new Thread(() =>
            {
                if (canSpeak)
                { 
                    canSpeak = false;
                    try
                    {
                        Type type = Type.GetTypeFromProgID("SAPI.SpVoice");
                        dynamic spVoice = Activator.CreateInstance(type);
                        spVoice.Speak(str);
                    }
                    catch
                    {

                    }
                    canSpeak = true;
                }

            }).Start();

        }

    }
}
