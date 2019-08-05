using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] private float fadeDist;
    [SerializeField] private float activeDist;
    [SerializeField] private float crossfadeTime;
    [SerializeField] private bool fadesSoundtrack = false;
    private AudioSource _realSource;
    private AudioSource _laserSource;
    private static List<AmbientSound> _ambientSources;
    private float _edgeFade;
    private float _realToLaser; // float that denotes the position in transition between worlds : 0 => real, 1 => laser
    private Transform _player;
    private Coroutine _crossfading;
    private float _playerDist;
    private State _state;

    private enum State
    {
        Inactive,
        Edge,
        Inside
    }

    private void Awake()
    {
        _edgeFade = 0;
        _crossfading = null;
        _realSource = transform.GetChild(0).GetComponent<AudioSource>();
        _laserSource = transform.GetChild(1).GetComponent<AudioSource>();

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

    private void Update()
    {
        ChangeStates();
        DoStates();
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

    private void SetSourceVolumes()
    {
        _realSource.volume = _edgeFade * (1 - _realToLaser);
        _laserSource.volume = _edgeFade * _realToLaser;
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
        if (_ambientSources != null)
        {
            foreach (var source in _ambientSources)
            {
                source.StopLocalCoroutines();
                source._crossfading = source.StartCoroutine(source.CrossfadeToReal());
            }
        }    
    }

    public static void CrossfadeAllToLaser()
    {
        if (_ambientSources != null)
        {
            foreach (var source in _ambientSources)
            {
                source.StopLocalCoroutines();
                source._crossfading = source.StartCoroutine(source.CrossfadeToLaser());
            }
        }    
    }

    public static float AmbientPercentage()
    {
        float ret = 0;

        if (_ambientSources != null)
        {
            foreach (var source in _ambientSources)
            {
                if (source.fadesSoundtrack)
                {
                    ret = Mathf.Max(ret, source._edgeFade);
                }
            }
        }
        
        return ret;
    }
    
    private void AddToList()
    {
        if (! _ambientSources.Contains(this))
            _ambientSources.Add(this);
    }

    private void RemoveFromList()
    {
        _ambientSources.Remove(this);
    }
    
    private void SetToProperWorldImmediate()
    {
        if (Toolbox.Instance.PlayerInLaser())
        {
            SetToLaserImmediate();
        }
        else
        {
            SetToRealImmediate();
        }
    }
    
    private void StopLocalCoroutines()
    {
        if (_crossfading != null)
            StopCoroutine(_crossfading);
    }

    private void ChangeStates()
    {
        _playerDist = Vector3.Distance(_player.position, transform.position);
        
        switch (_state)
        {
            case State.Inactive:
            {
                if (_playerDist < activeDist + fadeDist)
                {
                    _state = State.Edge;
                    EnableSources();
                    AddToList();
                    SetToProperWorldImmediate();
                }
                else if (_playerDist < activeDist)
                {
                    _state = State.Inside;
                    EnableSources();
                    AddToList();
                    SetToProperWorldImmediate();
                }
                break;
            }

            case State.Edge:
            {
                if (_playerDist > activeDist + fadeDist)
                {
                    _state = State.Inactive;
                    DisableSources();
                    RemoveFromList();
                }
                else if (_playerDist < activeDist)
                {
                    _state = State.Inside;
                }
                break;
            }

            case State.Inside:
            {
                if (_playerDist > activeDist + fadeDist)
                {
                    _state = State.Inactive;
                    DisableSources();
                    RemoveFromList();
                }
                else
                {
                    _state = State.Edge;
                }
                break;
            }
        }
    }

    private void DoStates()
    {
        switch (_state)
        {
            case State.Inactive:
            {
                _edgeFade = 0f;
                break;
            }

            case State.Edge:
            {
                _edgeFade = Mathf.Clamp( (fadeDist + activeDist - _playerDist) / fadeDist, 0f, 1f);
                SetSourceVolumes();
                break;
            }

            case State.Inside:
            {
                _edgeFade = 1f;
                SetSourceVolumes();
                break;
            }
        }
    }
    
    private void DrawGizmo(bool selected)
    {
        if(selected)
        {
            var green = new Color(0.1f, 0.8f, 0.4f, 0.2f);
            Gizmos.color = green;
            Gizmos.DrawSphere(transform.position, activeDist);
            Gizmos.DrawWireSphere(transform.position, activeDist + fadeDist);
        }
    }

    private void DrawOtherGizmo(bool selected)
    {
        if (selected)
        {
            var red = Color.red;
            Gizmos.color = red;
            Gizmos.DrawSphere(transform.position, activeDist + fadeDist);
            Gizmos.DrawWireSphere(transform.position, activeDist + fadeDist);
        }
    }

    public void OnDrawGizmos()
    {
        DrawGizmo(false);
    }
    public void OnDrawGizmosSelected()
    {
        DrawGizmo(true);
    }

}