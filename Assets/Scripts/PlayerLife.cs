using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : LivingEntity
{

    float MaxHealth = 50f;
    float timeBetattack = 1f;

    //점수UI 관련
    int _myScore = 0; //점수
    //public Text _scoreTxt;

    //돈 UI관련
    int _myMoney = 100; // 돈 
    //public Text _moneyTxt;

    //달팽이 UI관련
    int _mySlug = 0;
    //public Text _slugText;

    //플라워몬 Kill UI 관련
    int _killFlowers = 0;
    //public Text _flowerKill;

    // 플레이어킬 UI관련
    int _killPlayer = 0;
   // public Text _playerKill;

    /* 돈과 머니 최대수치 */
    int maxMoneyAndScore = 999999;

    // 체력표시 UI
   // public Slider _myHeathSlider;
    //public Text _myHealthValue;

    public bool attacked = false;
    SpriteRenderer playerSprite;
    private PlayerMovement _myMovement;


    private void Awake()
    {
        startHealth = MaxHealth; // 플레이어 헬스값 초기화

    }

    private void OnEnable()
    {

        // 아래 코드 순서 달라지니 이상하게 나왔음..
        base.OnEnable();

        //Debug.Log(this.startHealth.ToString());
        //Debug.Log(this.hp.ToString());

        _myMovement = this.gameObject.GetComponent<PlayerMovement>();


        // --------------------------------------------
        //UI 텍스트 초기화( 초기 버전 )
        //_myHeathSlider.maxValue = MaxHealth; // 체력 슬라이더의 최댓값을 기본 체력값으로 변경
        //_myHeathSlider.value = hp; // 체력 슬라이더 값을 현재값으로 변경


        //_myHealthValue.text = hp.ToString();
        //_moneyTxt.text = _myMoney.ToString();
        //_scoreTxt.text = _myScore.ToString();
        //_slugText.text = _mySlug.ToString();
        //_flowerKill.text = _killPlayer.ToString();

        //_myHeathSlider.gameObject.SetActive(true); // 체력슬라이더 활성화

        // ---------------------------------
        //여기서부터 바꾼 버전

        UIManager.instance._myHeathSlider.maxValue = MaxHealth; // 체력 슬라이더의 최댓값을 기본 체력값으로 변경
        UIManager.instance._myHeathSlider.value = hp; // 체력 슬라이더 값을 현재값으로 변경


        UIManager.instance._myHealthValue.text = hp.ToString();
        UIManager.instance._moneyTxt.text = _myMoney.ToString();
        UIManager.instance._scoreTxt.text = _myScore.ToString();
        UIManager.instance._slugText.text = _mySlug.ToString();
        UIManager.instance._flowerKill.text = _killFlowers.ToString();



        UIManager.instance._myHeathSlider.gameObject.SetActive(true); // 체력슬라이더 활성화

 
    }

    private void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        OnDeath += () => _myMovement.ImDead();
        _myMovement.ImRevie += () => RestoreHealth(MaxHealth/2f); // 부활시 체력의 반만 회복한다. 
        _myMovement.ImRevie += () => dead=false;

    }

    public override void OnDamage(float value)
    {
        base.OnDamage(value);

        //헬스 슬라이더 UI갱신
        UIManager.instance._myHeathSlider.value = hp;
        UIManager.instance._myHealthValue.text = hp.ToString();

        if (hp <= 0) return; //체력이 0이하로 되었을 때에는 색깔 바뀌는 걸 방지
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

            //헬스 슬라이더 UI갱신
            UIManager.instance._myHeathSlider.value = hp;
            UIManager.instance._myHealthValue.text = hp.ToString();
        }
    }

    public bool isFullHeath() // 체력회복 아이템 먹을 시 체력양이 풀이면 먹지 않도록 한다. 
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

    // UI 관련 
    public void UpdateScore(int value) // 스타 관련 UI
    {
        if (_myScore <= maxMoneyAndScore)
        {
            _myScore += value;
        }

        UIManager.instance._scoreTxt.text = GetThousandCommaText(_myScore).ToString();

    }

    public void UpdateMoney(int value) // 돈 관련 UI
    {
        if(value ==3) // 콤보코인인지 확인
        {
            value = BingoPanel.ComboMoney;
        }
        if(_myMoney <= maxMoneyAndScore)
        {
            _myMoney += value;
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

    public void UpdatePlayerKillUI(int value)
    {
        _killPlayer -= value;
        UIManager.instance._playerKill.text = GetThousandCommaText(_killPlayer).ToString();
    }

    public bool HasEnuoughItem(eBingoItem item, int value)
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
                    UpdatePlayerKillUI(value);
                    result = true;
                }
                break;
        }

        return result;
    }

    

    // 숫자 1전형적인 패턴000마다 콤마찍기
    string GetThousandCommaText(int data)
    {
        return string.Format("{0:#,###}", data);
    }

    


}

