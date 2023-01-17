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

    // ����� �ְ�


    public override void Update(float deltaTime)
    {
        Transform _target = context.target;
        if (_target)
        {
            if (context.isMoveLadderRight()) //��ٸ� �̵��� �켱�Ѵ�. 
            {
                
                stateMachine.ChangeState<LadderMoveState_Fox>();
            }
            else
            {
                context.Walk(); // �¿��̵�
            }

            return;
        }

        //Ÿ���� ������ Idle���·� ���ư���.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public virtual void OnExit()
    {
        _animator.SetBool(hasWalk, false);
    }


}
