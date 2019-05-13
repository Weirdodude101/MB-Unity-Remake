using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


public class EnemyController : GameBase
{
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
    bool onScreen;
    bool running;

    private bool _dead;
    private bool _canDamage;
    private bool _hitByShell;


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
        gameObject.AddComponent(type2Class[enemyType]);

        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        bcollider = GetComponent<BoxCollider2D>();

        enemy = GetComponent(type2Class[enemyType]);

        gbase.LoadSprites("Sprites/Enemy/enemy_sprites");

        SetType(enemyType);
        PrepareGameObject();

        enabled = false;
    }

    internal void SetEnemyInstance(dynamic e) {
        enemy = e;
    }

    void Update()
    {

        Vector3 screenPoint = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToViewportPoint(transform.position);
        onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        IEnumerator co = OffScreenDestroy();

        if (!onScreen && !running)
            StartCoroutine(co);


        if(GameObject.Find("Player").GetComponent<PlayerController>().IsDead())
        {
            SetSpeed(0);
            if (enemyType == ETypes.Koopa)
            {
                enemy.SetTempSpeed(0);
            }
        }

        isColliding = false;
    }

    void PrepareGameObject() {

        gameObject.name = enemyType.ToString();
        gameObject.layer = 9;
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
            child.gameObject.layer = 9;
        }

        AudioClip clip = Resources.Load<AudioClip>(String.Format("Audio/{0}_sound", enemyType.ToString()));
        gbase.SetMusic(audio, clip, false, false);

        transform.tag = enemyType.ToString();
        transform.localScale = vectors[String.Format("{0}", enemyType.ToString())];
        bcollider.size = vectors[String.Format("{0}_collider", enemyType.ToString())];
        bcollider.offset = vectors[String.Format("{0}_offset", enemyType.ToString())];
    }

    IEnumerator OffScreenDestroy()
    {
        running = true;
        yield return new WaitUntil(() => !onScreen);

        yield return new WaitForSeconds(1f);

        if (!onScreen)
            Destroy(gameObject);

        running = false;

    }

    public void SetHitByShell(bool hitByShell)
    {
        _hitByShell = hitByShell;
    }

    public bool GetHitByShell()
    {
        return _hitByShell;
    }

    public void SetCanDamage(bool canDamage)
    {
        _canDamage = canDamage;
    }

    public bool GetCanDamage()
    {
        return _canDamage;
    }

    public void SetType(ETypes type)
    {
        enemy.enemyType = type;
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
        enemy._dead = dead;
        enemy.anim.SetBool("isDead", enemy._dead);
    }

    public bool IsDead() {
        return enemy._dead;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isColliding) return;
        isColliding = true;

        if (enemyType == ETypes.Koopa)
        {
            if (enemy.GetInShell() && (col.gameObject.tag == "Block" && col.gameObject.layer == 10))
            {
                enemy.SetHitWall(true);
            }
        }

        if ((col.gameObject.tag == "Block" && col.gameObject.layer == 10) || (Enum.IsDefined(typeof(ETypes), col.gameObject.tag) && col.gameObject.GetComponent<EnemyController>().enemyType != ETypes.Goomba))
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
