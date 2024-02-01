using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image image;
    private GameObject button;
    public GameObject soundop;
    private void Start()
    {
        button = GameObject.Find("StartButton");
    }

    public void Fadebutton()
    {
        button.SetActive(false);
        StartCoroutine(FadeCoroutine());
    }

    public void soundopTrue()
    {
        soundop.SetActive(true);
    }

   public void soundopFalse()
    {
        soundop.SetActive(false);
    }
    public float fadeDur;
    private IEnumerator FadeCoroutine()
    {
        float fadeCount = 0;
        while (fadeCount < fadeDur)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.color = new Color(0, 0, 0, fadeCount);
        }

        SceneManager.LoadScene("TestScene");
    }



    public void QuitSceneChange()
    {
        Debug.Log("Á¾·á");
        Application.Quit();
    }
}
