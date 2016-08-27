using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GenericTxtDb
{
    public class TableFile : ListFile
    {
        public string Separator { get; private set; }
        public IList<IList<string>> TableRows { get; set; }

        public TableFile(string filePath) : base(filePath)
        {
            this.Separator = "|!|";
            this.TableRows = new List<IList<string>>();

            foreach (string line in this.Data)
                if (line.Contains(this.Separator))
                    this.TableRows.Add(
                        line.Split(
                            new string[] 
                            {
                                this.Separator
                            }, 
                            StringSplitOptions.None
                        ).Select(
                            x => 
                            this.TrimFirstAndLastQuotations(x)
                        ).ToList()
                    );
        }

        public override void Commit()
        {
            string[] commitData = new string[this.TableRows.Count];

            StringBuilder tableFileBuilder = new StringBuilder();
            for (int i = 0; i < this.TableRows.Count; i++)
            {
                tableFileBuilder.Clear();
                for (int j = 0; j < TableRows[i].Count; j++)
                    tableFileBuilder.AppendFormat("{0}{1}", this.TableRows[i][j], j < this.TableRows[i].Count ? this.Separator : string.Empty);
                commitData[i] = tableFileBuilder.ToString();
            }

            this.SetData(commitData);
            base.Commit();
        }
    }
}