using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    public enum ObjectType { Clickable, Sokoban1x1, Sokoban2x2, Morph, MorphOn, MorphOff, Wall, Null };
    public ObjectType objectType;

    [Header("Attributes")]

    public string itemName;
    public bool isKey;
    public string key = null;


    Renderer mRenderer;
    Material material;
    public Shader selectedShader;
    public bool unflippable;
    private RaycastManager rm;
    public bool objectCharge = true;

    [HideInInspector] public bool boost = false;
    [HideInInspector] public bool touchingzone = false;
    [HideInInspector] public bool inMotion = false;
    [HideInInspector]public int value;
    [HideInInspector] public bool touchingPlayer;
    [HideInInspector]public bool reducible;
    [HideInInspector]public int maxvalue;
    [HideInInspector]public int minvalue;

    private bool isRegen;

    public bool selected = false;

    private void Awake()
    {
        //sets all of the shaders to correct flip state [toggle that affects how selection looks]
        mRenderer = GetComponent<Renderer>();
        material = mRenderer.material;

        if (!unflippable){
            material.SetInt("_IsFlippable", 1);
        }

        else {material.SetInt("_IsFlippable", 0);}
    }

    private void Start()
    {
        rm = Toolbox.Instance.GetPlayer().GetComponent<RaycastManager>();
    }

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

    public void Select()
    {
        material.shader = selectedShader;

    }

    public void UnSelect()
    {
        if (this.gameObject.layer == 10)
        {
            material.shader = rm.laserWorldShader;
        }

        else { material.shader = rm.realWorldShader; }
    
    }

}
