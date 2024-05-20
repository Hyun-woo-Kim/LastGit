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
            // ã�� �� ���� ���� �۾� ����
            if (playerAim.isAimRing  == true)
            {
                Debug.Log("�÷��̾� ���� ���� O");
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
        PlayerUI.Instance.UIBeAcitve(); //UI ���� 
        SceneManager.LoadScene(2); // �� �̵�
    }
}
