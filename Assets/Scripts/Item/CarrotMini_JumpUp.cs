using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotMini_JumpUp : MonoBehaviour, IItem
{
    public float destroyDelayTime = 5f;
    public int jumpUpValue = 1;  //�����Ϳ��� ���� �����ؼ� ���������� ����.
    public float delayForUse = 1.0f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;


    //Ignorelayer�������
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
            // ����Ƚ���� ������Ų��. 
            PlayerMovement playerMove = target.GetComponent<PlayerMovement>();
            PlayerLife PlayerLife = target.GetComponent<PlayerLife>();

            if (playerMove != null && !PlayerLife.dead)
            {
                AudioManager.instance.PlaySFX("PlayerGetItem");
                // �÷��̾� ���� Ƚ�� ������Ų��. 
                playerMove.JumpCountUp(jumpUpValue);
                Destroy(this.gameObject);

                //��Ʈ��ũ���� �����ؾ��� ��.
                //// ��� Ŭ���̾�Ʈ���� �ڽ��� �ı�
                //PhotonNetwork.Destroy(gameObject);
            }
        }

    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}
