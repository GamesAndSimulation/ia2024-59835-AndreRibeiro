using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClass
{
   
    public AudioClip clip;

    public string name;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    [HideInInspector]
    public AudioSource source;



}
