using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseKeyItem : MonoBehaviour,IItem
{
    public float destroyDelayTime = 5f;
    int payMoney = 1000;
    public float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
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

        PlayerLife playerLife = target.GetComponent<PlayerLife>();
        if (playerLife.UseMoneyForKey(payMoney))
        {
            Destroy(this.gameObject);
        }



    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }

}
