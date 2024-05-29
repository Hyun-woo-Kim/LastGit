using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProPhetWeapon : MonoBehaviour
{
    CircleCollider2D collder;

    public bool isPlayerWeaponDamaged;
    public float RestraintTime;
    void Start()
    {
        collder = GetComponent<CircleCollider2D>();
        isPlayerWeaponDamaged = false; //생성되었으니 존재 true

       

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            isPlayerWeaponDamaged = true;
            StartCoroutine(DragonWeapon());
          
        }
      
    }

    IEnumerator DragonWeapon()
    {
        PlayerControllerRope plyerController = FindAnyObjectByType<PlayerControllerRope>();

        StartCoroutine(plyerController.PpRestraint(RestraintTime)); 

        yield return new WaitForSeconds(0.1f);

        WeaponDestroy();
        
    }


    void WeaponDestroy()
    {
        if(this.gameObject != null)
        {
            isPlayerWeaponDamaged = false;
            Destroy(this.gameObject);
        }
    }
}
