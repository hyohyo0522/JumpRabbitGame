using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Fox : State<AIFoxController>
{
    private Animator _animator;
    protected int hasMove = Animator.StringToHash("fxWalk");

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
    }

    // Start is called before the first frame update

    public override void OnEnter()
    {
        Debug.Log("Idle�� ���ƿ� ");
        _animator?.SetBool(hasMove, false);
    }

    // Update is called once per frame
    public override void Update(float deltaTime)
    {
        Transform _target = context.SearchTarget();

        if (_target) // Ÿ���� ������ ������Ʈ�� ��ȯ�Ѵ�.
        {
            if (context.IsAvailableAttack)
            {
                stateMachine.ChangeState<AttackState_Fox>();
            }
            else
            {
                Debug.Log("����� MoveState�� �̵�!!!");
                stateMachine.ChangeState<MoveState_Fox>();
            }

        }
        else
        {
            //�������� �����̴� �� �����غ��� �ʹ�.
        }
    }

    public override void OnExit()
    {

    }
}
