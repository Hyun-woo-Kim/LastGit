using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundTestManager : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider slider;
    public int count = 0;

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume / 10;
    }

    public void UpVolume()
    {
        if (count != 10)
        {
            count++;
            slider.value = count;
        }
    }

    public void DownVolume()
    {
        if (count != 0)
        {
            count--;
            slider.value = count;
        }

    }
}
