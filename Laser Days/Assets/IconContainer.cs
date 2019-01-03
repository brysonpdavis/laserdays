using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IconContainer : MonoBehaviour {

    public Sprite defaultSprite;
    public Sprite interactHover;
    public Sprite selectHover;
    public Sprite hold;
    public Sprite holdFill;

    public Sprite drag;
    public Sprite openHand;
    public Sprite openHandFill;
    public Sprite objectBounce;
    public Sprite playerBounce;
    public Sprite bothBounce;

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

    public void SetHoldFill()
    {
        canvasSprite.sprite = holdFill;
    }

    public void SetDrag()
    {
        canvasSprite.sprite = drag;
    }

    public void SetOpenHand()
    {
        canvasSprite.sprite = openHand;
    }

    public void SetOpenHandFill()
    {
        canvasSprite.sprite = openHandFill;
    }

    public void SetPlayerBounce()
    {
        canvasSprite.sprite = playerBounce;
    }

    public void SetObjectBounce()
    {
        canvasSprite.sprite = objectBounce;
    }

    public void SetBothBounce()
    {
        canvasSprite.sprite = bothBounce;
    }



}
