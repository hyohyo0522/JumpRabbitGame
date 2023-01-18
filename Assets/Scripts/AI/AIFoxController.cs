using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;


    //�÷��̿� ������ Ž�� ����

    public LayerMask maskLadder;// ��ٸ� ���̾� ����ũ
    public LayerMask targetMask;
    public Collider2D groundCollider;

    [SerializeField] public Transform target; // ������ǥ����
    [SerializeField] public Vector2 MiddlewayPoint; // ���� ��ǥ�� ���� ���� �߰� ��ǥ����

    [SerializeField] Collider2D currentLadder; //���� �����Ϸ��� �ϰų�, �̹� �����ϰ� �ִ� ��ٸ�
    [SerializeField] public bool initialTouchingurrentLadder;  // ��ٸ��� ó������ �ö����� ���ϰ� �ִ� ��ٸ�


    public float detectRadius = 300f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    [SerializeField] float attackRadius = 0.5f; //�÷��̾� ���ݹ�������, �÷��̾�� �����ߴٰ� �����ϴ� ������
    [SerializeField] float ladderDetectRange = 3f;
    [SerializeField] float _speed = 4f;
    [SerializeField] float _ladderSpeed = 7f;



    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;



    private void Start()
    {
        stateMachine = new StateMachine<AIFoxController>(this, new IdleState_Fox());
        stateMachine.AddState(new MoveState_Fox());
        stateMachine.AddState(new AttackState_Fox());
        stateMachine.AddState(new LadderMoveState_Fox());

        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

    }

    private void Update()
    {
        stateMachine.Update(Time.deltaTime);

        Debug.Log(stateMachine.CurrentState);


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
            return (HasArrived(target.position, attackRadius));
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetMask);
        if (playerInDetectRange.Length > 0) // ������� �÷��̾ �����ϰ�(�÷��̾� ���ݿ켱)
        {
            Debug.Log("���찡 �÷��̾ �迭�� ��Ҵ�");
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

    public void Walk(Vector2 dest) // �¿��̵�
    {
        float m_HorizontalMovement = (dest.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.gravityScale = 9.81f;
        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool isMoveLadderRight() // ��ٸ� ������ �Լ�
    {

        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, maskLadder);

        if (ladderDetect.Length > 0)
        {

            Transform ladderPosition = ladderDetect[0].transform.GetChild(0).transform; // ��ٸ� ��ġ ��������

            if (CompareYDirection(target.position, ladderPosition.position) == 0) return MovingLadder; // ��ġ ���ϸ� �Լ� ������.


            MiddlewayPoint = (ladderPosition.position - transform.position) * 2.05f;
            MiddlewayPoint.x = ladderPosition.position.x;

            ////��ٸ� ���� �����̴� �����Լ�
            //_rigidBody.gravityScale = 0f;
            //_rigidBody.velocity = new Vector2(0, _ladderSpeed * yDirection);
            currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            MovingLadder = true;

        }

        return MovingLadder;

    }

    public void MovingLadder(float _moveY)
    {
        Debug.Log("���찡 ��ٸ� ���� �������� �õ��ϰ� �ִ�."+ _moveY);
        
        _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

        if (!initialTouchingurrentLadder && _collider.IsTouching(currentLadder))
        {
            initialTouchingurrentLadder = true;
        }
    }


    public bool InTheLadder()
    {
        
        // �ֺ��� ��ٸ��� �ִ��� Ž���ϴ� ��ɰ��� �ٸ� ����̴�,
        // ��ٸ� �ȿ� �ִٰ� �������� �浹�̳� ��Ÿ������ ��ٸ��� ������ ���������� Ȯ��
        // ������ �������ٸ� �ٷ� LadderState���� MoveState�� ��ȯ�ϰ� �� ���̴�.

        //������ �����ϰ� �ִ� ��ٸ��� ������������ Ȯ���� ���̴�.
        bool result = false;
        result = _collider.IsTouching(currentLadder);

        return result;

    }



    public void ChageGavity(float value)
    {
        _rigidBody.gravityScale = value;
    }


    // ��ٸ� �̵����� ������ ��ٸ� �� �ö�Դ���(target���� Y�� �˻�)�� �˻��� �� ����� �Լ�
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y�� ��ġ ���ϸ� 0�̴�.


        // �� ��ġ�� Ÿ���� ��ġ�� �������������� �� 0 ����
        // (== ��ٸ���ġ�� �翬�� ���������̻��̰�, �������� �ȿ� �ִٸ� �÷��̾ ���� Floor�� �ִٰ� �����ϰ� 0�� ��ȯ�ϵ��� �Ѵ�.)
        //if (target.y - comparePosition.y < 0.1f) 
        //{
        //    return YDirection;
        
        //}

        // ��ٸ� �������� ���� Y���� ����
        if(target.y-transform.position.y<0 && comparePosition.y - transform.position.y < 0) { YDirection=-1; }
        if(target.y - transform.position.y > 0 && comparePosition.y - transform.position.y > 0) { YDirection = 1; }

        return YDirection;


    }

    public bool CompareXDistance(float dest, float range)
    {
        bool result = false;
        if (transform.position.x - dest < range)
        {
            result = true;
        }
        return result;
    }

    public bool HasArrived(Vector2 destination, float minDestinationRadius)
    {
        bool arrived = false;
        if(transform.position.x - destination.x< minDestinationRadius&& transform.position.y - destination.y < minDestinationRadius)
        {
            arrived = true;
        }

       // if(destination.x-transform.position.x<minDestinationRadius &&
      //      destination.y - transform.position.y < minDestinationRadius)
       // {
      //      arrived = true;
       // }

        Debug.Log("�����Ͽ�����? " + arrived);
        return arrived;
           
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ladderDetectRange);
    }



}
