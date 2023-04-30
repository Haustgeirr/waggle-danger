using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private TextMeshProUGUI comboText;
    private TextMeshProUGUI multiplierText;

    // Start is called before the first frame update
    void Start()
    {
        directionImages[0] = GameObject.Find("WaggleDirection0").GetComponent<Image>();
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();

        comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        multiplierText = GameObject.Find("MultiplierText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        selectedDirection = waggler.direction;
        if (selectedDirection == Vector2.zero)
        {
            selectedDirection = waggler.lastDirection;
        }

        SetSelectedDirectionSprites();
        SetComboText();
        SetMultiplierText();
    }

    void SetComboText()
    {
        var text = "";

        if (waggler.isPerfect)
        {
            text = "Perfect!";
        }
        else if (waggler.isGood)
        {
            text = "Good!";
        }
        else if (waggler.isMiss)
        {
            text = "Miss!";
        }

        comboText.text = text;
    }

    void SetMultiplierText()
    {
        multiplierText.text = waggler.comboMultiplier.ToString();
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
