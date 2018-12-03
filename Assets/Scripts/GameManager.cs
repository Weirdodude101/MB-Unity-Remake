using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name.StartsWith("model_", StringComparison.Ordinal))
                DontDestroyOnLoad(obj);
        }
    }

    


}
