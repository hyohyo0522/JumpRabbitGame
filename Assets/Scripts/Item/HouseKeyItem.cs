using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseKeyItem : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int payMoney = 1000;
    WaitForSeconds delayForUse = new WaitForSeconds(0.5f); // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;
    public bool isPaid { get; private set; } // �÷��̾�꿡�� ������ ���� �� Ȯ����


    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);

        StartCoroutine(makeDelay());

    }



    public void Use(GameObject target)
    {
        if (afterDelay)
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
