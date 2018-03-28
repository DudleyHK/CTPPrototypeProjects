using UnityEngine;

public class UIManager : MonoBehaviour
{ 
    public delegate void Parse();
    public static event Parse parse;

    public delegate void Generate();
    public static event Generate generate;

    public delegate void ResetLevel();
    public static event ResetLevel resetLevel;

    public delegate void BacktrackingChanged(int value);
    public static event BacktrackingChanged backtrackingChanged;

    [SerializeField]
    private UnityEngine.UI.Slider m_backtracking;
    [SerializeField]
    private UnityEngine.UI.Text m_backtrackingValue;
    [SerializeField]
    private Vector2 m_scrollPosition;


    private void Start()
    {
        m_backtracking.onValueChanged.AddListener((float value) => OnBacktrackChange(value));
        OnBacktrackChange(m_backtracking.value);
    }


    private void Update()
    {
        if(Input.GetButtonDown("Back"))
        {
            ResetButton();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            GenerateEffect();
        }

        if(Input.GetKeyDown(KeyCode.Space))
            ParseEffect();
    }

    private void OnGUI()
    {
        GUI.color = Color.black;

        m_scrollPosition = GUILayout.BeginScrollView(
            m_scrollPosition, 
            GUILayout.Width(300), 
            GUILayout.Height(450), 
            GUILayout.ExpandHeight(true), 
            GUILayout.ExpandWidth(true));

        GUILayout.Label(TransitionFileText());
        GUILayout.EndScrollView();
    }



    public void ParseEffect()
    {
        if(parse != null) parse();
    }


    public void GenerateEffect()
    {
        if(generate != null) generate();
    }


    public void ResetButton()
    {
        if(resetLevel != null) resetLevel();
    }


    private void OnBacktrackChange(float value)
    {
       if(backtrackingChanged != null) backtrackingChanged((int)value);
       m_backtrackingValue.text = value.ToString();
    }


    /// <summary>
    /// TODO: Put this in a better place for all classes to use.
    /// </summary>
    private string TransitionFileText()
    {
        var output = "";
        if(System.IO.File.Exists(Application.dataPath + "/Resources" + "/TransitionMatrix.txt"))
        {
            // Read the file. 
            var streamReader = new System.IO.StreamReader(Application.dataPath + "/Resources" + "/TransitionMatrix.txt");
            var line = "";
            
            while((line = streamReader.ReadLine()) != null)
                output += line + '\n';
            streamReader.Close();
        }
        else
        {
            Debug.LogWarning("INVALID: File doesn't exist. transitionmatrix will start fresh");
        }
        return output;
    }

}
