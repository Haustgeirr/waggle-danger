using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;
    public GameObject[] flowers;
    public BeeEntity[] bees;
    public int nectarCollected;
    public int nectarGoal;

    private float tickRate = 1.0f;
    private float tickTimer = 0.0f;

    public void CollectNectar()
    {
        nectarCollected++;

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
        else
        {
            instance = this;
        }

        // flowers = GameObject.FindGameObjectsWithTag("Flower");
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
        Debug.Log("Game is ticking");

        foreach (BeeEntity bee in bees)
        {
            bee.entity.Tick();
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
