using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    private enum SizeSelection
    {
        Higher,
        Lower,
        Random,
        Unassigned
    }

    [SerializeField]
    private SizeSelection m_sizeSelection = SizeSelection.Unassigned;

    private List<SceneObject> m_newLevel;


    public bool Generate(List<RuntimeMatrix> runtimeMatrix)
    {
        if(runtimeMatrix.Count <= 0) return false;

        m_newLevel = new List<SceneObject>();

        // start by picking a random shape.
        var currentShape    = "Quad";
        var currentSize     = Vector3.zero;
        var currentPosition = Vector3.zero;

        for(int i = 0; i < 5; i++)
        {
            var rtm    = runtimeMatrix.Find(n => { return n.Name == currentShape; });
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

        OutputNewLevel();
        return true;
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
            Debug.Log("MESSAGE: Size selection type has not been assigned. Defaulting at random.");
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


    private void OutputNewLevel()
    {
#if UNITY_EDITOR
        Debug.Log("New Level <Type, Size, Position>");
        foreach(var obj in m_newLevel)
        {
            Debug.Log(obj.Name + " : " + obj.Size + " : " + obj.Position);
        }
#endif
    }
}
