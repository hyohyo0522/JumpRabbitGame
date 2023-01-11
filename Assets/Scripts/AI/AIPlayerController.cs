using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    protected StateMachine<AIPlayerController> stateMachine;
    public StateMachine<AIPlayerController> StateMachine => stateMachine;


    //플레이와 아이템 탐지 관련
    int maskPlayer = 1 >> 8; // 플레이어 레이어 마스크 
    int maskItem = 1 >> 11;
    int maskground = 1 >> 6;
    [SerializeField] public Transform target;
    [SerializeField] float detectRadius=10f; // 탐지거리, 아이템과 플레이어 탐지
    [SerializeField] float attackRange; // 점프(공격) 가능 거리 

    private Animator animaotr;
    private Rigidbody2D rigidBody;



    private void Start()
    {
        stateMachine = new StateMachine<AIPlayerController>(this, new IdleState());

    }

    //점프와 무브 함수구현해서 states들에서 쓸 수 있게 하자!!!
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
        if (playerInDetectRange.Length > 0) // 가장먼저 플레이어를 감지하고(플레이어 공격우선)
        {
            target = playerInDetectRange[0].transform;
        }
        else 
        {
            //플레이어 없으면 근처 아이템 감지한다.
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



        if (distance.y > 0) // 타겟이 나보다 높은 위치에 있다면
        {
            //레이캐스트로 거리를 감지할 수 있을까?
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position,distance, 10f, maskground);
            if (hitInfo)
            {

            }
        }
    }


}
