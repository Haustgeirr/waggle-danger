using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Waggler : MonoBehaviour, IEntity
{
    public static int[] comboMultipliers = new int[] { 1, 2, 4, 8, 16 };

    public Vector2 direction;
    public Vector2 waggleDirection;
    private Vector2 inputDirection;

    public int comboCount = 0;
    public int comboMultiplier = comboMultipliers[0];
    private int maxComboCount = 4;

    [Header("Input Settings")]
    public InputAction directionInput;
    public InputAction waggleInput;

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
        // new Vector2(1, 0),
        // new Vector2(0, -1),
        // new Vector2(-1, 0),
    };

    public void Tick()
    {
        SelectDirection();
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
            CheckMultipleInputs();
            OnWaggle(ctx);
        };

        // if we have already received an input this frame, penalise the player
        directionInput.performed += ctx =>
        {
            CheckMultipleInputs();

            direction = ctx.ReadValue<Vector2>();

            if (direction != waggleDirection)
            {
                Debug.Log("Wrong direction!");
                isMiss = true;
            }

            ScoreInput();
        };
    }

    // Start is called before the first frame update
    void Start() { }

    void Update()
    {
        // HandleDirectionInput();

        inputTimer += Time.deltaTime;
    }

    void CheckMultipleInputs()
    {
        if (hasReceivedInput)
        {
            Debug.Log("TOo many Inputs!");
            isMiss = true;
            return;
        }

        hasReceivedInput = true;
    }

    void ScoreInput()
    {
        if (inputTimer <= goodDuration)
        {
            isGood = true;
        }

        if (inputTimer <= perfectDuration)
        {
            isPerfect = true;
        }

        if (isMiss)
        {
            isGood = false;
            isPerfect = false;
            comboCount = 0;
        }

        if (isPerfect)
        {
            comboCount = Mathf.Clamp(comboCount + 1, 0, maxComboCount);
        }

        comboMultiplier = comboMultipliers[comboCount];
    }

    void SelectDirection()
    {
        ResetInput();

        if (directionsOptions.Count == 0)
        {
            directionsOptions.AddRange(directionsBase);
        }

        int index = Random.Range(0, directionsOptions.Count);
        waggleDirection = directionsOptions[index];
        directionsOptions.RemoveAt(index);
    }

    void ResetInput()
    {
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
        Debug.Log("Waggle!");
    }
}
