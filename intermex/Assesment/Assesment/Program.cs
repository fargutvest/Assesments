using System;
using System.Threading;
using System.Windows.Forms;

namespace Assesment
{
    internal static class Program
    {
        private static string appGuid = "DE6373EA-8DA2-4E1E-87EA-60A38F14ED86";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (mutex.WaitOne(0, false) == false)
                {
                    MessageBox.Show("Instance already running", "Find files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Application.Run(new Form());
            }
         
        }
    }
}
