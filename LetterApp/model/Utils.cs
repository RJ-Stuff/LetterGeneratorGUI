namespace LetterApp.model
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    class Utils
    {
        public static bool Validate(Tuple<TextBox, string, string, string> tuple)
        {
            var control = tuple.Item1;
            var validate = tuple.Item3;
            var rgx = new Regex(validate);
            var text = control?.Text.Trim() ?? string.Empty;

            return control == null || (!string.IsNullOrEmpty(text) && rgx.IsMatch(text));
        }
    }
}
