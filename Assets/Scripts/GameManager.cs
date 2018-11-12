using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{


    public bool sendMethod(params object[] objects)
    {
        object[] args = new object[objects.Length];
        for (int i = 1; i < objects.Length; i++)
        {
            args[i - 1] = objects[i];
        }
        SendMessage(objects[0].ToString(), args);
        return true;
    }
}
