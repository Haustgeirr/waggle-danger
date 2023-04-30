using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimator : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 direction;
    public float bobAmount = 0.1f;
    private Vector3 startPosition;
    private float startTime;

    [Header("Sprite Settings")]
    public Sprite[] beeSprites;
    public SpriteRenderer spriteRenderer;
    private float spriteChangeTimer = 0.0f;
    public float spriteChangeDelay = 0.04f;
    private int spriteIndex = 0;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteChangeDelay = gameManager.tickRate / 16f;
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        FlipSpriteDirection();
        AnimateSprite();
        Bob();
    }

    void Bob()
    {
        var t = gameManager.tickTimer / gameManager.tickRate;
        var tInverse = (gameManager.tickRate - gameManager.tickTimer) / gameManager.tickRate;
        var tBounced = gameManager.tickTimer >= gameManager.tickRate / 2f ? tInverse : t;
        float offset = Mathf.Lerp(0, bobAmount, tBounced);

        // transform.position = startPosition + offset;
        transform.localPosition = new Vector3(
            startPosition.x,
            startPosition.y + offset,
            startPosition.z
        );
    }

    void AnimateSprite()
    {
        spriteChangeTimer += Time.deltaTime;

        if (spriteChangeTimer >= spriteChangeDelay)
        {
            spriteChangeTimer = 0.0f;
            spriteIndex = (spriteIndex + 1) % beeSprites.Length;
            spriteRenderer.sprite = beeSprites[spriteIndex];
        }
    }

    void FlipSpriteDirection()
    {
        var dir = gameManager.waggler.direction;
        var isRight = dir == Vector2.right;
        var isLeft = dir == Vector2.left;

        if (!isRight && !isLeft)
        {
            return;
        }

        // flip sprite if facing right
        if (gameManager.waggler.direction == Vector2.right)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void Show()
    {
        spriteRenderer.enabled = true;
    }

    void Hide()
    {
        spriteRenderer.enabled = false;
    }
}
