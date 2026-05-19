using System.Text.RegularExpressions;

namespace PrimeMarket.Helpers
{
    public class GenerateSlug
    {
        public static string Generate(string name)
        {
            return Regex.Replace(name.Trim().ToLower(), @"\s+", "-");
        }
    }
}
