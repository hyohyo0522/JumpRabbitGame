using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState_Fox : State<AIFoxController>
{
    private Animator animator;

    protected int hasMove = Animator.StringToHash("fxWalk");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
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
                stateMachine.ChangeState<MoveState_Fox>();
            }

        }
        else
        {
            //�������� �����̴� �� �����غ��� �ʹ�.
        }
    }
}
