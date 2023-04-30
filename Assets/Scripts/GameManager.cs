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

public enum VictoryState
{
    None,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [Header("Game Rules")]
    public int score = 0;
    public int nectarCollected = 0;
    public int nectarGoal = 10;
    public int availableNectar = 0;
    public float tickRate = 1.0f;
    public float tickTimer = 0.0f;
    public GameState gameState = GameState.Menu;
    public VictoryState victoryState = VictoryState.None;

    [Header("Gameplay Settings")]
    public int minFlowerCount = 3;
    public int maxFlowerCount = 10;
    public int flowerPlacementRange = 20;
    public int flowerDeadZone = 5;
    public float flowerRadius = 1.0f;

    public int difficulty = 0;
    public int difficultyNectarRequirement = 6;

    [Header("Game Objects")]
    public GameObject flowerPrefab;
    public List<GameObject> flowers = new List<GameObject>();
    public List<GameObject> bees = new List<GameObject>();
    public List<IEntity> entities = new List<IEntity>();
    public Waggler waggler;
    public GameObject player;
    public Bee playerBee;
    public GameObject crowObject;
    public Crow crow;

    [Header("UI Objects")]
    public GameObject menuUI;
    public GameObject waggleUI;
    public GameObject winUI;
    public GameObject loseUI;

    [Header("Audio")]
    public AudioSource musicSource;
    public bool isMusicPlaying = false;

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

        score += nectarAmount * waggler.comboMultiplier;

        var newDifficulty = nectarCollected / difficultyNectarRequirement;
        if (newDifficulty > difficulty)
        {
            difficulty = newDifficulty;
            crow.SetDifficulty(difficulty);
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
        playerBee = player.GetComponent<Bee>();
        crowObject = GameObject.Find("Crow");
        crow = crowObject.GetComponent<Crow>();

        menuUI = GameObject.Find("MenuUI");
        waggleUI = GameObject.Find("WaggleUI");
        winUI = GameObject.Find("WinUI");
        loseUI = GameObject.Find("LoseUI");

        musicSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.Space) && gameState != GameState.Playing)
        {
            SpaceAction();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Menu)
        {
            gameState = GameState.Playing;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.Playing)
        {
            gameState = GameState.Menu;
            return;
        }

        TickGame();
    }

    void SpaceAction()
    {
        if (gameState == GameState.Menu)
        {
            StartGame();
        }

        if (gameState == GameState.GameOver)
        {
            StartGame();
        }
    }

    void PlaceFlowers()
    {
        //destroy all flowers
        var flowersToDestroy = FindObjectsOfType<Flower>();
        foreach (var flower in flowersToDestroy)
        {
            Destroy(flower.gameObject);
        }

        // poisson disc placement
        var flowerCount = Random.Range(minFlowerCount, maxFlowerCount);
        var flowerRadiusSquared = flowerRadius * flowerRadius;
        var deadZoneSquared = flowerDeadZone * flowerDeadZone;
        var flowerPoints = new List<Vector3>();

        var loopLimit = 1000;
        while (flowerPoints.Count < flowerCount)
        {
            // choose a target within a circle of radius flowerPlacementRange
            var angle = Random.Range(0f, Mathf.PI * 2f);
            var distance = Random.Range(0f, flowerPlacementRange);
            var x = Mathf.Floor(Mathf.Cos(angle) * distance);
            var y = Mathf.Floor(Mathf.Sin(angle) * distance);

            var point = new Vector3(x, y, 0f);
            var tooClose = false;

            foreach (var flowerPoint in flowerPoints)
            {
                // discard iff too close to other flower or center
                var distanceToFlower = (point - flowerPoint).sqrMagnitude;
                var distanceToCentre = point.sqrMagnitude;

                if (distanceToFlower < flowerRadiusSquared || distanceToCentre < deadZoneSquared)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                flowerPoints.Add(point);
            }

            loopLimit--;

            if (loopLimit <= 0)
            {
                Debug.Log("Loop limit reached");
                break;
            }
        }

        foreach (var point in flowerPoints)
        {
            var flower = Instantiate(flowerPrefab);
            flower.transform.position = point;
            flowers.Add(flower);
        }
    }

    int GenerateDistantRange(int min, int max, int separation)
    {
        var halfRange = (max - min) / 2;
        var minRange = Random.Range(min, halfRange - separation);
        var maxRange = Random.Range(halfRange + separation, max);

        var roll = Random.Range(0, 2);
        return roll == 0 ? minRange : maxRange;
    }

    void StartGame()
    {
        nectarCollected = 0;
        tickTimer = 0.0f;
        gameState = GameState.Menu;
        victoryState = VictoryState.None;
        difficulty = 0;

        flowers.Clear();
        bees.Clear();
        // entities.Clear();

        PlaceFlowers();
        CalculateAvailableNectar();

        // entities.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IEntity>());

        playerBee.Init();
        waggler.Init();
        crow.Init();

        gameState = GameState.Playing;
    }

    void CalculateAvailableNectar()
    {
        availableNectar = 0;

        foreach (var flower in flowers)
        {
            availableNectar += flower.GetComponent<Flower>().nectarAmount;
        }
    }

    void CheckGameCondition()
    {
        if (nectarCollected >= nectarGoal)
        {
            WinGame();
        }

        if (bees.Count == 0)
        {
            LoseGame();
        }

        if (flowers.Count == 0)
        {
            var nectarDiff = nectarGoal - nectarCollected;
            var hasEnoughNectar = playerBee.nectarAmount >= nectarDiff;

            if (!hasEnoughNectar)
            {
                LoseGame();
            }
        }
    }

    void HandleGameState()
    {
        if (gameState == GameState.Playing)
        {
            menuUI.SetActive(false);
            SetPlayActive(true);
            HandleVictoryState();
        }
        else if (gameState == GameState.GameOver)
        {
            menuUI.SetActive(false);
            SetPlayActive(false);
            HandleVictoryState();
        }
        else
        {
            if (!isMusicPlaying)
            {
                musicSource.Play();
                isMusicPlaying = true;
            }

            menuUI.SetActive(true);
            SetPlayActive(false);
            HandleVictoryState();
        }
    }

    void HandleVictoryState()
    {
        if (victoryState == VictoryState.Win)
        {
            winUI.SetActive(true);
            loseUI.SetActive(false);
        }
        else if (victoryState == VictoryState.Lose)
        {
            winUI.SetActive(false);
            loseUI.SetActive(true);
        }
        else
        {
            winUI.SetActive(false);
            loseUI.SetActive(false);
        }
    }

    void SetPlayActive(bool active)
    {
        player.SetActive(active);
        waggler.gameObject.SetActive(active);
        waggleUI.SetActive(active);
        crowObject.SetActive(active);
    }

    void TickGame()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickRate)
        {
            if (gameState == GameState.Playing)
            {
                foreach (IEntity entity in entities)
                {
                    entity.Tick();
                }

                CheckGameCondition();
            }

            tickTimer = 0.0f;
        }
    }

    void WinGame()
    {
        Debug.Log("You win!");
        gameState = GameState.GameOver;
        victoryState = VictoryState.Win;
    }

    void LoseGame()
    {
        Debug.Log("You lose!");
        gameState = GameState.GameOver;
        victoryState = VictoryState.Lose;
    }
}
