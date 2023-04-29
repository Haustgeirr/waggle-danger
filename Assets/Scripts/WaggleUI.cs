using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaggleUI : MonoBehaviour
{
    public Image[] directionImages = new Image[4];

    // public Sprite perfectSprite;
    // public Sprite goodSprite;
    // public Sprite missSprite;

    public Sprite upDirection;
    public Sprite rightDirection;
    public Sprite downDirection;
    public Sprite leftDirection;

    private Vector2 selectedDirection;
    private Waggler waggler;
    private Sprite selectedDirectionSprite;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < directionImages.Length; i++)
        {
            directionImages[i] = GameObject.Find("WaggleDirection" + (i)).GetComponent<Image>();
        }

        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();
    }

    // Update is called once per frame
    void Update()
    {
        selectedDirection = waggler.waggleDirection;
        SetSelectedDirectionSprites();
    }

    void SetSelectedDirectionSprites()
    {
        for (int i = 0; i < directionImages.Length; i++)
        {
            directionImages[i].sprite = GetDirectionSprite(waggler.waggleDirections[i]);
        }
    }

    Sprite GetDirectionSprite(Vector2 direction)
    {
        var sprite = upDirection;
        switch (direction)
        {
            case Vector2 v when v.Equals(Vector2.up):
                sprite = upDirection;
                break;
            case Vector2 v when v.Equals(Vector2.right):
                sprite = rightDirection;
                break;
            case Vector2 v when v.Equals(Vector2.down):
                sprite = downDirection;
                break;
            case Vector2 v when v.Equals(Vector2.left):
                sprite = leftDirection;
                break;
        }

        return sprite;
    }
}
