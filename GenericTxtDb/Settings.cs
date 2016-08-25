namespace GenericTxtDb
{
    public static class Settings
    {
        internal static string DBPath { get; set; }
        public static void SetDBPath(string dbPath)
        {
            DBPath = dbPath;
        }
    }
}