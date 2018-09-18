using UnityEngine;
using System.Collections;

public class Koopa : EnemyController
{

    public const float hitSpeed = 2f;

    public bool shellMoving;
    public bool canKill;

    float x;
    bool collided;
    bool hitByKoopa;

    SpriteRenderer sprite;

    public static Koopa koopa;




	void Start()
	{
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bcollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

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
                BoxCollider2D collider2D = child.GetComponent<BoxCollider2D>();
                switch(child.tag) {
                    
                    case "koopa_side_collider":
                        collider2D.isTrigger = true;
                        break;

                    case "enemy_body_collider":
                        collider2D.isTrigger = false;
                        collider2D.size = new Vector2(0, 0);
                        break;

                }
            }
        }
     }

	public void shellMovement(object[] args) {
        x = (float)args[0];
        shellMoving = true;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name == "Collider") {
            collided = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        switch (col.gameObject.tag) {
            case "koopa_side_collider":
                if (col.gameObject.GetComponentInParent<Koopa>().shellMoving) {
                    
                    hitByKoopa = true;
                    rigidBody.AddForce(new Vector2(0.75f, 1f), ForceMode2D.Impulse);
                    StartCoroutine(Death(2f));
                }
                break;
        }
    }

    IEnumerator Death(float t)
    {
        while (true)
        {
            if (hitByKoopa)
            {
                sprite.flipY = true;
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


    IEnumerator shellMove()
    {
        while (true)
        {
            
            float v = hitSpeed;

            yield return new WaitUntil(() => Mathf.Abs(rigidBody.velocity.x) < hitSpeed);

            if (x > 0)
                v = -hitSpeed;
            
            if (collided)
                v *= -1;
            
            rigidBody.velocity = new Vector2(v, rigidBody.velocity.y);

            break;
        }
    }

    void Movement()
    {
        bool inShell = anim.GetBool("inShell");
        if (!inShell) {
            rigidBody.velocity = new Vector2(enemySpeed * -1, rigidBody.velocity.y);
        }
        if (inShell && shellMoving) {
            canKill = true;
            StartCoroutine(shellMove());
        }
    }

}
