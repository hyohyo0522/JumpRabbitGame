using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot_JumpUp : MonoBehaviour,IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int jumpUpValue = 5;  //에디터에서 수를 조정해서 범용적으로 쓰자.
    [SerializeField] float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
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
            // 점프횟수를 증가시킨다. 
            PlayerMovement playerMove = target.GetComponent<PlayerMovement>();
            PlayerLife PlayerLife = target.GetComponent<PlayerLife>();

            if (playerMove != null && !PlayerLife.dead)
            {
                AudioManager.instance.PlaySFX("PlayerGetItem");
                // 플레이어 점프 횟수 증가시킨다. 
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
