using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CSV
{
    public class CSVUtilities : ScriptableObject
    {

        /// <summary>
        /// Get a specific list of column data from the dataTable.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnID"></param>
        /// <returns></returns>
        public static List<object> Column(DataTable dataTable, int columnID)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: List is empty");
                return null;
            }
            if(columnID > dataTable.Data.Count)
            {
                Debug.Log("ERROR: ID is more than the size of the list");
                return null;
            }

            return null; // TODO: Return a complete column with all elements.
        }

        /// <summary>
        /// Get a specific list of row data from the dataTable.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        public static List<object> Row(DataTable dataTable, int rowID)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: List is empty");
                return null;
            }
            if(rowID > dataTable.Data.Count)
            {
                Debug.Log("ERROR: ID is more than the size of the list");
                return null;
            }
           
            return null; // TODO: Return a list of the elements from row ID
        }


        /// <summary>
        /// Get a specific element of the column.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object ColumnElement(DataTable dataTable, int columnID, int index)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: Column List is empty");
                return null;
            }
            if(columnID > dataTable.Data.Count)
            {
                Debug.Log("ERROR: ID is more than the size of the list");
                return null;
            }
            var column = Column(dataTable, columnID);
            
            return column[index];
        }


        /// <summary>
        /// Get a specific element of the row.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object RowElement(DataTable dataTable, int rowID, int index)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: Column List is empty");
                return null;
            }
            if(rowID > dataTable.Data.Count)
            {
                Debug.Log("ERROR: ID is more than the size of the list");
                return null;
            }
            var row = Row(dataTable, rowID);

            return row[index];
        }

        /// <summary>
        /// Return a list of Row Data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        private static List<object> GetRowList(List<object> data, int rowID)
        {
            if(data.Count <= 0)
            {
                Debug.Log("ERROR: Data count is less than zero");
                return null;
            }

            var rowData = new List<object>();
            for(int i = 0; i < data.Count; i++)
            {
                if(i != rowID) continue;
                rowData.Add(data[i]);
            }

            return rowData;
        }


        /// <summary>
        /// Return a list of column Data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columnID"></param>
        /// <returns></returns>
        private static List<object> GetColumnList(List<object> data, int columnID)
        {
            if(data.Count <= 0)
            {
                Debug.Log("ERROR: Data count is less than zero");
                return null;
            }

            var columnData = new List<object>();
            for(int i = 0; i < data.Count; i++)
            {
                if(columnID == 0 && i == 0)
                    columnData.Add(data[i]);

                if(i % columnID == 0)
                    columnData.Add(data[i]);
            }

            return columnData;
        }
    }
}