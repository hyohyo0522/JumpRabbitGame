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
            //Debug.Log("��弦���� : "+ collision.gameObject.name);
            PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
            if (!attackingPlayers.isGrounded) // ���� ���� ������ �۵����� HeadShot ȿ���� �۵����� �ʴ´�. 
            {
                GameObject isPlayer = collision.gameObject;

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)Tr.position; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);

                ///���ݴ������� �������� ���س��� �޼���.
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
