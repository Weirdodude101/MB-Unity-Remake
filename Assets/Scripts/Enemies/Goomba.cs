using UnityEngine;
using System.Collections;

public class Goomba : EnemyController
{

    public static Goomba goomba;



    bool hitByKoopa;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        ///setType(ETypes.Goomba);
    }

    void FixedUpdate()
    {
        Movement();
        CheckDead();
    }

    public void handleGoomba(object[] args)
    {
        enemyController = (EnemyController)args[0];
        enemyController.SetDead(true);
        audio.time = 0.5f;
        audio.Play();
        //Dead = true;

    }

    void CheckDead()
    {
        if (Dead == true)
        {
            enemySpeed = 0f;
            anim.SetBool("isDead", Dead);
            bcollider.size = new Vector2(bcollider.size.x, 0.04f);
            StartCoroutine(Death(0.3f));
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Koopa koopa = col.gameObject.GetComponentInParent<Koopa>();
        switch (col.gameObject.tag)
        {
            case "koopa_side_collider":
                if (koopa.shellMoving)
                {
                    hitByKoopa = true;
                    rigidBody.AddForce(new Vector2(0.75f, 1f), ForceMode2D.Impulse);
                    StartCoroutine(Death(10f));
                }

                break;
        }
    }

    void Movement()
    {
        if (!hitByKoopa)
            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
    }


    IEnumerator Death(float t)
    {
        while (true)
        {
            if (hitByKoopa)
            {

                foreach (Transform child in transform)
                {
                    BoxCollider2D collider2D = child.GetComponent<BoxCollider2D>();
                    collider2D.enabled = false;
                }
                spriteRenderer.flipY = true;
                bcollider.enabled = false;

            }
            
            yield return new WaitForSeconds(t);
            Destroy(gameObject);
        }
    }
}
