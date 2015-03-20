using BP.DA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.BLL
{
    public class DataTableClone
    {
        //columnName:"AtPara",columnValue:"@IsCC=1"    //@IsCC=1 将抄送去除
        public static DataTable CloneDataFromDataTable(DataTable source, Dictionary<string, string> arg)
        {
            DataTable destination = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                string columnName=arg["columnName"];
                string columnValue=arg["columnValue"];
                if(string.IsNullOrEmpty(columnName)==false&&string.IsNullOrEmpty(columnValue)==false)
                {
                    if (row[columnName].ToString().IndexOf(columnValue) != -1)
                    {
                        continue;
                    }
                }
                destination.Rows.Add(row.ItemArray);
            }
            return destination;
        }
        public static DataTable CloneDataFromDataTable(DataTable source)
        {
            DataTable destination = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                destination.Rows.Add(row.ItemArray);
            }
            return destination;
        }
        public static DataTable CloneStructureFromDataTable(DataTable source)
        {
            return source.Clone();
        }
    }
}
