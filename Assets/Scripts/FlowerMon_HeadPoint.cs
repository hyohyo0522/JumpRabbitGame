using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMon_HeadPoint : MonoBehaviour
{
    Transform Tr;
    const float headShotBouncePower = 2000f;
    FlowerEnemy _myflower;

    private void OnEnable()
    {
        Tr = GetComponent<Transform>();
        _myflower = GetComponentInParent<FlowerEnemy>();
        //_myflower.OnDeath += 람다식 말고 다른 거 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
            if (!attackingPlayers.isGrounded) //플레이어가 땅에 있을 때에는 작동하지 HeadShot 효과가 작동하지 않는다. 
            {

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)Tr.position; // 플레이어가 튕겨야하므로, 플레이어방향 - 현재 몬스터 방향으로 함 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);

                _myflower.GetHeadShot();

                // 헤드샷 당하고 나서 죽었는지 감지 : 람다식으로 했어야 하나? 
                if (_myflower.dead)
                {
                    PlayerLife attackingPlayersLife = collision.gameObject.GetComponent<PlayerLife>();
                    OndeathEvent(attackingPlayersLife);
                }


            }

        }
    }

    void OndeathEvent(PlayerLife whoAttacking)
    {
        whoAttacking.UpgateFlowerKillUI();
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
}
