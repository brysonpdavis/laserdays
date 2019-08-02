using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] private float fadeDist;
    [SerializeField] private float crossfadeTime;
    private AudioSource _realSource;
    private AudioSource _laserSource;
    private Collider _collider;
    private static List<AmbientSound> _ambientSources;
    private float _edgeFade;
    private float _realToLaser; // float that denotes the position in transition between worlds : 0 => real, 1 => laser
    private Transform _player;

    private void Awake()
    {
        _edgeFade = 0;
        _realSource = transform.GetChild(0).GetComponent<AudioSource>();
        _laserSource = transform.GetChild(1).GetComponent<AudioSource>();
        _collider = GetComponent<Collider>();

        if (_ambientSources == null)
        {
            _ambientSources = new List<AmbientSound>();
        }
    }

    private void Start()
    {
        _player = Toolbox.Instance.GetPlayer().transform;
        
        if (Toolbox.Instance.PlayerInReal())
        {
            SetToRealImmediate();
        }        
        else
        {
            SetToLaserImmediate();
        }

        _realSource.volume = 0;
        _laserSource.volume = 0;
        
        DisableSources();
    }

    private void EnableSources()
    {
        _laserSource.enabled = true;
        _realSource.enabled = true;
    }

    private void DisableSources()
    {
        _laserSource.enabled = false;
        _realSource.enabled = false;
    }

    private void SetToRealImmediate()
    {
        _realToLaser = 0;
    }

    private void SetToLaserImmediate()
    {
        _realToLaser = 1;
    }

    private IEnumerator CrossfadeToReal()
    {
        float start = _realToLaser;
        float end = 0f;
        float elapsed = 0f;
        float ratio = 0f;

        while (ratio < 1)
        {
            _realToLaser = Mathf.Lerp(start, end, TweeningFunctions.Linear(ratio));
            
            yield return null;

            elapsed += Time.deltaTime;
            ratio = elapsed / crossfadeTime;
        }
        
        _realToLaser = end;
    }

    private IEnumerator CrossfadeToLaser()
    {
        float start = _realToLaser;
        float end = 1f;
        float elapsed = 0f;
        float ratio = 0f;

        while (ratio < 1)
        {
            _realToLaser = Mathf.Lerp(start, end, TweeningFunctions.Linear(ratio));
            
            yield return null;

            elapsed += Time.deltaTime;
            ratio = elapsed / crossfadeTime;
        }

        _realToLaser = end;

    }

    public static void SetAllToRealImmediate()
    {
        foreach (var source in _ambientSources)
        {
            source.StopLocalCoroutines();
            source.SetToRealImmediate();
        }
        
    }

    public static void SetAllToLaserImmediate()
    {
        foreach (var source in _ambientSources)
        {
            source.StopLocalCoroutines();
            source.SetToLaserImmediate();
        }
    }

    public static void CrossfadeAllToReal()
    {
        foreach (var source in _ambientSources)
        {
            source.StartCoroutine(source.CrossfadeToReal());
        }
    }

    public static void CrossfadeAllToLaser()
    {
        foreach (var source in _ambientSources)
        {
            source.StartCoroutine(source.CrossfadeToLaser());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _ambientSources.Add(this);
        EnableSources();
        if (Toolbox.Instance.PlayerInReal())
        {
            _realToLaser = 0;
        }
        else
        {
            _realToLaser = 1;
        }
        
        // Debug.LogError("playerInReal: " + Toolbox.Instance.PlayerInReal());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ambientSources.Remove(this);
            DisableSources(); 
        }    
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _edgeFade = 1f; // Mathf.Clamp(Vector3.Distance(_collider.ClosestPointOnBounds(_player.position), _player.position) / fadeDist, 0f, 1f);
            _realSource.volume = _edgeFade * (1 - _realToLaser);
            _laserSource.volume = _edgeFade * _realToLaser;
            // Debug.LogError("edgeFade: " + _edgeFade + " realToLaser: " + _realToLaser);
        }    
    }

    private void StopLocalCoroutines()
    {
        StopCoroutine("CrossfadeToLaser");
        StopCoroutine("CrossfadeToReal");
    }
}