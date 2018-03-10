using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : AbstractBehaviour
{
    public float speed = 50f;
    public float runMultiplier = 2f;
	public bool running = false;


    private void Update()
    {
		running = false;

        var right = inputState.GetButtonValue(inputButtons[0]);
        var left = inputState.GetButtonValue(inputButtons[1]);
        var run = inputState.GetButtonValue(inputButtons[2]);


        if(right || left)
        {
            var tempSpeed = speed;

            if(run && runMultiplier > 0f)
            {
                tempSpeed *= runMultiplier;
				running = true;
            }

            var velX = tempSpeed * (float)inputState.direction;
            body2D.velocity = new Vector2(velX, body2D.velocity.y);
        }
    }
}
