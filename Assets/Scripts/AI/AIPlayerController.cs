using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    protected StateMachine<AIPlayerController> stateMachine;
    public StateMachine<AIPlayerController> StateMachine => stateMachine;


    //�÷��̿� ������ Ž�� ����
    int maskPlayer = 1 >> 8; // �÷��̾� ���̾� ����ũ 
    int maskItem = 1 >> 11;
    int maskground = 1 >> 6;
    [SerializeField] public Transform target;
    [SerializeField] float detectRadius=10f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    [SerializeField] float attackRange; // ����(����) ���� �Ÿ� 

    private Animator animaotr;
    private Rigidbody2D rigidBody;



    private void Start()
    {
        stateMachine = new StateMachine<AIPlayerController>(this, new IdleState());

    }

    //������ ���� �Լ������ؼ� states�鿡�� �� �� �ְ� ����!!!
    public void Jump()
    {
        float jumpForce = 2000f;
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(0, jumpForce));
    }

    public void Walk()
    {

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskPlayer);
        if (playerInDetectRange.Length > 0) // ������� �÷��̾ �����ϰ�(�÷��̾� ���ݿ켱)
        {
            target = playerInDetectRange[0].transform;
        }
        else 
        {
            //�÷��̾� ������ ��ó ������ �����Ѵ�.
            Collider2D[] ItemInDectectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskItem);
            if (ItemInDectectRange.Length > 0)
            {
                target = ItemInDectectRange[0].transform;
            }
            else
            {
                target = null;
            }
        }

        return target;

    }

    public void GetDirection(Transform target)
    {
        Vector2 distance = target.position - transform.position;



        if (distance.y > 0) // Ÿ���� ������ ���� ��ġ�� �ִٸ�
        {
            //����ĳ��Ʈ�� �Ÿ��� ������ �� ������?
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position,distance, 10f, maskground);
            if (hitInfo)
            {

            }
        }
    }


}
