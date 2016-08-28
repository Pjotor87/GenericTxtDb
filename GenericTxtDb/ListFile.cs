using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenericTxtDb
{
    public class ListFile
    {
        public string FilePath { get; private set; }
        public IList<string> Data { get; private set; }

        public ListFile(string filePath)
        {
            this.FilePath = filePath;
            this.Data = (File.Exists(this.FilePath)) ? this.Data = File.ReadAllLines(this.FilePath, Encoding.Default).ToList() : new List<string>();
        }

        virtual public void Commit()
        {
            File.WriteAllLines(this.FilePath, this.Data.Distinct(), Encoding.Default);
        }

        protected string TrimFirstAndLastQuotations(string str)
        {
            if (str.StartsWith("\""))
                if (str.EndsWith("\""))
                    return str.Remove(str.Length - 1).Remove(0, 1);
                else
                    return str.Remove(str.Length - 1);
            else if (str.EndsWith("\""))
                return str.Remove(str.Length - 1);
            else
                return str;
        }

        public void SetData(string[] data)
        {
            this.Data = data;
        }

        public void AddRange(IList<string> newEntries)
        {
            foreach (string newEntry in newEntries)
                this.Add(newEntry);
        }

        public virtual void Add(string newEntry)
        {
            this.Data.Add(newEntry);
        }

        public void RemoveRange(IList<string> entries)
        {
            foreach (string entry in entries)
                this.Remove(entry);
        }

        public virtual void Remove(string entry)
        {
            this.Data.Remove(entry);
        }
    }
}
