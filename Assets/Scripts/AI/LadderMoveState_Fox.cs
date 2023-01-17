using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderMoveState_Fox : State<AIFoxController> // ��ٸ� �����̵� 
{
    private Animator _animator;
    private Vector2 LadderDestination;
    private int hasClimb = Animator.StringToHash("fxClimb");
    float _moveYDirection;

    private float _gravity = 9.81f; // �¿��̵� �� ����Ǿ���� �߷°�

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();

    }

    public virtual void OnEnter()
    {
        _animator.SetBool(hasClimb, true);
        LadderDestination = context.MiddlewayPoint;
        _moveYDirection = LadderDestination.y > 0 ? 1f : -1f;
        context.ChageGavity(0f); //�����̵������� �߷����� x 


    }
    public override void Update(float deltaTime)
    {
        if (context.HasArrived(LadderDestination, 1f))
        {

            stateMachine.ChangeState<MoveState_Fox>();
        }
        context.MovingLadder(_moveYDirection); //���⼭ �� 0���� �Ѿ�� �ɱ�??

    }

    public virtual void OnExit()
    {

        context.ChageGavity(_gravity);
        _animator.SetBool(hasClimb, false); 
    }


}
