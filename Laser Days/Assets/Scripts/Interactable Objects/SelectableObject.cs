public abstract class SelectableObject : ReticleObject, ISelectable
{
    public bool selected;

    public override void Start()
    {
        base.Start();

        selected = false;
    }

    public virtual void OnSelect()
    {
        if (_selectionRenderChange)
        {
            _selectionRenderChange.OnHold();
        }
        
        SetMaterialFloatProp("_onHold", 1f);
        SetMaterialFloatProp("_Elapsed", 0f);
        
        selected = true;
        
        if (_action) 
            _action.Selected();
    }

    public virtual void OffSelect()
    {
        if (_selectionRenderChange)
            _selectionRenderChange.OnDrop();
        
        SetMaterialFloatProp("_onHold", 0f);
        //SetMaterialFloatProp("_Elapsed", 0f);

        selected = false;
    }

    public bool GetSelected()
    {
        return selected;
    }
}