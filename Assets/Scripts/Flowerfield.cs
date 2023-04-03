using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowerfield : MonoBehaviour
{
    public Transform flowerSpot;
    public bool hasFlowerMon = false;

    // Start is called before the first frame update
    void Awake()
    {
        flowerSpot.position = this.transform.position + new Vector3(0, 1, 0);
        //FlowerEnemySpawner.allflowerFields.Add(this);
        FlowerEnemySpawner.instance.AddFlowerFields(this.gameObject);
    }




}
