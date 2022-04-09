namespace Bdiebeak.InputSystemGeneration
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            str = char.ToUpperInvariant(str[0]) + str.Substring(1);
            for (var i = 0; i < str.Length; i++)
            {
                var symbol = str[i];
                if (symbol == ' ' || symbol == '_')
                {
                    str = str.Remove(i, 1);
                    str = str.Substring(0, i) + char.ToUpperInvariant(str[i]) + str.Substring(i + 1);
                    i--;
                }
            }

            return str;
        }
    }
}