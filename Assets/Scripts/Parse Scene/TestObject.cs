using System.Collections;
using System.Collections.Generic;
using UnityEngine;










// for i ... (sceneObjects - 1)
//	if runtimeMatrices contains values with Name == sceneObject.Name
//		AddRow(Name, sceneObject[i + 1].Name);
//	else if runtimeMatrices does not contain value with Name == sceneObject.Name
//		Create new runtimematrix
//		Set its name = sceneobject.Name
//		AddRow(Name, sceneObject[i + 1];
//	else
//		unsure



public class TestObject : MonoBehaviour 
{
	private List<SceneObject> 	m_sceneObjects    = new List<SceneObject>();
	private List<RuntimeMatrix> m_runtimeMatrices = new List<RuntimeMatrix>();


    private void Start()
    {
		// TODO: Iterate through all scene objects and automatically add them to the list.
		// HACK: it in for now.
		m_sceneObjects.Add(new SceneObject("Quad", 	   new Vector3(5, 5, 0),   new Vector3(1,  1, 0)));
		m_sceneObjects.Add(new SceneObject("Quad", 	   new Vector3(10, 5, 0),  new Vector3(5,  1, 0)));
		m_sceneObjects.Add(new SceneObject("Triangle", new Vector3(10, 5, 0),  new Vector3(6,  1, 0)));
		m_sceneObjects.Add(new SceneObject("Quad",     new Vector3(10, 2, 0),  new Vector3(7,  1, 0)));
		m_sceneObjects.Add(new SceneObject("Circle",   new Vector3(7, 7, 0),   new Vector3(9,  1, 0)));
		m_sceneObjects.Add(new SceneObject("Quad",     new Vector3(3, 11, 0),  new Vector3(10, 1, 0)));



        // Run through each object in the scene and add it to the runtime transition matrix. 
		for (int i = 0; i < (m_sceneObjects.Count - 1); i++)
		{
			var sceneObject = m_sceneObjects [i];
			var findMatrix = m_runtimeMatrices.Find(n => {return n.Name == sceneObject.Name;});

			if (findMatrix != null)
			{
				findMatrix.AddRow(sceneObject.Name, m_sceneObjects [i + 1].Name);
                findMatrix.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
            }
			else
			{
				var rtm = new RuntimeMatrix(sceneObject.Name);
                rtm.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
				rtm.AddRow (sceneObject.Name, m_sceneObjects [i + 1].Name);
				m_runtimeMatrices.Add (rtm);
			}
		}


        // Work out the Row Totals for each of the runtime transition matrices.
        foreach(var rtm in m_runtimeMatrices)
        {
            var denominator  = rtm.Total;
            foreach(var cell in rtm.TransitionMatrix)
            {
                //var from  = cell[0] as string;
                //var to    = cell[1] as string;
                var value = System.Convert.ToSingle(cell[2]);

                value /= denominator; 
                cell[2] = value;
            }
        }




















        //CSV.CSVManager.Init();
		//
        //var dataTable = CSV.CSVManager.GetTable("SceneData");
        //for(int i = 0; i < dataTable.Columns; i++)
        //{
        //    TransitionMatrixManager.BuildTransitionMatrix(dataTable, i);
        //}
         



       //var testData = CSV.CSVManager.LoadData("TestData.csv");
       //Debug.Log(testData.TableInfo());

        //ar cList = CSV.CSVUtilities.Column(testData, 3);
        //
        //foreach(var elem in cList)
        //{
        //    Debug.Log(elem);
        //}
        //
        //var rList = CSV.CSVUtilities.Row(testData, 2);
        //
        //foreach(var elem in rList)
        //{
        //    Debug.Log(elem);
        //}
    }



	public void Generate()
	{
		if (Input.GetKeyDown (KeyCode.G))
		{
			// TODO: Start by generating a string. 

			// Select a Type (Quad to start with)

			// Use the types runtime transition matrix to select the most likely next shape. 

			// Select a random sceneObject from the runtime transition matrix for that type.

		}
	}
}
