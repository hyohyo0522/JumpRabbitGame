using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Fox : State<AIFoxController>
{


    private Animator _animator;
    private SpriteRenderer _spriteRenderer;


    private int hasWalk = Animator.StringToHash("fxWalk");




    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();


    }


    public virtual void OnEnter()
    {

        _animator?.SetBool(hasWalk, true);



    }

    // 여우와 솔개


    public override void Update(float deltaTime)
    {
        Transform _target = context.target;
        if (_target)
        {
            if (context.isMoveLadderRight()) //사다리 이동을 우선한다. 
            {
                
                stateMachine.ChangeState<LadderMoveState_Fox>();
            }
            else
            {
                context.Walk(); // 좌우이동
            }

            return;
        }

        //타겟이 없으면 Idle상태로 돌아간다.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public virtual void OnExit()
    {
        _animator.SetBool(hasWalk, false);
    }


}
