using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseKeyItem : MonoBehaviour,IItem
{
    public float destroyDelayTime = 5f;
    int payMoney = 1000;
    public float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
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
        if (playerLife.UseMoney(payMoney))
        {
            Destroy(this.gameObject);
        }



    }

}
