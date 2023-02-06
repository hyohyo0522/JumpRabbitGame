using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour,IItem
{
    public void Use(GameObject target)
    {
        float health = 30f;

        PlayerLife playerLife = target.GetComponent<PlayerLife>();
        if (playerLife != null && !playerLife.dead)
        {
            AudioManager.instance.PlaySFX("PlayerGetHeal");
            playerLife.RestoreHealth(health);

            Destroy(this.gameObject);
            
            //네트워크에서 삭제해야할 때.
            //// 모든 클라이언트에서 자신을 파괴
            //PhotonNetwork.Destroy(gameObject);
        }
    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}
