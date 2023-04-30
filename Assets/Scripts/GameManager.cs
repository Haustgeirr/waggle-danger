using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [Header("Game Settings")]
    public int nectarCollected = 0;
    public int nectarGoal = 10;
    public float tickRate = 1.0f;
    public float tickTimer = 0.0f;
    public GameState gameState = GameState.Menu;

    [Header("Game Objects")]
    public List<GameObject> flowers = new List<GameObject>();
    public List<GameObject> bees = new List<GameObject>();
    public List<IEntity> entities = new List<IEntity>();
    public Waggler waggler;
    public GameObject player;
    public GameObject crow;

    [Header("UI Objects")]
    public GameObject menuUI;
    public GameObject gameOverUI;
    public GameObject waggleUI;

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
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();
        player = GameObject.Find("Player");
        crow = GameObject.Find("Crow");

        menuUI = GameObject.Find("MenuUI");
        gameOverUI = GameObject.Find("GameOverUI");
        waggleUI = GameObject.Find("WaggleUI");
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Menu)
        {
            gameState = GameState.Playing;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Playing)
        {
            gameState = GameState.Menu;
        }

        if (gameState == GameState.Playing)
        {
            TickGame();
        }
    }

    void HandleGameState()
    {
        Debug.Log("handle menu");

        if (gameState == GameState.Playing)
        {
            menuUI.SetActive(false);
            gameOverUI.SetActive(false);
            SetPlayActive(true);
        }
        else if (gameState == GameState.GameOver)
        {
            menuUI.SetActive(false);
            gameOverUI.SetActive(true);
            SetPlayActive(false);
        }
        else
        {
            menuUI.SetActive(true);
            gameOverUI.SetActive(false);
            SetPlayActive(false);
        }
    }

    void SetPlayActive(bool active)
    {
        player.SetActive(active);
        waggler.gameObject.SetActive(active);
        waggleUI.SetActive(active);
        crow.SetActive(active);
    }

    void TickGame()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRate)
        {
            foreach (IEntity entity in entities)
            {
                entity.Tick();
            }

            tickTimer = 0.0f;
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
