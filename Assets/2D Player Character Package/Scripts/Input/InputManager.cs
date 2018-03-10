using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour 
{
    public List<InputAxisState> inputs = new List<InputAxisState>();
    public InputState inputState;

	
	void Update () 
    {
        foreach(var input in inputs)
        {
            inputState.SetButtonValue(input.button, input.value);
        }
	}
}
