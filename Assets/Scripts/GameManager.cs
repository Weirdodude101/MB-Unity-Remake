using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        /*foreach (string enemy in Enum.GetNames(typeof(EnemyController.ETypes)))
        {
            Debug.Log(enemy);
            DontDestroyOnLoad(GameObject.Find("model_" + enemy));
        }*/
    }

    


}
