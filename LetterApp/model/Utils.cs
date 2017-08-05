namespace LetterApp.model
{
    using System.Windows.Forms;

    class Utils
    {
        public static bool Validate(TextBox control)
        {
            // todo hacer mejor validación
            return control == null || control.Text.Trim().Length != 0;
        }
    }
}
