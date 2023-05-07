using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_RestoreHealth : MonoBehaviour,IItem
{
    [SerializeField] float health = 30f; // �ν����Ϳ��� �����ؼ� ���������� ������ ����� �������.
    [SerializeField] float destroyDelayTime = 5f;
    WaitForSeconds delayForUse = new WaitForSeconds(1f); // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;


    private void OnEnable()
    {
        Destroy(this.gameObject, destroyDelayTime);
        StartCoroutine(makeDelay());

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

    IEnumerator makeDelay()
    {
        afterDelay = false;
        yield return delayForUse;
        afterDelay = true;
    }




    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}

