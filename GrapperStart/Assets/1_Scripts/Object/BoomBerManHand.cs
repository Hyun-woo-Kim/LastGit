using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.XR;
using static UnityEditor.Experimental.GraphView.GraphView;


public class BoomBerManHand : MonoBehaviour
{
    [SerializeField]
    private GameObject effPrefab;
    public Transform collisionPos; // 충돌한 오브젝트의 충돌체를 저장할 변수
    public bool isParticleSpawned = false; // 이펙트가 생성되었는지 여부를 나타내는 플래그
    
    private IObjectPool<BMPunchEff> _Pool;


    PlayerHP playerHP;
    BManAction BmAction;
    PlayerControllerRope player;
    private void Awake()
    {
        playerHP = FindFirstObjectByType <PlayerHP>();
        BmAction = FindFirstObjectByType <BManAction>();
        player = FindFirstObjectByType <PlayerControllerRope>();
        _Pool = new ObjectPool<BMPunchEff>(CreateEff, OnGetEff, OnReleaseEff, OnDestroyEff, maxSize: 3);
    }

    public bool isDamaging = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Debug.Log("BB");
            isDamaging = true;
            playerHP.TakeDamage(1.0f);
            collisionPos = collision.transform;
            var eff = _Pool.Get();
            eff.TransformEff();
            isParticleSpawned = true;
        }
    }

   


    private void Update()
    {
       



    }

    private BMPunchEff CreateEff()
    {
       

        BMPunchEff eff = Instantiate(effPrefab, player.transform.position, UnityEngine.Quaternion.identity).GetComponent<BMPunchEff>();

       

        eff.SetManagedPool(_Pool);
     
        return eff;
    }

    private void OnGetEff(BMPunchEff eff)
    {
        eff.transform.localScale = BmAction.transform.localScale;

        eff.gameObject.SetActive(true);
        
        eff.transform.position = this.collisionPos.position;//이거 아니면 collision의 히트포인트
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