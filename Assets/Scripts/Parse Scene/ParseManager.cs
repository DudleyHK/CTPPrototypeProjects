using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParseManager : MonoBehaviour
{
    #region SpriteTiles

    [SerializeField]
    private List<SceneObject> m_sceneObjects = new List<SceneObject>();
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
            Destroy(obj);
    }

    /// <summary>
    /// Currently doing a scene object dump. This means the order in which objects are found in the scene
    ///     isn't being considered.
    /// </summary>
    /// <returns></returns>
    public bool InitSceneObjects(out uint levelSize)
    {
        levelSize = 0;

        m_visibleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("LevelObject"));
        if(m_visibleObjects.Count <= 0)
        {
            Debug.LogWarning("ERROR: There are no level objects in the scene.. Have you applied the tags?");
            return false;
        }

        m_visibleObjects = new List<GameObject>(m_visibleObjects.OrderBy(n => n.transform.localPosition.x));

        foreach(var obj in m_visibleObjects)
        {
            var name = obj.name;
            var size = obj.transform.lossyScale;
            var position = obj.transform.position;

            m_sceneObjects.Add(new SceneObject(name, size, position));
            //Debug.Log("Tile X - " + position.x);
        }
        levelSize = (uint)m_sceneObjects.Count;
        return true;
    }


    #endregion


    #region NumbersAsHeight

    [SerializeField]
    public Dictionary<string, List<int>> TransitionMatrix = new Dictionary<string, List<int>>();

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
    /// Create a list of height IDs based on their offset from the previous token.
    /// </summary>
    /// <returns></returns>
    public bool ParseDeltaLevel(float startYPosition)
    {
        var currHeight = m_visibleObjects[0].transform.localPosition.y;
        var heightID = ParseDeltaFile.WorldHeightStep((int)currHeight, (int)startYPosition);

        m_tileHeights = new List<int>();
        m_tileHeights.Add(heightID);

        for(int i = 1; i < m_visibleObjects.Count; i++)
        {
            var previousHeight = m_visibleObjects[i - 1].transform.localPosition.y;

            currHeight = m_visibleObjects[i].transform.localPosition.y;
            heightID = ParseDeltaFile.WorldHeightStep((int)currHeight, (int)previousHeight);
            m_tileHeights.Add(heightID);
        }
        return true;
    }

    /// <summary>
    /// Add each to value to the bag of to values.
    /// </summary>
    public void ParseHeightLevel(int backTracking, bool parseLevel)
    {
        ClearTransitionMatrix();
        LoadTransitionMatrix();

        if(parseLevel)
        {
            ParseHeightInput(m_tileHeights, backTracking);
        }

        SaveTransitionMatrix();
    }


    /// <summary>
    /// Use with caution.
    /// Clears the transitionmatrix text file.
    /// </summary>
    public static void FlushTextFile()
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
        if(TransitionMatrix.Count <= 0 || TransitionMatrix == null)
            return;

        TransitionMatrix.Clear();
    }


    /// <summary>
    /// if parselevel is true parse the heights passed in.
    /// // NOTE: List<int> levelInput is hardcoded as type for the m_transitionMatrix
    /// </summary>
    /// <param name="parseLevel"></param>
    private void ParseHeightInput(List<int> levelInput, int backTracking)
    {
        // Calculate the transition of each level height using back tracking.
        for(var i = (backTracking - 1); i < levelInput.Count; i++)
        {
            var toID = i + 1;
            if(toID == levelInput.Count)
                break;

            var fromBackBlock = BackTrackingBlock(levelInput, backTracking, i);
            AddTransitionMatrix(levelInput, fromBackBlock, toID);
        }

        // Put this last so the singles are at the end of the block.
        // Calculate the transitions of each individual level height.
        for(var i = 0; i < levelInput.Count; i++)
        {
            var toID = i + 1;
            if(toID == levelInput.Count)
                break;

            var fromSingleBlock = BackTrackingBlock(levelInput, 1, i);
            AddTransitionMatrix(levelInput, fromSingleBlock, toID);
        }
    }

    /// <summary>
    /// Add to already existing transition matrix, or create a new one.
    /// </summary>
    /// <param name="levelInput"></param>
    /// <param name="from"></param>
    /// <param name="toID"></param>
    private void AddTransitionMatrix(List<int> levelInput, string from, int toID)
    {
        if(!TransitionMatrix.ContainsKey(from))
        {
            TransitionMatrix.Add(from, new List<int>(new int[] { levelInput[toID] }));
        }
        else
        {
            TransitionMatrix[from].Add(levelInput[toID]);
        }
    }

    /// <summary>
    /// Create a block of input of any amount.
    /// </summary>
    /// <param name="levelInput"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private string BackTrackingBlock(List<int> levelInput, int backTracking, int index)
    {
        // This loop calculates a new set of backtracking size.
        var back = 0;
        var from = "[";

        for(int j = 0; j < backTracking; j++)
        {
            char symbol;
            if(j == (backTracking - 1))
            {
                symbol = ']';
            }
            else
            {
                symbol = ',';
            }
            from += levelInput[index - back++].ToString() + symbol;
        }
        return from;
    }

    // TODO: Put in utility function.
    private void SaveTransitionMatrix()
    {
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            var streamWriter = new System.IO.StreamWriter(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");

            foreach(var fromValue in TransitionMatrix)
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
            Debug.LogWarning("Warning: File doesn't exist. transitionmatrix will start fresh.");
        }
    }

    private void LoadTransitionMatrix()
    {
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            // Read the file.
            var streamReader = new System.IO.StreamReader(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");
            var fromValue = "";
            var line = "";

            // get the current line, while the current line is not null
            while((line = streamReader.ReadLine()) != null)
            {
                if(line == "")
                    continue;

                // To get the correct char variables
                var row = line.Split(new char[] { ':', ' ' });

                var typeChar = row[0][0]; // The first char in the first part of the row.
                var rowData = row[1];    // Get a list of the values.

                if(typeChar == 'T')
                {
                    if(TransitionMatrix.ContainsKey(rowData))
                        break;

                    // Get the from part of this transition matrix row.
                    TransitionMatrix.Add(rowData, new List<int>());
                    fromValue = rowData;
                }
                else if(typeChar == 'P')
                {
                    for(var i = 0; i < rowData.Length; i++)
                    {
                        var value = rowData[i];
                        if(value == ',') continue;

                        if(TransitionMatrix.ContainsKey(fromValue))
                        {
                            // Handle negitive values. 
                            if(value == '-')
                            {
                                var next = rowData[i + 1];
                                int result;

                                if(int.TryParse(next.ToString(), out result))
                                {
                                    result *= -1;
                                    TransitionMatrix[fromValue].Add(result);
                                    i++; // Skip next value in list.
                                }
                            }
                            else
                            {
                                TransitionMatrix[fromValue].Add(int.Parse(value.ToString()));
                            }
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
            Debug.LogWarning("Warning: File doesn't exist. transitionmatrix will start fresh");
        }
    }

    #endregion



}
