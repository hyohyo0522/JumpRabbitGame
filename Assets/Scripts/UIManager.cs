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

    //��Ʈ�����̳� ����(��ƮUI)
    //private GameObject[] heartContainers;
    //private Image[] heartFills;
    //public Transform heartsParent; // ��Ʈ��ġ
    //public GameObject heartContainerPrefab;
    //public delegate void OnHealthChangedDelegate();
    //public OnHealthChangedDelegate onHealthChangedCallback;

    //[SerializeField]
    //private float health;
    //[SerializeField]
    //private float maxHealth;
    //[SerializeField]
    //private float maxTotalHealth;

    //public float Health { get { return health; } }
    //public float MaxHealth { get { return maxHealth; } }
    //public float MaxTotalHealth { get { return maxTotalHealth; } }





    //�÷��̾�����
    float movePower;
    [SerializeField] float moveSpeed = 5f;

    //ȯ�漳������
    public GameObject SettingPanel;







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


    public void GameClear()
    {
        string WinnerName = PlayerPrefs.GetString("_myNick");
        PlayerPrefs.SetString("Winner", WinnerName);
        SceneManager.LoadScene("EndingScene");
    }


    #region HeartContainerUI 



    #endregion HeartContainerUI 


}

