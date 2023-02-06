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
            
            //��Ʈ��ũ���� �����ؾ��� ��.
            //// ��� Ŭ���̾�Ʈ���� �ڽ��� �ı�
            //PhotonNetwork.Destroy(gameObject);
        }
    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}
