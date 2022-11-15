using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMon_HeadPoint : MonoBehaviour
{
    Transform Tr;
    float headShotBouncePower = 2500f;

    private void OnEnable()
    {
        Tr = GetComponent<Transform>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //_tfGenPos = transform.GetChild(0);??
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("헤드샷성공 : "+ collision.gameObject.name);
            PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
            if (!attackingPlayers.isGrounded) // 땅에 있을 때에는 작동하지 HeadShot 효과가 작동하지 않는다. 
            {

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)Tr.position; // 플레이어가 튕겨야하므로, 플레이어방향 - 현재 몬스터 방향으로 함 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);

                ///공격당했을때 아이템을 토해내는 메서드.
                FlowerEnemy _myflowerMon = GetComponentInParent<FlowerEnemy>();
                _myflowerMon.GetHeadShot();
                if (_myflowerMon.dead)
                {
                    PlayerLife attackingPlayersLife = collision.gameObject.GetComponent<PlayerLife>();
                    attackingPlayersLife.UpgateFlowerKillUI();
                    if(this.gameObject != null)
                    {
                        Destroy(this.gameObject);
                    }
                }

            }

        }
    }
}
