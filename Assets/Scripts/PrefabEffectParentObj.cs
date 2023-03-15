using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabEffectParentObj : MonoBehaviour
{
    GameObject[] myChilds;


    private void Start()
    {
        myChilds = new GameObject[transform.childCount];
        for (int i = 0; i < myChilds.Length; i++)
        {
            myChilds[i] = transform.GetChild(i).gameObject;
        }

        Invoke("DestroyAll", 1.5f);

    }

    private void DestroyAll()
    {
        foreach(GameObject child in myChilds)
        {
            if(child != null)
            {
                Destroy(child);
            }
        }

        Destroy(this.gameObject);
    }

}
