namespace GenericTxtDb
{
    public static class Settings
    {
        internal const string DEFAULT_TABLEFILESEPARATOR = "|!|";
        internal static string DBPath { get; set; }
        public static void SetDBPath(string dbPath)
        {
            DBPath = dbPath;
        }
    }
}