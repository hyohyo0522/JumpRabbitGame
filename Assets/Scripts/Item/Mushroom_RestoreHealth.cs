using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_RestoreHealth : MonoBehaviour,IItem
{
    [SerializeField] float health = 30f; // �ν����Ϳ��� �����ؼ� ���������� ������ ����� �������.
    [SerializeField] float destroyDelayTime = 5f;
    [SerializeField] float delayForUse = 1f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;

    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);

        float timeAfterInstantiate = 0;
        while (!afterDelay)
        {
            timeAfterInstantiate += Time.deltaTime;
            if (timeAfterInstantiate > delayForUse)
            {
                afterDelay = true;
            }
        }
    }

    public void Use(GameObject target)
    {
        if (afterDelay)
        {
            PlayerLife playerLife = target.GetComponent<PlayerLife>();
            if (playerLife != null && !playerLife.dead)
            {
                if (!playerLife.isFullHeath())
                {
                    AudioManager.instance.PlaySFX("PlayerGetHeal");
                    playerLife.RestoreHealth(health);

                    Destroy(this.gameObject);

                }

            }
        }

    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}

