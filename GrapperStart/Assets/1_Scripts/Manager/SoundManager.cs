using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgSound; //배경음
    public AudioClip[] bglist;
    public static SoundManager S_instance;

    private void Awake()
    {
        if(S_instance == null)
        {
            S_instance = this;
            DontDestroyOnLoad(S_instance);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else 
        {
            Destroy(S_instance);
        }
    }

    //배경음 호출
    private void OnSceneLoaded(Scene arg0,LoadSceneMode arg1)
    {
        for(int i = 0; i < bglist.Length; i++) 
        {
            if(arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]); 
                //씬이름과 사운드 이름을 같게 하여 씬과 이름이 같은 노래를 출력한다
            }
        }
        
    }

    public void BGSoundVolume(float val)
    {
        mixer.SetFloat("BGSoundVolume",Mathf.Log(val)*20);
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName+"Sound");
        AudioSource audioSoure = go.AddComponent<AudioSource>();
        audioSoure.outputAudioMixerGroup = mixer.FindMatchingGroups("SFXSound")[0];
        audioSoure.clip = clip;
        audioSoure.Play();

        Destroy(go, clip.length);
        //사운드를 넣을 곳에 SoundManager.S_instance.SFXPlay("Hook",clip); 이런식으로 대입
    }

    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();
    }
}
