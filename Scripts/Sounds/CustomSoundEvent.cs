using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using System;

[Serializable]
public class SoundEventClass
{
    public FMOD.Studio.EventInstance soundEvent;
    public EventReference soundRef;
    public bool m_is3D = false;

    private ATTRIBUTES_3D attribute3D;

    public void InitSound()
    {
    //    if (soundRef.Path == null)
    //    {
    //        UnityEngine.Debug.Log("Event not assign");
    //    }

        soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundRef);
    }

    public void InitSound(Transform transform)
    {
        //if (soundRef.Path == null)
        //{
        //    UnityEngine.Debug.Log("Event not assign  " + transform.name) ;
        //}

        soundEvent = FMODUnity.RuntimeManager.CreateInstance(soundRef);

        if (m_is3D)
        {
            attribute3D = FMODUnity.RuntimeUtils.To3DAttributes(transform);
            soundEvent.set3DAttributes(attribute3D);
        }
    }

    public void PlaySound()
    {
        soundEvent.start();
    }

    public void StopSoundDirect()
    {
        soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopSoundFadeout()
    {
        soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}


public class CustomSoundEvent : MonoBehaviour
{

    public SoundEventClass m_sound;
    public void Start()
    {
        m_sound.InitSound(transform);
    }

    public void ActiveSound()
    {
        m_sound.PlaySound();
    }

}
