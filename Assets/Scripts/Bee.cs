using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeeState
{
    Buzzing,
    Gathering,
    Storing
}

public class Bee : MonoBehaviour, IEntity
{
    public GameObject hiveGameObject;
    public float targetFlowerDistance = Mathf.Infinity;
    public GameObject targetFlowerGameObject;
    public float hiveDistance = Mathf.Infinity;
    public BeeState beeState = BeeState.Buzzing;

    public int nectarCapacity = 10;
    public int nectarAmount = 0;
    public bool hasNectar = false;
    public bool isFull = false;

    private IGatherable targetFlowerGatherable;
    private IStorable hiveStorable;
    private GameManager gameManager;
    private float interactionRange = 0.5f;
    private int gatherAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.bees.Add(this.gameObject);
        // gameManager.entities.Add(this.GetComponent<IEntity>());

        hiveGameObject = GameObject.Find("Hive");
        hiveStorable = hiveGameObject.GetComponent<IStorable>();
    }

    // Update is called once per frame
    void Update() { }

    public void Tick()
    {
        FindNearestFlower();
        CheckBeeState();

        switch (beeState)
        {
            case BeeState.Buzzing:
                break;
            case BeeState.Gathering:
                Gather();
                break;
            case BeeState.Storing:
                Store();
                break;
        }

        CheckNectarAmount();
    }

    void Store()
    {
        hiveStorable.Store(gatherAmount);
        nectarAmount -= gatherAmount;
    }

    // handle all enter/exit state logic here
    void CheckBeeState()
    {
        hiveDistance = Vector3.Distance(transform.position, hiveGameObject.transform.position);

        if (hiveDistance <= interactionRange && hasNectar)
        {
            beeState = BeeState.Storing;
            return;
        }

        if (targetFlowerGameObject && targetFlowerDistance <= interactionRange && !isFull)
        {
            beeState = BeeState.Gathering;
            return;
        }

        beeState = BeeState.Buzzing;
    }

    void CheckNectarAmount()
    {
        hasNectar = nectarAmount > 0;
        isFull = nectarAmount >= nectarCapacity;
    }

    void Gather()
    {
        var toGather = Mathf.Min(gatherAmount, nectarCapacity - nectarAmount);
        var amountGathered = targetFlowerGatherable.Gather(toGather);

        nectarAmount += amountGathered;
    }

    void FindNearestFlower()
    {
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
            targetFlowerGameObject = null;
            targetFlowerDistance = Mathf.Infinity;
            targetFlowerGatherable = null;

            return;
        }

        targetFlowerGameObject = potentialFlower;
        targetFlowerDistance = closestDistance;
        targetFlowerGatherable = targetFlowerGameObject.GetComponent<IGatherable>();
    }
}
