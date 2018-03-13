using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour 
{
    public List<InputAxisState> inputs = new List<InputAxisState>();
    public InputState inputState;
    

    private void Start()
    {
        FindPlayer();
    }

    private void Update() 
    {
        if(!inputState)
        {
            FindPlayer();
            return;
        }

        foreach(var input in inputs)
            inputState.SetButtonValue(input.button, input.value);
	}


    private void FindPlayer()
    {
        var findPlayer = GameObject.FindGameObjectWithTag("Player");
        if(!findPlayer)
        {
            Debug.Log("Player tag cannot be found or object is not set in the CameraManager class");
            return;
        }
        else
        {
            inputState = findPlayer.GetComponent<InputState>();
        }
    }
}
