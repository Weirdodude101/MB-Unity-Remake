using UnityEngine;
using System.Collections;
public class PlayerController : GameBase
{

    GameManager _gameManager;

    Rigidbody2D rigidBody;

    Animator anim;
    AudioSource music;

    public bool onGround;
    public bool Dead;
    public Vector2 velocity;
    public AudioClip deathSound;

    bool facingRight = true;

    [SerializeField]
    float playerSpeed = 1f;

    [SerializeField]
    float jumpHeight = 1f;

    void Start()
    {
        //_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Setup();

        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        music = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        transform.tag = "Player";
    }

    void FixedUpdate()
    {
        velocity = rigidBody.velocity;

        float horizontal = Input.GetAxis("Horizontal");

        Movement(horizontal);
        Flip(horizontal);

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Block")
        {
            if (gbase.getSide(gameObject.transform, col.gameObject.transform, true) == 0)
                onGround = true;
        }

    }

    bool handlePlayerDeath(EnemyController enemy = null)
    {
        if (enemy)
        {
            if (!enemy.Dead)
            {
                StartCoroutine(Death());
                return true;
            }
            return false;
        }
        return false;
    }

    IEnumerator Death()
    {
        Dead = true;
        anim.SetBool("death", Dead);

        gbase.setMusic(music, deathSound, false, true);

        playerSpeed = 0;

        while (true) {
            rigidBody.AddForce(new Vector2(0, jumpHeight-0.75f), ForceMode2D.Impulse);
            Destroy(GetComponent<BoxCollider2D>());
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyController enemy = col.gameObject.GetComponentInParent<EnemyController>();
        switch (col.gameObject.tag)
        {

            case "enemy_body_collider":
                if (!enemy.IsDead())
                    handlePlayerDeath(enemy);
                break;

            case "koopa_side_collider":
                Koopa koopa = col.gameObject.GetComponentInParent<Koopa>();
                if (koopa.canKill)
                    handlePlayerDeath(enemy);


                koopa.ShellMove(col.gameObject.transform.localPosition.x);
                break;


            default:
                Jump(jumpHeight - 0.25f);
                enemy.sendMethod(string.Format("Handle{0}", enemy.enemyType.ToString()), enemy);
                break;
        }
    }


    void Movement(float horizontal)
    {
        rigidBody.velocity = new Vector2(horizontal * playerSpeed, rigidBody.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (onGround && !Dead)
            {
                Jump(jumpHeight);
                onGround = false;

                GetComponent<AudioSource>().Play();
            }
        }

        anim.SetFloat("speed", Mathf.Abs(horizontal));
        anim.SetBool("onGround", onGround);
        anim.SetBool("spacePressed", !onGround);

    }

    void Jump(float height) {
        rigidBody.velocity = new Vector2(0, height);
    }

    void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }
}
