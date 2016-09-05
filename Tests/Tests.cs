using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using GenericTxtDb;
using System.Collections.Generic;
using FileTree;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        #region Initialize

        [TestInitialize]
        public void TestdataExists()
        {
            // TestdataExists
            foreach (string filepath in Constants.TestData.FILEPATHS)
                Assert.IsTrue(File.Exists(filepath));
            // TestdataContainsData
            foreach (string filepath in Constants.TestData.FILEPATHS)
                Assert.IsFalse(string.IsNullOrEmpty(File.ReadAllText(filepath)));
            // Ensure DBExists
            if (!Directory.Exists(Constants.TestData.DBPATH))
                Directory.CreateDirectory(Constants.TestData.DBPATH);
            // Ensure DB has testdata files
            foreach (string testdataFilepath in Constants.TestData.FILEPATHS)
                File.Copy(testdataFilepath, string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Path.GetFileName(testdataFilepath)), true);
            // Ensure DB has tempfiles (Used in tests for modifying db files)
            foreach (string testdataFilepath in Constants.TestData.FILEPATHS)
                File.Copy(testdataFilepath, string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Path.GetFileName(testdataFilepath)), true);
        }

        [TestMethod]
        public void DBCreatedByTestInitialization()
        {
            Assert.IsTrue(Directory.Exists(Constants.TestData.DBPATH));
            Assert.IsNotNull(Directory.GetFiles(Constants.TestData.DBPATH));
        }

        #endregion

        #region File

        [TestMethod]
        public void CanReadFilesInDB()
        {
            foreach (string filename in Constants.TestData.FILENAMES)
            {
                ListFile file = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, filename));
                Assert.IsTrue(File.Exists(file.FilePath));
                ListFile tempfile = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, filename));
                Assert.IsTrue(File.Exists(tempfile.FilePath));
            }
        }

        #endregion

        #region List

        [TestMethod]
        public void ListLine1Matches()
        {
            string expected = "One value";
            ListFile file = new ListFile(string.Concat(Constants.TestData.DBFILEPATHS[0]));
            Assert.AreEqual(expected, file.Data[0]);
        }

        [TestMethod]
        public void ListLine2Matches()
        {
            string expected = "Another value";
            ListFile file = new ListFile(string.Concat(Constants.TestData.DBFILEPATHS[0]));
            Assert.AreEqual(expected, file.Data[1]);
        }

        [TestMethod]
        public void ListLine3Matches()
        {
            string expected = "           A third value with leading and trailing whitespace            ";
            ListFile file = new ListFile(string.Concat(Constants.TestData.DBFILEPATHS[0]));
            Assert.AreEqual(expected, file.Data[2]);
        }

        [TestMethod]
        public void CanAddAndRemoveNewEntryInListFile()
        {
            string newEntry = "UnitTestEntry";
            ListFile tempFile = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
            {// ADD ENTRY
                Assert.IsFalse(tempFile.Data.Contains(newEntry));
                tempFile.Add(newEntry);
                Assert.IsTrue(tempFile.Data.Contains(newEntry));
            }
            {// COMMIT
                ListFile tempFile2 = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
                Assert.IsFalse(tempFile2.Data.Contains(newEntry));
                tempFile.Commit();
                Assert.IsTrue(tempFile.Data.Contains(newEntry));
                ListFile tempFile3 = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
                Assert.IsTrue(tempFile3.Data.Contains(newEntry));
            }
            {// REMOVE ENTRY
                ListFile tempFile4 = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
                Assert.IsTrue(tempFile4.Data.Contains(newEntry));
                tempFile4.Remove(newEntry);
                Assert.IsFalse(tempFile4.Data.Contains(newEntry));
                ListFile tempFile5 = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
                Assert.IsTrue(tempFile5.Data.Contains(newEntry));
                tempFile4.Commit();
                ListFile tempFile6 = new ListFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.LIST_FILENAME));
                Assert.IsFalse(tempFile6.Data.Contains(newEntry));
            }
        }

        #endregion

        #region KeyValuePair

        [TestMethod]
        public void KeyValuePairLine1Matches()
        {
            string expectedLine = "\"One key\"=\"One value\"";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[0]);
            string expectedKey = "One key";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[0].Key);
            string expectedValue = "One value";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[0].Value);
        }

        [TestMethod]
        public void KeyValuePairLine2Matches()
        {
            string expectedLine = "Another key\"=Another value";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[1]);
            string expectedKey = "Another key";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[1].Key);
            string expectedValue = "Another value";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[1].Value);
        }

        [TestMethod]
        public void KeyValuePairLine3Matches()
        {
            string expectedLine = "A third key=A third value";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[2]);
            string expectedKey = "A third key";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[2].Key);
            string expectedValue = "A third value";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[2].Value);
        }

        [TestMethod]
        public void KeyValuePairLine4Matches()
        {
            string expectedLine = "\"A fourth\" ke\"y\"=\"A fourth value\"\"";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[3]);
            string expectedKey = "A fourth\" ke\"y";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[3].Key);
            string expectedValue = "A fourth value\"";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[3].Value);
        }

        [TestMethod]
        public void KeyValuePairLine5Matches()
        {
            string expectedLine = "A fifth= key=A fifth= value";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[4]);
            foreach (KeyValuePair<string, string> kvPair in file.KeyValuePairs)
                if (kvPair.Key.Contains("fifth"))
                    Assert.Fail("The key value pair should not be added");
        }

        [TestMethod]
        public void KeyValuePairLine6Matches()
        {
            string expectedLine = "\"A sixth= key\"=\"A sixth val=ue\"";
            KeyValuePairFile file = new KeyValuePairFile(string.Concat(Constants.TestData.DBFILEPATHS[1]));
            Assert.AreEqual(expectedLine, file.Data[5]);
            string expectedKey = "A sixth= key";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[4].Key);
            string expectedValue = "A sixth val=ue";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[4].Value);
        }

        [TestMethod]
        public void CanAddAndRemoveNewEntryInKeyValuePairFile1()
        {
            KeyValuePair<string, string> newEntry = new KeyValuePair<string, string>("UnitTestKey", "UnitTestValue");
            
            KeyValuePairFile tempFile = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
            {// ADD ENTRY
                Assert.IsFalse(tempFile.KeyValuePairs.Contains(newEntry));
                tempFile.Add(newEntry);
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
            }
            {// COMMIT
                KeyValuePairFile tempFile2 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile2.KeyValuePairs.Contains(newEntry));
                tempFile.Commit();
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile3 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile3.KeyValuePairs.Contains(newEntry));
            }
            {// REMOVE ENTRY
                KeyValuePairFile tempFile4 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile4.KeyValuePairs.Contains(newEntry));
                tempFile4.Remove(newEntry);
                Assert.IsFalse(tempFile4.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile5 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile5.KeyValuePairs.Contains(newEntry));
                tempFile4.Commit();
                KeyValuePairFile tempFile6 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile6.KeyValuePairs.Contains(newEntry));
            }
        }

        [TestMethod]
        public void CanAddAndRemoveNewEntryInKeyValuePairFile2()
        {
            KeyValuePair<string, string> newEntry = new KeyValuePair<string, string>("UnitTestKey", string.Empty);

            KeyValuePairFile tempFile = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
            {// ADD ENTRY
                Assert.IsFalse(tempFile.KeyValuePairs.Contains(newEntry));
                tempFile.Add(newEntry.Key);
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
            }
            {// COMMIT
                KeyValuePairFile tempFile2 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile2.KeyValuePairs.Contains(newEntry));
                tempFile.Commit();
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile3 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile3.KeyValuePairs.Contains(newEntry));
            }
            {// REMOVE ENTRY
                KeyValuePairFile tempFile4 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile4.KeyValuePairs.Contains(newEntry));
                tempFile4.Remove(newEntry.Key);
                Assert.IsFalse(tempFile4.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile5 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile5.KeyValuePairs.Contains(newEntry));
                tempFile4.Commit();
                KeyValuePairFile tempFile6 = new KeyValuePairFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile6.KeyValuePairs.Contains(newEntry));
            }
        }

        #endregion

        #region Table

        [TestMethod]
        public void TableLine1Matches()
        {
            string expectedLine = "\"FirstRowValue1\"|!|\"FirstRowValue2\"|!|\"FirstRowValue3\"";
            TableFile file = new TableFile(string.Concat(Constants.TestData.DBFILEPATHS[2]));
            Assert.AreEqual(expectedLine, file.Data[0]);

            IList<string> expectedCellValues = new List<string> { "FirstRowValue1", "FirstRowValue2", "FirstRowValue3" };
            Assert.AreEqual(expectedCellValues.Count, file.TableRows[0].Count);
            for (int i = 0; i < expectedCellValues.Count; i++)
                Assert.AreEqual(expectedCellValues[i], file.TableRows[0][i]);
        }

        [TestMethod]
        public void TableLine2Matches()
        {
            string expectedLine = "\"SecondRowValue1\"|!|\"SecondRowValue2\"|!|\"SecondRowValue3\"";
            TableFile file = new TableFile(string.Concat(Constants.TestData.DBFILEPATHS[2]));
            Assert.AreEqual(expectedLine, file.Data[1]);

            IList<string> expectedCellValues = new List<string> { "SecondRowValue1", "SecondRowValue2", "SecondRowValue3" };
            Assert.AreEqual(expectedCellValues.Count, file.TableRows[1].Count);
            for (int i = 0; i < expectedCellValues.Count; i++)
                Assert.AreEqual(expectedCellValues[i], file.TableRows[1][i]);
        }

        [TestMethod]
        public void CanAddAndRemoveNewEntryInTableFile()
        {
            string[] newEntry = new string[] { "UnitTestKey", "UnitTestValue1", "UnitTestValue2" };

            TableFile tempFile = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
            {// ADD ENTRY
                Assert.IsFalse(tempFile.KeyExists(newEntry[0]));
                tempFile.Add(newEntry);
                Assert.IsTrue(tempFile.KeyExists(newEntry[0]));
            }
            {// COMMIT
                TableFile tempFile2 = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
                Assert.IsFalse(tempFile2.KeyExists(newEntry[0]));
                tempFile.Commit();
                Assert.IsTrue(tempFile.KeyExists(newEntry[0]));
                TableFile tempFile3 = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
                Assert.IsTrue(tempFile3.KeyExists(newEntry[0]));
                Assert.IsTrue(tempFile3.Data.Contains(string.Concat(newEntry[0], "|!|", newEntry[1], "|!|", newEntry[2])));
            }
            {// REMOVE ENTRY
                TableFile tempFile4 = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
                Assert.IsTrue(tempFile4.KeyExists(newEntry[0]));
                tempFile4.Remove(newEntry[0]);
                Assert.IsFalse(tempFile4.KeyExists(newEntry[0]));
                TableFile tempFile5 = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
                Assert.IsTrue(tempFile5.KeyExists(newEntry[0]));
                tempFile4.Commit();
                TableFile tempFile6 = new TableFile(string.Concat(Constants.TestData.DBPATH, Path.DirectorySeparatorChar, Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.TABLE_FILENAME));
                Assert.IsFalse(tempFile6.KeyExists(newEntry[0]));
            }
        }

        #endregion

        #region Db

        [TestMethod]
        public void CanInitializeDefaultDbWhenDbDoesNotExist()
        {
            Db db = new Db();
            Assert.IsNotNull(db);
        }

        [TestMethod]
        public void CanCreateDefaultDbWhenDbDoesNotExistUsingTryInitializeFlag()
        {
            Db db1 = new Db(false);
            if (db1.Initialized)
                Directory.Delete(db1.DBPath, true);
            Db db2 = new Db(false);
            Assert.IsFalse(db2.Initialized);
            Db db3 = new Db(true);
            Assert.IsTrue(db3.Initialized);
            Db db4 = new Db(true);
            Assert.IsTrue(db4.Initialized);
        }

        [TestMethod]
        public void DbIdentifiesFiles()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int expectedListFiles = 2;
            int expectedKeyValuePairFiles = 2;
            int expectedTableFiles = 2;
            Assert.IsFalse(db.ListFiles.Count == expectedListFiles + expectedKeyValuePairFiles + expectedTableFiles);
            Assert.AreEqual(expectedListFiles, db.ListFiles.Count);
            Assert.AreEqual(expectedKeyValuePairFiles, db.KeyValuePairFiles.Count);
            Assert.AreEqual(expectedTableFiles, db.TableFiles.Count);
        }

        [TestMethod]
        public void CanCreateDbBackupUsingCustomPath()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int expectedFileCount = (db.ListFiles.Count + db.KeyValuePairFiles.Count + db.TableFiles.Count);
            Assert.IsTrue(expectedFileCount > 0);

            db.SetBackupDirectoryName(Path.GetFileName(Constants.TestData.DBBACKUPPATH));
            db.CreateDbBackup();

            Assert.IsTrue(Directory.Exists(Constants.TestData.DBBACKUPPATH));
            Folder folder = new Folder(Constants.TestData.DBBACKUPPATH);
            Assert.IsTrue(folder.HasSubFolders);
            int expectedFolderCount = 1;
            Assert.AreEqual(expectedFolderCount, folder.SubFolders.Count);
            int actualFileCount = 0;
            bool foundBackupsFolderMaybe = false;
            foreach (var subfolder in folder.SubFolders)
            {
                if (subfolder.HasFiles)
                    foundBackupsFolderMaybe = true;
                Assert.IsTrue(foundBackupsFolderMaybe);
                foreach (var file in subfolder.Files)
                    actualFileCount++;
            }
            Assert.IsTrue(foundBackupsFolderMaybe);
            Assert.AreEqual(expectedFileCount, actualFileCount);
        }

        [TestMethod]
        public void CanCreateDbBackupUsingDefaultPath()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int expectedFileCount = (db.ListFiles.Count + db.KeyValuePairFiles.Count + db.TableFiles.Count);
            Assert.IsTrue(expectedFileCount > 0);

            db.CreateDbBackup();

            Folder testDataFolder = new Folder(Constants.TestData.TESTDATA_PATH);
            bool foundBackupsFolderMaybe = false;
            foreach (var subfolder in testDataFolder.SubFolders)
                if (subfolder.HasSubFolders)
                    foreach (var subSubfolder in subfolder.SubFolders)
                    {
                        int actualFileCount = 0;
                        if (subSubfolder.HasFiles)
                            foundBackupsFolderMaybe = true;
                        Assert.IsTrue(foundBackupsFolderMaybe);
                        foreach (var file in subSubfolder.Files)
                            actualFileCount++;
                        Assert.AreEqual(expectedFileCount, actualFileCount);
                    }
            Assert.IsTrue(foundBackupsFolderMaybe);
        }

        [TestMethod]
        public void CanCreateDbBackupWhenEnvironmentCurrentDirectoryChanged()
        {
            string currentDirectory = Environment.CurrentDirectory;
            try
            {
                Assert.IsTrue(Directory.Exists(Constants.TestData.TESTDATA_PATH));
                string environmentChangedDirectory = string.Concat(Constants.TestData.TESTDATA_PATH, Path.DirectorySeparatorChar, "EnvironmentChangedDirectory");
                if (!Directory.Exists(environmentChangedDirectory))
                    Directory.CreateDirectory(environmentChangedDirectory);
                Assert.IsTrue(Directory.Exists(environmentChangedDirectory));

                string environmentChangedDbDirectory = string.Concat(environmentChangedDirectory, Path.DirectorySeparatorChar, Constants.TestData.DBNAME);
                if (!Directory.Exists(environmentChangedDbDirectory))
                    Directory.CreateDirectory(environmentChangedDbDirectory);
                Assert.IsTrue(Directory.Exists(environmentChangedDbDirectory));
                
                foreach (string testdataFilepath in Constants.TestData.FILEPATHS)
                    File.Copy(testdataFilepath, string.Concat(environmentChangedDbDirectory, Path.DirectorySeparatorChar, Path.GetFileName(testdataFilepath)));
                Assert.IsTrue(new Folder(environmentChangedDbDirectory)
                    .Files.Count() == Constants.TestData.FILEPATHS.Length);

                Environment.CurrentDirectory = environmentChangedDirectory;

                Db db = new Db();
                string expectedDbPath = Path.GetFullPath(environmentChangedDbDirectory);
                Assert.AreEqual(expectedDbPath, db.DBPath);

                int expectedFileCount = (db.ListFiles.Count + db.KeyValuePairFiles.Count + db.TableFiles.Count);
                Assert.IsTrue(expectedFileCount > 0);

                db.CreateDbBackup();
                Folder testDataFolder = new Folder(Environment.CurrentDirectory);
                bool foundBackupsFolderMaybe = false;
                foreach (var subfolder in testDataFolder.SubFolders)
                    if (subfolder.HasSubFolders)
                        foreach (var subSubfolder in subfolder.SubFolders)
                        {
                            int actualFileCount = 0;
                            if (subSubfolder.HasFiles)
                                foundBackupsFolderMaybe = true;
                            Assert.IsTrue(foundBackupsFolderMaybe);
                            foreach (var file in subSubfolder.Files)
                                actualFileCount++;
                            Assert.AreEqual(expectedFileCount, actualFileCount);
                        }
                Assert.IsTrue(foundBackupsFolderMaybe);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Concat(ex.Message, ex.StackTrace));
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
            }
        }

        [TestMethod]
        public void DbBackupsFound()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            Assert.IsNotNull(db.DbBackups);
            Assert.IsTrue(db.ListFiles.Count > 0 || db.KeyValuePairFiles.Count > 0 || db.TableFiles.Count > 0);
            db.CreateDbBackup();
            Assert.IsTrue(db.DbBackups.Count > 0);
            db = new Db(Constants.TestData.DBPATH);
            Assert.IsTrue(db.DbBackups.Count > 0);
        }

        [TestMethod]
        public void CanCommitNewFile()
        {
            {// List
                Db db = new Db(Constants.TestData.DBPATH);
                string tempFilename1 = "FileThatDoesNotExist1.txt";
                string tempFilename2 = "FileThatDoesNotExist2.txt";
                string tempFilename3 = "FileThatDoesNotExist3.txt";
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                db.AddFileByFilename(tempFilename1, FileType.List);
                db.AddFileByFilename(tempFilename2, FileType.List);
                db.AddFileByFilename(tempFilename3, FileType.List);
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                var tempFile1 = db.GetListFile(Path.GetFileNameWithoutExtension(tempFilename1));
                var tempFile2 = db.GetListFile(Path.GetFileNameWithoutExtension(tempFilename2));
                var tempFile3 = db.GetListFile(Path.GetFileNameWithoutExtension(tempFilename3));
                Assert.IsNotNull(tempFile1);
                Assert.IsNotNull(tempFile2);
                Assert.IsNotNull(tempFile3);
                Assert.IsTrue(tempFile1.Data.Count == 0);
                Assert.IsTrue(tempFile2.Data.Count == 0);
                Assert.IsTrue(tempFile3.Data.Count == 0);

                tempFile1.Commit();
                tempFile2.Add("Entry");
                tempFile2.Commit();
                tempFile3.Add(string.Empty);
                tempFile3.Commit();

                db = new Db(Constants.TestData.DBPATH);
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
            }
            
            {// Key value pair
                Db db = new Db(Constants.TestData.DBPATH);
                string tempFilename1 = "FileThatDoesNotExist4.txt";
                string tempFilename2 = "FileThatDoesNotExist5.txt";
                string tempFilename3 = "FileThatDoesNotExist6.txt";
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                db.AddFileByFilename(tempFilename1, FileType.KeyValuePair);
                db.AddFileByFilename(tempFilename2, FileType.KeyValuePair);
                db.AddFileByFilename(tempFilename3, FileType.KeyValuePair);
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                var tempFile1 = db.GetKeyValuePairFile(Path.GetFileNameWithoutExtension(tempFilename1));
                var tempFile2 = db.GetKeyValuePairFile(Path.GetFileNameWithoutExtension(tempFilename2));
                var tempFile3 = db.GetKeyValuePairFile(Path.GetFileNameWithoutExtension(tempFilename3));
                Assert.IsNotNull(tempFile1);
                Assert.IsNotNull(tempFile2);
                Assert.IsNotNull(tempFile3);
                Assert.IsTrue(tempFile1.Data.Count == 0);
                Assert.IsTrue(tempFile2.Data.Count == 0);
                Assert.IsTrue(tempFile3.Data.Count == 0);

                tempFile1.Commit();
                tempFile2.Add("Entry");
                tempFile2.Commit();
                tempFile3.Add(string.Empty);
                tempFile3.Commit();

                db = new Db(Constants.TestData.DBPATH);
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
            }

            {// Table
                Db db = new Db(Constants.TestData.DBPATH);
                string tempFilename1 = "FileThatDoesNotExist7.txt";
                string tempFilename2 = "FileThatDoesNotExist8.txt";
                string tempFilename3 = "FileThatDoesNotExist9.txt";
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                db.AddFileByFilename(tempFilename1, FileType.Table);
                db.AddFileByFilename(tempFilename2, FileType.Table);
                db.AddFileByFilename(tempFilename3, FileType.Table);
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
                var tempFile1 = db.GetTableFile(Path.GetFileNameWithoutExtension(tempFilename1));
                var tempFile2 = db.GetTableFile(Path.GetFileNameWithoutExtension(tempFilename2));
                var tempFile3 = db.GetTableFile(Path.GetFileNameWithoutExtension(tempFilename3));
                Assert.IsNotNull(tempFile1);
                Assert.IsNotNull(tempFile2);
                Assert.IsNotNull(tempFile3);
                Assert.IsTrue(tempFile1.Data.Count == 0);
                Assert.IsTrue(tempFile2.Data.Count == 0);
                Assert.IsTrue(tempFile3.Data.Count == 0);

                tempFile1.Commit();
                tempFile2.Add("Entry");
                tempFile2.Commit();
                tempFile3.Add(string.Empty);
                tempFile3.Commit();

                db = new Db(Constants.TestData.DBPATH);
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename1)));
                Assert.IsTrue(db.Exists(Path.GetFileNameWithoutExtension(tempFilename2)));
                Assert.IsFalse(db.Exists(Path.GetFileNameWithoutExtension(tempFilename3)));
            }
        }

        [TestMethod]
        public void CanDeleteDbBackup()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int dbBackupsCountBeforeCreateBackup = db.DbBackups.Count;
            db.CreateDbBackup();
            int dbBackupsCountAfterCreateBackup = db.DbBackups.Count;
            Assert.IsTrue(dbBackupsCountAfterCreateBackup == (dbBackupsCountBeforeCreateBackup + 1));
            db = new Db(Constants.TestData.DBPATH);
            Assert.AreEqual(dbBackupsCountAfterCreateBackup, db.DbBackups.Count);
            db.DeleteDbBackup(db.DbBackups[0]);
            Assert.IsTrue(db.DbBackups.Count == dbBackupsCountBeforeCreateBackup);
            db = new Db(Constants.TestData.DBPATH);
            Assert.IsTrue(db.DbBackups.Count == dbBackupsCountBeforeCreateBackup);
        }

        [TestMethod]
        public void CanDeleteDbBackupsOlderThan()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int dbBackupsCountBeforeCreateBackup = db.DbBackups.Count;
            Assert.AreEqual(0, dbBackupsCountBeforeCreateBackup);

            DateTime now = DateTime.Now;
            db.CreateDbBackup(now);
            db.CreateDbBackup(now.AddSeconds(5));
            db.CreateDbBackup(now.AddSeconds(10));
            Assert.AreEqual(3, db.DbBackups.Count);
            db = new Db(Constants.TestData.DBPATH);
            Assert.AreEqual(3, db.DbBackups.Count);

            db.DeleteDbBackupsOlderThan(now.AddSeconds(2));
            Assert.AreEqual(2, db.DbBackups.Count);
            db = new Db(Constants.TestData.DBPATH);
            Assert.AreEqual(2, db.DbBackups.Count);

            db.CreateDbBackup();
            Assert.AreEqual(3, db.DbBackups.Count);
            db = new Db(Constants.TestData.DBPATH);
            Assert.AreEqual(3, db.DbBackups.Count);

            db.DeleteDbBackupsOlderThan(now.AddSeconds(15));
            Assert.AreEqual(0, db.DbBackups.Count);
            db = new Db(Constants.TestData.DBPATH);
            Assert.AreEqual(0, db.DbBackups.Count);
        }
        
        #endregion

        #region Cleanup

        [TestCleanup]
        public void RemoveDB()
        {
            Folder testDataFolder = new Folder(Constants.TestData.TESTDATA_PATH);
            if (testDataFolder.HasSubFolders)
                foreach (var subfolderPath in testDataFolder.SubFolders.Select(x => x.Path))
                    Directory.Delete(subfolderPath, true);
            //Directory.Delete(Constants.TestData.DBPATH, true);
            //if (Directory.Exists(Constants.TestData.DBBACKUPPATH))
            //    Directory.Delete(Constants.TestData.DBBACKUPPATH, true);
            //string defaultDbPath = new Db().DBPath;
            //if (Directory.Exists(defaultDbPath))
            //    Directory.Delete(defaultDbPath);
        }

        #endregion
    }
}