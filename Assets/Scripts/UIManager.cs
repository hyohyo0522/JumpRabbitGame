using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class  UIManager : MonoBehaviour
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

    //����Ϲ�ư �÷��̾��̵����� 


    float movePower;
    [SerializeField] float moveSpeed = 5f;


    private void OnEnable() 
    {

        //�ַ��÷��� �������, ��Ƽ�÷��̾� ��������� ���� _playerKill �̹��� �ٸ��� ����! 
        GameObject _IconImageForMultiPlay = _playerKillUI.transform.GetChild(1).gameObject;
        GameObject _IconImageForSinglePlay = _playerKillUI.transform.GetChild(2).gameObject;

        string _myGameMode = PlayerPrefs.GetString("Mode");

        if(_myGameMode == "Single") //���Ӹ�尡 �̱��϶� ����
        {
            _IconImageForMultiPlay.SetActive(false);
            _IconImageForSinglePlay.SetActive(true);
        }
        if(_myGameMode == "Multi") //���Ӹ�尡 ��Ƽ�� �� ����
        {
            _IconImageForMultiPlay.SetActive(true);
            _IconImageForSinglePlay.SetActive(false);
        }


    }


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

}
