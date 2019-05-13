using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Block : GameBase
{
    SpriteRenderer spriteRenderer;
    Animator anim;
    PlayerController player;

    public enum BlockTypes { Coin, Brick, Used}
    public BlockTypes blockType;

    public enum Contains {Empty, Coin, Powerup, Star, Life, Vine };
    public Contains contains;

    readonly Dictionary<BlockTypes, int> type2Id = new Dictionary<BlockTypes, int>
    {
        {BlockTypes.Coin, 0},
        {BlockTypes.Brick, 5},
        {BlockTypes.Used, 6}
    };
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        gbase.LoadSprites("Sprites/Tiles/item_block_sprites");
        
        SetType(blockType);

    }



    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag != "Koopa" || col.gameObject.tag != "Goomba")
        {
            if (GetSide(col.gameObject.transform, gameObject.transform, true) == 1 && !player.GetGrounded())
            {
                Activate();
            }
        }

        if (col.gameObject.tag == "Koopa")
        {
            if (col.gameObject.GetComponent<Koopa>().GetShellMoving())
                Activate();
        }
    }

    void Activate()
    {
        if (blockType != BlockTypes.Used)
        {
            if (blockType == BlockTypes.Brick && _gameManager.GetPlayerState() == GameManager.PlayerStates.Small)
            {
                StartCoroutine(Bounce());
                return;
            }
            switch(contains)
            {
                case Contains.Coin:
                    _gameManager.IncrementCoins();
                    break;
            }
            
            StartCoroutine(Bounce());
            SetType(BlockTypes.Used);
        }
    }


    void SetType(BlockTypes type) {
        blockType = type;

        spriteRenderer.sprite = gbase.dictSprites["item_block_sprites_" + type2Id[blockType]];

        if (blockType != BlockTypes.Coin)
            anim.enabled = false;
    }

    IEnumerator Bounce()
    {
        float startTime = Time.time;
        float moveBy = 0.09375f;
        float overTime = 0.125f;

        Vector2 point = new Vector2(transform.position.x, transform.position.y + moveBy);

        while (Time.time < startTime + overTime)
        {
            transform.position = Vector2.Lerp(transform.position, point, (Time.time - startTime) / overTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.0016f);

        startTime = Time.time;

        point.y -= moveBy;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector2.Lerp(transform.position, point, (Time.time - startTime) / overTime);
            yield return null;
        }

    }
}
