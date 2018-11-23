using UnityEngine;
using System.Collections;

public class GameBase : MonoBehaviour
{

    protected GameBase gbase;

    protected void Setup() {
        gbase = this;
    }

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

    public void setMusic(AudioSource source, AudioClip clip, bool loop)
    {
        source.Stop();
        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    public int getSide(Transform t0, Transform t1, bool top)
    {
        Vector2 dir = (t0.position - t1.position).normalized;
        //Debug.Log(dir.x);
        if (top)
        {
            if (dir.y > 0)
                // TOP
                return 0;

            if (dir.y < 0)
                // BOTTOM
                return 1;
        }

        if (dir.x > 0)
            // RIGHT
            return 2;

        if (dir.x < 0)
            // LEFT
            return 3;

        return 4;


    }
}
