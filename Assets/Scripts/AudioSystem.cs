using System;
using UnityEngine;
using UnityEngine.Audio;

/*
 * This class is used to manage the audio in the game that corresponds to the sound effects, music, and voice lines heard in the center of the game.
 * Because of this enemy sounds are not included in this class, as well as footsteps as they are more dynamic and are handled in the EnemySystem.cs 
 * and PlayerMovement.cs respectively.
 * 
*/
public class AudioSystem : MonoBehaviour
{

    public SoundClass[] sfx;

    public SoundClass[] music;

    public SoundClass[] voiceLines;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (SoundClass sound in sfx)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

        foreach (SoundClass sound in music)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

        foreach (SoundClass sound in voiceLines)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void PlaySFXByName(String name)
    {
        SoundClass sound = Array.Find(sfx, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Play();
    }

    public void PlayOnceSFXByName(string name)
    {
        SoundClass sound = Array.Find(sfx, sound => sound.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            if(!sound.source.isPlaying)
                sound.source.Play();
    }

    public void PlayMusicByName(String name)
    {
        SoundClass sound = Array.Find(music, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Play();
    }

    public void PlayMusicByIndex(int index)
    {
        music[index].source.Play();
    }

    public void PlayVoiceLineByName(String name)
    {
        SoundClass sound = Array.Find(voiceLines, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Play();
    }

    public void StopSFXByName(String name)
    {
        SoundClass sound = Array.Find(sfx, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Stop();
    }

    public void StopMusicByName(String name)
    {
        SoundClass sound = Array.Find(music, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Stop();
    }

    public void StopMusicByIndex(int index)
    {
        music[index].source.Stop();
    }

    public void StopVoiceLineByName(String name)
    {
        SoundClass sound = Array.Find(voiceLines, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
            sound.source.Stop();
    }

    public void PlayVoiceLineByIndex(int index)
    {
        voiceLines[index].source.Play();
    }

    
}


