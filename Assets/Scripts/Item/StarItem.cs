using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour, IItem
{
    
    [SerializeField] float destroyDelayTime = 5f; // 파괴시간
    bool afterDelay = false;
    [SerializeField] int starValue = 1;
    [SerializeField] float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임


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
            PlayerLife _myPlayer = target.GetComponent<PlayerLife>();

            if (!_myPlayer.dead)
            {
                AudioManager.instance.PlaySFX("PlayerGetItem");
                _myPlayer.UpdateScore(starValue);
                Destroy(this.gameObject);
            }

        }
    }

    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }
}
