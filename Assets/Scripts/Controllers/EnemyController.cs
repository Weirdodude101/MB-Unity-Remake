using UnityEngine;

public class EnemyController : MonoBehaviour
{


    public enum ETypes { Goomba, Koopa, test2 };

    public ETypes enemyType;
    public bool isDead = false;
    public Animator anim;
    public AudioSource audio;

    internal BoxCollider2D bcollider;
    bool facingRight;

    [SerializeField]
    protected float enemySpeed = 0.5f;

    protected Rigidbody2D rigidBody;

    [SerializeField]
    protected int id = 0;

    public static EnemyController enemy;


    void Start()
    {

    }


    public void setType(ETypes type)
    {
        enemyType = type;

    }

    public int getId()
    {
        return id;
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

    public void setSpeed(float speed)
    {
        enemySpeed = speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Collider")
        {
            if (enemySpeed >= 0)
            {
                Flip();
            }
        }
    }

    protected void Flip()
    {
        enemySpeed *= -1;
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
