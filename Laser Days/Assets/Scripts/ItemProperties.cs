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
    [SerializeField] public bool touchingzone = false;

    [SerializeField] public int value;
    [SerializeField] public bool touchingPlayer;
    [SerializeField] public bool reducible;
    [SerializeField] public bool unflippable;
    [SerializeField] public int maxvalue;
    [SerializeField] public int minvalue;
    private bool isRegen;

    public bool selected = false;

    void Update()
    {
      if (touchingzone == false && isRegen == false)
      {
        StartCoroutine(check());
      }
    }

    private IEnumerator check()
    {
      isRegen = true;
      while(value<maxvalue)
      {
        value += 1;
        yield return new WaitForSeconds(1);
      }
      isRegen = false;
    }

}
