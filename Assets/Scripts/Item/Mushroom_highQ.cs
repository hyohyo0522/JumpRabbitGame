using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_highQ : MonoBehaviour, IItem
{
    public void Use(GameObject target)
    {
        float health = 50f;

        PlayerLife playerLife = target.GetComponent<PlayerLife>();
        if (playerLife != null && !playerLife.dead)
        {
            if (!playerLife.isFullHeath())
            {
                playerLife.RestoreHealth(health);

            Destroy(this.gameObject);

            //��Ʈ��ũ���� �����ؾ��� ��.
            //// ��� Ŭ���̾�Ʈ���� �ڽ��� �ı�
            //PhotonNetwork.Destroy(gameObject);
            }

        }
    }
}