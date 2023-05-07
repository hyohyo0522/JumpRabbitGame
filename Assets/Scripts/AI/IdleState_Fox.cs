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
        Debug.Log("Idle로 돌아옴 ");
        _animator?.SetBool(hasMove, false);
    }

    // Update is called once per frame
    public override void Update(float deltaTime)
    {
        Transform _target = context.SearchTarget();

        if (_target) // 타겟이 있으면 스테이트를 전환한다.
        {
            if (context.IsAvailableAttack)
            {
                stateMachine.ChangeState<AttackState_Fox>();
            }
            else
            {
                Debug.Log("여우몬 MoveState로 이동!!!");
                stateMachine.ChangeState<MoveState_Fox>();
            }

        }
        else
        {
            //랜덤으로 움직이는 걸 구현해보고 싶다.
        }
    }

    public override void OnExit()
    {

    }
}
