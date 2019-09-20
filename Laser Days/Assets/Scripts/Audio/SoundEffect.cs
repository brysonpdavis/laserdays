using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    [SerializeField] 
    [Range(0.0f, 1.0f)] 
    private float maxVolume = 1f;
    
    private AudioSource _source;


    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.volume = 0;
        _source.Play();
    }
    private void Update()
    {
        _source.volume = maxVolume * SoundTrackManager.GetGlobalVolume();
    }
}