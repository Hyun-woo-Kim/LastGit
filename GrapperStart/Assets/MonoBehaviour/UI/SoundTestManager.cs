using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTestManager : MonoBehaviour
{
    public AudioSource musicSource;
   
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
