using UnityEngine;

public class EnemyController : GameManager
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
    protected int id;

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
