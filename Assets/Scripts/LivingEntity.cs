using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour
{
    protected float startHealth = 100f;
    protected float hp = 100f;
    public bool dead { get; protected set; } //사망상태


    public event Action OnDeath; 

    //생성자, 생성자 활용하는 방법은 없을까아?
    //public LivingEntity(float hpValue, float damageValue)
    //{
    //    hp = hpValue;
    //    damage = damageValue;
    //}

    ////기본생성자
    //public LivingEntity()
    //{

    //}


    protected virtual void OnEnable()
    {

        dead = false;
        hp = startHealth;

    }

    public virtual void OnDamage (float value)
    {
        if (dead) return;
        if (hp <= 0 && !dead)
        {
            Die();
            return;
        }

        hp -= value;
    }


    public virtual void Die()
    {
        dead = true;
        bool isDeathIsNull = OnDeath == null ? true: false;

        //첫번째 의심 포인트 :  OnDeath가 null인지 확인 
        Debug.Log($"PlayerLife : player is dead? :{dead} , IsOnDeathIsNull? : {isDeathIsNull}"); ;
        if (OnDeath != null)
        {

            OnDeath();

        }
    }

}
