using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_Fox : State<AIFoxController>
{
    private Animator _animator;
    private int hasAttack = Animator.StringToHash("fxAttack");
    PlayerLife targetPlayerLife;

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        Debug.Log("���ݸ��� ���Դ�!");
        targetPlayerLife = context.target.GetComponent<PlayerLife>();

        if (context.IsAvailableAttack)
        {
            _animator?.SetTrigger(hasAttack);
            targetPlayerLife?.OnDamage(context.damagePower);

        }
        else
        {
            Debug.Log("���ݸ�忡�� ������!!!");
            stateMachine.ChangeState<IdleState_Fox>();

        }

    }
    public override void Update(float deltaTime)
    {


    }

    public override void OnExit()
    {

    }
}
