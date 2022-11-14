using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : LivingEntity
{

    float MaxHealth = 50f;
    float timeBetattack = 1f;

    //����UI ����
    int _myScore = 0; //����
    public Text _scoreTxt;

    //�� UI����
    int _myMoney = 100; // �� 
    public Text _moneyTxt;

    //������ UI����
    int _mySlug = 0;
    public Text _slugText;

    //�ö���� Kill UI ����
    int _killFlowers = 0;
    public Text _flowerKill;

    // �÷��̾�ų UI����
    int _killPlayer = 0;
    public Text _playerKill;

    /* ���� �Ӵ� �ִ��ġ */
    int maxMoneyAndScore = 999999;

    // ü��ǥ�� UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;

    public bool attacked = false;
    SpriteRenderer playerSprite;
    private PlayerMovement _myMovement;


    private void Awake()
    {
        startHealth = MaxHealth; // �÷��̾� �ｺ�� �ʱ�ȭ

    }

    private void OnEnable()
    {

        // �Ʒ� �ڵ� ���� �޶����� �̻��ϰ� ������..
        base.OnEnable();

        //Debug.Log(this.startHealth.ToString());
        //Debug.Log(this.hp.ToString());

        _myMovement = this.gameObject.GetComponent<PlayerMovement>();

        _myHeathSlider.maxValue = MaxHealth; // ü�� �����̴��� �ִ��� �⺻ ü�°����� ����
        _myHeathSlider.value = hp; // ü�� �����̴� ���� ���簪���� ����

        //UI �ؽ�Ʈ �ʱ�ȭ
        _myHealthValue.text = hp.ToString();
        _moneyTxt.text = _myMoney.ToString();
        _scoreTxt.text = _myScore.ToString();
        _slugText.text = _mySlug.ToString();
        _flowerKill.text = _killPlayer.ToString();


        _myHeathSlider.gameObject.SetActive(true); // ü�½����̴� Ȱ��ȭ

 
    }

    private void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        OnDeath += () => _myMovement.ImDead();
        _myMovement.ImRevie += () => RestoreHealth(MaxHealth/2f); // ��Ȱ�� ü���� �ݸ� ȸ���Ѵ�. 
        _myMovement.ImRevie += () => dead=false;

    }

    public override void OnDamage(float value)
    {
        base.OnDamage(value);

        //�ｺ �����̴� UI����
        _myHeathSlider.value = hp;
        _myHealthValue.text = hp.ToString();

        if (hp <= 0) return; //ü���� 0���Ϸ� �Ǿ��� ������ ���� �ٲ�� �� ����
        StartCoroutine(onDamageforChange());

    }



    IEnumerator onDamageforChange()
    {
        playerSprite.color = Color.red;
        attacked = true;
        yield return new WaitForSeconds(timeBetattack);
        playerSprite.color = Color.white;
        attacked = false;
    }

    public void RestoreHealth(float value)
    {
        if (hp < MaxHealth)
        {
            float addHealth = value;
            if(addHealth >= (MaxHealth - hp))
            {
                addHealth = (MaxHealth - hp);
            }
            hp += addHealth;

            //�ｺ �����̴� UI����
            _myHeathSlider.value = hp;
            _myHealthValue.text = hp.ToString();
        }
    }

    public bool isFullHeath() // ü��ȸ�� ������ ���� �� ü�¾��� Ǯ�̸� ���� �ʵ��� �Ѵ�. 
    {
        bool isFull;
        if (hp >= MaxHealth)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
        return isFull;
    }

    // UI ���� 
    public void UpdateScore(int value) // ��Ÿ ���� UI
    {
        if (_myScore <= maxMoneyAndScore)
        {
            _myScore += value;
        }
        
        _scoreTxt.text = GetThousandCommaText(_myScore).ToString();

    }

    public void UpdateMoney(int value) // �� ���� UI
    {
        if(_myMoney <= maxMoneyAndScore)
        {
            _myMoney += value;
        }


        _moneyTxt.text = GetThousandCommaText(_myMoney).ToString();

    }

    public void UpdateSlugUI(int value)
    {
        _mySlug += value;
        _slugText.text = GetThousandCommaText(_mySlug).ToString();

    }

    public void UpgateFlowerKillUI()
    {
        _killFlowers ++;
        _flowerKill.text = GetThousandCommaText(_killFlowers).ToString();
    }

    

    // ���� 1�������� ��������000���� �޸����
    string GetThousandCommaText(int data)
    {
        return string.Format("{0:#,###}", data);
    }

}

