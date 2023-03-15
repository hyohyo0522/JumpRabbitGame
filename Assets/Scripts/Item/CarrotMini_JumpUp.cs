using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotMini_JumpUp : MonoBehaviour, IItem
{
    public float destroyDelayTime = 5f;
    public int jumpUpValue = 1;  //에디터에서 수를 조정해서 범용적으로 쓰자.
    public float delayForUse = 1.0f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;


    //Ignorelayer사용위함
    int  playerMaskInt;



    private void Start()
    {
        playerMaskInt= LayerMask.NameToLayer("Player");
        StartCoroutine("makeDelay");
        Destroy(this.gameObject, destroyDelayTime);
    }

    IEnumerator makeDelay()
    {
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, playerMaskInt, true);
        afterDelay = false;
        yield return new WaitForSeconds(delayForUse);
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, playerMaskInt, true);
        afterDelay = true;
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

                //네트워크에서 삭제해야할 때.
                //// 모든 클라이언트에서 자신을 파괴
                //PhotonNetwork.Destroy(gameObject);
            }
        }

    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}
