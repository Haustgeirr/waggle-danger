using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatAnimator : MonoBehaviour
{
    private Image beatImage;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        beatImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // lerp opacity based on tick
        var t = 1 - Mathf.Clamp01(gameManager.tickTimer / gameManager.tickRate);
        beatImage.color = new Color(1, 1, 1, t);
    }
}
