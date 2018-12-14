using UnityEngine;
using System;
using System.Collections;


public class Koopa : EnemyController
{

    public const float hitSpeed = 4f;
    float tempSpeed;

    public bool shellMoving;
    public bool canKill;

    float time_left = 8f;


    internal bool collided;
    bool hitByKoopa;

    float storedSpeed;

    public static Koopa koopa;


    void Start()
    {
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        anim.SetFloat("time_left", time_left);

    }

    void FixedUpdate()
    {
        Movement();
    }

    public void ResetKoopa()
    {
        anim.SetBool("inShell", false);
        shellMoving = false;

        bcollider.enabled = true;
        SetSpeed(storedSpeed);

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
                    c.size = vectors["enemy_body_collider"];
                    break;

                case "koopa_shell":
                    c.enabled = false;
                    break;
            }

        }
    }

    public void ResetShellTimer() {
        time_left = 8f;
        anim.SetFloat("time_left", time_left);
    }

    public void DecrementShellTimer() {
        time_left -= 1;
        anim.SetFloat("time_left", time_left);
    }

    public void HandleKoopa(object[] args)
    {
        if (!anim.GetBool("inShell"))
        {

            anim.SetBool("inShell", true);
            bcollider.enabled = false;
            storedSpeed = GetSpeed();
            SetSpeed(0f);
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
                        c.size = vectors["Koopa_shell"];
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

    public void ShellMove(int side)
    {
        if (anim.GetBool("inShell") && !shellMoving)
        {
            canKill = true;
            shellMoving = true;
            ResetShellTimer();
            StartCoroutine(shellMove(side));
        }
    }

    IEnumerator shellMove(int side)
    {
        while (shellMoving)
        {
            
            yield return new WaitUntil(() => Mathf.Abs(rigidBody.velocity.x) < hitSpeed);

            tempSpeed = -hitSpeed;
            if (side == 3)
            {
                tempSpeed = hitSpeed;
            }

            if (collided)
            {
                tempSpeed *= -1;
            }

        }
    }

    IEnumerator Death(float t)
    {
        while (true)
        {
            if (hitByKoopa)
            {
                spriteRenderer.flipY = true;
                Dead = true;

                bcollider.enabled = false;
                foreach (Transform child in transform)
                {
                    child.GetComponent<BoxCollider2D>().enabled = false;
                }


            }
            yield return new WaitForSeconds(t);
            Destroy(gameObject);
        }
    }

    IEnumerator koopa_countdown()
    {
        while (time_left > 0 && !shellMoving)
        {
            yield return new WaitForSeconds(1f);
            DecrementShellTimer();
        }

        if (time_left <= 0) {
            ResetKoopa();
        }

    }

    public float getXVel()
    {
        return rigidBody.velocity.x;
    }

    void Movement()
    {

        while (shellMoving)
        {
            rigidBody.velocity = new Vector2(tempSpeed, rigidBody.velocity.y);
            break;
        }

        if (!anim.GetBool("inShell"))
        {
            if (time_left < 8)
                ResetShellTimer();

            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
        }
        if (GetSpeed() <= 0 && !shellMoving && anim.GetBool("inShell")) {
            rigidBody.velocity = new Vector2(0f, 0f);
        }
    }

}
