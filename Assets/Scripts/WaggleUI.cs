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
    public Sprite inputBlocked;

    private Vector2 selectedDirection;
    private Waggler waggler;
    private Sprite selectedDirectionSprite;

    // Start is called before the first frame update
    void Start()
    {
        directionImages[0] = GameObject.Find("WaggleDirection0").GetComponent<Image>();
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();
    }

    // Update is called once per frame
    void Update()
    {
        selectedDirection = waggler.direction;
        SetSelectedDirectionSprites();
    }

    void SetSelectedDirectionSprites()
    {
        directionImages[0].enabled = true;
        var blocked = waggler.isMiss || waggler.inputBlocked;
        directionImages[0].sprite = GetDirectionSprite(selectedDirection, blocked);
    }

    Sprite GetDirectionSprite(Vector2 direction, bool blocked)
    {
        if (blocked)
        {
            return inputBlocked;
        }

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
            default:
                directionImages[0].enabled = false;
                break;
        }

        return sprite;
    }
}
