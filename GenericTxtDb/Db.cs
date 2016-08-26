using System.Collections.Generic;
using System.Linq;
using FileTree;
using System.IO;
using System;

namespace GenericTxtDb
{
    public class Db
    {
        private Folder DbFolder { get; set; }
        public string DBPath { get; private set; }
        public bool Initialized { get; set; }
        public IList<ListFile> ListFiles { get; set; }
        public IList<KeyValuePairFile> KeyValuePairFiles { get; set; }
        public IList<TableFile> TableFiles { get; set; }

        public Db()
        {
            this.DbFactory("Db", string.Empty, false);
        }

        public Db(bool tryInitialize)
        {
            this.DbFactory("Db", string.Empty, tryInitialize);
        }

        public Db(string dbPath)
        {
            this.DbFactory(dbPath, string.Empty, false);
        }

        public Db(string dbPath, bool tryInitialize)
        {
            this.DbFactory(dbPath, string.Empty, tryInitialize);
        }

        public Db(string dbPath, string tableFileSeparator)
        {
            this.DbFactory(dbPath, tableFileSeparator, false);
        }

        public Db(string dbPath, string tableFileSeparator, bool tryInitialize)
        {
            this.DbFactory(dbPath, tableFileSeparator, tryInitialize);
        }

        private void DbFactory(string dbPath, string tableFileSeparator, bool tryInitialize)
        {
            if (string.IsNullOrEmpty(tableFileSeparator))
                tableFileSeparator = "|!|";
            this.DBPath = dbPath;
            this.DbFolder = Directory.Exists(this.DBPath) ? new Folder(this.DBPath) : null;
            this.Initialized = this.DbFolder != null;
            if (!this.Initialized && tryInitialize)
                try
                {
                    Directory.CreateDirectory(this.DBPath);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    this.DbFolder = Directory.Exists(this.DBPath) ? new Folder(this.DBPath) : null;
                    this.Initialized = this.DbFolder != null;
                }
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

                        if (unidentifiedFile.Data != null && unidentifiedFile.Data.Length > 0)
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