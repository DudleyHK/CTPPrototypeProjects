



public class UIManager : UnityEngine.MonoBehaviour
{ 
    public delegate void Parse();
    public static event Parse parse;

    public delegate void Generate();
    public static event Generate generate;



    public void ParseEffect()
    {
        if(parse != null) parse();
    }


    public void GenerateEffect()
    {
        if(generate != null) generate();
    }

    

}
