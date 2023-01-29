using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextObjPrefab : MonoBehaviour
{

    private void OnEnable()
    {
        Destroy(this.gameObject, 2.5f);

    }

    private void Update()
    {
        this.transform.position += new Vector3(0, Time.deltaTime, 0);

    }
}
