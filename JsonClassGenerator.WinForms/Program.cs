using System;
using System.Windows.Forms;

namespace Xamasoft.JsonClassGenerator.WinForms
{
    public static class Program
    {
        /// <summary>The main entry point for the application.</summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (MainForm form = new MainForm())
            {
                Application.Run(form);
            }
        }
    }
}
