using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot_JumpUp : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int jumpUpValue = 5;  //�����Ϳ��� ���� �����ؼ� ���������� ����.
    [SerializeField] float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;




    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);

        float timeAfterInstantiate = 0;
        while(!afterDelay)
        {
            timeAfterInstantiate += Time.deltaTime;
            if(timeAfterInstantiate> delayForUse)
            {
                afterDelay = true;
            }
        }
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

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }

}
