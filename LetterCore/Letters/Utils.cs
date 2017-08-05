namespace LetterCore.Letters
{
    using System.IO;

    using Newtonsoft.Json.Linq;

    public class Utils
    {
        public static JToken LoadConfiguration(string configurationPath)
        {
            return JToken.Parse(File.ReadAllText(configurationPath));
        }
    }
}
