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
        Debug.Log("����� MoveState : Enter�� ����!!!");
        _animator.SetBool(hasWalk, true);

    }

    // ����� �ְ�


    public override void Update(float deltaTime)
    {
        Debug.Log("����� MoveState : Update�� ����!!!");
        Transform _target = context.target ??context.SearchTarget();
        Debug.Log($"������� Ÿ���� !:  {_target}");
        if (_target)
        {
            if (context.isMoveLadderRight()) //��ٸ� �̵��� �켱�Ѵ�. 
            {
                //���⼭���� �ȳѾ ?? ��???
                Debug.Log("����� ��ٸ� ��������?");
                if (!context.CompareXDistance(context.MiddlewayPoint.x, 0.1f))
                { // �̵��ؾ��� ��ٸ� ��ġ�� �¿��̵��Ѵ�.
                    context.Walk(context.MiddlewayPoint);
                    Debug.Log("����� ��ٸ��� ������ �մϴ�. ");
                    return;
                }
                Debug.Log("����� : �׳� LadderMove�� �̵��ϰڴٰ� �� ?");
                stateMachine.ChangeState<LadderMoveState_Fox>();

            }

            Debug.Log($"������ٸ� ��� : {context.isMoveLadderRight()}");
 
            if (!context.IsAvailableAttack)
            {
                if (context.beDamaged)
                {
                    Debug.Log("����� ������ �޴� ���̶� �������� �ʽ��ϴ�.");
                    return; //���찡 ������ �޴� ���϶����� ������ �� ����. 
                }
                context.Walk(_target.position); // �¿��̵�
                Debug.Log("������� �ɾ����ϴ�. ");
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
