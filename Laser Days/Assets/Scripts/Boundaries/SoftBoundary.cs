using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBoundary : MonoBehaviour
{
    [SerializeField]
    private float speedMultiplier;
    [SerializeField]
    private float routineDuration = 2f;
    
    private static Coroutine _coroutine;
    private static float _volume = 0f;
    private static List<SoftBoundary> _boundariesWithPlayerInside;

    private void Start()
    {
        if (_boundariesWithPlayerInside == null)
        {
            _boundariesWithPlayerInside = new List<SoftBoundary>();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
            Debug.LogError("volume: " + _volume);
    }

    private IEnumerator OutputVolumeRoutineOn()
    {
        var start = _volume;
        var end = 1f;
        var elapsed = 0f;
        var ratio = 0f;

        while (elapsed < routineDuration)
        {
            yield return null; 
            elapsed += Time.deltaTime;
            ratio = elapsed / routineDuration;

            _volume = TweeningFunctions.EaseIn(Mathf.Lerp(start, end, ratio));
        }

        _volume = end;
        _coroutine = null;
    }
    
    private IEnumerator OutputVolumeRoutineOff()
    {
        var start = _volume;
        var end = 0f;
        var elapsed = 0f;
        var ratio = 0f;

        while (elapsed < routineDuration)
        {
            yield return null; 
            elapsed += Time.deltaTime;
            ratio = elapsed / routineDuration;
            
            _volume = TweeningFunctions.EaseOut(Mathf.Lerp(start, end, ratio));
        }

        _volume = end;
        _coroutine = null;
    }

    private void StopLocalCoroutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_boundariesWithPlayerInside.Count == 0)
            {
                MFPP.Player.SetBoundaryMultiplier(speedMultiplier);
                StopLocalCoroutine();
                _coroutine = StartCoroutine(OutputVolumeRoutineOn());
            }
            
            _boundariesWithPlayerInside.Add(this);
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _boundariesWithPlayerInside.Remove(this);

            if (_boundariesWithPlayerInside.Count == 0)
            {
                MFPP.Player.SetBoundaryMultiplier(1f);
                StopLocalCoroutine();
                _coroutine = StartCoroutine(OutputVolumeRoutineOff());
            }        
        }
    }

    public static float GetVolume()
    {
        return _volume;
    } 
}