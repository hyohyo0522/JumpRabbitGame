using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_RestoreHealth : MonoBehaviour,IItem
{
    public float health = 30f;
    public float destroyDelayTime = 5f;
    public float delayForUse = 1f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;

    private void OnEnable()
    {
        StartCoroutine("makeDelay");
    }

    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);
    }

    IEnumerator makeDelay()
    {
        afterDelay = false;
        yield return new WaitForSeconds(delayForUse);
        afterDelay = true;
    }

    public void Use(GameObject target)
    {
        if (afterDelay)
        {
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
}

