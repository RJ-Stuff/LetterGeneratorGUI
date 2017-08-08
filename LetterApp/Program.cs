namespace LetterApp
{
    using System;
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

            var mainWindow = new MainWindow();
            new MainWindowPresenter(mainWindow);
            
            Application.Run(mainWindow);
        }
    }
}
