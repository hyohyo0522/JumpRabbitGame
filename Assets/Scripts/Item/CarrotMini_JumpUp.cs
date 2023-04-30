using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotMini_JumpUp : MonoBehaviour, IItem
{
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] int jumpUpValue = 1;  //에디터에서 수를 조정해서 범용적으로 쓰자.
    [SerializeField] float delayForUse = 1.0f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;


    //Ignorelayer사용위함
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
