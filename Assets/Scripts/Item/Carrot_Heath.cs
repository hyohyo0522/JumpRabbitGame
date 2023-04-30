using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot_Heath : MonoBehaviour,IItem
{
    [SerializeField] float health = 30f;

    private void Start()
    {
        Destroy(this.gameObject, 4f);
    }

    public void Use(GameObject target)
    {


        PlayerLife playerLife = target.GetComponent<PlayerLife>();
        if (playerLife != null && !playerLife.dead)
        {
            AudioManager.instance.PlaySFX("PlayerGetHeal");
            playerLife.RestoreHealth(health);

            Destroy(this.gameObject);


        }
    }
    public void DestoySelf()
    {
        Destroy(this.gameObject);
    }

}
