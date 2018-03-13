using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraManager : MonoBehaviour 
{
    [SerializeField]
    private float m_speedMulti = 0.1f;
    [SerializeField]
    private float m_acc = 0.1f;

    private GameObject m_player;
    private Rigidbody2D m_playerRB;


    private void Start()
    {
        FindPlayer();
    }

    private void FixedUpdate()
    {
        if(!m_player)
        {
            FindPlayer();
            return;
        }
        
        Vector2 vel = (m_player.transform.position - this.transform.position) * m_acc;
        vel += m_playerRB.velocity * m_speedMulti;

        transform.Translate(vel.x * Time.fixedDeltaTime, 0, 0);

    }


    private void FindPlayer()
    {
        if(m_player == null)
        {
            var findPlayer = GameObject.FindGameObjectWithTag("Player");
            if(!findPlayer)
            {
                Debug.Log("Player tag cannot be found or object is not set in the CameraManager class");
                return;
            }
            else
            {
                m_player = findPlayer;
                m_playerRB = m_player.GetComponent<Rigidbody2D>();
            }
        }
    }
}
