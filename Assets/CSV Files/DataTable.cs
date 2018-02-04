using System.Collections.Generic;


namespace CSV
{
    /// <summary>
    /// Use this class as a wrapper for the data which is read in from the CSV files. 
    /// </summary>
    public class DataTable
    {
        // TODO: Add file Directory.
        public List<object> Data { get; set; }
        public string Name       { get; set; }
        public int    Rows       { get; set; }
        public int    Columns    { get; set; }



        public DataTable()
        {
            Data    = new List<object>();
            Name    = "";
            Rows    = 0;
            Columns = 0;
        }

        public DataTable(List<object> data, string name, int rows, int columns)
        {
            Data    = new List<object>(data);
            Name    = name;
            Rows    = rows;
            Columns = columns;
        }


        /// <summary>
        /// Get a string of all the data in this DataTable instance.
        /// </summary>
        /// <returns></returns>
        public string TableInfo()
        {
            string output = "Table - " + Name + "\n";

            output += "Data Table: ";
            for(var i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    output += Data[(Columns * i) + j] + " ";
                }
                output += "\n";
            }
            output.TrimEnd(' ');
            output += "\n";

            output += "Rows: "    + Rows    + "\n";
            output += "Columns: " + Columns + "\n";

            return output;
        }
    }
}
