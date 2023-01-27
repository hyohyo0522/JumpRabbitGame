using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMoveState_Fox : State<AIFoxController> // ��ٸ� �����̵� 
{
    private Animator _animator;
    private Collider2D _collider;
    private Vector2 LadderDestination;
    private int hasClimb = Animator.StringToHash("fxClimb");
    float _moveYDirection;


    private float _gravity = 9.81f; // �¿��̵� �� ����Ǿ���� �߷°�

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _collider = context.GetComponent<Collider2D>();
    }

    public override void OnEnter()
    {
        //Physics2D.IgnoreCollision(_collider, context.groundCollider,true); >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����
        LadderDestination = context.MiddlewayPoint;
        Debug.Log("�ö󰡾��� ���� LadderDestination: " + LadderDestination);

        //_animator.SetBool(hasClimb, true); >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����
        _moveYDirection =context.MiddlewayPoint.y > 0 ? 1f : -1f;
        //context.ChageGavity(0f); //�����̵������� �߷����� x >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����


    }
    public override void Update(float deltaTime)
    {

        if (context.HasArrived(LadderDestination, 0.8f)) // ��ٸ��� �̿��� ���� �̵����� ���ϴ� ��ġ�� �������ٸ� �¿��̵��ϴ� MoveState�� ��ȯ�Ѵ�.
        {

            stateMachine.ChangeState<MoveState_Fox>();
        }

        //��ٸ��ݶ��̴��� ������ ����� �ٷ� ������Ʈ�� ��ȯ�ϵ��� �Ѵ�.

        context.MovingLadder(_moveYDirection); //���⼭ �� 0���� �Ѿ�� �ɱ�??

        if (context.initialTouchingurrentLadder) // ��ٸ��� ù ������ �̷�����°�?
        {
            if (!context.InTheLadder()) // ��ٸ��� ó������ ������ ����, ��ٸ� ������ �������ٸ�, MoveState�� ��ȯ�Ѵ�.
            {
                stateMachine.ChangeState<MoveState_Fox>();
            }

        }

        if (!context.SearchTarget()) // Ÿ���� �������� IdleState�� ��ȯ�Ѵ�.
        {
            stateMachine.ChangeState<IdleState_Fox>();
        }
    }

    public override void OnExit()
    {
        Physics2D.IgnoreCollision(_collider, context.groundCollider, false);
        context.initialTouchingurrentLadder = false;
        context.ChageGavity(_gravity);
        _animator.SetBool(hasClimb, false); 
    }


}