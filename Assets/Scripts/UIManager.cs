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


    // ü��ǥ�� UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;

    //����Ϲ�ư �÷��̾��̵����� 
    bool LeftMove;
    bool RightMove;
    bool isMoving;
    float movePower;
    [SerializeField] float moveSpeed;



    public void UpdateCarrotText(int value)
    {
        myCarrotValue = value;
        _myCarrotCount.text = value.ToString();
    }

    public void LeftBtnDown() //���� ��ư ������ ��
    {
        //isMoving = true;
        movePower = -1;
        LeftMove = true;
    }

    public void LeftBtnUp() //���� ��ư���� �� ������ ��
    {
        //isMoving = false;
        movePower = 0;
        LeftMove = false;
    }

    public void RightBtnDown() //������ ��ư ������ ��
    {
        //isMoving = true;
        movePower = 1;
        RightMove = true;
    }

    public void RightBtnUp() //������ ��ư���� �� ������ �� 
    {
        //isMoving = false;
        movePower = 0;
        RightMove = false;
    }

    public float GetHorizontalValue()
    {
        return movePower;
    }

}
