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
            this.Data = (File.Exists(this.FilePath)) ? File.ReadAllLines(this.FilePath, Encoding.Default).ToList() : new List<string>();
        }

        virtual public void Commit()
        {
            IEnumerable<string> distinctData = this.Data.Distinct();
            File.WriteAllLines(this.FilePath, distinctData, Encoding.Default);
            if (distinctData == null || 
                distinctData.Count() == 0 || 
                (distinctData.Count() == 1 && string.IsNullOrEmpty(distinctData.First())))
                File.Delete(this.FilePath);
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
