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
        Debug.Log("여우몬 MoveState : Enter로 들어옴!!!");
        _animator.SetBool(hasWalk, true);

    }

    // 여우와 솔개


    public override void Update(float deltaTime)
    {
        Debug.Log("여우몬 MoveState : Update로 들어옴!!!");
        Transform _target = context.target ??context.SearchTarget();
        Debug.Log($"여우몬의 타겟은 !:  {_target}");
        if (_target)
        {
            if (context.isMoveLadderRight()) //사다리 이동을 우선한다. 
            {
                //여기서부터 안넘어감 ?? 왜???
                Debug.Log("여우몬 사다리 오르나요?");
                if (!context.CompareXDistance(context.MiddlewayPoint.x, 0.1f))
                { // 이동해야할 사다리 위치로 좌우이동한다.
                    context.Walk(context.MiddlewayPoint);
                    Debug.Log("여우는 사다리를 오려고 합니다. ");
                    return;
                }
                Debug.Log("여우몬 : 그냥 LadderMove로 이동하겠다고 함 ?");
                stateMachine.ChangeState<LadderMoveState_Fox>();

            }

            Debug.Log($"여우몬사다리 결과 : {context.isMoveLadderRight()}");
 
            if (!context.IsAvailableAttack)
            {
                if (context.beDamaged)
                {
                    Debug.Log("여우몬 공격을 받는 중이라 움직이지 않습니다.");
                    return; //여우가 공격을 받는 중일때에는 움직일 수 없다. 
                }
                context.Walk(_target.position); // 좌우이동
                Debug.Log("여우몬이 걸었습니다. ");
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
