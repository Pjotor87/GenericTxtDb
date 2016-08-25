using System.IO;
using System.Linq;

namespace Tests
{
    internal sealed class Constants
    {
        internal sealed class TestData
        {
            internal const string BIN_DEBUG_RELATIVE_PATH_ADJUSTMENT = "..\\..\\";
            internal const string TESTDATA_FOLDERNAME = "TestData";
            internal static string TESTDATA_PATH
            {
                get
                {
                    return 
                        string.Concat(
                            BIN_DEBUG_RELATIVE_PATH_ADJUSTMENT, 
                            TESTDATA_FOLDERNAME
                        );
                }
            }

            internal const string LIST_FILENAME = "List.txt";
            internal const string KEYVALUEPAIR_FILENAME = "KeyValuePair.txt";
            internal const string TABLE_FILENAME = "Table.txt";
            internal static string[] FILENAMES
            {
                get
                {
                    return 
                        new string[] 
                        {
                            LIST_FILENAME,
                            KEYVALUEPAIR_FILENAME,
                            TABLE_FILENAME
                        };
                }
            }
            internal static string[] FILEPATHS
            {
                get
                {
                    return 
                        FILENAMES.Select(
                            x => 
                            string.Concat(
                                BIN_DEBUG_RELATIVE_PATH_ADJUSTMENT, 
                                TESTDATA_FOLDERNAME, 
                                Path.DirectorySeparatorChar, 
                                x
                            )
                        ).ToArray();
                }
            }

            internal const string DBNAME = "Db";
            internal static string DBPATH
            {
                get
                {
                    return 
                        string.Concat(
                            BIN_DEBUG_RELATIVE_PATH_ADJUSTMENT, 
                            TESTDATA_FOLDERNAME, 
                            Path.DirectorySeparatorChar, 
                            DBNAME
                        );
                }
            }

            internal static string TEMPFILENAME_PREFIX = "Temp_";
        }
    }
}
