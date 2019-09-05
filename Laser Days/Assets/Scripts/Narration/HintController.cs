using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    private Text _text;
    private GameObject _container;
    private Animator _animator;
    private Image _icon;
    

    private void Start()
    {
        _container = gameObject;
        _text = _container.GetComponentInChildren<Text>();
        _animator = _container.GetComponent<Animator>();
        _icon = _container.transform.GetChild(0).Find("ControlIcon").GetComponent<Image>();

    }
    
    public void TriggerHint(TextAsset textAsset, Sprite controlIcon)
    {
        _text.text = textAsset.text;
        _icon.sprite = controlIcon;
        _container.SetActive(true);
        _animator.Play("Popup_Dialog");
    }

    public void Clear()
    {
        _container.SetActive(false);
    }
}
