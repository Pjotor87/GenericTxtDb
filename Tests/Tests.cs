﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using GenericTxtDb;
using System.Collections.Generic;
using FileTree;
using System;

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

            db.CreateDbBackup(Constants.TestData.DBBACKUPPATH);

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