using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgSound; //�����
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

    //����� ȣ��
    private void OnSceneLoaded(Scene arg0,LoadSceneMode arg1)
    {
        for(int i = 0; i < bglist.Length; i++) 
        {
            if(arg0.name == bglist[i].name)
            {
                BgSoundPlay(bglist[i]); 
                //���̸��� ���� �̸��� ���� �Ͽ� ���� �̸��� ���� �뷡�� ����Ѵ�
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
        //���带 ���� ���� SoundManager.S_instance.SFXPlay("Hook",clip); �̷������� ����
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
