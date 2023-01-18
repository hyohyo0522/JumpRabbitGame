using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;


    //플레이와 아이템 탐지 관련

    public LayerMask maskLadder;// 사다리 레이어 마스크
    public LayerMask targetMask;
    public Collider2D groundCollider;

    [SerializeField] public Transform target; // 최종목표지점
    [SerializeField] public Vector2 MiddlewayPoint; // 최종 목표로 가기 위한 중간 목표지점

    [SerializeField] Collider2D currentLadder; //현재 접촉하려고 하거나, 이미 접촉하고 있는 사다리
    [SerializeField] public bool initialTouchingurrentLadder;  // 사다리에 처음으로 올랐는지 인하고 있는 사다리


    public float detectRadius = 300f; // 탐지거리, 아이템과 플레이어 탐지
    [SerializeField] float attackRadius = 0.5f; //플레이어 공격범위이자, 플레이어에게 도착했다고 감지하는 범위값
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

            //도착했다고 가정한다.
            return (HasArrived(target.position, attackRadius));
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetMask);
        if (playerInDetectRange.Length > 0) // 가장먼저 플레이어를 감지하고(플레이어 공격우선)
        {
            Debug.Log("여우가 플레이어를 배열에 담았다");
            target = playerInDetectRange[0].transform;
        }
        //else // 일단 플레이어만 쫓게 만들자. 
        //{
        //    //플레이어 없으면 근처 아이템 감지한다.
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


    //워크와 무브 함수구현해서 states들에서 쓸 수 있게 하자!!!

    public void Walk(Vector2 dest) // 좌우이동
    {
        float m_HorizontalMovement = (dest.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.gravityScale = 9.81f;
        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool isMoveLadderRight() // 사다리 오르는 함수
    {

        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, maskLadder);

        if (ladderDetect.Length > 0)
        {

            Transform ladderPosition = ladderDetect[0].transform.GetChild(0).transform; // 사다리 위치 가져오기

            if (CompareYDirection(target.position, ladderPosition.position) == 0) return MovingLadder; // 일치 안하면 함수 나오기.


            MiddlewayPoint = (ladderPosition.position - transform.position) * 2.05f;
            MiddlewayPoint.x = ladderPosition.position.x;

            ////사다리 위를 움직이는 물리함수
            //_rigidBody.gravityScale = 0f;
            //_rigidBody.velocity = new Vector2(0, _ladderSpeed * yDirection);
            currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            MovingLadder = true;

        }

        return MovingLadder;

    }

    public void MovingLadder(float _moveY)
    {
        Debug.Log("여우가 사다리 위를 오르려고 시도하고 있다."+ _moveY);
        
        _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

        if (!initialTouchingurrentLadder && _collider.IsTouching(currentLadder))
        {
            initialTouchingurrentLadder = true;
        }
    }


    public bool InTheLadder()
    {
        
        // 주변에 사다리가 있는지 탐지하는 기능과는 다른 기능이다,
        // 사다리 안에 있다가 물리적인 충돌이나 기타이유로 사다리와 접촉이 끊어졌는지 확인
        // 접촉이 끊어졌다면 바로 LadderState에서 MoveState로 전환하게 할 것이다.

        //기존에 접촉하고 있는 사다리와 끊어졌는지를 확인할 것이다.
        bool result = false;
        result = _collider.IsTouching(currentLadder);

        return result;

    }



    public void ChageGavity(float value)
    {
        _rigidBody.gravityScale = value;
    }


    // 사다리 이동할지 유무와 사다리 다 올라왔는지(target과의 Y값 검사)를 검사할 때 사용할 함수
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y값 일치 안하면 0이다.


        // 내 위치와 타겟의 위치가 일정범위이하일 때 0 도출
        // (== 사다리위치는 당연히 일정범위이상이고, 일정범위 안에 있다면 플레이어가 같은 Floor에 있다고 생각하고 0을 반환하도록 한다.)
        //if (target.y - comparePosition.y < 0.1f) 
        //{
        //    return YDirection;
        
        //}

        // 사다리 오르내릴 때의 Y방향 도출
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

        Debug.Log("도착하였나요? " + arrived);
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
