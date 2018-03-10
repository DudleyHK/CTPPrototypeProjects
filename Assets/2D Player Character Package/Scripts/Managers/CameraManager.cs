using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraManager : MonoBehaviour 
{
    public GameObject player;
    public float acceleration = 0.1f;


    private void Start()
    {
        if(player == null)
        {
            var playerTag = GameObject.FindGameObjectWithTag("Player");
            if(!playerTag)
            {
                Debug.Log("Player tag cannot be found or object is not set in the CameraManager class");
            }
        }
    }

    private void FixedUpdate()
    {
        var cameraPosition = this.transform.position;
        var velocity = (player.transform.position - cameraPosition) * acceleration;
        transform.Translate(velocity.x, 0, 0);
    }
}
