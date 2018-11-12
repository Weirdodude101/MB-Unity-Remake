using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class Block : GameManager
{


    public enum BlockTypes { Coin, Brick }
    public BlockTypes blockType;

    public enum Contains { Coin, Mushroom, Flower, Star, Life, Vine };
    public Contains contains;

    readonly Dictionary<object, int> type2Id = new Dictionary<object, int>
    {
        {BlockTypes.Coin, 0},
        {BlockTypes.Brick, 5}
    };

    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();

    void Start()
    {
        
        /*Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
        foreach (Sprite s in sprites) {
            Console.WriteLine(s.name);
            dictSprites.Add(s.name, s);
        }*/
        Debug.Log(type2Id[blockType]);

       
        //GetComponent<SpriteRenderer>().sprite = dictSprites["item_block_sprites_" + type2Id[blockType]];
        //gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)sprites[type2Id[blockType]-1];
        //anim.enabled = false;

    }
}
