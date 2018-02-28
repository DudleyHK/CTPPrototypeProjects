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
        if(runtimeMatrix.Count <= 0) return false;

        Clean();

        // start by picking a random shape.
        var currentShape    = InitialShape(runtimeMatrix);
        var currentSize     = Vector3.zero;
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
            var posID  = sizeID;

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
            obj.transform.position   = data.Position;
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
        m_newLevel      = new List<SceneObject>();
    }

    #endregion

    #region NumbersAsHeight


    /// <summary>
    /// Generate a single level looking only at the one previous tile.
    /// </summary>
    /// <param name="transitionMatrix"></param>
    public void GenerateNumberLevel(Dictionary<string, List<int>> transitionMatrix)
    {
        var output = "";
        // start from the first from in the Dictonary
        var value = transitionMatrix.First().Key;
        for(int i = 0; i < 10; i++)
        {
            // TODO: Write to file. 
            // Output to text file/ debug output or list.
            output += value + ", ";
            
            // get the list from that from value.
            var fromList = transitionMatrix[value];

            // TODO: Based on x number of previous tiles what is the next one going to be. 
            // select random index/ value from the list.
            var randID = Random.Range(0, fromList.Count);
            var from   = fromList[randID];
            
            // set from value as value.
            value = from.ToString(); 
        }

        Debug.Log("Level Heights: " + output);
    }

    #endregion
}
