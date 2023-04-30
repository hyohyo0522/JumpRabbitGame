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
        Debug.Log("공격모드로 들어왔다!");
        targetPlayerLife = context.target.GetComponent<PlayerLife>();

        if (context.IsAvailableAttack)
        {
            if (!targetPlayerLife.attacked)
            {
                _animator?.SetTrigger(hasAttack);
                targetPlayerLife?.OnDamage(context.DamagePower);
                float xPos = Random.Range(context.transform.position.x, context.target.transform.position.x);
                float yPos = Random.Range(context.transform.position.y, context.target.transform.position.y);
                Vector2 effectPosition = new Vector2(xPos, yPos);

                GameObject pungItemPlay = MonoBehaviour.Instantiate(context.pung, effectPosition, Quaternion.identity);
                MonoBehaviour.Destroy(pungItemPlay.gameObject, 0.55f);
            }

        }
        else
        {
            Debug.Log("공격모드에서 나간다!!!");
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
