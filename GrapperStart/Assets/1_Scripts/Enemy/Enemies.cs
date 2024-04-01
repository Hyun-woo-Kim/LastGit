using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemies
{
    public abstract IEnumerator GraplingAtkDamaged(float damage);
    public abstract IEnumerator baseDamaged();
    public abstract void PlayerToDamaged();

    public abstract IEnumerator Died();

    public abstract void SpeedDown();
    public abstract void EnemySet();
    public abstract void UpdateOutline(bool outline);
  


}
