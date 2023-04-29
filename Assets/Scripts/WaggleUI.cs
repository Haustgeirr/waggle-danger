using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaggleUI : MonoBehaviour
{
    public Image currentDirectionImage;

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
        currentDirectionImage = GameObject.Find("WaggleDirection").GetComponent<Image>();
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedDirection != waggler.waggleDirection)
        {
            selectedDirection = waggler.waggleDirection;
            SetSelectedDirectionSprite();
        }
    }

    void SetSelectedDirectionSprite()
    {
        switch (selectedDirection)
        {
            case Vector2 v when v.Equals(Vector2.up):
                currentDirectionImage.sprite = upDirection;
                break;
            case Vector2 v when v.Equals(Vector2.right):
                currentDirectionImage.sprite = rightDirection;
                break;
            case Vector2 v when v.Equals(Vector2.down):
                currentDirectionImage.sprite = downDirection;
                break;
            case Vector2 v when v.Equals(Vector2.left):
                currentDirectionImage.sprite = leftDirection;
                break;
        }
    }
}
