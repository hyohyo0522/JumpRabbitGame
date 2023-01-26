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

    //모바일버튼 플레이어이동관련 


    float movePower;
    [SerializeField] float moveSpeed = 5f;


    private void OnEnable() 
    {

        //솔로플레이 모드인지, 멀티플레이어 모드인지에 따라 _playerKill 이미지 다르게 하자! 
        GameObject _IconImageForMultiPlay = _playerKillUI.transform.GetChild(1).gameObject;
        GameObject _IconImageForSinglePlay = _playerKillUI.transform.GetChild(2).gameObject;

        string _myGameMode = PlayerPrefs.GetString("Mode");

        if(_myGameMode == "Single") //게임모드가 싱글일때 설정
        {
            _IconImageForMultiPlay.SetActive(false);
            _IconImageForSinglePlay.SetActive(true);
        }
        if(_myGameMode == "Multi") //게임모드가 멀티일 때 설정
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

}
