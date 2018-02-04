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
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static List<object> Column(DataTable dataTable, int columnNumber)
        {
            columnNumber--; // Adjust to grid type.

            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: List is empty");
                return null;
            }
            if(columnNumber > dataTable.Data.Count)
            {
                Debug.Log("ERROR: ID is more than the size of the list");
                return null;
            }

            return GetColumnList(dataTable, columnNumber);
        }

        /// <summary>
        /// Get a specific list of row data from the dataTable.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        public static List<object> Row(DataTable dataTable, int rowID)
        {
            rowID--;

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
           
            return GetRowList(dataTable, rowID);
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
        /// Edit a single cell of data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public static void EditSingleCell(DataTable data, object value, int id)
        {
            data.Data[id] = value;
        }


        /// <summary>
        /// TODO: Make this not hardcoded.
        /// Edit a row of a DataTables data.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="rowID"></param>
        /// <param name="values"></param>
        public static void EditRow(DataTable dataTable, int rows, int columns, int rowID, params object[] values)
        {
            rowID--;

            // HACKHACK: This should be managed somewhere else.
            if(dataTable.Data.Count <= 0)
            {
                dataTable.Rows    = values.Length;
                dataTable.Columns = values.Length;

                for(int i = 0; i < (values.Length * values.Length); i++)
                {
                    dataTable.Data.Add("");
                }
            }

            int kk = 0;
            for(int i = 0; i < rows; i++)
            {
                if(rowID != i)
                    continue;

                for(int j = 0; j < columns; j++)
                {
                    var index = Index(columns, i, j);
                    dataTable.Data[index] =  values[kk++];
                }
            }
        }


        /// <summary>
        /// TODO: MAke this not hardcoded
        /// Edit a column of a DataTables data.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnID"></param>
        /// <param name="values"></param>
        public static void EditColumn(DataTable dataTable, int rows, int columns, int columnID, params object[] values)
        {
            columnID--;

            // HACKHACK: This should be managed somewhere else.
            if(dataTable.Data.Count <= 0)
            {
                dataTable.Rows = values.Length;
                dataTable.Columns = values.Length;

                for(int i = 0; i < (values.Length * values.Length); i++)
                {
                    dataTable.Data.Add("");
                }
            }

            int kk = 0;
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    if(columnID == j)
                    {
                        var index = Index(columns, i, j);
                        dataTable.Data[index] = values[kk++];
                    }
                }
            }
        }


        /// <summary>
        /// CSV Lists require a slightly different structure for getting the index. 
        ///     Heres a helper so you don't have to remember it.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static int Index(int width, int i, int j)
        {
            return (width * i) + j;
        }


        /// <summary>
        /// Create a new file at this path with this name.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        public static void NewFile(string path, string filename)
        {
            System.IO.File.Create(path + filename);
        }

        /// <summary>
        /// Return a list of Row Data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        private static List<object> GetRowList(DataTable dataTable, int rowID)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: Data count is less than zero");
                return null;
            }

            var rowData = new List<object>();
            for(int i = 0; i < dataTable.Rows; i++)
            {
                if(rowID != i) continue;

                for(int j = 0; j < dataTable.Columns; j++)
                {
                    var index = Index(dataTable.Columns, i, j);
                    rowData.Add(dataTable.Data[index]);
                }
            }
            return rowData;
        }


        /// <summary>
        /// Return a list of column Data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columnID"></param>
        /// <returns></returns>
        private static List<object> GetColumnList(DataTable dataTable, int columnID)
        {
            if(dataTable.Data.Count <= 0)
            {
                Debug.Log("ERROR: Data count is less than zero");
                return null;
            }

            var columnData = new List<object>();
            for(int i = 0; i < dataTable.Rows; i++)
            {
                for(int j = 0; j < dataTable.Columns; j++)
                {
                    if(columnID == j)
                    {
                        var index = Index(dataTable.Columns, i, j);
                        columnData.Add(dataTable.Data[index]);
                    }
                }
            }
            return columnData;
        }
    }
}