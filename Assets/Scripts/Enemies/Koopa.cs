using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Koopa : EnemyController
{

    public const float hitSpeed = 2f;

    public bool shellMoving;
    public bool canKill;

    float time_left = 8f;


    internal bool collided;
    bool hitByKoopa;


    public static Koopa koopa;

    readonly Dictionary<string, Vector2> sizes = new Dictionary<string, Vector2>
    {
        {"enemy_body_collider", new Vector2(1.27f, 1.43f)},
        {"koopa_shell", new Vector2(0.16f, 0.14f)}
    };

    void Start()
    {
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        anim.SetFloat("time_left", time_left);
        setType(ETypes.Koopa);

    }

    void FixedUpdate()
    {
        if (time_left <= 1)
        {
            anim.SetBool("inShell", false);
            shellMoving = false;
            time_left = 8f;
            bcollider.enabled = true;
            setSpeed(0.5f);

            foreach (Transform child in transform)
            {
                BoxCollider2D c = child.GetComponent<BoxCollider2D>();
                switch (child.tag)
                {
                    case "koopa_side_collider":
                        c.isTrigger = false;
                        break;

                    case "enemy_body_collider":
                        c.isTrigger = true;
                        c.size = sizes["enemy_body_collider"];
                        break;

                    case "koopa_shell":
                        c.enabled = false;
                        break;

                }
            }

        }

        Movement();
    }

    public void handleKoopa(object[] args)
    {
        enemy = (EnemyController)args[0];
        if (!enemy.anim.GetBool("inShell"))
        {

            enemy.anim.SetBool("inShell", true);
            enemy.bcollider.enabled = false;
            enemy.setSpeed(0f);

            foreach (Transform child in transform)
            {
                BoxCollider2D c = child.GetComponent<BoxCollider2D>();
                switch (child.tag)
                {

                    case "koopa_side_collider":
                        c.isTrigger = true;
                        break;

                    case "enemy_body_collider":
                        c.isTrigger = false;
                        c.size = new Vector2(0, 0);
                        break;

                    case "koopa_shell":
                        c.enabled = true;
                        c.size = sizes["koopa_shell"];
                        break;

                }
            }
            StartCoroutine(koopa_countdown());
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
                    float x = 0.125f;
                    if (koopa.anim.GetBool("inShell") && koopa.getXVel() > 0 && anim.GetBool("inShell"))
                    {
                        enemySpeed *= -1;
                        x *= -1;
                    }

                    hitByKoopa = true;
                    
                    rigidBody.AddForce(new Vector2(x, 1f), ForceMode2D.Impulse);
                    StartCoroutine(Death(2f));
                }
                break;
        }
    }

    public void ShellMove(float localPos)
    {
        if (anim.GetBool("inShell") && shellMoving)
        {
            canKill = true;
            time_left = 8f;
            StartCoroutine(shellMove(localPos));
        }
    }

    IEnumerator Death(float t)
    {
        while (true)
        {
            if (hitByKoopa)
            {
                spriteRenderer.flipY = true;
                isDead = true;

                bcollider.enabled = false;
                foreach (Transform child in transform)
                {
                    BoxCollider2D collider2D = child.GetComponent<BoxCollider2D>();
                    collider2D.enabled = false;
                }


            }
            yield return new WaitForSeconds(t);
            Destroy(gameObject);
        }
    }


    IEnumerator shellMove(float localPos)
    {
        while (true)
        {

            float v = hitSpeed;

            yield return new WaitUntil(() => Mathf.Abs(rigidBody.velocity.x) < hitSpeed);

            if (localPos > 0)
            {
                Debug.Log("v = -hitSpeed: " + localPos);
                v = -hitSpeed;
            }

            if (collided)
            {
                Debug.Log("v *= -1");
                v *= -1;
            }



            rigidBody.velocity = new Vector2(v, rigidBody.velocity.y);

            break;
        }
    }

    IEnumerator koopa_countdown()
    {
        while (time_left > 0 && !shellMoving)
        {
            yield return new WaitForSeconds(1f);
            time_left -= 1;
            anim.SetFloat("time_left", time_left);
        }
    }

    public float getXVel()
    {
        return rigidBody.velocity.x;
    }

    void Movement()
    {

        if (!anim.GetBool("inShell"))
        {
            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
        }
    }

}
