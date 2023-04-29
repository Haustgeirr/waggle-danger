using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour, IEntity
{
    public GameObject targetFlowerGameObject;
    public float targetFlowerDistance;
    public int nectarCapacity = 10;
    public int nectarAmount = 0;
    public bool hasNectar = false;
    public bool isFull = false;

    private IGatherable targetFlowerGatherable;
    private GameManager gameManager;
    private float gatherDistance = 0.5f;
    private int gatherAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.bees.Add(this.gameObject);
        gameManager.entities.Add(this.GetComponent<IEntity>());
    }

    // Update is called once per frame
    void Update() { }

    public void Tick()
    {
        Debug.Log("Bee is ticking");

        CheckNectarAmount();

        if (isFull)
        {
            Debug.Log("Bee is full");
            return;
        }

        FindNearestFlower();

        if (targetFlowerGameObject == null)
        {
            return;
        }

        AttemptGather();
    }

    void CheckNectarAmount()
    {
        hasNectar = nectarAmount > 0;
        isFull = nectarAmount >= nectarCapacity;
    }

    void AttemptGather()
    {
        if (targetFlowerDistance <= gatherDistance)
        {
            var toGather = Mathf.Min(gatherAmount, nectarCapacity - nectarAmount);
            var amountGathered = targetFlowerGatherable.Gather(toGather);

            nectarAmount += amountGathered;

            if (nectarAmount >= nectarCapacity)
            {
                isFull = true;
            }
        }
    }

    void FindNearestFlower()
    {
        Debug.Log("Finding nearest flower");

        float closestDistance = Mathf.Infinity;
        GameObject potentialFlower = null;

        foreach (GameObject flower in gameManager.flowers)
        {
            float distance = Vector3.Distance(transform.position, flower.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                potentialFlower = flower;
            }
        }

        if (potentialFlower == null)
        {
            Debug.Log("No flowers found");
            targetFlowerGameObject = null;
            targetFlowerDistance = Mathf.Infinity;
            targetFlowerGatherable = null;

            return;
        }

        targetFlowerGameObject = potentialFlower;
        targetFlowerDistance = closestDistance;
        targetFlowerGatherable = targetFlowerGameObject.GetComponent<IGatherable>();

        Debug.Log("Closest flower is " + targetFlowerGameObject.name);
    }
}
