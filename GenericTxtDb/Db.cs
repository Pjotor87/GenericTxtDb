using System.Collections.Generic;
using System.Linq;
using FileTree;
using System.IO;

namespace GenericTxtDb
{
    public class Db
    {
        public Folder DbFolder { get; set; }
        public IList<ListFile> ListFiles { get; set; }
        public IList<KeyValuePairFile> KeyValuePairFiles { get; set; }
        public IList<TableFile> TableFiles { get; set; }

        public Db()
        {
            this.DbFactory("Db", Settings.DEFAULT_TABLEFILESEPARATOR);
        }

        public Db(string dbPath)
        {
            this.DbFactory(dbPath, Settings.DEFAULT_TABLEFILESEPARATOR);
        }

        public Db(string dbPath, string tableFileSeparator)
        {
            this.DbFactory(dbPath, tableFileSeparator);
        }

        private void DbFactory(string dbPath, string tableFileSeparator)
        {
            this.DbFolder = Directory.Exists(dbPath) ? new Folder(dbPath) : null;
            this.ListFiles = new List<ListFile>();
            this.KeyValuePairFiles = new List<KeyValuePairFile>();
            this.TableFiles = new List<TableFile>();

            {// Identify file type by looking at its data and checking the first 5 lines.
                if (this.DbFolder != null)
                    foreach (string file in this.DbFolder.Files)
                    {
                        IDictionary<FileType, bool> fileTypePossibility = new Dictionary<FileType, bool>()
                        {
                            { FileType.List, true },
                            { FileType.KeyValuePair, true },
                            { FileType.Table, true }
                        };

                        string filename = Path.GetFileName(file);
                        ListFile unidentifiedFile = new ListFile(filename);
                        if (unidentifiedFile.Data.Length > 0)
                        {
                            int linesToCheck = unidentifiedFile.Data.Length < 5 ? unidentifiedFile.Data.Length : 5;
                            for (int i = 0; i < linesToCheck; i++)
                            {
                                if (!unidentifiedFile.Data[i].Contains('='))
                                    fileTypePossibility[FileType.KeyValuePair] = false;
                                if (!unidentifiedFile.Data[i].Contains(tableFileSeparator))
                                    fileTypePossibility[FileType.Table] = false;
                            }
                        }
                        else
                        {
                            fileTypePossibility[FileType.KeyValuePair] = false;
                            fileTypePossibility[FileType.Table] = false;
                        }

                        if (fileTypePossibility[FileType.Table])
                            this.TableFiles.Add(new TableFile(filename));
                        else if (fileTypePossibility[FileType.KeyValuePair])
                            this.KeyValuePairFiles.Add(new KeyValuePairFile(filename));
                        else
                            this.ListFiles.Add(unidentifiedFile);
                    }
            }
        }
    }
}