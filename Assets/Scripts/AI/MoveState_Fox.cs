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

    // ����� �ְ�


    public override void Update(float deltaTime)
    {
        Transform _target = context.target;
        if (_target)
        {
            if (context.isMoveLadderRight()) //��ٸ� �̵��� �켱�Ѵ�. 
            {
                if (!context.CompareXDistance(context.MiddlewayPoint.x, 0.1f)){ // �̵��ؾ��� ��ٸ� ��ġ�� �¿��̵��Ѵ�.
                    context.Walk(context.MiddlewayPoint);
                    Debug.Log("����� ��ٸ��� ������ �մϴ�. ");
                    return;
                } 
               
                stateMachine.ChangeState<LadderMoveState_Fox>();
                
            }
            if (!context.IsAvailableAttack)
            {
                if (context.beDamaged) return; //������ �޴� ���̸� �������� �ʴ´�. 
                context.Walk(_target.position); // �¿��̵�
                return;
            }
            else // ���ݰ��ɹ����� �ִٸ� Idle���·� ��ȯ�Ѵ�.
            {
                stateMachine.ChangeState<IdleState_Fox>();
            }
        }

        //Ÿ���� ������ Idle���·� ���ư���.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public override void OnExit()
    {
        _animator.SetBool(hasWalk, false);
    }


}
