using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;

public class Koopa : EnemyController
{

    public const float hitSpeed = 2f;

    public bool shellMoving = false;
    public static Koopa koopa;


	void Start()
	{
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();
        setType(ETypes.Koopa);

	}

    void FixedUpdate()
    {
        Movement();
    }

    public void handleKoopa(object[] args)
    {
        enemy = (EnemyController)args[0];
        if (!enemy.anim.GetBool("inShell"))
        {
            enemy.anim.SetBool("inShell", true);
            enemy.bcollider.size = new Vector2(0.16f, 0.14f);
            enemy.setSpeed(0f);

            GameObject bodyCollider = GameObject.FindGameObjectWithTag("enemy_body_collider");
            BoxCollider2D bodyCollider2D = bodyCollider.GetComponent<BoxCollider2D>();
            bodyCollider2D.isTrigger = false;
            bodyCollider2D.size = new Vector2(0, 0);

            foreach (Transform child in transform) {
                switch(child.tag) {
                    case "koopa_side_collider":
                        BoxCollider2D sideCollider2D = child.GetComponent<BoxCollider2D>();
                        sideCollider2D.isTrigger = true;
                        break;
                    case "enemy_body_collider":
                        BoxCollider2D eCollider = child.GetComponent<BoxCollider2D>();
                        eCollider.isTrigger = false;
                        eCollider.size = new Vector2(0, 0);
                        break;

                }
            }
        }
     }


    public void shellMovement(object[] args) {
        float x = (float)args[1];
        float v = hitSpeed;
        if (x > 0) {
            v = hitSpeed * -1;
        }

        rigidBody.velocity = new Vector2(v, rigidBody.velocity.y);
        shellMoving = true;
    }

    void Movement()
    {
        if (!anim.GetBool("inShell")) {
            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
        }
    }

}
