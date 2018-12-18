using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    Transform Player;



    Vector3 offset = new Vector3(1.5f, 0f, -10f);

    bool freeMoving;

    readonly float minX = -3.293317f;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player").transform;
        
        gameObject.layer = 8;
    }

    // Update is called once per frame
    void Update()
    {
        // Debugging code so I can move past the edge of the screen
        if (Input.GetKeyDown(KeyCode.F))
        {
            freeMoving = !freeMoving;
        }

        if (!freeMoving)
        {
            if (Player.transform.position.x > transform.position.x - offset.x)
            {
                transform.position = new Vector3(Player.position.x + offset.x, offset.y, offset.z);
            }
        } else {
            transform.position = new Vector3(Player.position.x + offset.x, offset.y, offset.z);
        }
    }
}
