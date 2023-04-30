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
    public float targetFlowerDistance;
    public GameObject targetFlowerGameObject;
    public float hiveDistance;
    public BeeState beeState = BeeState.Buzzing;
    public BeeState previousBeeState = BeeState.Buzzing;
    public SpriteRenderer[] spriteRenderers;
    public AudioSource audioSource;

    public int nectarCapacity = 10;
    public int nectarAmount = 0;
    public bool hasNectar = false;
    public bool isFull = false;

    private IGatherable targetFlowerGatherable;
    private IStorable hiveStorable;
    private GameManager gameManager;
    private BeeAnimator beeAnimator;
    public float interactionRange = 0.5f;
    private int gatherAmount = 1;

    private bool enteredHiveThisTick = false;

    private CameraShake cameraShake;

    [Header("Audio")]
    public AudioClip buzz;
    public AudioClip gather;
    public AudioClip enterHive;
    public AudioClip loseNectar;
    public AudioClip missInput;
    public AudioClip nectarCollect;

    public void PlaySound(AudioClip clip, float pitch = 1f)
    {
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Init()
    {
        gameManager = GameManager.Instance;
        gameManager.bees.Add(this.gameObject);

        hiveGameObject = GameObject.Find("Hive");
        hiveStorable = hiveGameObject.GetComponent<IStorable>();

        targetFlowerDistance = Mathf.Infinity;
        hiveDistance = Mathf.Infinity;

        beeState = BeeState.Storing;
        nectarAmount = 0;
        hasNectar = false;
        isFull = false;
        SetSpriteVisible(false);

        transform.position = hiveGameObject.transform.position;
    }

    public void GetHit()
    {
        if (nectarAmount > 0)
        {
            beeAnimator.LoseNectar();
            KnockOutNectar();
        }
        else
        {
            Knockout();
        }
    }

    public void Tick()
    {
        FindNearestFlower();
        var newState = CheckBeeState();
        TransitionState(newState);

        switch (beeState)
        {
            case BeeState.Buzzing:
                break;
            case BeeState.Gathering:
                Gather();
                break;
            case BeeState.Storing:
                if (enteredHiveThisTick)
                {
                    enteredHiveThisTick = false;
                    break;
                }
                Store();
                break;
        }

        CheckNectarAmount();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.bees.Add(this.gameObject);
        // gameManager.entities.Add(this.GetComponent<IEntity>());

        hiveGameObject = GameObject.Find("Hive");
        hiveStorable = hiveGameObject.GetComponent<IStorable>();

        targetFlowerDistance = Mathf.Infinity;
        hiveDistance = Mathf.Infinity;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        beeAnimator = GetComponentInChildren<BeeAnimator>();
    }

    // Update is called once per frame
    void Update() { }

    void Store()
    {
        PlaySound(nectarCollect);
        beeAnimator.GetNectar();
        var storeAmount = gatherAmount * 2;
        hiveStorable.Store(storeAmount);
        nectarAmount -= storeAmount;
    }

    // handle all enter/exit state logic here
    BeeState CheckBeeState()
    {
        hiveDistance = Vector3.Distance(transform.position, hiveGameObject.transform.position);

        if (hiveDistance <= interactionRange && hasNectar)
        {
            return BeeState.Storing;
        }

        if (targetFlowerGameObject && targetFlowerDistance <= interactionRange && !isFull)
        {
            return BeeState.Gathering;
        }

        return BeeState.Buzzing;
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

        var pitch = Mathf.Lerp(0.8f, 1.2f, nectarAmount / (float)nectarCapacity);
        PlaySound(gather, pitch);
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

    void KnockOutNectar()
    {
        PlaySound(loseNectar);
        nectarAmount = 0;
    }

    void Knockout()
    {
        gameManager.bees.Remove(this.gameObject);
        this.gameObject.SetActive(false);
    }

    void SetSpriteVisible(bool visible)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = visible;
        }
    }

    void SetInHive(bool inHive)
    {
        SetSpriteVisible(!inHive);
        PlaySound(enterHive);

        if (inHive)
        {
            enteredHiveThisTick = true;
        }
    }

    void OnEnterState(BeeState state)
    {
        switch (state)
        {
            case BeeState.Buzzing:
                break;
            case BeeState.Gathering:
                break;
            case BeeState.Storing:
                SetInHive(true);
                break;
        }
    }

    void OnExitState(BeeState state)
    {
        switch (state)
        {
            case BeeState.Buzzing:
                break;
            case BeeState.Gathering:
                break;
            case BeeState.Storing:
                SetInHive(false);
                break;
        }
    }

    void OnStateChange(BeeState state)
    {
        OnExitState(previousBeeState);
        OnEnterState(state);
    }

    void TransitionState(BeeState state)
    {
        if (state == beeState)
        {
            return;
        }

        previousBeeState = beeState;
        beeState = state;
        OnStateChange(state);
    }
}
