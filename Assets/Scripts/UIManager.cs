using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    // �ʿ��� UI�� ��� �����ϰ� ������ �� �ֵ��� ����ϴ� UI �Ŵ���
    // �̱��� ���ٿ� ������Ƽ
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

    private static UIManager m_instance; // �̱����� �Ҵ�� ����

    public Text _myCarrotCount; // ������� Ƚ��
    public int myCarrotValue; //�÷��̾� ĳ������ ĳ������ ����

    public Text _scoreTxt;     // ��ŸUI ����
    public Text _moneyTxt;      //�� UI����
    public Text _slugText; // ������ �� ���� UI
    public Text _flowerKill;    //�ö���� Kill UI ����
    public Text _playerKill;    // �÷��̾�ų UI����
    public GameObject _playerKillUI;
    public Text _KeyNum;


    // ü��ǥ�� UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;

    #region ��Ʈ�����̳� ����(��ƮUI)
    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent; // ��Ʈ��ġ
    public GameObject heartContainerPrefab;


    #endregion ��Ʈ�����̳� ����(��ƮUI)

    //�÷��̾�����
    float movePower;
    [SerializeField] float moveSpeed = 5f;

    //ȯ�漳������
    public GameObject SettingPanel;

    //������â ����
    bool isGameTipOn= true;
    public Text gameTipTitle;
    public Text gameTipMsg;

    float timeBetGameTip = 7f; // ���Ӹ޽����� �Ұ���
    float timeOfGameMsg = 5f; // ���Ӹ޽��� ������� �ð�
    public readonly WaitForSeconds m_waitForSecondsForGameTip = new WaitForSeconds(7f);
    [SerializeField] float realtimeForGameTip; 



    private void OnEnable()
    {

        //�ַ��÷��� �������, ��Ƽ�÷��̾� ��������� ���� _playerKill �̹��� �ٸ��� ����! 
        GameObject _IconImageForMultiPlay = _playerKillUI.transform.GetChild(1).gameObject;
        GameObject _IconImageForSinglePlay = _playerKillUI.transform.GetChild(2).gameObject;

        string _myGameMode = PlayerPrefs.GetString("Mode");

        if (_myGameMode == "Single") //���Ӹ�尡 �̱��϶� ����
        {
            _IconImageForMultiPlay.SetActive(false);
            _IconImageForSinglePlay.SetActive(true);
        }
        if (_myGameMode == "Multi") //���Ӹ�尡 ��Ƽ�� �� ����
        {
            _IconImageForMultiPlay.SetActive(true);
            _IconImageForSinglePlay.SetActive(false);
        }


    }

    private void Start()
    {
        IsOpenSettingPanel(false); // ȯ�漳�� �г� �ϴ� ����


        #region  HeartContainerUI StartSetting

        heartContainers = new GameObject[(int)PlayerHeartStat.Instance.MaxToTalHealth];
        heartFills = new Image[(int)PlayerHeartStat.Instance.MaxToTalHealth];

        PlayerHeartStat.Instance.onHeartChandedCallback += UpdateHeartsUI;
        InstantiateHeartContainers();
        UpdateHeartsUI();
        

        #endregion HeartContainerUI StartSetting
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

    #region PlayerMove

    public void LeftBtnDown() //���� ��ư ������ ��
    {
        //isMoving = true;
        movePower = -1;

    }

    public void LeftBtnUp() //���� ��ư���� �� ������ ��
    {
        //isMoving = false;
        movePower = 0;

    }

    public void RightBtnDown() //������ ��ư ������ ��
    {
        //isMoving = true;
        movePower = 1;

    }

    public void RightBtnUp() //������ ��ư���� �� ������ �� 
    {
        //isMoving = false;
        movePower = 0;

    }

    public float GetHorizontalValue()
    {
        return movePower;
    }

    #endregion PlayerMove


    #region SettingPanel : ȯ�漳��â ����
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

    #endregion SettingPanel : ȯ�漳��â ����

    #region ������â

    public void OnOffGameTip() //������â �Ѱ���� ���
    {
        bool OnOff = !(gameTipTitle.IsActive());
        isGameTipOn = OnOff;
        gameTipTitle.gameObject.SetActive(isGameTipOn);
        gameTipMsg.gameObject.SetActive(isGameTipOn);
    }

    private void ResetGameTip()
    {
        if (!isGameTipOn) return;
        //Enum���� ���� �޽��� ���ϱ�

        //�����޽��� ����

        //�����޽��� �ٲٰ�, ���� �����ϰ� Time�����ϱ�
    }

    
    public void UrgentGameTip(string id) //Enum���� ������ ȣ���ϵ��� ����.
    {
        if (!isGameTipOn) return;
        //Enum���� ������ ������ ȣ��
        //���� �����ϱ� 
        //�����޽��� �����ؼ� ����
        //Time�����ϱ�

    }

    public void MakeTermBetGame()
    {
        if (!isGameTipOn) return;


    }
    // �ڷ�ƾ �Ἥ ������!!!

    IEnumerator MakeTermBetGameTip()
    {

        //���⿡ ��� ������!!!
        yield return m_waitForSecondsForGameTip;
    }

    #endregion ������â



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
        //if (PlayerHeartStat.Instance.Health %1 != 0) //�������� ����ϴ� ��
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

