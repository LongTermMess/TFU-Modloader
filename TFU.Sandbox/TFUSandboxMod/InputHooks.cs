using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFUSandboxMod
{
    public static class InputHooks
    {
        static public Action<ConsoleKey> InputDown = new Action<ConsoleKey>(InputDownEvent);
        static public Action<ConsoleKey> InputUp = new Action<ConsoleKey>(InputUpEvent);
        static ILogger logger;
        static public bool InputLock = false;
        public static void Init(ILogger log)
        {
            logger = log;
        }

        static void InputDownEvent(ConsoleKey KeyId)
        {
            logger.WriteLine("InputDownEvent");
            //logger.WriteLine($"InputDown {KeyId}");
        }
        static void InputUpEvent(ConsoleKey KeyId)
        {
            //logger.WriteLine($"InputUp {KeyId}");
        }








    }
}
