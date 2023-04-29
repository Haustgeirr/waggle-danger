using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimator : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 direction;
    public float speed = 1.0f;
    public float bobAmount = 0.1f;

    [Header("Sprite Settings")]
    public Sprite[] beeSprites;
    public SpriteRenderer spriteRenderer;
    private float spriteChangeTimer = 0.0f;
    public float spriteChangeDelay = 0.04f;
    private int spriteIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateSprite();
        Bob();
    }

    void Bob()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + Mathf.Sin(Time.time * speed) * bobAmount * Time.deltaTime,
            transform.position.z
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
}
