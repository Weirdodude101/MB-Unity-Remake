using UnityEngine;

public class EnemyController : MonoBehaviour
{


    public enum ETypes { Goomba, Koopa, test2 };

    public ETypes enemyType;
    public bool isDead = false;
    public Animator anim;
    public AudioSource audio;

    internal BoxCollider2D bcollider;

    [SerializeField]
    protected float enemySpeed = 0.5f;

    protected Rigidbody2D rigidBody;

    [SerializeField]
    protected int id = 0;

    protected SpriteRenderer sprite;

    public static EnemyController enemy;


    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
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
        if (enemyType == ETypes.Koopa)
        {
            Koopa koopa = GetComponent<Koopa>();
            if (koopa.anim.GetBool("inShell") && col.gameObject.tag == "collider")
            {
                koopa.collided = true;
            }
                
        }

        if (col.gameObject.tag == "collider")
            Flip();
    }

    protected void Flip()
    {
        if (enemySpeed >= 0)
        {
            setSpeed(-enemySpeed);
            sprite.flipX = true;
        }
    }
}
