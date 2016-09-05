﻿using System.Collections.Generic;
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
        public string DbBackupDirectoryName { get; set; }
        public IList<DateTime> DbBackups { get; set; }

        public Db()
        {
            this.DbFactory(string.Concat(Environment.CurrentDirectory, Path.DirectorySeparatorChar, "Db"), string.Empty, string.Empty, false);
        }

        public Db(bool tryInitialize)
        {
            this.DbFactory(string.Concat(Environment.CurrentDirectory, Path.DirectorySeparatorChar, "Db"), string.Empty, string.Empty, tryInitialize);
        }

        public Db(bool tryInitialize, string dbBackupDirectoryName)
        {
            this.DbFactory(string.Concat(Environment.CurrentDirectory, Path.DirectorySeparatorChar, "Db"), dbBackupDirectoryName, string.Empty, tryInitialize);
        }

        public Db(string dbPath)
        {
            this.DbFactory(dbPath, string.Empty, string.Empty, false);
        }

        public Db(string dbPath, bool tryInitialize)
        {
            this.DbFactory(dbPath, string.Empty, string.Empty, tryInitialize);
        }

        public Db(string dbPath, bool tryInitialize, string dbBackupDirectoryName)
        {
            this.DbFactory(dbPath, dbBackupDirectoryName, string.Empty, tryInitialize);
        }

        public Db(string dbPath, string tableFileSeparator)
        {
            this.DbFactory(dbPath, string.Empty, tableFileSeparator, false);
        }
        
        public Db(string dbPath, string dbBackupDirectoryName, string tableFileSeparator)
        {
            this.DbFactory(dbPath, dbBackupDirectoryName, tableFileSeparator, false);
        }

        public Db(string dbPath, string tableFileSeparator, bool tryInitialize)
        {
            this.DbFactory(dbPath, string.Empty, tableFileSeparator, tryInitialize);
        }

        public Db(string dbPath, string dbBackupDirectoryName, string tableFileSeparator, bool tryInitialize)
        {
            this.DbFactory(dbPath, dbBackupDirectoryName, tableFileSeparator, tryInitialize);
        }

        private void DbFactory(string dbPath, string dbBackupDirectoryName, string tableFileSeparator, bool tryInitialize)
        {
            if (string.IsNullOrEmpty(tableFileSeparator))
                tableFileSeparator = "|!|";

            this.DBPath = dbPath;
            this.DbFolder = Directory.Exists(this.DBPath) ? new Folder(this.DBPath) : null;
            this.Initialized = this.DbFolder != null;
            if (!this.Initialized && tryInitialize)
                TryInitialize();

            this.ListFiles = new List<ListFile>();
            this.KeyValuePairFiles = new List<KeyValuePairFile>();
            this.TableFiles = new List<TableFile>();
            if (this.DbFolder != null)
                foreach (string file in this.DbFolder.Files)
                    this.AddFile(file, IdentifyFileType(file, tableFileSeparator));

            this.SetBackupDirectoryName(!string.IsNullOrEmpty(dbBackupDirectoryName) ? dbBackupDirectoryName : "DB_Backups");
            this.DbBackups = new List<DateTime>();
            if (this.DbFolder != null)
            {
                Folder parentFolder = this.DbFolder.ParentFolder;
                ICollection<Folder> parentFolderSubFolders = parentFolder.SubFolders;
                Folder backupsFolder = parentFolderSubFolders.Where(x => x.Name == this.DbBackupDirectoryName).SingleOrDefault();

                if (backupsFolder != null && backupsFolder.HasSubFolders)
                    foreach (Folder backup in backupsFolder.SubFolders)
                    {
                        string dateTime = backup.Name.Substring(backup.Name.LastIndexOf("_-_") + "_-_".Length);
                        string date = dateTime.Split('_')[0];
                        string time = dateTime.Split('_')[1];
                        this.DbBackups.Add(
                            new DateTime(
                                Convert.ToInt32(date.Split('-')[0]),
                                Convert.ToInt32(date.Split('-')[1]),
                                Convert.ToInt32(date.Split('-')[2]),
                                Convert.ToInt32(time.Split('-')[0]),
                                Convert.ToInt32(time.Split('-')[1]),
                                Convert.ToInt32(time.Split('-')[2])
                            )
                        );
                    }
            }
        }

        public void SetBackupDirectoryName(string dbBackupDirectoryName)
        {
            this.DbBackupDirectoryName = dbBackupDirectoryName;
        }

        private void TryInitialize()
        {
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
        }

        public FileType IdentifyFileType(string filePath, string tableFileSeparator)
        {
            IDictionary<FileType, bool> fileTypePossibility = new Dictionary<FileType, bool>()
            {
                { FileType.List, true },
                { FileType.KeyValuePair, true },
                { FileType.Table, true }
            };
            
            ListFile unidentifiedFile = new ListFile(filePath);

            if (unidentifiedFile.Data != null && unidentifiedFile.Data.Count > 0)
            {
                int linesToCheck = unidentifiedFile.Data.Count < 5 ? unidentifiedFile.Data.Count : 5;
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

            return
                (fileTypePossibility[FileType.Table]) ? FileType.Table :
                (fileTypePossibility[FileType.KeyValuePair]) ? FileType.KeyValuePair :
                FileType.List;
        }

        private void AddFile(string filePath, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.List:
                    this.ListFiles.Add(new ListFile(filePath));
                    break;
                case FileType.KeyValuePair:
                    this.KeyValuePairFiles.Add(new KeyValuePairFile(filePath));
                    break;
                case FileType.Table:
                    this.TableFiles.Add(new TableFile(filePath));
                    break;
            }
        }

        public void AddFileByFilename(string filename, FileType fileType)
        {
            this.AddFile(
                !filename.StartsWith(this.DBPath) ? Path.Combine(this.DBPath, filename) : filename,
                fileType
            );
        }

        public void CreateDbBackup()
        {
            DateTime now = DateTime.Now;
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory(
                this.DBPath,
                Path.Combine(
                    Path.Combine(
                        Path.GetDirectoryName(this.DBPath), 
                        this.DbBackupDirectoryName
                    ),
                    string.Concat(
                        Path.GetFileNameWithoutExtension(this.DBPath),
                        "_-_",
                        now.ToString("yyyy-MM-dd_hh-mm-ss")
                    )
                )
            );
            this.DbBackups.Add(now);
        }

        public ListFile GetListFile(string filenameWithoutExtension)
        {
            return this.ListFiles.Where(x => x.FilePath.EndsWith(string.Concat(filenameWithoutExtension, ".txt"))).SingleOrDefault();
        }

        public KeyValuePairFile GetKeyValuePairFile(string filenameWithoutExtension)
        {
            return this.KeyValuePairFiles.Where(x => x.FilePath.EndsWith(string.Concat(filenameWithoutExtension, ".txt"))).SingleOrDefault();
        }

        public TableFile GetTableFile(string filenameWithoutExtension)
        {
            return this.TableFiles.Where(x => x.FilePath.EndsWith(string.Concat(filenameWithoutExtension, ".txt"))).SingleOrDefault();
        }

        public bool Exists(string filenameWithoutExtension)
        {
            return
                this.GetListFile(filenameWithoutExtension) != null ||
                this.GetKeyValuePairFile(filenameWithoutExtension) != null ||
                this.GetTableFile(filenameWithoutExtension) != null;
        }
    }
}