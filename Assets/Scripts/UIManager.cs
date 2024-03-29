using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;



public class UIManager : MonoBehaviour
{
    // 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public Text _myCarrotCount; // 점프당근 횟수
    public int myCarrotValue; //플레이어 캐릭터의 캐럿수를 저장

    public Text _scoreTxt;     // 스타UI 관련
    public Text _moneyTxt;      //돈 UI관련
    public Text _slugText; // 슬러그 겟 관련 UI
    public Text _flowerKill;    //플라워몬 Kill UI 관련
    public Text _playerKill;    // 플레이어킬 UI관련
    public GameObject _playerKillUI;
    public Text _KeyNum;


    // 체력표시 UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;

    #region 하트콘테이너 관련(하트UI)
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent; // 하트위치
    public GameObject heartContainerPrefab;


    #endregion 하트콘테이너 관련(하트UI)

    //플레이어무브관련
    float movePower;
    [SerializeField] float moveSpeed = 5f;

    //플레이어Dig관련 (땅파서 당근얻기 기능)
    public Slider digGageSlider;
    float digGage = 0;
    float sliderGagePower = 5f; // 슬라이더 올라가는 양
    public event Action DigGageFullFilled;

    //환경설정관련
    public GameObject SettingPanel;

    //게임팁창 관련
    bool isGameTipOn= true;
    public GameObject gameTipTitle;
    public GameObject gameTipMsg;
    Color normalTipColor = new Color(255f, 255f, 255f, 220f);
    Color UrgentTipColor = Color.red;

    float UrgentGameTipTime = 5f; // 긴급게임메시지 띄워지는 시간 
    public readonly WaitForSeconds m_waitForSecondsForGameTip = new WaitForSeconds(7f); //게임메시지 띄워지는 시간or게임메시지간 텀간격

    [TextArea]
    public string[] NormalGuideTips; //게임메시지 저장소(노멀)
    [TextArea]
    public string[] UrgentGuideTips; //게임메시지 저장소(Urgent)
    #region UrgentGuideTips : 메시지별 키값 정리

    public const int ZeroHeart = 0; //하트 0개일 때 메시지
    public const int KeyDestroyed = 1; // 돈이 부족해서 키가 파괴될 때 메시지
    public const int AllKeyGatherd = 2; // 열쇠 모두 모았을 때 메시지
    public const int CarrotShortage = 3; // 당근이 부족할 때 메시지
    public const int ZeroCarrot = 4; // 당근이 0개일 때 메시지

    #endregion UrgentGuideTips : 키값 정리



    private void OnEnable()
    {

        //솔로플레이 모드인지, 멀티플레이어 모드인지에 따라 _playerKill 이미지 다르게 하자! 
        GameObject _IconImageForMultiPlay = _playerKillUI.transform.GetChild(1).gameObject;
        GameObject _IconImageForSinglePlay = _playerKillUI.transform.GetChild(2).gameObject;

        string _myGameMode = PlayerPrefs.GetString("Mode");

        if (_myGameMode == "Single") //게임모드가 싱글일때 설정
        {
            _IconImageForMultiPlay.SetActive(false); 
            _IconImageForSinglePlay.SetActive(true);
        }
        if (_myGameMode == "Multi") //게임모드가 멀티일 때 설정
        {
            _IconImageForMultiPlay.SetActive(true);
            _IconImageForSinglePlay.SetActive(false);
        }




    }

    private void Start()
    {
        IsOpenSettingPanel(false); // 환경설정 패널 일단 끄기

        digGageSlider.value = digGage; // 땅파기 게이지 초기화


        #region  HeartContainerUI StartSetting

        heartContainers = new GameObject[(int)PlayerHeartStat.Instance.MaxToTalHealth];
        heartFills = new Image[(int)PlayerHeartStat.Instance.MaxToTalHealth];

        PlayerHeartStat.Instance.onHeartChandedCallback += UpdateHeartsUI;
        InstantiateHeartContainers();
        UpdateHeartsUI();


        #endregion HeartContainerUI StartSetting

        #region GameTipSetUp

        isGameTipOn = true;
        gameTipTitle.gameObject.SetActive(isGameTipOn);
        gameTipMsg.gameObject.SetActive(isGameTipOn);
        StartCoroutine("GameTipUI");

        #endregion GameTipSetUp

    }

    #region Item UI

    public void UpdateCarrotText(int value)
    {
        myCarrotValue = value;
        _myCarrotCount.text = value.ToString();
    }

    public void UpdateKeyNumUI(int value)
    {
        int KeyNumber = value;
        _KeyNum.text = KeyNumber.ToString();

    }

    #endregion Item UI

    #region PlayerMove And Dig

    public void LeftBtnDown() //왼쪽 버튼 눌렸을 때
    {
        //isMoving = true;
        movePower = -1;

    }

    public void LeftBtnUp() //왼쪽 버튼에서 손 떼졌을 때
    {
        //isMoving = false;
        movePower = 0;

    }

    public void RightBtnDown() //오른쪽 버튼 눌렸을 때
    {
        //isMoving = true;
        movePower = 1;

    }

