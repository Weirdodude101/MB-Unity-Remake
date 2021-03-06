﻿using UnityEngine;
using System.Collections;
public class PlayerController : GameBase
{
    Rigidbody2D rigidBody;

    Animator anim;
    AudioSource music;

    public AudioClip deathSound;

    private AudioSource _jumpSound;
    private bool _dead;
    private bool _grounded;
    private bool _facingRight = true;
    private bool _sprinting;

    [SerializeField]
    private float _speed = 1.5f;
    private float _speedStore;

    private float _speedDif = 2f;
    private float _jumpHeight = 5f;

    void Start()
    {

        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        music = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        _jumpSound = GetComponent<AudioSource>();
        _speedStore =  _speed;
        transform.tag = "Player";
    }

    public void SetJumpHeight(float jumpHeight)
    {
        _jumpHeight = jumpHeight;
    }

    public float GetJumpHeight()
    {
        return _jumpHeight;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public float GetSpeed()
    {
        if (!IsDead() && GetGrounded())
        {
            float dTime = -Time.deltaTime;
            float sprintSpeed = _speedStore + _speedDif;
            
            if (GetSprinting())
                dTime *= -1;
            
            if ((!GetSprinting() && _speed >= sprintSpeed) || (GetSprinting() && _speed <= sprintSpeed))
            {
                SetSpeed(_speed+dTime);
            }

        }

        return _speed;
    }

    public void SetSprinting(bool sprinting)
    {
        if (!sprinting)
           SetSpeed(_speedStore);

        _sprinting = sprinting;
    }

    public bool GetSprinting()
    {
        return _sprinting;
    }

    public Vector2 GetVelocity()
    {
        return rigidBody.velocity;
    }

    public void SetFacingRight(bool facingRight)
    {
        _facingRight = facingRight;
    }

    public bool GetFacingRight()
    {
        return _facingRight;
    }

    public void SetGrounded(bool grounded)
    {
        _grounded = grounded;
    }

    public bool GetGrounded()
    {
        return _grounded;
    }

    public void SetDead(bool dead)
    {
        _dead = true;
    }

    public bool IsDead()
    {
        return _dead;
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 10)
        {
            if (gbase.GetSide(gameObject.transform, col.gameObject.transform, true) == 0)
                SetGrounded(true);
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        EnemyController enemy = col.gameObject.GetComponentInParent<EnemyController>();
        switch (col.gameObject.tag)
        {

            case "enemy_body_collider":
                if (!enemy.IsDead())
                    HandlePlayerDeath(enemy);
                break;

            case "koopa_side_collider":
                Koopa koopa = col.gameObject.GetComponentInParent<Koopa>();
                if (koopa.GetCanDamage())
                    goto case "enemy_body_collider";


                koopa.ShellMove(gbase.GetSide(gameObject.transform, col.gameObject.transform, false));
                break;

            default:
                Jump(3f);
                enemy.SendMethod(string.Format("Handle{0}", enemy.enemyType.ToString()), enemy);
                break;
        }
    }

    public void HandlePlayerDeath(EnemyController enemy = null)
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        SetDead(true);
        anim.SetBool("death", IsDead());

        gbase.SetMusic(music, deathSound, false, true);

        SetSpeed(0);

        while (true)
        {
            if (GetGrounded())
                rigidBody.AddForce(new Vector2(0, GetJumpHeight() - 0.75f), ForceMode2D.Impulse);
                
            Destroy(GetComponent<BoxCollider2D>());
            yield return new WaitForSeconds(3f);

            _gameManager.DecrementLives();
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Movement(horizontal);
        Flip(horizontal);
    }

    void Movement(float horizontal)
    { 
        rigidBody.velocity = new Vector2(horizontal * GetSpeed(), rigidBody.velocity.y);

        if (!IsDead())
        {
            SetSprinting(Input.GetKey(KeyCode.LeftShift));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GetGrounded())
                {
                    Jump(GetJumpHeight());
                }
            }
        }

        anim.SetFloat("speed", Mathf.Abs(horizontal));
        anim.SetBool("onGround", GetGrounded());
        anim.SetBool("spacePressed", !GetGrounded());

    }

    void Jump(float height) {
        rigidBody.velocity = new Vector2(0, height);
        SetGrounded(false);
        _jumpSound.Play();
    }

    void Flip(float horizontal)
    {
        if (horizontal > 0 && !GetFacingRight() || horizontal < 0 && GetFacingRight())
        {
            SetFacingRight(!GetFacingRight());
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }
}
