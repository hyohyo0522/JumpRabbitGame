using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ////float normalY = collision.contacts[0].normal.y;
        ////Debug.Log(normalY);
        //if (collision.contacts[0].normal.y < -0.4)
        //{ 
        //    playerTouched();
        //}
    }

}
