using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraplingRange : MonoBehaviour
{
    public int grapRangeCount;
    public float grapRangeCool;
    public bool grapRangeSkillCool;
    public bool isRange = false;
    public bool isobjSkill = false;
    public bool isenemySkill = false;
    public Transform skillPos;
    public float radius;
    public bool isUnViewObjRangeRunning = false; // 범위 비활성화 시 for문 반복 제어 변수 추가
    public bool isFifthchild = false; //skillRange 오브젝트 활성화 유무 변수 추가 

    // Update is called once per frame
    void Update()
    {
        GrapRangeCool();
        CollObjRange();

        if (Input.GetKeyDown(KeyCode.Q)) //스킬 사용했을 때 Ring충돌 유무 판별하는 구문
        {
            PlayerRangeSkill();
        }


    }
    private void GrapRangeCool()
    {
        if (grapRangeCount >= 1) //기본 공격 카운트가 1보다 클 때
        {

            grapRangeCool -= Time.deltaTime; //기본 공격 시간이 2에서 계속 줄어든다.
            if (grapRangeCool <= 0.0f)//기본 공격 시간이 0보다 작을 때(0이 됐을 때)
            {
                grapRangeCool = 3.0f; //다시 처음으로 기본 공격시간을 2.0초로 맞춘다.
                grapRangeCount = 0;      // 기본 공격 카운트가 0으로 맞춘다.
                Debug.Log("범위 스킬 쿨타임 초기화");
            }

        }

    }


    
    private SpriteRenderer GetFifthChildSpriteRenderer()
    {
        Transform fifthChild = transform.GetChild(4);
        if (fifthChild.gameObject.activeSelf)
        {
            isFifthchild = true;
        }
            return fifthChild.GetComponent<SpriteRenderer>();
    }

    public void PlayerRangeSkill()
    {
        Transform fifthChild = transform.GetChild(4);
        GameObject fchild = fifthChild.gameObject;
        fchild.SetActive(true);

        if (grapRangeCount == 0)
        {
            grapRangeCount++;
            isRange = true;

          
                StartCoroutine(isobjSkill ? ViewObjRange() : UnViewObjRange());
            
            


        }
    }

    private void CollObjRange()
    {
        isenemySkill = false;
        isobjSkill = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skillPos.position, radius);

        foreach (Collider2D colliderA in colliders)
        {
            if (colliderA.CompareTag("Ring"))
            {
                isobjSkill = true;
            }
            if (colliderA.CompareTag("Enemy"))
            {
                isobjSkill = true;
                isenemySkill = true;
            }
        }


        
        if (grapRangeCount == 0 || grapRangeCount == 1)
        {

            if (isobjSkill == false)
            {
                if (isUnViewObjRangeRunning == false)
                {
                    StartCoroutine(UnViewObjRange());
                    isUnViewObjRangeRunning = true;


                }

            }


        }


    }
    
    IEnumerator ViewObjRange()
    {
        SpriteRenderer objspr = GetFifthChildSpriteRenderer();
        isUnViewObjRangeRunning = false;
        objspr.enabled = true;

        yield return null;

    }

    const float ViewObjRangeDuration = 0.3f;
    IEnumerator UnViewObjRange()
    {

        SpriteRenderer objspr = GetFifthChildSpriteRenderer();
        


        if (isFifthchild == true)
        {
            isRange = false;
            for (int i = 0; i < 6; i++)
            {
                objspr.enabled = !objspr.enabled;
                yield return new WaitForSeconds(ViewObjRangeDuration);
            }

            if (isUnViewObjRangeRunning == true)
            {
                objspr.enabled = false;
            }

        }





        //StartCoroutine(stopViewRange());

    }

  


   
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(skillPos.position,radius);//DrawWireCube(pos.position,boxsize)
        //Gizmos.DrawWireCube(skillattackpos.position, skillAtkboxSize);//DrawWireCube(pos.position,boxsize)
    }

}
