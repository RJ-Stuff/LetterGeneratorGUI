using Newtonsoft.Json.Linq;
using System.IO;

namespace Letters
{
    public class Utils
    {
        public static JToken LoadConfiguration(string configurationPath)
        {
            return JToken.Parse(File.ReadAllText(configurationPath));
        }
    }
}
