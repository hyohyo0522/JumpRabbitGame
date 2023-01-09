using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    protected StateMachine<AIPlayerController> stateMachine;
    public StateMachine<AIPlayerController> StateMachine => stateMachine;

    public LayerMask targetMask;
    public Transform target;
    public float detectRadius; // 탐지거리, 아이템과 플레이어 탐지
    public float attackRange; // 점프(공격) 가능 거리 

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
}
