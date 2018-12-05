using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    Transform Player;


    [SerializeField]
    Vector3 offset;

    float minX = -3.293317f;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.position.x >= minX)
        {
            transform.position = new Vector3(Player.position.x + offset.x, offset.y, offset.z);
        }
    }
}
