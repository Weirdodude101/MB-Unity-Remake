using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Block : GameBase
{
    SpriteRenderer spriteRenderer;
    Animator anim;
    PlayerController player;

    public enum BlockTypes { Regular, Brick, Used}
    public BlockTypes blockType;

    public enum Contains {Empty, Coin, Powerup, Star, Life, Vine };
    public Contains contains;

    private bool _bouncing;

    readonly Dictionary<BlockTypes, int> type2Id = new Dictionary<BlockTypes, int>
    {
        {BlockTypes.Regular, 0},
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
                Activate(byEnemy: true);
        }
    }

    void GenerateItem(string name, Vector2 scale, bool byEnemy = false)
    {

        if (CanActivate() || byEnemy)
        {
            GameObject item = GameObject.Find(name);
            item.transform.localScale = scale;
            Instantiate(item, transform);
            Transform child = transform.GetChild(0);
            child.GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    private bool CanActivate()
    {
        return !(blockType == BlockTypes.Brick && _gameManager.GetPlayerState() == GameManager.PlayerStates.Small);
    }

    void Activate(bool byEnemy=false)
    {
        if (blockType != BlockTypes.Used && !IsBouncing() && (CanActivate() || byEnemy))
        {
            switch(contains)
            {
                case Contains.Coin:
                    GenerateItem("Coin", new Vector2(0.75f, 0.75f), byEnemy: byEnemy);
                    StartCoroutine(Bounce(transform.GetChild(0), moveBy: 0.5f, overTime: 0.3125f));
                    _gameManager.IncrementCoins();
                    break;
            }
            StartCoroutine(Bounce(transform, byEnemy: byEnemy));
            SetType(BlockTypes.Used);
        }
    }


    void SetType(BlockTypes type) {
        blockType = type;

        spriteRenderer.sprite = gbase.dictSprites["item_block_sprites_" + type2Id[blockType]];

        if (blockType != BlockTypes.Regular)
            anim.enabled = false;
    }

    public bool IsBouncing()
    {
        return _bouncing;
    }

    IEnumerator Bounce(Transform transf, float moveBy = 0.09375f, float overTime = 0.125f, bool byEnemy = false)
    {
        if (transf.tag == "Block")
            _bouncing = true;

        float startTime = Time.time;

        Vector2 point = new Vector2(transf.position.x, transf.position.y + moveBy);

        while (Time.time < startTime + overTime)
        {
            transf.position = Vector2.Lerp(transf.position, point, (Time.time - startTime) / overTime);
            yield return null;
        }

        if (transf.tag == "Block")
            yield return new WaitForSeconds(0.0016f);
        else
            yield return new WaitForFixedUpdate();

        startTime = Time.time;

        point.y -= moveBy;
        while (Time.time < startTime + overTime)
        {
            transf.position = Vector2.Lerp(transf.position, point, (Time.time - startTime) / overTime);
            yield return null;
        }

        if (transf.tag == "Block")
            _bouncing = false;
    }
}
