using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Fox : State<AIFoxController>
{
    private Animator _animator;
    private int hasAttack = Animator.StringToHash("fxAttack");

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {

        if (context.IsAvailableAttack)
        {
            _animator?.SetTrigger(hasAttack);
        }
        else
        {
            stateMachine.ChangeState<IdleState_Fox>();
        }

    }
    public override void Update(float deltaTime)
    {


    }
}
