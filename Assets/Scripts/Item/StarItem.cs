using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour, IItem
{
    
    public float destroyDelayTime = 5f; // �ı��ð�
    bool afterDelay = false;
    public int starValue = 1;
    public float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��

    private void OnEnable()
    {
        StartCoroutine(makeDelay());
    }

    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime);
    }

    IEnumerator makeDelay()
    {
        afterDelay = false;
        yield return new WaitForSeconds(delayForUse);
        afterDelay = true;
    }

    public void Use(GameObject target)
    {
        if (afterDelay)
        {
            PlayerLife _myPlayer = target.GetComponent<PlayerLife>();

            if (!_myPlayer.dead)
            {
                _myPlayer.UpdateScore(starValue);
                Destroy(this.gameObject);
            }

        }
    }
}
