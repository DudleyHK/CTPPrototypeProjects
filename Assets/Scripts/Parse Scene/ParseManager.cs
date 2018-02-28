using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParseManager : MonoBehaviour
{ 
    #region SpriteTiles

    [SerializeField]
    private List<SceneObject> m_sceneObjects    = new List<SceneObject>();
    [SerializeField]
    private List<GameObject> m_visibleObjects;

    
    /// <summary>
    /// Parse all features in the level. 
    /// </summary>
    /// <returns></returns>
    public bool ParseLevel(out List<RuntimeMatrix> runtimeMatrix)
    {
        runtimeMatrix = new List<RuntimeMatrix>();

        // Run through each object in the scene and add it to the runtime transition matrix. 
        for(int i = 0; i <= (m_sceneObjects.Count - 1); i++)
        {
            var sceneObject = m_sceneObjects[i];
            var findMatrix = runtimeMatrix.Find(n => { return n.Name == sceneObject.Name; });

            if(findMatrix != null)
            {
                // HACK: 
                if(i == (m_sceneObjects.Count - 1))
                {
                    // Add the last value in a line end.
                    findMatrix.AddRow(sceneObject.Name, "/0");
                }
                else
                {
                    findMatrix.AddRow(sceneObject.Name, m_sceneObjects[i + 1].Name);
                }
                
                findMatrix.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
            }
            else
            {
                var rtm = new RuntimeMatrix(sceneObject.Name);
                rtm.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
                
                // HACK: If last i set to valut to default.
                if(i == (m_sceneObjects.Count - 1))
                {
                    // Add the last value in a line end.
                    //rtm.AddRow(sceneObject.Name, "/0");
                }
                else
                {
                    rtm.AddRow(sceneObject.Name, m_sceneObjects[i + 1].Name);
                }
                runtimeMatrix.Add(rtm);
            }
        }

        // Work out the Row Totals for each of the runtime transition matrices.
        foreach(var rtm in runtimeMatrix)
        {
            var denominator = rtm.Total;
            foreach(var cell in rtm.TransitionMatrix)
            {
                //var from = cell[0] as string;
                //var to = cell[1] as string;
                var value = System.Convert.ToSingle(cell[2]);

                value /= denominator;
                cell[2] = value;
            }
        }
        return true;
    }


    /// <summary>
    /// Destory all the objects visible in the scene.
    /// </summary>
    public void ClearScene()
    {
        foreach(var obj in m_visibleObjects)
        {
            Destroy(obj);
        }
    }

    /// <summary>
    /// Currently doing a scene object dump. This means the order in which objects are found in the scene
    ///     isn't being considered.
    /// </summary>
    /// <returns></returns>
    public bool InitSceneObjects(out uint levelSize)
    {
        /// Test Level Objects.
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(5, 5, 0), new Vector3(1, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(10, 5, 0), new Vector3(5, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Triangle", new Vector3(10, 5, 0), new Vector3(6, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(10, 2, 0), new Vector3(7, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Circle", new Vector3(7, 7, 0), new Vector3(9, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(3, 11, 0), new Vector3(10, 1, 0)));
        
        levelSize = 0;

        m_visibleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("LevelObject"));
        if(m_visibleObjects.Count <= 0)
        {
            Debug.LogWarning("ERROR: There are no level objects in the scene.. Have you applied the tags?");
            return false;
        }

        foreach(var obj in m_visibleObjects)
        {
            var name     = obj.name;
            var size     = obj.transform.lossyScale;
            var position = obj.transform.position;

            m_sceneObjects.Add(new SceneObject(name, size, position));
        }

        levelSize = (uint)m_sceneObjects.Count;
        return true;
    }


    #endregion


    #region NumbersAsHeight

    [SerializeField]
    private Dictionary<string, List<int>> m_transitionMatrix = new Dictionary<string, List<int>>();
    
    private List<int> m_tileHeights;
    /// <summary>
    /// This version is to order the tiles in order of x position. 
    /// </summary>
    /// <returns></returns>
    public bool ParseLevel(List<float> heights)
    {
        var tempVisibleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("LevelObject"));
        if(tempVisibleObjects.Count <= 0)
        {
            Debug.LogWarning("Warning: There are no level objects in the scene.. Have you applied the tags?");
            return false;
        }
        m_visibleObjects = new List<GameObject>(tempVisibleObjects.OrderBy(n => n.transform.localPosition.x));

       // TODO: Reset on command
        m_tileHeights = new List<int>();
        foreach(var tile in m_visibleObjects)
        {
            var heightID = heights.IndexOf(tile.transform.localPosition.y);
            m_tileHeights.Add(heightID);
        }


        return true;
    }

    /// <summary>
    /// Add each to value to the bag of to values.
    /// </summary>
    public Dictionary<string, List<int>> ParseHeightLevel(bool parseLevel = false)
    {
        // TODO: Load the m_transitionMatrix from the PlayerPrefs if it exists. 
        ClearTransitionMatrix();
        LoadTransitionMatrix();
        ParseHeightInput(m_tileHeights, parseLevel);
        SaveTransitionMatrix();

        return m_transitionMatrix;
    }


    /// <summary>
    /// Use with caution. 
    /// Clears the transitionmatrix text file. 
    /// </summary>
    public void FlushTextFile()
    {
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            var streamWriter = new System.IO.StreamWriter(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");
            streamWriter.Flush();
            streamWriter.Close();
        }
    }



    /// <summary>
    /// Clear the matrix is it has been tampered with in anyway. 
    /// </summary>
    private void ClearTransitionMatrix()
    {
        if(m_transitionMatrix.Count <= 0 || m_transitionMatrix == null)
            return;

        m_transitionMatrix.Clear();
    }


    /// <summary>
    /// if parselevel is true parse the heights passed in. 
    /// // NOTE: List<int> levelInput is hardcoded as type for the m_transitionMatrix
    /// </summary>
    /// <param name="parseLevel"></param>
    private void ParseHeightInput(List<int> levelInput, bool parseLevel)
    {
        if(parseLevel)
        {
            for(var i = 0; i < levelInput.Count; i++)
            {
                var from = levelInput[i].ToString();
                var toID = i + 1;

                if(toID == levelInput.Count)
                    break;

                if(!m_transitionMatrix.ContainsKey(from))
                {
                    m_transitionMatrix.Add(from, new List<int>(new int[] { levelInput[toID] }));
                }
                else
                {
                    m_transitionMatrix[from].Add(levelInput[toID]);
                }
            }
        }
    }


    // TODO: Put in utility function.
    private void SaveTransitionMatrix()
    {
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            var streamWriter = new System.IO.StreamWriter(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");

            foreach(var fromValue in m_transitionMatrix)
            {
                streamWriter.WriteLine("Type:" + fromValue.Key);
                streamWriter.Write("Probabilities:");
                foreach(var toValue in fromValue.Value)
                {
                    streamWriter.Write(toValue + ",");
                }
                streamWriter.WriteLine();
                streamWriter.WriteLine();

            }
            streamWriter.Close();
        }
        else
        {
            Debug.LogWarning("INVALID: File doesn't exist. transitionmatrix will start fresh.");
        }
    }

    private void LoadTransitionMatrix()
    {
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            // Read the file. 
            var streamReader = new System.IO.StreamReader(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");
            var type = "";
            var line = "";

            // get the current line, while the current line is not null 
            while((line = streamReader.ReadLine()) != null)
            {
                if(line == "") continue;
                
                // To get the correct char variables
                var row = line.Split(new char[] { ':', ' ' });
                
                var typeChar = row[0][0]; // The first char in the first part of the row. 
                var data     = row[1];    // Get a list of the values.

                if(typeChar == 'T')
                {
                    foreach(var value in data)
                    {                        
                        if(value == ',') continue;

                        // Get the from part of this transition matrix row. 
                        type = value.ToString();
                        m_transitionMatrix.Add(type, new List<int>());
                    }
                }
                else if(typeChar == 'P')
                {
                    foreach(var value in data)
                    {
                        if(value == ',') continue;
                        if(m_transitionMatrix.ContainsKey(type))
                        {
                            // TODO: This is specific to a List<int>
                            m_transitionMatrix[type].Add(int.Parse(value.ToString()));
                        }
                        else
                        {
                            Debug.LogWarning("Warning: Probability values are being writing before type.");
                        }
                    }
                }
                else
                {
                    // Do nothing. 
                }
            }
            streamReader.Close();
        }
        else
        {
            Debug.LogWarning("INVALID: File doesn't exist. transitionmatrix will start fresh");
        }
    }

    #endregion
}
