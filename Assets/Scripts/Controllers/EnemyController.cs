using UnityEngine;
using System.Collections.Generic;
using System;


public class EnemyController : GameBase
{

    public bool Dead;
    public Animator anim;
    public AudioSource audio;

    internal BoxCollider2D bcollider;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected float enemySpeed = 0.5f;


    public static EnemyController enemyController;


    public enum ETypes { Goomba, Koopa };
    public ETypes enemyType;

    bool isColliding;

    protected readonly Dictionary<string, Vector2> vectors = new Dictionary<string, Vector2>
    {
        {"Koopa_collider", new Vector2(0.16f, 0.23f)},
        {"Koopa_offset", new Vector2(0f, 0f)},
        {"Koopa_shell", new Vector2(0.16f, 0.14f)},
        {"Koopa", new Vector2(1.5f, 1.5f)},
        {"Goomba_collider", new Vector2(0.16f, 0.12f)},
        {"Goomba_offset", new Vector2(0f, -0.02f)},
        {"Goomba", new Vector2(2f, 2f)},

        {"enemy_body_collider", new Vector2(1.27f, 1.43f)},
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

    public dynamic enemy;
    
    void Start()
    {
        Setup();

        gameObject.AddComponent(type2Class[enemyType]);

        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        bcollider = GetComponent<BoxCollider2D>();

        enemy = GetComponent(type2Class[enemyType]);

        gbase.LoadSprites("Sprites/Enemy/enemy_sprites");

        SetType(enemyType);
        PrepareGameObject();
    }

    void Update()
    {
        isColliding = false;
    }

    void PrepareGameObject() {

        gameObject.name = enemyType.ToString();

        GameObject model_load = GameObject.Find(String.Format("model_{0}", gameObject.name));
        foreach (Transform child in model_load.transform)
        {
            Instantiate(child, transform);
        }

        foreach (Transform child in transform) {
            if (!child.gameObject.name.StartsWith(enemyType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Destroy(child.gameObject);
                continue;
            }

            child.gameObject.name = child.gameObject.name.Substring(0, child.name.Length - 7);

        }

        AudioClip clip = Resources.Load<AudioClip>(String.Format("Audio/{0}_sound", enemyType.ToString()));
        gbase.setMusic(audio, clip, false, false);

        transform.tag = enemyType.ToString();
        transform.localScale = vectors[String.Format("{0}", enemyType.ToString())];
        bcollider.size = vectors[String.Format("{0}_collider", enemyType.ToString())];
        bcollider.offset = vectors[String.Format("{0}_offset", enemyType.ToString())];
    }

    public void SetType(ETypes type)
    {
        enemyType = type;
        spriteRenderer.sprite = gbase.dictSprites["enemy_sprites_" + type2Id[enemyType].ToString()];
        anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(string.Format("Animations/Enemies/{0}/{1}_controller", enemyType.ToString(), enemyType.ToString().ToLower()));
    }

    public void SetSpeed(float speed)
    {
        enemySpeed = speed;

    }

    public float GetSpeed(){
        return enemySpeed;
    }
    
    public void SetDead(bool dead)
    {
        enemy.Dead = dead;
    }

    public bool IsDead() {
        return enemy.Dead;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isColliding) return;
        isColliding = true;

        if (enemyType == ETypes.Koopa)
        {
            if (enemy.anim.GetBool("inShell") && (col.gameObject.tag == "collider" || col.gameObject.tag == "Block"))
            {
                enemy.collided = true;
            }
                
        }

        if (col.gameObject.tag == "collider" || col.gameObject.tag == "Block" || (Enum.IsDefined(typeof(ETypes), col.gameObject.tag) && col.gameObject.GetComponent<EnemyController>().enemyType != ETypes.Goomba))
        {
            Flip();
        }
    }

    protected void Flip()
    {
        if (Mathf.Abs(enemySpeed) >= 0)
        {
            SetSpeed(-enemySpeed);
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
}
