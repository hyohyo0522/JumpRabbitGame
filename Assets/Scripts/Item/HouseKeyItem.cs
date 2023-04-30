using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseKeyItem : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int payMoney = 1000;
    [SerializeField] float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;
    public bool isPaid { get; private set; } // �÷��̾�꿡�� ������ ���� �� Ȯ����


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
        if (playerLife.UseMoneyForKey(payMoney)) // ���� ���� ���� �ִ�.
        {
            AudioManager.instance.PlaySFX("PlayerGeKey");
            isPaid = true;


        }
        else //���� ���� ���� ����. 
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
