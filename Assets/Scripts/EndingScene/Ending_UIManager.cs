using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ending_UIManager : MonoBehaviour
{
    public static Ending_UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<Ending_UIManager>();
            }

            return m_instance;
        }
    }

    private static Ending_UIManager m_instance; // 싱글톤이 할당될 변수


    // 게임 승패 결과 관련 변수
    [SerializeField] string winnerNick;
    [SerializeField] Text houseOwner; // 인스펙터에서 할당한다.

    //모바일버튼 플레이어이동관련 

    float movePower;
    [SerializeField] float moveSpeed = 5f;



    private void Start()
    {
        winnerNick = PlayerPrefs.GetString("Winner"); // 우승자 이름 저장
        houseOwner.text = winnerNick;  // 우승자 이름을 집에 표시

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


    public void QuitApp()
    {
        Application.Quit();
    }

    public void PressRestatBtn()
    {
        SceneManager.LoadScene("StartScene");
    }


}
