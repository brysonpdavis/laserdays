using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    [Header("Your Clickables")]

    public string itemName;
    public bool isKey;
    public string key = null;

    [SerializeField] public bool objectCharge = true;
    [SerializeField] public bool boost = false;

    [SerializeField] public int value;
    [SerializeField] public bool touchingPlayer;

    public bool selected = false;
}


