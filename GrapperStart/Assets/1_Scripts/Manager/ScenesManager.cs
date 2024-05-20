using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : SingleTonGeneric<ScenesManager>
{
    // Start is called before the first frame update
    public void SceneSingleTonSet()
    {

    }

    private void Start()
    {
        
    }
    public void FindEnemiesInScene()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Aiming playerAim = FindFirstObjectByType<Aiming>();
       
        foreach (GameObject enemy in enemies)
        {
            // 찾은 각 적에 대한 작업 수행
            if (playerAim.isAimRing  == true)
            {
                Debug.Log("플레이어 전등 조준 O");
                enemy.GetComponentInParent<Enemies>().SpeedDown();
            }
            else if (playerAim.isAimRing == false)
            {
                enemy.GetComponentInParent<Enemies>().EnemySet();
            }


        }
    }


    public void Nest()
    {
        PlayerUI.Instance.UIBeAcitve(); //UI 끄고 
        SceneManager.LoadScene(2); // 씬 이동
    }
}
