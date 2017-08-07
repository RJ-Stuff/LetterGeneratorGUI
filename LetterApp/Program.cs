namespace LetterApp
{
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    using LetterApp.view;

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

            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["culture"]))
            //{
            //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["culture"]);
            //}

            var mainWindow = new MainWindow();
            new MainWindowPresenter(mainWindow);
            
            Application.Run(mainWindow);
        }
    }
}
