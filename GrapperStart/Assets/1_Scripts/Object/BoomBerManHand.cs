using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.Experimental.GraphView.GraphView;


public class BoomBerManHand : MonoBehaviour
{
    [SerializeField]
    private GameObject effPrefab;
    public Transform collisionPos; // 충돌한 오브젝트의 충돌체를 저장할 변수
    public bool isParticleSpawned = false; // 이펙트가 생성되었는지 여부를 나타내는 플래그
    
    private IObjectPool<BMPunchEff> _Pool;

    private void Awake()
    {

        _Pool = new ObjectPool<BMPunchEff>(CreateEff, OnGetEff, OnReleaseEff, OnDestroyEff, maxSize: 3);
    }
   
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            Debug.Log("BB");
            collisionPos = collision.transform;
            var eff = _Pool.Get();
            eff.TransformEff();
            isParticleSpawned = true;
        }
    }


    private BMPunchEff CreateEff()
    {
        SpriteRenderer effSpr = effPrefab.GetComponent<SpriteRenderer>();
        if (collisionPos.transform.position.x < transform.position.x)
        {
            effSpr.flipX = false;
        }
        else
        {
            effSpr.flipX = true;
        }

        BMPunchEff eff = Instantiate(effPrefab, collisionPos.transform.position, UnityEngine.Quaternion.identity).GetComponent<BMPunchEff>();
        eff.SetManagedPool(_Pool);
     
        return eff;
    }

    private void OnGetEff(BMPunchEff eff)
    {
        eff.gameObject.SetActive(true);
    }
    private void OnReleaseEff(BMPunchEff eff)
    {
        eff.gameObject.SetActive(false);
    }

    private void OnDestroyEff(BMPunchEff eff)
    {
        
        Destroy(eff.gameObject);
    }
}