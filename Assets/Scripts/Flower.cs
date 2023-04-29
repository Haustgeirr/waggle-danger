using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour, IGatherable
{
    private GameManager gameManager;

    public int nectarAmount = 10;
    public bool hasNectar = true;

    [Header("Sprite Settings")]
    public Sprite[] flowerSprites;
    public Sprite[] wiltedSprites;
    public SpriteRenderer spriteRenderer;

    private int flowerVariant = 0;

    public int Gather(int gatherAmount)
    {
        if (nectarAmount > gatherAmount)
        {
            nectarAmount -= gatherAmount;
        }
        else
        {
            gatherAmount = nectarAmount;
            nectarAmount = 0;
            hasNectar = false;
            Wilt();
        }

        return gatherAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.flowers.Add(this.gameObject);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // set flower variant
        flowerVariant = Random.Range(0, flowerSprites.Length);
        spriteRenderer.sprite = flowerSprites[flowerVariant];
    }

    // Update is called once per frame
    void Update() { }

    // when the flower is empty, remove it from the game
    void Wilt()
    {
        Debug.Log("Flower is wilting");
        gameManager.flowers.Remove(this.gameObject);
        spriteRenderer.sprite = wiltedSprites[flowerVariant];
    }
}
