using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed = 5f;
    public List<Buttons> input = new List<Buttons>();
    
    private Rigidbody2D body2D;
    private InputState inputState;


	void Start () 
    {
	    body2D = GetComponent<Rigidbody2D>();
        inputState = GetComponent<InputState>();
    }
	


	void Update () 
    {
        var right = inputState.GetButtonValue(input[0]);
        var left = inputState.GetButtonValue(input[1]);
        var velX = speed;

        if(right || left)
        {
            velX *= left ? -1 : 1;
        }
        else
        {
            velX = 0f;
        }

        body2D.velocity = new Vector2(velX, body2D.velocity.y);
    }
}
