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

    public Text _scoreTxt;     // ��ŸUI ����
    public Text _moneyTxt;      //�� UI����
    public Text _slugText; // ������ �� ���� UI
    public Text _flowerKill;    //�ö���� Kill UI ����
    public Text _playerKill;    // �÷��̾�ų UI����


    // ü��ǥ�� UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;




    public void UpdateCarrotText(int value)
    {
        _myCarrotCount.text = value.ToString();
    }



}
