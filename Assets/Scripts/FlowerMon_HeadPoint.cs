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
        //_myflower.OnDeath += ���ٽ� ���� �ٸ� �� 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
            if (!attackingPlayers.isGrounded) //�÷��̾ ���� ���� ������ �۵����� HeadShot ȿ���� �۵����� �ʴ´�. 
            {

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)Tr.position; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);

                _myflower.GetHeadShot();

                // ��弦 ���ϰ� ���� �׾����� ���� : ���ٽ����� �߾�� �ϳ�? 
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
