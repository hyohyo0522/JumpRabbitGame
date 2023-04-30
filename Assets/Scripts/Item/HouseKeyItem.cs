using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseKeyItem : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int payMoney = 1000;
    [SerializeField] float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;
    public bool isPaid { get; private set; } // 플레이어무브에서 아이템 먹을 때 확인함


    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);

        float timeAfterInstantiate = 0;
        while (!afterDelay)
        {
            timeAfterInstantiate += Time.deltaTime;
            if (timeAfterInstantiate > delayForUse)
            {
                afterDelay = true;
            }
        }
    }



    public void Use(GameObject target)
    {

        PlayerLife playerLife = target.GetComponent<PlayerLife>();
        if (playerLife.UseMoneyForKey(payMoney)) // 열쇠 먹을 돈이 있다.
        {
            AudioManager.instance.PlaySFX("PlayerGeKey");
            isPaid = true;


        }
        else //열쇠 먹을 돈이 없다. 
        {
            AudioManager.instance.PlaySFX("PlayerNoKey");
            UIManager.instance.UrgentGameTip(UIManager.KeyDestroyed);
            isPaid = false;
        }

        Destroy(this.gameObject);

    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }

}
