using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using GenericTxtDb;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        #region Initialize

        [TestInitialize]
        public void TestdataExists()
        {
            Settings.SetDBPath(Constants.TestData.DBPATH);
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
                ListFile file = new ListFile(filename);
                Assert.IsTrue(file.Exists);
                ListFile tempfile = new ListFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, filename));
                Assert.IsTrue(tempfile.Exists);
            }
        }

        #endregion

        #region List

        [TestMethod]
        public void ListLine1Matches()
        {
            string expected = "One value";
            ListFile file = new ListFile(Constants.TestData.LIST_FILENAME);
            Assert.AreEqual(expected, file.Data[0]);
        }

        [TestMethod]
        public void ListLine2Matches()
        {
            string expected = "Another value";
            ListFile file = new ListFile(Constants.TestData.LIST_FILENAME);
            Assert.AreEqual(expected, file.Data[1]);
        }

        [TestMethod]
        public void ListLine3Matches()
        {
            string expected = "           A third value with leading and trailing whitespace            ";
            ListFile file = new ListFile(Constants.TestData.LIST_FILENAME);
            Assert.AreEqual(expected, file.Data[2]);
        }

        #endregion

        #region KeyValuePair

        [TestMethod]
        public void KeyValuePairLine1Matches()
        {
            string expectedLine = "\"One key\"=\"One value\"";
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
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
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
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
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
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
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
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
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
            Assert.AreEqual(expectedLine, file.Data[4]);
            foreach (KeyValuePair<string, string> kvPair in file.KeyValuePairs)
                if (kvPair.Key.Contains("fifth"))
                    Assert.Fail("The key value pair should not be added");
        }

        [TestMethod]
        public void KeyValuePairLine6Matches()
        {
            string expectedLine = "\"A sixth= key\"=\"A sixth val=ue\"";
            KeyValuePairFile file = new KeyValuePairFile(Constants.TestData.KEYVALUEPAIR_FILENAME);
            Assert.AreEqual(expectedLine, file.Data[5]);
            string expectedKey = "A sixth= key";
            Assert.AreEqual(expectedKey, file.KeyValuePairs[4].Key);
            string expectedValue = "A sixth val=ue";
            Assert.AreEqual(expectedValue, file.KeyValuePairs[4].Value);
        }

        [TestMethod]
        public void CanAddAndRemoveNewEntryInKeyValuePairFile()
        {
            KeyValuePair<string, string> newEntry = new KeyValuePair<string, string>("UnitTestKey", "UnitTestValue");
            KeyValuePairFile tempFile = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
            {// ADD ENTRY
                Assert.IsFalse(tempFile.KeyValuePairs.Contains(newEntry));
                tempFile.Add(newEntry);
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
            }
            {// COMMIT
                KeyValuePairFile tempFile2 = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile2.KeyValuePairs.Contains(newEntry));
                tempFile.Commit();
                Assert.IsTrue(tempFile.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile3 = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile3.KeyValuePairs.Contains(newEntry));
            }
            {// REMOVE ENTRY
                KeyValuePairFile tempFile4 = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile4.KeyValuePairs.Contains(newEntry));
                tempFile4.Remove(newEntry);
                Assert.IsFalse(tempFile4.KeyValuePairs.Contains(newEntry));
                KeyValuePairFile tempFile5 = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsTrue(tempFile5.KeyValuePairs.Contains(newEntry));
                tempFile4.Commit();
                KeyValuePairFile tempFile6 = new KeyValuePairFile(string.Concat(Constants.TestData.TEMPFILENAME_PREFIX, Constants.TestData.KEYVALUEPAIR_FILENAME));
                Assert.IsFalse(tempFile6.KeyValuePairs.Contains(newEntry));
            }
        }

        #endregion

        #region Table

        [TestMethod]
        public void TableLine1Matches()
        {
            string expectedLine = "\"FirstRowValue1\"|!|\"FirstRowValue2\"|!|\"FirstRowValue3\"";
            TableFile file = new TableFile(Constants.TestData.TABLE_FILENAME);
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
            TableFile file = new TableFile(Constants.TestData.TABLE_FILENAME);
            Assert.AreEqual(expectedLine, file.Data[1]);

            IList<string> expectedCellValues = new List<string> { "SecondRowValue1", "SecondRowValue2", "SecondRowValue3" };
            Assert.AreEqual(expectedCellValues.Count, file.TableRows[1].Count);
            for (int i = 0; i < expectedCellValues.Count; i++)
                Assert.AreEqual(expectedCellValues[i], file.TableRows[1][i]);
        }

        #endregion

        #region Db

        [TestMethod]
        public void CanInitializeDefaultDb()
        {
            Db db = new Db();
            Assert.IsNotNull(db);
        }

        [TestMethod]
        public void DbGetsFiles()
        {
            Db db = new Db(Constants.TestData.DBPATH);
            int expectedListFiles = 2;
            Assert.AreEqual(expectedListFiles, db.ListFiles.Count);
            int expectedKeyValuePairFiles = 2;
            Assert.AreEqual(expectedKeyValuePairFiles, db.KeyValuePairFiles.Count);
            int expectedTableFiles = 2;
            Assert.AreEqual(expectedTableFiles, db.TableFiles.Count);
        }

        #endregion

        #region Cleanup

        [TestCleanup]
        public void RemoveDB()
        {
            Settings.SetDBPath(Constants.TestData.DBPATH);
            Directory.Delete(Constants.TestData.DBPATH, true);
        }

        #endregion
    }
}