using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public List<GameObject> flowers = new List<GameObject>();
    public List<GameObject> bees = new List<GameObject>();
    public List<IEntity> entities = new List<IEntity>();
    public int nectarCollected = 0;
    public int nectarGoal = 10;

    public float tickRate = 1.0f;
    private float tickTimer = 0.0f;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    public void CollectNectar(int nectarAmount)
    {
        nectarCollected += nectarAmount;

        if (nectarCollected >= nectarGoal)
        {
            WinGame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        entities.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IEntity>());
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRate)
        {
            Tick();
            tickTimer = 0.0f;
        }
    }

    void Tick()
    {
        foreach (IEntity entity in entities)
        {
            entity.Tick();
        }
    }

    void WinGame()
    {
        Debug.Log("You win!");
    }

    void LoseGame()
    {
        Debug.Log("You lose!");
    }
}
