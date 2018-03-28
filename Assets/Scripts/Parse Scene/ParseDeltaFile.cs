using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ParseDeltaFile : MonoBehaviour
{

    [HideInInspector]
    public bool Init { get; private set; }

    [SerializeField]
    private List<int> values;
    [SerializeField]
    private List<float> heights;


    public void ReadDeltaFile()
    {
        string line;
        // Read File
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/DeltaFile.txt"))
        {
            // Read the file.
            var streamReader = new System.IO.StreamReader(Application.dataPath + "/Resources" + "/DeltaFile.txt");
            line = streamReader.ReadLine();
        }
        else
        {
            return;
        }

        var lineValues = StringToList(line);
        if(lineValues.Count > 0) Init = true;
        else Init = false;

        values = lineValues;
    }


    public List<float> WorldHeightList(float startYPosition)
    {
        if(!Init) return null;
        heights = new List<float>();

        var currentHeight = HeightStep(values[0], (int)startYPosition);
        heights.Add(currentHeight);

        for(int i = 1; i < values.Count; i++)
        {
            currentHeight = HeightStep(values[i], currentHeight);
            heights.Add(currentHeight);

            ///var previousValue = values[i - 1];
            ///if(values[i] != previousValue)
            ///{
            ///    // update height value.
            ///    currentHeight = HeightStep(values[i], currentHeight);
            ///    heights.Add(currentHeight);
            ///}
            ///else
            ///{
            ///    heights.Add(heights[i - 1]);
            ///}
        }
        return heights;
    }

    /// <summary>
    /// Convert the delta line into a row of lines.
    /// IF the character is preceeded with '-' make the value negative.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private List<int> StringToList(string line)
    {
        var output = new List<int>();
        int result;
        char next;

        for(var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if(c == '-')
            {
                next = line[i + 1];
                if(int.TryParse(next.ToString(), out result))
                {
                    result *= -1;
                    output.Add(result);
                    i++; // Skip next value in line.
                }
            }
            else
            {
                if(int.TryParse(c.ToString(), out result))
                    output.Add(result);
            }
        }
        return output;
    }



    public int HeightStep(int id, int height)
    {
        if(id < 0)
            for(var i = 0; i < Mathf.Abs(id); i++)
                height -= 69;
        if(id >= 0)
            for(var i = 0; i < id; i++)
                height += 69;
        return height;
    }
}
