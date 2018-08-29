using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IconContainer : MonoBehaviour {

    public Sprite defaultSprite;
    public Sprite interactHover;
    public Sprite selectHover;
    public Sprite hold;
    public Sprite drag;
    public Sprite openHand;

    public Image canvasSprite;


    public void SetDefault()
    {
        canvasSprite.sprite = defaultSprite;
    }

    public void SetInteractHover()
    {
        canvasSprite.sprite = interactHover;
    }

    public void SetSelectHover()
    {
        canvasSprite.sprite = selectHover;
    }

    public void SetHold()
    {
        canvasSprite.sprite = hold;
    }

    public void SetDrag()
    {
        canvasSprite.sprite = drag;

    }

    public void SetOpenHand()
    {
        canvasSprite.sprite = openHand;

    }
}
