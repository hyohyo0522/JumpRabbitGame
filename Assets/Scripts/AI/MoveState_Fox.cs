using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Fox : State<AIFoxController>
{


    private Animator _animator;


    private int hasWalk = Animator.StringToHash("fxWalk");




    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();


    }


    public override void OnEnter()
    {

        _animator.SetBool(hasWalk, true);

    }

    // 여우와 솔개


    public override void Update(float deltaTime)
    {
        Transform _target = context.target;
        if (_target)
        {
            if (context.isMoveLadderRight()) //사다리 이동을 우선한다. 
            {
                if (!context.CompareXDistance(context.MiddlewayPoint.x, 0.1f)){ // 이동해야할 사다리 위치로 좌우이동한다.
                    context.Walk(context.MiddlewayPoint);
                    Debug.Log("여우는 사다리를 오려고 합니다. ");
                    return;
                } 
               
                stateMachine.ChangeState<LadderMoveState_Fox>();
                
            }
            if (!context.IsAvailableAttack)
            {
                if (context.beDamaged) return; //데미지 받는 중이면 움직이지 않는다. 
                context.Walk(_target.position); // 좌우이동
                return;
            }
            else // 공격가능범위에 있다면 Idle상태로 전환한다.
            {
                stateMachine.ChangeState<IdleState_Fox>();
            }
        }

        //타겟이 없으면 Idle상태로 돌아간다.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public override void OnExit()
    {
        _animator.SetBool(hasWalk, false);
    }


}
