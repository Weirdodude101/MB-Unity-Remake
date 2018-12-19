using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    
    GameObject Player;



    Vector3 offset = new Vector3(1.5f, 0f, -10f);

    bool freeMoving;

    readonly float minX = -3.293317f;

    void Start()
    {
        Player = GameObject.Find("Player");
        
        gameObject.layer = 8;
    }

    void Update()
    {
        // Debugging code so I can move past the edge of the screen
        if (Input.GetKeyDown(KeyCode.F))
        {
            freeMoving = !freeMoving;
        }

        if (Player != null)
        {
            if (!freeMoving)
            {
                if (Player.transform.position.x > transform.position.x - offset.x)
                {
                    transform.position = new Vector3(Player.transform.position.x + offset.x, offset.y, offset.z);
                }
            }
            else
            {
                transform.position = new Vector3(Player.transform.position.x + offset.x, offset.y, offset.z);
            }
        }
    }
}
