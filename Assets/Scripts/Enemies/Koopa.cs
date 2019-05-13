using UnityEngine;
using System;
using System.Collections;


public class Koopa : EnemyController
{

    public const float hitSpeed = 3f;

    float time_left = 8f;


    private bool _shellMoving;
    private bool _inShell;
    private bool _countingDown;
    private bool _hitWall;
    private float _tempSpeed;
    private float _storedSpeed;

    public static Koopa koopa;


    void Start()
    {
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        anim.SetFloat("time_left", time_left);

        SetEnemyInstance(this);


    }

    void FixedUpdate()
    {
        Movement();
    }

    public void SetHitWall(bool hitWall)
    {
        _hitWall = hitWall;
    }

    public bool GetHitWall()
    {
        return _hitWall;
    }

    public void SetCountingDown(bool countingDown)
    {
        _countingDown = countingDown;
    }

    public bool GetCountingDown()
    {
        return _countingDown;
    }

    public void SetTempSpeed(float tempSpeed)
    {
        _tempSpeed = tempSpeed;
    }

    public float GetTempSpeed()
    {
        return _tempSpeed;
    }

    public void SetStoredSpeed(float storedSpeed)
    {
        _storedSpeed = storedSpeed;
    }

    public float GetStoredSpeed()
    {
        return _storedSpeed;
    }


    public void SetInShell(bool inShell)
    {
        _inShell = inShell;
        anim.SetBool("inShell", _inShell);
    }

    public bool GetInShell()
    {
        return _inShell;
    }

    public void SetShellMoving(bool moving)
    {
        _shellMoving = moving;
    }

    public bool GetShellMoving()
    {
        return _shellMoving;
    }

    public void ResetKoopa()
    {
        SetInShell(false);
        SetShellMoving(false);
        SetCountingDown(false);
        bcollider.enabled = true;
        SetSpeed(GetStoredSpeed());
        

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

    public void ResetShellTimer()
    {
        time_left = 8f;
        anim.SetFloat("time_left", time_left);
    }

    public void DecrementShellTimer()
    {
        time_left -= 1;
        anim.SetFloat("time_left", time_left);
    }

    public void StopShell()
    {
        ResetShellTimer();
        SetShellMoving(false);
        SetCanDamage(false);
    }

    public void FlipShell()
    {

        if (gbase.isPrime((int)Math.Floor(Time.time)))
        {
            SetStoredSpeed(-GetStoredSpeed());
            if (GetStoredSpeed() < 0)
                spriteRenderer.flipX = true;
            
            if (GetStoredSpeed() > 0)
                spriteRenderer.flipX = false;
        }
    }

    public void HandleKoopa(object[] args)
    {
        if (!GetInShell())
        {
            SetInShell(true);
            bcollider.enabled = false;
            SetStoredSpeed(GetSpeed());
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
        } else
        {
        
            StopShell();
        }
        StartCoroutine(koopa_countdown());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Koopa koopa = col.gameObject.GetComponentInParent<Koopa>();
        switch (col.gameObject.tag)
        {
            case "koopa_side_collider":
                if (koopa.GetShellMoving())
                {
                    float x = 0.125f;
                    if (koopa.GetInShell() && koopa.getXVel() > 0 && GetInShell())
                    {
                        enemySpeed *= -1;
                        x *= -1;
                    }

                    SetHitByShell(true);
                    
                    rigidBody.AddForce(new Vector2(x, 1f), ForceMode2D.Impulse);
                    StartCoroutine(Death(2f));
                }
                break;
        }
    }

    public void ShellMove(int side)
    {
        if (GetInShell() && !GetShellMoving())
        {
            SetCanDamage(true);
            SetShellMoving(true);
            ResetShellTimer();
            StartCoroutine(ShellMoveCo(side));
        }
    }

    IEnumerator ShellMoveCo(int side)
    {
        SetTempSpeed(-hitSpeed);
        if (side == 3) {
            SetTempSpeed(hitSpeed);
        }
        while (GetShellMoving())
        {

            yield return new WaitUntil(() => Mathf.Abs(rigidBody.velocity.x) < hitSpeed);
            
            if (GetHitWall())
            {
                SetTempSpeed(-GetTempSpeed());
            }

            yield return new WaitForFixedUpdate();

        }
    }

    IEnumerator Death(float t)
    {
        while (true)
        {
            if (GetHitByShell())
            {
                spriteRenderer.flipY = true;
                SetDead(true);

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
        if(!GetCountingDown())
        {
            SetCountingDown(true);
            while (time_left > 0 && !GetShellMoving())
            {
                yield return new WaitForSeconds(1f);
                DecrementShellTimer();
            }

            if (time_left <= 0) {
                ResetKoopa();
            }
        }
    }

    public float getXVel()
    {
        return rigidBody.velocity.x;
    }

    void Movement()
    {

        while (GetShellMoving())
        {    
            rigidBody.velocity = new Vector2(GetTempSpeed(), rigidBody.velocity.y);
            break;
        }

        if (!GetInShell())
        {
            if (time_left < 8)
                ResetShellTimer();

            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
        }
        if (GetSpeed() <= 0 && !GetShellMoving() && GetInShell()) {
            rigidBody.velocity = new Vector2(0f, 0f);
        }
    }

}
