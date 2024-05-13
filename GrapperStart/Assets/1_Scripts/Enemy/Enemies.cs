using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemies
{
    public abstract IEnumerator GraplingAtkDamaged();
    public abstract IEnumerator baseDamaged();
    public abstract void PlayerToDamaged();

    public abstract IEnumerator Died();

    public abstract void SpeedDown();
    public abstract void EnemySet();
    public abstract IEnumerator NotFindPlayer(SpriteRenderer sprite);
    public abstract IEnumerator EnemyAtkStop();
    public abstract void UpdateOutline(bool outline);
  


}
