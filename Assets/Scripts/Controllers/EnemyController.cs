using UnityEngine;
using System.Collections.Generic;
using System;


public class EnemyController : GameBase
{

    public bool Dead { get; set; }
    public Animator anim;
    public AudioSource audio;

    internal BoxCollider2D bcollider;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected int id;

    [SerializeField]
    protected float enemySpeed = 0.5f;


    public static EnemyController enemyController;


    public enum ETypes { Goomba, Koopa };
    public ETypes enemyType;

    protected readonly Dictionary<string, Vector2> sizes = new Dictionary<string, Vector2>
    {
        {"enemy_body_collider", new Vector2(1.27f, 1.43f)},
        {"koopa_shell", new Vector2(0.16f, 0.14f)}
    };

    readonly Dictionary<ETypes, int> type2Id = new Dictionary<ETypes, int>
    {
        {ETypes.Goomba, 0},
        {ETypes.Koopa, 6},
    };

    readonly Dictionary<ETypes, Type> type2Class = new Dictionary<ETypes, Type>
    {
        {ETypes.Goomba, typeof(Goomba)},
        {ETypes.Koopa, typeof(Koopa)},
    };
    
    void Start()
    {


        Setup();

        gameObject.AddComponent(type2Class[enemyType]);

        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //enemy = GetComponent(type2Class[enemyType]);

        gbase.LoadSprites("Sprites/Enemy/enemy_sprites");
        //enemy.Start();
        SetType(enemyType);



       

        //enemy = null;

        //enemy = 
        //enemy = Convert.ChangeType(enemy, type2Class[enemyType].GetType());
        //enemy = Activator.CreateInstance(type2Class[enemyType].GetType());

    }


    public void SetType(ETypes type)
    {
        enemyType = type;
        spriteRenderer.sprite = gbase.dictSprites["enemy_sprites_" + type2Id[enemyType].ToString()];
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(string.Format("Animations/Enemies/{0}/{1}_controller", enemyType.ToString(), enemyType.ToString().ToLower()));
    }

    public int GetId()
    {
        return id;
    }

    public void SetSpeed(float speed)
    {
        enemySpeed = speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        /*
        if (enemyType == ETypes.Koopa)
        {
            Koopa koopa = GetComponent<Koopa>();
            if (koopa.anim.GetBool("inShell") && col.gameObject.tag == "collider")
            {
                koopa.collided = true;
            }
                
        }*/

        if (col.gameObject.tag == "collider")
            Flip();
    }

    protected void Flip()
    {
        if (enemySpeed >= 0)
        {
            SetSpeed(-enemySpeed);
            spriteRenderer.flipX = true;
        }
    }
}
