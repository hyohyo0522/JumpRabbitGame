using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot_JumpUp : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int jumpUpValue = 5;  //�����Ϳ��� ���� �����ؼ� ���������� ����.
    WaitForSeconds delayForUse = new WaitForSeconds(0.5f); // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
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
            // ����Ƚ���� ������Ų��. 
            PlayerMovement playerMove = target.GetComponent<PlayerMovement>();
            PlayerLife PlayerLife = target.GetComponent<PlayerLife>();

            if (playerMove != null && !PlayerLife.dead)
            {
                AudioManager.instance.PlaySFX("PlayerGetItem");
                // �÷��̾� ���� Ƚ�� ������Ų��. 
                playerMove.JumpCountUp(jumpUpValue);
                Destroy(this.gameObject);
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
