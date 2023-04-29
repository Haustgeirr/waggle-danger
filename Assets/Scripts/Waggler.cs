using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Waggler : MonoBehaviour, IEntity
{
    public static int[] comboMultipliers = new int[] { 1, 2, 4, 8, 16 };

    private GameManager gameManager;

    public Vector2 direction;
    public Vector2 lastDirection;
    public List<Vector2> waggleDirections = new List<Vector2>();
    public Vector2 waggleDirection;
    private Vector2 inputDirection;

    public int comboCount = 0;
    public int comboMultiplier = comboMultipliers[0];
    private int maxComboCount = 4;

    [Header("Input Settings")]
    public InputAction directionInput;
    public InputAction waggleInput;

    public bool inputBlocked = false;
    public float inputTimer = 0.0f;
    public float perfectDuration = 0.2f;
    public float goodDuration = 0.4f;
    public bool isPerfect = false;
    public bool isGood = false;
    public bool isMiss = false;
    public bool hasReceivedInput = false;

    public List<Vector2> directionsOptions = new List<Vector2>();
    private Vector2[] directionsBase = new Vector2[]
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
    };

    public GameObject Player;

    public void Tick()
    {
        ResetInput();
        // GetNextDirection();
    }

    void OnEnable()
    {
        directionInput.Enable();
        waggleInput.Enable();
    }

    void OnDisable()
    {
        directionInput.Disable();
        waggleInput.Disable();
    }

    void Awake()
    {
        // assign a callback for the "waggleInput" action.
        waggleInput.performed += ctx =>
        {
            // IsInputBlocked();
            // OnWaggle(ctx);
        };

        // if we have already received an input this frame, penalise the player
        directionInput.performed += ctx =>
        {
            // var hasMultiple = IsInputBlocked();

            if (!IsInputBlocked())
            {
                direction = ctx.ReadValue<Vector2>();
                Player.transform.position += new Vector3(direction.x, direction.y, 0);

                ScoreInput();
            }
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        Player = GameObject.Find("Player");

        for (int i = 0; i < 10; i++)
        {
            waggleDirections.Add(SelectDirection());
        }
    }

    void Update()
    {
        inputTimer += Time.deltaTime;
    }

    void GetNextDirection()
    {
        waggleDirections.RemoveAt(0);
        waggleDirection = waggleDirections[0];
        waggleDirections.Add(SelectDirection());
    }

    Vector2 SelectDirection()
    {
        if (directionsOptions.Count == 0)
        {
            directionsOptions.AddRange(directionsBase);
        }

        int index = Random.Range(0, directionsOptions.Count);
        var dir = directionsOptions[index];
        directionsOptions.RemoveAt(index);

        return dir;
    }

    bool IsInputBlocked()
    {
        if (inputBlocked)
        {
            return true;
        }

        if (hasReceivedInput)
        {
            Debug.Log("TOo many Inputs!");
            isMiss = true;
            return true;
        }

        hasReceivedInput = true;
        return false;
    }

    void ScoreInput()
    {
        if (inputTimer <= perfectDuration)
        {
            isPerfect = true;
        }
        else if (inputTimer <= goodDuration)
        {
            isGood = true;
        }
        else
        {
            isMiss = true;
        }

        if (isMiss)
        {
            comboCount = 0;
            inputBlocked = true;
        }

        if (isPerfect)
        {
            comboCount = Mathf.Clamp(comboCount + 1, 0, maxComboCount);
        }

        comboMultiplier = comboMultipliers[comboCount];
    }

    void ResetInput()
    {
        if (inputBlocked)
        {
            inputBlocked = false;
        }

        if (isMiss)
        {
            inputBlocked = true;
        }

        if (direction != Vector2.zero)
        {
            lastDirection = direction;
        }

        direction = new Vector2(0, 0);
        isPerfect = false;
        isGood = false;
        isMiss = false;
        hasReceivedInput = false;
        inputTimer = 0.0f;
    }

    void OnWaggle(InputAction.CallbackContext context)
    {
        ScoreInput();

        if (isMiss)
        {
            Debug.Log("Missed the waggle!");
            return;
        }

        Waggle();
    }

    void Waggle()
    {
        // do waggle stuff here.
        Debug.Log("Waggle! " + direction);
        Debug.Log("Combo Count: " + comboCount);

        // Player.transform.position += new Vector3(waggleDirection.x, waggleDirection.y, 0);
    }
}
