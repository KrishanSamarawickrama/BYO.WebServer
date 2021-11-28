using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYO.WebServer.Helpers
{
    internal static class ConsoleHelper
    {
        internal static void ConsoleWriteException(Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }
    }
}
