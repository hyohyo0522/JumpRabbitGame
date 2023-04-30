using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMoveState_Fox : State<AIFoxController> // 사다리 수직이동 
{
    private Animator _animator;
    private Vector2 LadderDestination;
    private int hasClimb = Animator.StringToHash("fxClimb");
    float _moveYDirection;

    string NextStateName;


    private const float _gravity = 9.81f; // 좌우이동 때 적용되어야할 중력값

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();

    }

    public override void OnEnter()
    {
        //Physics2D.IgnoreCollision(_collider, context.groundCollider,true); >> context에서 initialTouchingurrentLadder 가 true가 되는 시점에 바꾸는 걸로 변경
        LadderDestination = context.MiddlewayPoint;

        //_animator.SetBool(hasClimb, true); >> context에서 initialTouchingurrentLadder 가 true가 되는 시점에 바꾸는 걸로 변경
        _moveYDirection =context.MiddlewayPoint.y > 0 ? 1f : -1f;
        //context.ChageGavity(0f); //상하이동때에는 중력적용 x >> context에서 initialTouchingurrentLadder 가 true가 되는 시점에 바꾸는 걸로 변경

        _animator.SetBool(hasClimb, true);

    }
    public override void Update(float deltaTime)
    {

        if (context.HasArrived(LadderDestination, 0.5f)) // 사다리를 이용해 상하 이동으로 원하는 위치에 도달헀다면 좌우이동하는 MoveState로 전환한다.
        {

            stateMachine.ChangeState<MoveState_Fox>();
            NextStateName = "MoveState_Fox + 1"; //여기!
            return;  // >> ★ 여기를 추가해주니 여우가 갑자기 땅 밑으로 콜라이더 무시하면서 사라지는 현상 개선된 것으로 보임!!
                    //이 이후 context.MovingLadder(_moveYDirection); 가 실행되면서 콜라이더 무시가 된 것으로 보임!!


        }

        //사다리콜라이더와 접촉이 끊기면 바로 스테이트를 전환하도록 한다.
        context.MovingLadder(_moveYDirection); 

        if (context.initialTouchingurrentLadder) // 사다리에 첫 접촉이 이루어졌는가? // ★ 이거 지금 안되고 있음!!!!
        {
            if (!context.InTheLadder) // 사다리에 처음으로 접촉한 이후, 사다리 접촉이 끊어졌다면(떨어졌다.), MoveState로 전환한다.
            {
                context.IgnoreGround(false, "ladderMoveState,context.initialTouchingurrentLadder "); // 좀 더 확실하게 해주기 위해 추가함
                stateMachine.ChangeState<MoveState_Fox>();

                NextStateName = "MoveState_Fox + 2";
            }

        }

        //if (!context.SearchTarget()) // 타겟이 없어지면 IdleState로 전환한다.
        //{
        //    NextStateName = "IdleState_Fox";
        //    stateMachine.ChangeState<IdleState_Fox>(); //여기!
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
