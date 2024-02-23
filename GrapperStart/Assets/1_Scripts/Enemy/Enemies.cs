using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemies
{
    public abstract IEnumerator GraplingAtkDamaged();
    public abstract IEnumerator baseDamaged();
    public abstract void PlayerToDamaged();

    public abstract IEnumerator Died();

  


}
