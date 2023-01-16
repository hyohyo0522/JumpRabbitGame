using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;


    //�÷��̿� ������ Ž�� ����
    int maskground = 1 >> 6; // �� ���̾� ����ũ
    int maskLadder = 1 >> 7; // ��ٸ� ���̾� ����ũ
    int maskPlayer = 1 >> 8; // �÷��̾� ���̾� ����ũ 
    int maskItem = 1 >> 11; // ������ ���̾� ����ũ

    [SerializeField] public Transform target; // ������ǥ����
   // [SerializeField] public Transform wayPoint; // ���� ��ǥ�� ���� ���� �߰� ��ǥ����
    [SerializeField] float detectRadius = 50f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    [SerializeField] float attackRadius = 0.5f; //�÷��̾� ���ݹ�������, �÷��̾�� �����ߴٰ� �����ϴ� ������
    [SerializeField] float ladderDetectRange = 0.5f;
    [SerializeField] float _speed = 10f;
    [SerializeField] float _ladderSpeed = 7f;


    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;



    private void Start()
    {
        stateMachine = new StateMachine<AIFoxController>(this, new IdleState_Fox());
        stateMachine.AddState(new MoveState_Fox());
        stateMachine.AddState(new AttackState_Fox());

    }




    public bool IsAvailableAttack
    {
        get
        {
            if (!target)
            {
                return false;
            }

            //�����ߴٰ� �����Ѵ�.
            return (target.position.x - transform.position.x < attackRadius && target.position.y - transform.position.y < attackRadius);
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskPlayer);
        if (playerInDetectRange.Length > 0) // ������� �÷��̾ �����ϰ�(�÷��̾� ���ݿ켱)
        {
            target = playerInDetectRange[0].transform;
        }
        //else // �ϴ� �÷��̾ �Ѱ� ������. 
        //{
        //    //�÷��̾� ������ ��ó ������ �����Ѵ�.
        //    Collider2D[] ItemInDectectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskItem);
        //    if (ItemInDectectRange.Length > 0)
        //    {
        //        target = ItemInDectectRange[0].transform;
        //    }
        //    else
        //    {
        //        target = null;
        //    }
        //}

        return target;

    }

    public Vector2 getDistance()
    {
        Vector2 distance = target.position - transform.position;
        return distance;
    }


    //��ũ�� ���� �Լ������ؼ� states�鿡�� �� �� �ְ� ����!!!

    public void Walk(Vector2 arriveSpot) // �¿��̵�
    {

        float m_HorizontalMovement = (arriveSpot.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool MoveLadder() // ��ٸ� ������ �Լ�
    {
        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, maskLadder);

        if (ladderDetect.Length > 0)
        {
            Transform ladderPosition = ladderDetect[0].transform.GetChild(0).transform; // ��ٸ� ��ġ ��������

            if (CompareYDirection(target.position, ladderPosition.position) == 0) return MovingLadder; // ��ġ ���ϸ� �Լ� ������.

            float yDirection = CompareYDirection(target.position, ladderPosition.position);


            //��ٸ� ���� �����̴� �����Լ�
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _ladderSpeed * yDirection);
            MovingLadder = true;

        }

        return MovingLadder;

    }



    // ��ٸ� �̵����� ������ ��ٸ� �� �ö�Դ���(target���� Y�� �˻�)�� �˻��� �� ����� �Լ�
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y�� ��ġ ���ϸ� 0�̴�.


        // �� ��ġ�� Ÿ���� ��ġ�� �������������� �� 0 ����
        // (== ��ٸ���ġ�� �翬�� ���������̻��̰�, �������� �ȿ� �ִٸ� �÷��̾ ���� Floor�� �ִٰ� �����ϰ� 0�� ��ȯ�ϵ��� �Ѵ�.)
        if (target.y - comparePosition.y < attackRadius) 
        {
            return YDirection;

        }

        // ��ٸ� �������� ���� Y���� ����
        if(target.y-transform.position.y<0 && comparePosition.y - transform.position.y < 0) { YDirection=-1; }
        if(target.y - transform.position.y > 0 && comparePosition.y - transform.position.y > 0) { YDirection = 1; }

        return YDirection;


    }

    private void OnDrawGizmos2D()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
