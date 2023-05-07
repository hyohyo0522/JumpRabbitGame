using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_RestoreHealth : MonoBehaviour,IItem
{
    [SerializeField] float health = 30f; // 인스펙터에서 조정해서 범용적으로 프리팹 만들어 사용하자.
    [SerializeField] float destroyDelayTime = 5f;
    WaitForSeconds delayForUse = new WaitForSeconds(1f); // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;


    private void OnEnable()
    {
        Destroy(this.gameObject, destroyDelayTime);
        StartCoroutine(makeDelay());

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
                    AudioManager.instance.PlaySFX("PlayerGetHeal");
                    playerLife.RestoreHealth(health);

                    Destroy(this.gameObject);

                }

            }
        }

    }

    IEnumerator makeDelay()
    {
        afterDelay = false;
        yield return delayForUse;
        afterDelay = true;
    }




    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}

