using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
public class PlayerController : MonoBehaviour {

    Rigidbody2D rigidBody;
    Animator anim;
    BoxCollider2D collider;
    AudioSource audio;
    AudioSource music;

    Transform Player;
    Transform Ground;


    public bool onGround;
    public bool Dead = false;
    public AudioClip deathSound;
    bool facingRight;
    bool spacePressed;


    [SerializeField]
    float playerSpeed = 1f;

    [SerializeField]
    float jumpHeight = 1f;

    float groundPosY;



    void Start () {
        facingRight = true;
        onGround = false;

        rigidBody = GetComponent<Rigidbody2D> ();
        anim = GetComponent<Animator> ();
        audio = GetComponent<AudioSource> ();
        collider = GetComponent<BoxCollider2D> ();

        Player = GameObject.Find ("Player").transform;
        Ground = GameObject.Find ("Ground").transform;
        music = GameObject.Find ("Main Camera").GetComponent<AudioSource> ();

    }

    void FixedUpdate () {
        float horizontal = Input.GetAxis ("Horizontal");
        Movement (horizontal);
        Flip (horizontal);

    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            onGround = true;
            spacePressed = false;
        }

    }

    bool handleEnemyFunc(EnemyController enemy) {
        enemy.sendMethod(string.Format("handle{0}", enemy.enemyType.ToString()), enemy);
        return true;

    }

    bool handlePlayerDeath(EnemyController enemy = null) {
        if (enemy)
        {
            if (!enemy.isDead)
            {
                anim.SetBool("death", true);
                Dead = true;

                music.Stop();
                music.clip = deathSound;
                music.loop = false;
                music.Play();

                playerSpeed = 0;


                StartCoroutine(Death());
                return true;
            }
            return false;
        }
        return false;
    }

    IEnumerator Death() {
        while (true) {
            rigidBody.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            Destroy(collider);
            yield return new WaitForSeconds(3f);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        EnemyController enemy = col.gameObject.GetComponentInParent<EnemyController>();
        switch (col.gameObject.tag)
        {
            case "enemy_body_collider":
                handlePlayerDeath(enemy);
                break;

            case "koopa_side_collider":
                Koopa koopa = col.gameObject.GetComponentInParent<Koopa>();
                if (koopa.canKill)
                    handlePlayerDeath(enemy);
                enemy.sendMethod("shellMovement", col.gameObject.transform.localPosition.x);
                break;

            default:
                enemy.sendMethod(string.Format("handle{0}", enemy.enemyType.ToString()), enemy);
                break;
        }
    }

    void OnCollisionExit2D(Collision2D col) {
        
        if (col.gameObject.name == "Ground") {
            onGround = false;
        }
    }

    void Movement(float horizontal) {
        rigidBody.velocity = new Vector2 (horizontal * playerSpeed, rigidBody.velocity.y);

        if (Input.GetKeyDown (KeyCode.Space)) {
            if (onGround && !Dead) {
                spacePressed = true;
                rigidBody.AddForce (new Vector2 (0, jumpHeight), ForceMode2D.Impulse);
                onGround = false;
                audio.Play ();
            }
        }
        anim.SetFloat ("speed", Mathf.Abs(horizontal));
        anim.SetBool ("onGround", onGround);
        anim.SetBool ("spacePressed", spacePressed);

    }

    void Flip(float horizontal) {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
