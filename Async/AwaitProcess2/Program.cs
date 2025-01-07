using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpAwaitProcess2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // see https://docs.microsoft.com/de-de/dotnet/api/system.windows.forms.application.threadexception?view=windowsdesktop-6.0
            Application.ThreadException += new ThreadExceptionEventHandler(AsyncProcessDemo2.DlgException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AsyncProcessDemo2());
        }
    }
}
