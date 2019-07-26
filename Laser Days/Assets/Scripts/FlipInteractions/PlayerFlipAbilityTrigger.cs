using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFlipAbilityTrigger : MonoBehaviour
{

    [SerializeField] 
    private GameObject prefab;
    
    [SerializeField]
    private bool allowFlip = true;

    private GameObject _uiObject;
    
    private Image _whiteBackground;

    [SerializeField]
    private float fadeDuration;

    [SerializeField] 
    private float readDuration;

    private List<Text> texts;

    private GameObject _canvas;

    private MFPP.Player player;

    
    private IEnumerator FadeRoutine()
    {
        texts = new List<Text>();
        player = Toolbox.Instance.GetPlayer().GetComponent<MFPP.Player>();
        
        player.Movement.AllowMovement = false;
        player.Movement.AllowMouseMove = false;

        _canvas = Toolbox.Instance.mainCanvas;
        _uiObject = Instantiate(prefab, _canvas.transform, false);

        _whiteBackground = _uiObject.GetComponent<Image>();
        
        foreach (Transform child in _uiObject.transform)
        {
            if (child.parent == _uiObject.transform)
            {
                texts.Add(child.GetComponent<Text>());
                child.gameObject.SetActive(false);
            }
        }
        

        var backgroundColor = _whiteBackground.color;
        backgroundColor.a = 0;
        
        _whiteBackground.color = backgroundColor;

        // fade in
        
        var elapsed = 0f;
        var ratio = 0f;
        var startAlpha = 0f;
        var endAlpha = 1f;

        while (elapsed < fadeDuration)
        {
            yield return null;

            elapsed += Time.deltaTime;

            ratio = elapsed / fadeDuration;
            
            backgroundColor.a = Mathf.Lerp(startAlpha, endAlpha, TweeningFunctions.Linear(ratio));
            
            _whiteBackground.color = backgroundColor;
        }

        int i = 0;


        while (i < texts.Count)
        {
            
            var textColor = Color.black;
            textColor.a = 0;

            texts[i].gameObject.SetActive(true);
            
            texts[i].color = textColor;

            elapsed = 0;

            ratio = 0;

            startAlpha = 0;

            endAlpha = 1f;
            
            while (elapsed < fadeDuration)
            {
                yield return null;

                elapsed += Time.deltaTime;

                ratio = elapsed / fadeDuration;

                textColor.a = Mathf.Lerp(startAlpha, endAlpha, TweeningFunctions.Linear(ratio));

                texts[i].color = textColor;
            }
            
            yield return new WaitForSeconds(readDuration);

            elapsed = 0;

            ratio = 0;

            startAlpha = 1f;

            endAlpha = 0f;
            
            while (elapsed < fadeDuration)
            {
                yield return null;

                elapsed += Time.deltaTime;

                ratio = elapsed / fadeDuration;

                textColor.a = Mathf.Lerp(startAlpha, endAlpha, TweeningFunctions.Linear(ratio));

                texts[i].color = textColor;
            }

            
            
            i++;
        }

        // fade out
        
        elapsed = 0f;
        ratio = 0f;
        startAlpha = 1f;
        endAlpha = 0f;
        
        while (elapsed < fadeDuration)
        {
            yield return null;

            elapsed += Time.deltaTime;

            ratio = elapsed / fadeDuration;
            
            backgroundColor.a = Mathf.Lerp(startAlpha, endAlpha, TweeningFunctions.Linear(ratio));

            _whiteBackground.color = backgroundColor;
        }
        
        player.Movement.AllowMovement = true;
        player.Movement.AllowMouseMove = true;

        Destroy(_uiObject);

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (allowFlip)
                Toolbox.Instance.GetFlip().canFlip = true;
            else
                Toolbox.Instance.GetFlip().canFlip = false;
            
            StartCoroutine(FadeRoutine());

            GetComponent<Collider>().enabled = false;
        }
            
    }
}
