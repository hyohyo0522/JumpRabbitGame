using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpotForPlayer : MonoBehaviour
{
    public Transform reviveSpot;

    void Start()
    {
        PlayerManager.instance.allReviveSpots.Add(this);
    }


}
