using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class  UIManager : MonoBehaviour
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

    public Text _scoreTxt;     // 스타UI 관련
    public Text _moneyTxt;      //돈 UI관련
    public Text _slugText; // 슬러그 겟 관련 UI
    public Text _flowerKill;    //플라워몬 Kill UI 관련
    public Text _playerKill;    // 플레이어킬 UI관련


    // 체력표시 UI
    public Slider _myHeathSlider;
    public Text _myHealthValue;




    public void UpdateCarrotText(int value)
    {
        _myCarrotCount.text = value.ToString();
    }



}
