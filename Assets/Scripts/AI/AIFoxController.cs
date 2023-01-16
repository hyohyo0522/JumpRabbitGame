using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;


    //플레이와 아이템 탐지 관련
    int maskground = 1 >> 6; // 땅 레이어 마스크
    int maskLadder = 1 >> 7; // 사다리 레이어 마스크
    int maskPlayer = 1 >> 8; // 플레이어 레이어 마스크 
    int maskItem = 1 >> 11; // 아이템 레이어 마스크

    [SerializeField] public Transform target; // 최종목표지점
   // [SerializeField] public Transform wayPoint; // 최종 목표로 가기 위한 중간 목표지점
    [SerializeField] float detectRadius = 50f; // 탐지거리, 아이템과 플레이어 탐지
    [SerializeField] float attackRadius = 0.5f; //플레이어 공격범위이자, 플레이어에게 도착했다고 감지하는 범위값
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

            //도착했다고 가정한다.
            return (target.position.x - transform.position.x < attackRadius && target.position.y - transform.position.y < attackRadius);
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskPlayer);
        if (playerInDetectRange.Length > 0) // 가장먼저 플레이어를 감지하고(플레이어 공격우선)
        {
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

    public void Walk(Vector2 arriveSpot) // 좌우이동
    {

        float m_HorizontalMovement = (arriveSpot.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool MoveLadder() // 사다리 오르는 함수
    {
        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, maskLadder);

        if (ladderDetect.Length > 0)
        {
            Transform ladderPosition = ladderDetect[0].transform.GetChild(0).transform; // 사다리 위치 가져오기

            if (CompareYDirection(target.position, ladderPosition.position) == 0) return MovingLadder; // 일치 안하면 함수 나오기.

            float yDirection = CompareYDirection(target.position, ladderPosition.position);


            //사다리 위를 움직이는 물리함수
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _ladderSpeed * yDirection);
            MovingLadder = true;

        }

        return MovingLadder;

    }



    // 사다리 이동할지 유무와 사다리 다 올라왔는지(target과의 Y값 검사)를 검사할 때 사용할 함수
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y값 일치 안하면 0이다.


        // 내 위치와 타겟의 위치가 일정범위이하일 때 0 도출
        // (== 사다리위치는 당연히 일정범위이상이고, 일정범위 안에 있다면 플레이어가 같은 Floor에 있다고 생각하고 0을 반환하도록 한다.)
        if (target.y - comparePosition.y < attackRadius) 
        {
            return YDirection;

        }

        // 사다리 오르내릴 때의 Y방향 도출
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
