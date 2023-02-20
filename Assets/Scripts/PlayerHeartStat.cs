using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeartStat : MonoBehaviour
{
    public delegate void OnHeartChangeDelegate();
    public OnHeartChangeDelegate onHeartChandedCallback;

    #region Singleton

    private static PlayerHeartStat instance;
    public static PlayerHeartStat Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerHeartStat>();
            }
            return instance;
        }
    }

    #endregion Singleton

    private int health = 3;
    private int maxhealth = 3;
    private int maxTotalHealth = 5;

    public int Health { get { return health; } }
    public int MaxHealth { get { return maxhealth; } }
    public int MaxToTalHealth { get { return maxTotalHealth; } }

    public void ChangeFilledHeartNum(int value)
    {

        this.health += value;
        ClampHeart();

        if (health == 0)
        {
            UIManager.instance.UrgentGameTip(UIManager.ZeroHeart);
        }
    }

    public void AddMaxHealth()
    {
        if (maxhealth < maxTotalHealth)
        {
            maxhealth += 1;
            health = maxhealth;

            if (onHeartChandedCallback != null)
            {
                onHeartChandedCallback.Invoke();
            }
        }
    }

    void ClampHeart() //최대최솟값 안에만 있을 수 있도록 한다.
    {
        health = Mathf.Clamp(health, 0, maxhealth);
        if (onHeartChandedCallback != null)
        {
            onHeartChandedCallback.Invoke();
        }
    }

}



