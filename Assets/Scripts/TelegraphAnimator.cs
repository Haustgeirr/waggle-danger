using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float minOpacity = 0.0f;
    public float maxOpacity = 1.0f;
    public float moveAmount = 1.0f;

    private float opacity = 0.0f;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        opacity = Mathf.Lerp(maxOpacity, minOpacity, gameManager.tickTimer / gameManager.tickRate);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, opacity);

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y - moveAmount / gameManager.tickRate * Time.deltaTime,
            transform.localPosition.z
        );
    }

    public void Show(Vector3 position, Vector2 direction)
    {
        opacity = maxOpacity;
        transform.position = position;
        // transform.Rotate(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
