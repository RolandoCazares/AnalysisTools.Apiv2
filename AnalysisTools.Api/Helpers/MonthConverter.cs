namespace analysistools.api.Helpers
{
    /// <summary>
    /// Translate english abreviated months to spanish.
    /// Local optical stations store images with abreviated spanish month names.
    /// </summary>
    public static class MonthConverter
    {
        public static string EngToEsp(string month)
        {
            string translatedMoth = month;
            switch (month)
            {
                case "JAN":
                    translatedMoth = "ENE";
                    break;
                case "APR":
                    translatedMoth = "ABR";
                    break;
                case "AUG":
                    translatedMoth = "AGO";
                    break;
                case "DEC":
                    translatedMoth = "DIC";
                    break;
            }
            return translatedMoth;
        }
    }
}
