using System.Text.RegularExpressions;

namespace CollectW.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex RegexExpression = new Regex(@"\/(?<expression>.+)\/",
            RegexOptions.IgnoreCase);

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        
        public static Regex AsRegex(this string value)
        {
            var match = RegexExpression.Match(value);
            if (match.Success)
            {
                return new Regex(match.Groups["expression"].Value, RegexOptions.IgnoreCase);    
            }
            return null;
        }
    }
}