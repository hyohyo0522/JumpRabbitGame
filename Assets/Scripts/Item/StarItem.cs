using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour, IItem
{
    
    [SerializeField] float destroyDelayTime = 5f; // �ı��ð�
    bool afterDelay = false;
    [SerializeField] int starValue = 1;
    WaitForSeconds delayForUse = new WaitForSeconds(0.5f);// �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��


    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);
        StartCoroutine(makeDelay());

    }


    public void Use(GameObject target)
    {
        if (afterDelay)
        {
            PlayerLife _myPlayer = target.GetComponent<PlayerLife>();

            if (!_myPlayer.dead)
            {
                AudioManager.instance.PlaySFX("PlayerGetItem");
                _myPlayer.UpdateScore(starValue);
                Destroy(this.gameObject);
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
