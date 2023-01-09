using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    protected StateMachine<AIPlayerController> stateMachine;
    public StateMachine<AIPlayerController> StateMachine => stateMachine;

    public LayerMask targetMask;
    public Transform target;
    public float detectRadius; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    public float attackRange; // ����(����) ���� �Ÿ� 

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
}
