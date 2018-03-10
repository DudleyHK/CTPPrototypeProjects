using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbstractBehaviour : MonoBehaviour 
{
    public List<Buttons> inputButtons = new List<Buttons>();

    protected InputState inputState;
    protected Rigidbody2D body2D;
	protected CollisionState collisionState;


    protected virtual void Awake ()
    {
		inputState     = GetComponent<InputState>();
        body2D         = GetComponent<Rigidbody2D>();
		collisionState = GetComponent<CollisionState>();
	}
}
