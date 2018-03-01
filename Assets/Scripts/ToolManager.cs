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

    [SerializeField]
    [Range(1, 5)]
    private int m_backTracking = 3;
    [SerializeField]
    private List<float> m_heights;
    [SerializeField]
    private int m_height = 0;
    [SerializeField]
    private bool m_parseLevel = true;

    // TODO: Draw a gizmo box which changes size in the editor. 
    [SerializeField]
    private float m_lowestYPosition = 0;
    [SerializeField]
    private float m_lowestXPosition = 0;

    // TODO: Draw a gizmo box which changes size in the editor. 
    [SerializeField]
    private float m_YSize = 69f; // NOTE: This is hardcoded for this sprite tilemap.
    [SerializeField]
    private float m_XSize = 69f; // NOTE: This is hardcoded for this sprite tilemap.

    private Dictionary<string, List<int>> m_transitionMatrix;


    private void Awake()
    {
        // TODO: Error check these.
        m_parseManager = GetComponent<ParseManager>();
        m_generationManager = GetComponent<GenerationManager>();

        if(m_allParts.Count <= 0)
        {
            Debug.LogWarning("ERROR: Missing level objects in allParts list. Must contain all parts of a level.");
        }

        if(!m_parseManager.InitSceneObjects(out m_levelSize))
        {
            Debug.LogWarning("ERROR: Initialising ParseManager scene objects failed.");
        }


        // TODO: Check if height is < 0
        // Calculate the number of tiles up. ie height of level. 
        m_heights = new List<float>();

        for(int i = 0; i < m_height; i++)
        {
            m_heights.Add(m_lowestYPosition);
            m_lowestYPosition += m_YSize;
        }

        // Gather all tiles of the scene, only once. 
        m_parseManager.ParseLevel(m_heights);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            m_transitionMatrix = m_parseManager.ParseHeightLevel(m_backTracking, m_parseLevel);
            Debug.LogWarning("Warning: Level Parsed. Saving the Transition Matrix is set to " + m_parseLevel);
           
            //if(Parse())
            //{
            //    Generate();
            //}
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            var textHeightLevel = m_generationManager.GenerateNumberLevel(m_transitionMatrix, m_backTracking);
            if(textHeightLevel != "")
            {
                m_generationManager.MapTiles(m_heights, textHeightLevel, m_lowestXPosition, m_XSize);

                Debug.LogWarning("Warning: Level Generated");
            }
            else
            {
                Debug.LogError("Error: Generating level.");
            }


        }
    }


    private void Generate()
    {
        if(!m_generationManager.Generate(m_runtimeMatrix, m_allParts, m_levelSize))
            Debug.LogWarning("ERROR: Generating level.");
    }
}
