using System;
using System.Windows.Forms;

namespace AxonSoft.Assesment
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            Application.Run(new Form1());
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs ex)
        {
            Exception exception = (Exception)ex.ExceptionObject;
            MessageBox.Show(exception.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
