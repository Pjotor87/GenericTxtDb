using System;
using System.Collections.Generic;
using System.Text;

namespace GenericTxtDb
{
    public class KeyValuePairFile : ListFile
    {
        public IList<KeyValuePair<string, string>> KeyValuePairs { get; private set; }

        public KeyValuePairFile(string fileName) : base(fileName)
        {
            this.KeyValuePairs = new List<KeyValuePair<string, string>>();
            foreach (string line in this.Data)
                this.ParseAndAdd(line);
        }

        public override void Commit()
        {
            string[] commitData = new string[this.KeyValuePairs.Count];
            for (int i = 0; i < this.KeyValuePairs.Count; i++)
                commitData[i] = string.Format("\"{0}\"=\"{1}\"", this.KeyValuePairs[i].Key, this.KeyValuePairs[i].Value);
            this.SetData(commitData);
            base.Commit();
        }

        private void ParseAndAdd(string line)
        {
            if (!string.IsNullOrEmpty(line) && line.Contains("="))
            {
                string[] lineParts = line.Split('=');

                if (lineParts.Length == 2)
                    this.KeyValuePairs.Add(
                        new KeyValuePair<string, string>(
                            TrimFirstAndLastQuotations(lineParts[0]),
                            TrimFirstAndLastQuotations(lineParts[1])
                        )
                    );
                else
                {
                    StringBuilder key = new StringBuilder();
                    StringBuilder value = new StringBuilder();
                    bool reachedEndOfKey = false;
                    bool edgeCase = false;
                    for (int i = 0; i < lineParts.Length; i++)
                    {
                        if (!reachedEndOfKey)
                        {
                            key.Append(lineParts[i]);
                            if (lineParts[i].EndsWith("\""))
                                reachedEndOfKey = true;
                            else
                                key.Append('=');
                        }
                        else
                        {
                            value.Append(lineParts[i]);
                            if (lineParts.Length > i + 1)
                            {
                                value.Append('=');
                                if (lineParts[i].EndsWith("\""))
                                    edgeCase = true;
                            }

                        }
                    }

                    if (!string.IsNullOrEmpty(key.ToString()) &&
                        !string.IsNullOrEmpty(value.ToString()) &&
                        !edgeCase)
                        this.KeyValuePairs.Add(
                            new KeyValuePair<string, string>(
                                TrimFirstAndLastQuotations(key.ToString()),
                                TrimFirstAndLastQuotations(value.ToString())
                            )
                        );
                }
            }
        }

        public void AddRange(IList<KeyValuePair<string, string>> newEntries)
        {
            foreach (KeyValuePair<string, string> newEntry in newEntries)
                this.Add(newEntry);
        }

        public void AddRange(IList<string[]> newEntries)
        {
            foreach (string[] newEntry in newEntries)
                this.Add(newEntry);
        }

        /// <param name="newEntry">Array with two values. First value is key. Second value is value.</param>
        public void Add(string[] newEntry)
        {
            if (newEntry.Length >= 2)
                this.Add(new KeyValuePair<string, string>(newEntry[0], newEntry[1]));
            else
                throw new Exception("Array has too few values. Needs at least 2.");
        }

        public void Add(KeyValuePair<string, string> newEntry)
        {
            if (!string.IsNullOrEmpty(newEntry.Key) && !string.IsNullOrEmpty(newEntry.Value))
                this.KeyValuePairs.Add(newEntry);
        }

        public void RemoveRange(IList<KeyValuePair<string, string>> entries)
        {
            foreach (KeyValuePair<string, string> entry in entries)
                this.Remove(entry);
        }

        public void RemoveRange(IList<string[]> entries)
        {
            foreach (string[] entry in entries)
                this.Remove(entry);
        }

        /// <param name="newEntry">Array with two values. First value is key. Second value is value.</param>
        public void Remove(string[] entry)
        {
            if (entry.Length >= 2)
                this.Remove(new KeyValuePair<string, string>(entry[0], entry[1]));
            else
                throw new Exception("Array has too few values. Needs at least 2.");
        }

        public void Remove(KeyValuePair<string, string> entry)
        {
            if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value))
                this.KeyValuePairs.Remove(entry);
        }
    }
}