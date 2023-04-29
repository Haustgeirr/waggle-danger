using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour, IGatherable
{
    private GameManager gameManager;

    private int nectarAmount = 10;

    public int Gather(int gatherAmount)
    {
        Debug.Log("Gathering flower");

        if (nectarAmount > gatherAmount)
        {
            nectarAmount -= gatherAmount;
        }
        else
        {
            gatherAmount = nectarAmount;
            nectarAmount = 0;
            Wilt();
        }

        return gatherAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.flowers.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update() { }

    // when the flower is empty, remove it from the game
    void Wilt()
    {
        Debug.Log("Flower is wilting");
        gameManager.flowers.Remove(this.gameObject);
    }
}

public struct FlowerStruct
{
    public GameObject gameObject { get; set; }
    public Flower flower { get; set; }
    public float distance { get; set; }
    public IGatherable gatherable { get; set; }
}
