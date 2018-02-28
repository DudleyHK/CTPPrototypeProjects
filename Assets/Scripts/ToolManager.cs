using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour 
{
    [SerializeField]
    private List<GameObject> m_allParts = new List<GameObject>();
    [SerializeField]
    private List<RuntimeMatrix> m_runtimeMatrix;
    [SerializeField]
    private ParseManager m_parseManager; 
    [SerializeField]
    private GenerationManager m_generationManager;
    [SerializeField]
    private uint m_levelSize = 0;


    private void Awake()
    {
        // TODO: Error check these.
        m_parseManager      = GetComponent<ParseManager>();
        m_generationManager = GetComponent<GenerationManager>();

        if(m_allParts.Count <= 0)
        {
            Debug.LogWarning("ERROR: Missing level objects in allParts list. Must contain all parts of a level.");
        }

        if(!m_parseManager.InitSceneObjects(out m_levelSize))
        {
            Debug.LogWarning("ERROR: Initialising ParseManager scene objects failed.");
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Parse())
            { 
                //Generate();
            }
        }
    }


    private bool Parse()
    {
        var transitionMatrix = m_parseManager.ParseHeightLevel(true);

        m_generationManager.GenerateNumberLevel(transitionMatrix);


        //if(!m_parseManager.ParseLevel(out m_runtimeMatrix))
        //{
        //    Debug.LogWarning("ERROR: Parsing level.");
        //    return false;
        //}
        //m_parseManager.ClearScene();
        return true;
    }


    private void Generate()
    {
        if(!m_generationManager.Generate(m_runtimeMatrix, m_allParts, m_levelSize)) Debug.LogWarning("ERROR: Generating level.");
    }
}
