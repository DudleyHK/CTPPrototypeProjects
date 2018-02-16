using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TransitionMatrixManager : MonoBehaviour
{ 



    public static void BuildTransitionMatrix(CSV.DataTable dataTable, int columnID)
    {
        // Get a list of all the different tokens. 
        var tokens     = new List<string>();
        var columnData = CSV.CSVUtilities.Column(dataTable, columnID);


        for(int i = 0; i < columnData.Count; i++)
        {
            if(i == 0) continue; // TODO: Unhardcode the headers.

            var cellStr = (string)columnData[i];

            if(!tokens.Contains(cellStr))
            {
                tokens.Add(cellStr);
                Debug.Log(cellStr);
            }
        }

        ///CSV.CSVUtilities.EditRow   (CSV.CSVManager.GetTable("ShapeTransitionMatrix"), tokens.Count, tokens.Count, 2, tokens.ToArray());
        ///CSV.CSVUtilities.EditColumn(CSV.CSVManager.GetTable("ShapeTransitionMatrix"), tokens.Count, tokens.Count, 1, tokens.ToArray());
        ///
        ///CSV.CSVManager.WriteAll("ShapeTransitionMatrix.csv", CSV.CSVManager.GetTable("ShapeTransitionMatrix"));
    }
}
