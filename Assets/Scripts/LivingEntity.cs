using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour
{
    protected float startHealth = 100f;
    protected float hp = 100f;
    public bool dead { get; protected set; } //�������


    public event Action OnDeath;

    //������, ������ Ȱ���ϴ� ����� �������?
    //public LivingEntity(float hpValue, float damageValue)
    //{
    //    hp = hpValue;
    //    damage = damageValue;
    //}

    ////�⺻������
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
        if (OnDeath != null)
        {
            OnDeath();

        }
    }

}
