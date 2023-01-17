using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMoveState_Fox : State<AIFoxController> // 사다리 수직이동 
{
    private Animator _animator;
    private Vector2 LadderDestination;
    private int hasClimb = Animator.StringToHash("fxClimb");
    float _moveYDirection;

    private float _gravity = 9.81f; // 좌우이동 때 적용되어야할 중력값

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();

    }

    public virtual void OnEnter()
    {
        _animator.SetBool(hasClimb, true);
        LadderDestination = context.MiddlewayPoint;
        _moveYDirection = LadderDestination.y > 0 ? 1f : -1f;
        context.ChageGavity(0f); //상하이동때에는 중력적용 x 


    }
    public override void Update(float deltaTime)
    {
        if (context.HasArrived(LadderDestination, 1f))
        {

            stateMachine.ChangeState<MoveState_Fox>();
        }
        context.MovingLadder(_moveYDirection); //여기서 왜 0으로 넘어가는 걸까??

    }

    public virtual void OnExit()
    {

        context.ChageGavity(_gravity);
        _animator.SetBool(hasClimb, false); 
    }


}
