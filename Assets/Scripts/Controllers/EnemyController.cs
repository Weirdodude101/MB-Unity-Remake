using UnityEngine;
using System.Collections.Generic;


public class EnemyController : GameBase
{

    public bool isDead;
    public Animator anim;
    public AudioSource audio;

    internal BoxCollider2D bcollider;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected int id;

    [SerializeField]
    protected float enemySpeed = 0.5f;


    public static EnemyController enemy;


    public enum ETypes { Goomba, Koopa };
    public ETypes enemyType;

    readonly Dictionary<ETypes, int> type2Id = new Dictionary<ETypes, int>
    {
        {ETypes.Goomba, 0},
        {ETypes.Koopa, 6},
    };

    readonly Dictionary<ETypes, object> type2Class = new Dictionary<ETypes, object>
    {
        {ETypes.Goomba, typeof(Goomba)},
        {ETypes.Koopa, typeof(Koopa)},
    };

    void Start()
    {
        Setup();

        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        gbase.LoadSprites("Sprites/Enemy/enemy_sprites");



        setType(enemyType);
    }


    public void setType(ETypes type)
    {
        enemyType = type;

        spriteRenderer.sprite = gbase.dictSprites["enemy_sprites_" + type2Id[enemyType]];
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(string.Format("Animations/Enemies/{0}/{1}_controller", enemyType.ToString(), enemyType.ToString().ToLower()));
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
            spriteRenderer.flipX = true;
        }
    }
}
