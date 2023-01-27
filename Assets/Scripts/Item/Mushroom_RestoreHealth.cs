using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_RestoreHealth : MonoBehaviour,IItem
{
    public float health = 30f;
    public float destroyDelayTime = 5f;
    public float delayForUse = 1f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
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

                    //네트워크에서 삭제해야할 때.
                    //// 모든 클라이언트에서 자신을 파괴
                    //PhotonNetwork.Destroy(gameObject);
                }

            }
        }

    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}

