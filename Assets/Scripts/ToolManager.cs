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


    private void Awake()
    {
        // TODO: Error check these.
        m_parseManager      = GetComponent<ParseManager>();
        m_generationManager = GetComponent<GenerationManager>();

        if(m_allParts.Count <= 0)
        {
            Debug.LogWarning("Missing level objects in allParts list. Must contain all parts of a level.");
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Parse()) Generate();
        }
    }



    private bool Parse()
    {
        if(!m_parseManager.ParseLevel(out m_runtimeMatrix))
        {
            Debug.Log("ERROR: Parsing level.");
            return false;
        }
        m_parseManager.ClearScene();
        return true;
    }


    private void Generate()
    {
        if(!m_generationManager.Generate(m_runtimeMatrix, m_allParts)) Debug.Log("ERROR: Generating level.");
    }
}
