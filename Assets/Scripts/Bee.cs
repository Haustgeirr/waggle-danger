using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour, IEntity
{
    public FlowerStruct targetFlower;

    private GameManager gameManager;

    private float gatherDistance = 0.5f;
    private int gatherAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.bees.Add(new BeeEntity { gameObject = this.gameObject, entity = this });
    }

    // Update is called once per frame
    void Update() { }

    void Tick()
    {
        Debug.Log("Bee is ticking");

        FindNearestFlower();

        if (targetFlower.distance <= gatherDistance)
        {
            var amountGathered = targetFlower.gatherable.Gather(gatherAmount);
        }
    }

    void FindNearestFlower()
    {
        Debug.Log("Finding nearest flower");

        float closestDistance = Mathf.Infinity;
        var potentialFlower = null;

        foreach (GameObject flower in gameManager.flowers)
        {
            float distance = Vector3.Distance(transform.position, flower.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                potentialFlower = flower;
            }
        }

        targetFlower = new FlowerStruct
        {
            gameObject = potentialFlower,
            flower = potentialFlower.GetComponent<Flower>(),
            distance = distance,
            gatherable = potentialFlower.GetComponent<IGatherable>()
        };

        Debug.Log("Closest flower is " + targetFlower.gameObject.name);
    }
}

public struct BeeEntity
{
    public GameObject gameObject { get; set; }
    public IEntity entity { get; set; }
}
