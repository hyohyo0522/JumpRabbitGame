using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMoveState_Fox : State<AIFoxController> // ��ٸ� �����̵� 
{
    private Animator _animator;
    private Vector2 LadderDestination;
    private int hasClimb = Animator.StringToHash("fxClimb");
    float _moveYDirection;

    string NextStateName;


    private const float _gravity = 9.81f; // �¿��̵� �� ����Ǿ���� �߷°�

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();

    }

    public override void OnEnter()
    {
        //Physics2D.IgnoreCollision(_collider, context.groundCollider,true); >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����
        LadderDestination = context.MiddlewayPoint;

        //_animator.SetBool(hasClimb, true); >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����
        _moveYDirection =context.MiddlewayPoint.y > 0 ? 1f : -1f;
        //context.ChageGavity(0f); //�����̵������� �߷����� x >> context���� initialTouchingurrentLadder �� true�� �Ǵ� ������ �ٲٴ� �ɷ� ����

        _animator.SetBool(hasClimb, true);

    }
    public override void Update(float deltaTime)
    {

        if (context.HasArrived(LadderDestination, 0.5f)) // ��ٸ��� �̿��� ���� �̵����� ���ϴ� ��ġ�� �������ٸ� �¿��̵��ϴ� MoveState�� ��ȯ�Ѵ�.
        {

            stateMachine.ChangeState<MoveState_Fox>();
            NextStateName = "MoveState_Fox + 1"; //����!
            return;  // >> �� ���⸦ �߰����ִ� ���찡 ���ڱ� �� ������ �ݶ��̴� �����ϸ鼭 ������� ���� ������ ������ ����!!
                    //�� ���� context.MovingLadder(_moveYDirection); �� ����Ǹ鼭 �ݶ��̴� ���ð� �� ������ ����!!


        }

        //��ٸ��ݶ��̴��� ������ ����� �ٷ� ������Ʈ�� ��ȯ�ϵ��� �Ѵ�.
        context.MovingLadder(_moveYDirection); 

        if (context.initialTouchingurrentLadder) // ��ٸ��� ù ������ �̷�����°�? // �� �̰� ���� �ȵǰ� ����!!!!
        {
            if (!context.InTheLadder) // ��ٸ��� ó������ ������ ����, ��ٸ� ������ �������ٸ�(��������.), MoveState�� ��ȯ�Ѵ�.
            {
                context.IgnoreGround(false, "ladderMoveState,context.initialTouchingurrentLadder "); // �� �� Ȯ���ϰ� ���ֱ� ���� �߰���
                stateMachine.ChangeState<MoveState_Fox>();

                NextStateName = "MoveState_Fox + 2";
            }

        }

        //if (!context.SearchTarget()) // Ÿ���� �������� IdleState�� ��ȯ�Ѵ�.
        //{
        //    NextStateName = "IdleState_Fox";
        //    stateMachine.ChangeState<IdleState_Fox>(); //����!
        //}
    }

    public override void OnExit()
    {
        context.IgnoreGround(false, " + LadderMoveState,context.OnExit" + NextStateName);
        context.initialTouchingurrentLadder = false;
        context.ChageGavity(_gravity);
        _animator.SetBool(hasClimb, false); 
    }


}
