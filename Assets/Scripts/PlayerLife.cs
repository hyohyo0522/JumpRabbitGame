using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : LivingEntity
{

    float MaxHealth = 100f;
    float timeBetattack = 0.5f;

    //���� UI����
    int _myKeyNum = 0;

    //��Ÿ(����)UI ����
    int _myScore = 0; //����
    //public Text _scoreTxt;

    //�� UI����
    int _myMoney = 100; // �� 
    //public Text _moneyTxt;

    //������ UI����
    int _mySlug = 0;
    //public Text _slugText;

    //�ö���� Kill UI ����
    int _killFlowers = 0;
    //public Text _flowerKill;

    // �÷��̾�ų UI����
    int _killPlayer = 0;
   // public Text _playerKill;

    /* ���� �Ӵ� �ִ��ġ */
    int maxMoneyAndScore = 999999;

    // ü��ǥ�� UI
   // public Slider _myHeathSlider;
    //public Text _myHealthValue;

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


        // --------------------------------------------
        //UI �ؽ�Ʈ �ʱ�ȭ( �ʱ� ���� )
        //_myHeathSlider.maxValue = MaxHealth; // ü�� �����̴��� �ִ��� �⺻ ü�°����� ����
        //_myHeathSlider.value = hp; // ü�� �����̴� ���� ���簪���� ����


        //_myHealthValue.text = hp.ToString();
        //_moneyTxt.text = _myMoney.ToString();
        //_scoreTxt.text = _myScore.ToString();
        //_slugText.text = _mySlug.ToString();
        //_flowerKill.text = _killPlayer.ToString();

        //_myHeathSlider.gameObject.SetActive(true); // ü�½����̴� Ȱ��ȭ

        // ---------------------------------
        //���⼭���� �ٲ� ����

        UIManager.instance._myHeathSlider.maxValue = MaxHealth; // ü�� �����̴��� �ִ��� �⺻ ü�°����� ����
        UIManager.instance._myHeathSlider.value = hp; // ü�� �����̴� ���� ���簪���� ����
        UIManager.instance._myHealthValue.text = hp.ToString();

        UIManager.instance.UpdateKeyNumUI(_myKeyNum);
        UIManager.instance._moneyTxt.text = _myMoney.ToString();
        UIManager.instance._scoreTxt.text = _myScore.ToString();
        UIManager.instance._slugText.text = _mySlug.ToString();
        UIManager.instance._flowerKill.text = _killFlowers.ToString();
        UIManager.instance._playerKill.text = _killPlayer.ToString();



        UIManager.instance._myHeathSlider.gameObject.SetActive(true); // ü�½����̴� Ȱ��ȭ

 
    }

    private void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        OnDeath += () => _myMovement.ImDead();
        _myMovement.ImRevie += () => RestoreHealth(MaxHealth/2f); // ��Ȱ�� ü���� �ݸ� ȸ���Ѵ�. 
        _myMovement.ImRevie += () => UpdateHearthUI();
        _myMovement.ImRevie += () => dead=false;

    }

    public override void OnDamage(float value)
    {

        if (attacked) return;
        //���� attacked ������ ���⼭ �ϸ� ���� �� ����!!
        base.OnDamage(value);
        AudioManager.instance.PlaySFX("PlayeDamaged");

        //�ｺ �����̴� UI����
        UIManager.instance._myHeathSlider.value = hp;
        UIManager.instance._myHealthValue.text = hp.ToString();

        if (hp <= 0) return; //ü���� 0���Ϸ� �Ǿ��� ������ ���� �ٲ�� �� ����
        StartCoroutine(onDamageforChange());

    }



    IEnumerator onDamageforChange()
    {
        _myMovement.playerAnimator.SetTrigger("Damaged");
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
            if (addHealth >= (MaxHealth - hp))
            {
                addHealth = (MaxHealth - hp);
            }
            hp += addHealth;

            //�ｺ �����̴� UI����
            UIManager.instance._myHeathSlider.value = hp;
            UIManager.instance._myHealthValue.text = hp.ToString();
        }
    }

    public void UpdateHearthUI()
    {
        PlayerHeartStat.Instance.ChangeFilledHeartNum(-1);


        //��Ȱ�Ҹ� �����Ű�� ���ڴ�.
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

    public void UpdateMyKeyNum(int value)
    {
        _myKeyNum += value;
        UIManager.instance.UpdateKeyNumUI(_myKeyNum);
        if(_myKeyNum == HouseDoor.NeededKeyNumForHouse)
        {
            UIManager.instance.UrgentGameTip(UIManager.AllKeyGatherd);
        }
    }

    // UI ���� 
    public void UpdateScore(int value) // ��Ÿ ���� UI
    {
        if (_myScore <= maxMoneyAndScore)
        {
            _myScore += value;
        }

        UIManager.instance._scoreTxt.text = GetThousandCommaText(_myScore).ToString();

    }

    public void UpdateMoney(int value) // ������ ������ �� �ø� ��
    {
        string audioClipName = "BingoGetMoney";

        if (value ==3) // �޺��������� Ȯ��
        {
            value = BingoPanel.ComboMoney;
            audioClipName = "BingoGetComboMoney";
        }
        if(_myMoney <= maxMoneyAndScore)
        {
            _myMoney += value;
            AudioManager.instance.PlaySFX(audioClipName);
        }


        UIManager.instance._moneyTxt.text = GetThousandCommaText(_myMoney).ToString();

    }

    public void UpdateSlugUI(int value)
    {
        _mySlug += value;
        UIManager.instance._slugText.text = GetThousandCommaText(_mySlug).ToString();

    }

    public void UpgateFlowerKillUI()
    {
        _killFlowers ++;
        UIManager.instance._flowerKill.text = GetThousandCommaText(_killFlowers).ToString();
    }

    //
    public void UpdatePlayerKillUI(int value)
    {
        _killPlayer += value;
        UIManager.instance._playerKill.text = GetThousandCommaText(_killPlayer).ToString();
    }
    

    public bool HasEnuoughItem(eBingoItem item, int value) //������ ��ư Ŭ���� ȣ��
    {
        bool result = false;
        switch (item)
        {
            case eBingoItem.carrot:
                if (UIManager.instance.myCarrotValue >= value)
                {
                    _myMovement.JumpCountDown(value);
                    result = true;
                }
                break;
            case eBingoItem.star:
                if (_myScore >= value)
                {
                    UpdateScore(-value);
                    result = true;
                }
                break;
            case eBingoItem.flower:
                if (_killFlowers >= value)
                {
                    _killFlowers -= value;
                    UIManager.instance._flowerKill.text = GetThousandCommaText(_killFlowers).ToString();
                    result = true;
                }
                break;
            case eBingoItem.slug:
                if (_mySlug >= value)
                {
                    UpdateSlugUI(-value);
                    result = true;
                }
                break;
            case eBingoItem.player:
                if (_killPlayer >= value)
                {
                    UpdatePlayerKillUI(-value);
                    result = true;
                }
                break;
        }

        return result;
    }

    public bool UseMoneyForKey(int payMoney) //���� ���� �� �� �ִ��� Ȯ��
    {
        bool hasPaid = false;
        if (_myMoney >= payMoney)
        {
            _myMoney -= payMoney;
            hasPaid = true;
            UIManager.instance._moneyTxt.text = GetThousandCommaText(_myMoney).ToString();
            UpdateMyKeyNum(1); // ���� �ѹ� 1 �ö�



            // �ڵ� ���ҵǴ� �Ҹ� �����Ű��!!!!
            return hasPaid;

        }

        // �� �� �� �԰� �������� �Ҹ� �����Ű��!!
        return hasPaid;
    }

    public bool CheckKeyNumForHouseMsg(int NeededKeyNumForEnding)
    {
        bool hasEnoughKey = false;
        if (_myKeyNum >= NeededKeyNumForEnding)
        {
            hasEnoughKey = true;
        }
        return hasEnoughKey;
    }
    

    // ���� 1�������� ����000���� �޸����
    string GetThousandCommaText(int data)
    {
        string result = data.ToString();
        if (data != 0)
        {
            result = string.Format("{0:#,###}", data);
        }

            return result;
    }

    


}



