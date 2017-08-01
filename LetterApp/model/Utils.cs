using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LetterApp.model
{
    class Utils
    {
        public static bool Validate(TextBox control)
        {
            //todo hacer mejor validación
            return control == null || control.Text.Trim().Length != 0;
        }
    }
}
