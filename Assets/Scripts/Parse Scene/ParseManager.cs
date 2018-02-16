using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParseManager : MonoBehaviour
{
    private List<SceneObject> m_sceneObjects    = new List<SceneObject>();
    private List<GameObject> m_visibleObjects;


    private void Start()
    {
        if(!InitSceneObjects()) return;
    }


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
                findMatrix.AddRow(sceneObject.Name, m_sceneObjects[i + 1].Name);
                findMatrix.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
            }
            else
            {
                var rtm = new RuntimeMatrix(sceneObject.Name);
                rtm.SetSizeAndPosition(sceneObject.Size, sceneObject.Position);
                
                // HACKHACK: If last i set to valut to default.
                if(i == (m_sceneObjects.Count - 1))
                {
                    // Add the last value in a line end.
                    rtm.AddRow(sceneObject.Name, "/0");
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
    private bool InitSceneObjects()
    {
        /// Test Level Objects.
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(5, 5, 0), new Vector3(1, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(10, 5, 0), new Vector3(5, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Triangle", new Vector3(10, 5, 0), new Vector3(6, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(10, 2, 0), new Vector3(7, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Circle", new Vector3(7, 7, 0), new Vector3(9, 1, 0)));
        /// m_sceneObjects.Add(new SceneObject("Quad", new Vector3(3, 11, 0), new Vector3(10, 1, 0)));

        m_visibleObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("LevelObject"));
        if(m_visibleObjects.Count <= 0)
        {
            Debug.Log("ERROR: There are no level objects in the scene.. Have you applied the tags?");
            return false;
        }

        foreach(var obj in m_visibleObjects)
        {
            var name     = obj.name;
            var size     = obj.transform.lossyScale;
            var position = obj.transform.position;

            m_sceneObjects.Add(new SceneObject(name, size, position));
        }
        return true;
    }
}
