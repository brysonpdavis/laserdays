using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReticleObject : MonoBehaviour 
{
	
	[HideInInspector]public string itemName;

	protected IconContainer _iconContainer;
	protected Renderer _renderer;
	protected Material _material;
	protected SelectionRenderChange _selectionRenderChange;
	protected float _activateDistance  = 2f;
	protected TakeActionOnAction _action;

	public virtual void Start ()
	{
		_iconContainer = Toolbox.Instance.GetIconContainer();
		_renderer = GetComponent<Renderer>();
        if (_renderer)
		    _material = _renderer.material;
		SetMaterialFloatProp("_Flippable", 0);
		SetMaterialFloatProp("_onHold", 0f);

		_selectionRenderChange = GetComponent<SelectionRenderChange>();
		_action = GetComponent<TakeActionOnAction>();
	}

	public virtual void OnHover()
	{
        SetMaterialFloatProp("_onHover", 1);

		if (_selectionRenderChange)
			_selectionRenderChange.SwitchRenderersOn();

		if (_action)
			_action.Hovered();
	}

	public virtual void OffHover()
	{
        SetMaterialFloatProp("_onHover", 0);

		if (_selectionRenderChange)
			_selectionRenderChange.SwitchRenderersOff();
		
	}

	public virtual void DistantIconHover()
	{
		_iconContainer.SetDefault();
	}

	public virtual void CloseIconHover()
	{
		_iconContainer.SetDefault();
	}

	public virtual void SetMaterialFloatProp(string property, float value)
	{
        if (_material)
            _material.SetFloat(property, value);
	}
}
