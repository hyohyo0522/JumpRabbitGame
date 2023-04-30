using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotMini_JumpUp : MonoBehaviour, IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int jumpUpValue = 1;  //�����Ϳ��� ���� �����ؼ� ���������� ����.
    [SerializeField] float delayForUse = 1.0f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;


    //Ignorelayer�������
    int  playerMaskInt;



    private void Start()
    {
        playerMaskInt= LayerMask.NameToLayer("Player");
        Destroy(this.gameObject, destroyDelayTime);
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, playerMaskInt, true);

        float timeAfterInstantiate = 0;
        while (!afterDelay)
        {
            timeAfterInstantiate += Time.deltaTime;
            if (timeAfterInstantiate > delayForUse)
            {
                afterDelay = true;
                Physics2D.IgnoreLayerCollision(this.gameObject.layer, playerMaskInt, false);
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
