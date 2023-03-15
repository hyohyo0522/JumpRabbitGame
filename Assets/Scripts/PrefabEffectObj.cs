using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabEffectObj : MonoBehaviour
{

    public void DestroySelf()
    {
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }

    }

}