    public void RightBtnUp() //오른쪽 버튼에서 손 떼졌을 때 
    {
        //isMoving = false;
        movePower = 0;

    }

    public float GetHorizontalValue()
    {
        return movePower;
    }

    //PlayerDig관련
    public void FillDigGage()
    {
        // 플레이어가 IsGrounded 일때에만 작동이 되어야하므로 PlayerMovement.cs에서 해당 함수를 호출한다.
        digGage += Time.deltaTime*sliderGagePower;
        digGageSlider.value = digGage;

        if(digGage >= 1)
        {
            //게이지 초기화
            digGage = 0;
            digGageSlider.value = digGage;
            if(DigGageFullFilled != null)
            {
                DigGageFullFilled();
            }
        }
    }

    #endregion PlayerMove And Dig

    #region SettingPanel : 환경설정창 관련
    public void IsOpenSettingPanel(bool isOn)
    {
        if (SettingPanel.gameObject.activeSelf != isOn)
        {
            AudioManager.instance.PlaySFX("SettingBtn");
            SettingPanel.SetActive(isOn);
        }

    }

    public void GoBackStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    #endregion SettingPanel : 환경설정창 관련

    #region 게임팁창

    public void OnOffGameTip() //게임팁창 켜고끄는 기능
    {
        isGameTipOn = !(gameTipTitle.activeSelf);
        gameTipTitle.gameObject.SetActive(isGameTipOn);
        gameTipMsg.gameObject.SetActive(isGameTipOn);

        if (!isGameTipOn)
        {
            StopCoroutine("GameTipUI");
        }
        else
        {
            StartCoroutine("GameTipUI");
        }
    }


    private void ResetGameTip()
    {
        if (!isGameTipOn) return;
        //랜덤 메시지 추출

        int index = Random.Range(0, NormalGuideTips.Length);
        string selectedMsg = NormalGuideTips[index];
        Debug.Log(selectedMsg);
        gameTipMsg.GetComponent<Text>().text = selectedMsg;

        // Debug.Log("메시지를 리셋했다!");

        //랜덤메시지 바꾸고, 색깔 지정하고 Time리셋하기
        gameTipMsg.GetComponent<Text>().color = normalTipColor;


    }

    
    public void UrgentGameTip(int MsgkeyId) //Enum으로 게임팁 호출하도록 하자.
    {
        if (!isGameTipOn) return;
        //Enum으로 지정된 게임팁 호출

        //긴급메시지가 여러개 들어올 경우를 대비하여 위 함수에서 맨 아랫줄 Invoke된 함수를 취소처리한다. 
        CancelInvoke("StartGameTipUICouroutine");

        //코루틴 멈추기
        StopCoroutine("GameTipUI");

        //색깔 지정하기 
        gameTipMsg.GetComponent<Text>().color = UrgentTipColor;

        //긴급메시지 텍스트 적용
        string selectedMsg = UrgentGuideTips[MsgkeyId];
        gameTipMsg.GetComponent<Text>().text = selectedMsg;


        ////코루틴 멈춘 것 Invoke로 다시시작
        Invoke("StartGameTipUICouroutine", UrgentGameTipTime);
    }

    public void MakeTermBetGameTip()
    {
        gameTipMsg.GetComponent<Text>().text = "  ";

    }

    public void StartGameTipUICouroutine() // Invoke로 지연시간 줘서 코루틴 시작하기 위해 만든 함수
    {
        StartCoroutine("GameTipUI");
    }

    // 코루틴 써서 만들자!!!
    IEnumerator GameTipUI()
    {

        while (isGameTipOn)
        {
            //if (realtimeForGameTip < timeOfGameMsg)
            //{
            //    realtimeForGameTip += Time.deltaTime;
            //    realtimeForGameTip += Time.deltaTime;
            //    continue;
            //}

            MakeTermBetGameTip();
            yield return m_waitForSecondsForGameTip;
            ResetGameTip();
            yield return m_waitForSecondsForGameTip;
        }
    }

    #endregion 게임팁창



    public void GameClear()
    {
        string WinnerName = PlayerPrefs.GetString("_myNick");
        PlayerPrefs.SetString("Winner", WinnerName);
        SceneManager.LoadScene("EndingScene");
    }


    #region HeartContainerUI 

    public void UpdateHeartsUI()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerHeartStat.Instance.MaxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for(int i =0; i < heartFills.Length; i++)
        {
            if (i < PlayerHeartStat.Instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }

        #region WhenUsingHeartSystemByFloatUnit(NotUseNow)
        //if (PlayerHeartStat.Instance.Health %1 != 0) //나머지를 계산하는 식
        //{
        //    int lastPos = Mathf.FloorToInt(PlayerHeartStat.Instance.Health);
        //    heartFills[lastPos].fillAmount = PlayerHeartStat.Instance.Health % 1;
        //}
        #endregion WhenUsingHeartSystemByFloatUnit(NotUseNow)
    }

    void InstantiateHeartContainers()
    {
        for(int i =0; i < PlayerHeartStat.Instance.MaxToTalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }



    #endregion HeartContainerUI 


}

