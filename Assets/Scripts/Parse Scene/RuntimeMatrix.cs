using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  RuntimeMatrix is a wrapper for any transition matrix generated at run-time and stored in memory.
/// </summary>
public class RuntimeMatrix
{
    // TODO: Change to Type.
    public string Name { get; private set; }

    /// <summary>
    /// The sum total of all cells (value) in the TransitionMatrix
    /// </summary>
    public ushort Total { get; private set; }


    /// <summary>
    /// What is the probability of this runtime transition matrix being chosen?
    /// </summary>
    public float Probability { get; private set; }


    /// <summary>
    /// TransitionMatrix is a List of an Array List with the structure [From, To, Value]
    /// 	From - String, To - String, Value - float
    /// The Value or Cell, will start as a total count and be turned into a Probability value between 0 and 1.
    /// </summary>
    /// <value>The transition matrix.</value>
    public List<ArrayList> TransitionMatrix { get; private set; } // TODO: Change to 2D list. From is always Name.


    /// <summary>
    /// A list of all the sizes which a scene object has been.
    /// </summary>
    public List<Vector3> SizeList { get; private set; }


    /// <summary>
    /// A list of all the positions a scene object has been.
    /// </summary>
    public List<Vector3> PositionList { get; private set; }


    public RuntimeMatrix(string name)
    {
        Name = name;
        TransitionMatrix = new List<ArrayList>();
        SizeList = new List<Vector3>();
        PositionList = new List<Vector3>();

    }


    /// <summary>
    /// Add or update a row in the transition matrix. The format of a row is (from, to, value).
    /// Update the Total value at the same time. 
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="value">Value.</param>
    public void AddRow(string from, string to)
    {
        int index = -1;
        for(int i = 0; i < TransitionMatrix.Count; i++)
        {
            var curr = TransitionMatrix[i];
            if(((string)curr[0] == from) && ((string)curr[1] == to))
            {
                index = i;
                break;
            }
        }

        if(index == -1)
        {
            var list = Increment(new ArrayList { from, to, 0 });
            TransitionMatrix.Add(list);
        }
        else
        {
            var list = Increment(TransitionMatrix[index]);
            TransitionMatrix[index] = list;
        }

        Total++;
    }


    /// <summary>
    /// Insert the size into the size list where is fits based on the total area. 
    /// Use that index to inset the position at that ID in the position list.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="position"></param>
    public void SetSizeAndPosition(Vector3 size, Vector3 position)
    {
        // Init list.
        if(SizeList.Count <= 0)
        {
            SizeList.Add(size);
            PositionList.Add(position);
            return;
        }

        var currArea = Area(size);
        for(int i = 0; i < SizeList.Count; i++)
        {
            var listArea = Area(SizeList[i]);
            if(currArea < listArea)
            {
                SizeList.Insert(i, size);
                PositionList.Insert(i, position);
                return;
            }
        }

        // Add them to the end.
        SizeList.Add(size);
        PositionList.Add(position);
    }


    private float Area(Vector3 size)
    {
        if(size.z == 0)
            size.z = 1;
        return size.x * size.y * size.z;
    }



    /// <summary>
    /// Increment the value part of the array list and return the list.
    /// </summary>
    /// <param name="list">List.</param>
    private ArrayList Increment(ArrayList list)
    {
        var value = System.Convert.ToSingle(list[2]);
        value++;
        list[2] = value;

        return list;
    }
}