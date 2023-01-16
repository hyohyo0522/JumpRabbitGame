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

    // ����� �ְ�


    public override void Update(float deltaTime)
    {
        Transform _target = context.target;
        if (_target)
        {
            if (context.MoveLadder()) //��ٸ� �̵��� �켱�Ѵ�. 
            {
                _animator.SetBool(hasWalk, false); // �ϴ� ��������
                _animator.SetBool(hasClimb, true); // �ϴ� ��������
            }
            else
            {
                _animator.SetBool(hasClimb, false);
                _animator.SetBool(hasWalk, true); // �ϴ� ��������
                context.Walk(context.target.position); // �¿��̵�
            }

            return;
        }

        //Ÿ���� ������ Idle���·� ���ư���.
        stateMachine.ChangeState<IdleState_Fox>();

    }

    public virtual void OnExit()
    {
        _animator.SetBool("DoWalk", false);
    }


}
