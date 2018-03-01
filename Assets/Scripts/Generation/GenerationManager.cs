using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    #region SpriteTiles


    // TODO: User defined/ txt files. 
    private enum FirstShape
    {
        Rectangle,
        Circle,
        Unassigned
    }
    private enum SizeSelection
    {
        Higher,
        Lower,
        Random,
        Unassigned
    }

    // TODO: Not used yet,
    //[SerializeField]
    private SizeSelection m_sizeSelection = SizeSelection.Unassigned;
    //[SerializeField]
    private FirstShape m_initialShape = FirstShape.Unassigned;

    private List<SceneObject> m_newLevel;
    private List<GameObject> m_spritePrefabs;


    public bool Generate(List<RuntimeMatrix> runtimeMatrix, List<GameObject> buildingMaterials, uint levelSize)
    {
        if(runtimeMatrix.Count <= 0)
            return false;

        Clean();

        // start by picking a random shape.
        var currentShape = InitialShape(runtimeMatrix);
        var currentSize = Vector3.zero;
        var currentPosition = Vector3.zero;

        Debug.Log("MESSAGE: Number of scene objects " + levelSize);
        for(var i = 0; i < levelSize; i++)
        {
            var rtm = runtimeMatrix.Find(n =>
            {
                return n.Name == currentShape;
            });

            if(rtm == null)
            {
                Debug.LogWarning("ERROR: runtime matrix is null for currentShape - " + currentShape);
                break;
            }

            // Check for end of runtimeMatrix.
            if(currentShape == "/0")
            {
                currentShape = NextShape(rtm);
                Debug.LogWarning("ERROR: currentShape is /0.. Force select new - " + currentShape);
                continue;
            }


            var sizeID = SizeID(rtm);

            // TODO: Add different ways to get the position ID. Currently
            //          it's hard coded to get the same as what is applied 
            //          when the level is parsed.
            var posID = sizeID;

            //TODO: Error check ids for < 0

            currentSize = rtm.SizeList[sizeID];
            currentPosition = rtm.PositionList[posID];

            // Add new scene object to list.
            m_newLevel.Add(new SceneObject(currentShape, currentSize, currentPosition));

            // Get the next shape.
            currentShape = NextShape(rtm);
        }

        OutputNewLevelString();
        BuildLevel(buildingMaterials);

        return true;
    }

    /// <summary>
    /// Currently select the first shape in the list. 
    /// TODO: Implement different types of selection for this.
    /// </summary>
    /// <param name="rtm"></param>
    /// <returns></returns>
    private string InitialShape(List<RuntimeMatrix> rtm)
    {
        switch(m_initialShape)
        {
            case FirstShape.Rectangle:
                return FirstShape.Rectangle.ToString();
            case FirstShape.Circle:
                return FirstShape.Circle.ToString();
        }

        // TODO: Change this to use the shape furthest left.  
        return rtm[0].Name;
    }

    /// <summary>
    /// Get the ID of size list to use.
    ///  TODO: Different types of getting the size.
    /// </summary>
    /// <param name="rtm"></param>
    /// <returns></returns>
    private int SizeID(RuntimeMatrix rtm)
    {
        int ID = -1;

        SizeSelection sizeSelection = m_sizeSelection;
        if(sizeSelection == SizeSelection.Unassigned)
        {
            // Debug.Log("MESSAGE: Size selection type has not been assigned. Defaulting at random.");
            sizeSelection = SizeSelection.Random;
        }

        switch(sizeSelection)
        {
            case SizeSelection.Higher:
                break;

            case SizeSelection.Lower:
                break;

            case SizeSelection.Random:
                ID = Random.Range(0, rtm.SizeList.Count);
                break;
        }
        return ID;
    }


    private int PositionID(RuntimeMatrix rtm)
    {
        int ID = -1;

        return ID;
    }



    /// <summary>
    /// Get the sum of all values in the list. Choose a random value
    ///     between 0 and 1 and subract from the rand number until
    ///     the random number is less than the probability.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private string NextShape(RuntimeMatrix rtm)
    {
        List<float> probabilities = new List<float>();
        foreach(var arrayList in rtm.TransitionMatrix)
        {
            var value = System.Convert.ToSingle(arrayList[2]);
            probabilities.Add(value);
        }

        // TODO: Check if anything has been added to the list. 

        float sum = 0;
        for(int i = 0; i < probabilities.Count; i++)
            sum += probabilities[i];

        var rand = Random.Range(0f, sum);

        var index = 0;
        for(int i = 0; i < probabilities.Count; i++)
        {
            if(rand < probabilities[i])
            {
                index = i;
                break;
            }
            else
            {
                rand -= probabilities[i];
            }
        }

        var cell = rtm.TransitionMatrix[index];
        var to = cell[1] as string;

        return to;
    }


    /// <summary>
    /// Debug output the each object to be generated.
    /// </summary>
    private void OutputNewLevelString()
    {
#if UNITY_EDITOR
        Debug.Log("New Level <Type, Size, Position>");
        foreach(var obj in m_newLevel)
        {
            Debug.Log(obj.Name + " : " + obj.Size + " : " + obj.Position);
        }
#endif
    }


    private void BuildLevel(List<GameObject> buildingMaterials)
    {
        foreach(var data in m_newLevel)
        {
            var buildingMaterial = buildingMaterials.Find(n => { return n.name == data.Name; });
            if(buildingMaterial == null)
            {
                Debug.LogWarning("Building materials does not contain the object - " + data.Name);
                continue;
            }

            var obj = Instantiate(buildingMaterial);
            obj.transform.localScale = data.Size;
            obj.transform.position = data.Position;
            m_spritePrefabs.Add(obj);
        }
    }


    private void Clean()
    {
        if(m_spritePrefabs != null)
        {
            foreach(var obj in m_spritePrefabs)
            {
                Destroy(obj);
            }
        }

        m_spritePrefabs = new List<GameObject>();
        m_newLevel = new List<SceneObject>();
    }

    #endregion

    #region NumbersAsHeight

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private int m_levelLength = 10;

    private Dictionary<string, List<int>> m_transitionMatrix;
    private List<GameObject> tiles;
    private int m_backTracking;

    /// <summary>
    /// Generate a single level looking only at the one previous tile.
    /// </summary>
    /// <param name="m_transitionMatrix"></param>
    public string GenerateNumberLevel(Dictionary<string, List<int>> transitionMatrix, int backTracking)
    {
        m_transitionMatrix = new Dictionary<string, List<int>>(transitionMatrix);
        m_backTracking     = backTracking;
        var output = "";
        var randID = 0;

        var value = SelectInitialValue();
        if(value == "") return output;

        for(int i = 0; i < m_levelLength / m_backTracking; i++)
        {
            output += ConvertToPureCSV(value);
                       
            var toList = FindNextToken(value, out randID);
            value = CreateNextBlock(toList, output, randID);
        }
        Debug.Log("Level Heights: " + output);

        return output;
    }


    public void MapTiles(List<float> heights, string textHeightLevel, float startX, float increment)
    {
        // Set level on screen to inactive.
        if(tiles != null)
            foreach(var tile in tiles)
                Destroy(tile);
        //////////////////////////////////


        tiles = new List<GameObject>();
        for(int i = 0; i < textHeightLevel.Length; i++)
        {
            var elem = textHeightLevel[i];
            var heightID = 0;

            if(!int.TryParse(elem.ToString(), out heightID))
                continue;

            // Element is used as the index for the heights list. 
            var yPos = heights[heightID];
            var xPos = startX;

            var tile = Instantiate(tilePrefab, new Vector2(xPos, yPos), Quaternion.identity);
            tiles.Add(tile);

            startX += increment;
        }
    }

    /// <summary>
    /// Select the first Type in transition matrix which is the same size as the desired
    ///     backtracking value. 
    /// Throw warning message if failed.
    /// </summary>
    /// <returns></returns>
    private string SelectInitialValue()
    {
        string value = "";
        foreach(var matrix in m_transitionMatrix)
        {
            var fromValue = matrix.Key;
            var items = Count(fromValue);

            if(items == m_backTracking)
            {
                value = fromValue;
                break;
            }
        }
        if(value == "")
        {
            Debug.LogWarning("Warning: Type in transition Matrix of size - " + m_backTracking + " not found");
        }
        return value;
    }


    /// <summary>
    /// Use the backtracking block, if a new block is created and doesn't exist in the m_transitionmatrix,
    ///     use the individual values in the transition matrix.
    /// </summary>
    /// <param name="toList"></param>
    /// <param name="randID"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private List<int> FindNextToken(string value, out int randID)
    {
        // get the list from that from value.
        if(!m_transitionMatrix.ContainsKey(value))
        {
            value = Last(value);
        }

        // select random index/ value from the list.
        randID = Random.Range(0, m_transitionMatrix[value].Count);
        return m_transitionMatrix[value];
    }


    /// <summary>
    /// TODO: Error check
    /// </summary>
    /// <param name="output"></param>
    /// <returns></returns>
    private string Last(string valueStr)
    {
        var value = valueStr[valueStr.Length - 2].ToString();
        var formattedValue = "[" + value + "]";
        return formattedValue;
    }



    /// <summary>
    /// Create the next block of numbers which are used for backtracking.
    /// 
    /// 
    /// [0, 0,[1]],  
    ///   |    |  
    ///   |    |  
    ///   |    Current number
    ///   |     
    ///   |
    ///  The backtracking values.  
    /// 
    /// 
    /// </summary>
    /// <param name="toList"></param>
    /// <param name="output"></param>
    /// <param name="randID"></param>
    /// <returns></returns>
    private string CreateNextBlock(List<int> toList, string output, int randID)
    {
        char symbol;
        string height;

        var back = 2;
        var from = "";


        for(int j = 0; j < m_backTracking; j++)
        {
            // TODO: This is hacked in at the moment.
            if(m_backTracking == 1)
            {
                symbol = ']';
                height = toList[randID].ToString();
                from += '[' + height + symbol;
            }
            else if(j == (m_backTracking - 1))
            {
                // TODO: Fix if backtracking is 1
                symbol = ']';
                height = toList[randID].ToString();
                from += ',' + height + symbol;
            }
            else
            {
                if(j == 0)
                {
                    symbol = '[';
                }
                else
                {
                    symbol = ',';
                }

                height = output[output.Length - back].ToString();
                back += 2;

                from += symbol + height;
            }
        }
        return from;
    }

    /// <summary>
    /// Convert the block text into a csv format ready to be read by the sprite tile function.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string ConvertToPureCSV(string value)
    {
        var output = "";
        value = value.Trim(' ');

        // build each block at a time. 
        for(int j = 0; j < value.Length; j++)
        {
            var valueChar = value[j];
            if(valueChar == '[' || valueChar == ',') continue;
            if(valueChar == ']') break;

            output += valueChar + ",";
        }
        return output;
    }


    /// <summary>
    /// Count all seperate non-symbol items.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int Count(string value)
    {
        value = value.Trim(' ', '[', ']');
        
        var count = 0;
        foreach(var item in value)
        {
            if(item == ',') continue;
            count++;
        }
        return count;
    }

    #endregion
}
