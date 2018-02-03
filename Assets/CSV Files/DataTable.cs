using System.Collections.Generic;


namespace CSV
{
    /// <summary>
    /// Use this class as a wrapper for the data which is read in from the CSV files. 
    /// </summary>
    public class DataTable
    {
        public List<object> Data { get; set; }
        public int Rows          { get; set; }
        public int Columns       { get; set; }


        public DataTable()
        {
            Data = new List<object>();
            Rows = 0;
            Columns = 0;
        }

        public DataTable(List<object> data, int rows, int columns)
        {
            Data    = new List<object>(data);
            Rows    = rows;
            Columns = columns;
        }
    }
}
