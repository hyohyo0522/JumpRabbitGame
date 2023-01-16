using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState_Fox : State<AIFoxController>
{
    [SerializeField] float _speed = 10f;

    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;


    private int hasWalk = Animator.StringToHash("fxWalk");
    private int hasClimb = Animator.StringToHash("fxClimb");


    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _rigidBody = context.GetComponent<Rigidbody2D>();
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
            if (context.MoveLadder()) //사다리 이동을 우선한다. 
            {
                _animator.SetBool(hasWalk, false); // 일단 움직여야
                _animator.SetBool(hasClimb, true); // 일단 움직여야
            }
            else
            {
                _animator.SetBool(hasClimb, false);
                _animator.SetBool(hasWalk, true); // 일단 움직여야
                context.Walk(context.target.position); // 좌우이동
            }

            return;
        }

        //타겟이 없으면 Idle상태로 돌아간다.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public virtual void OnExit()
    {
        _animator.SetBool("DoWalk", false);
    }


}
