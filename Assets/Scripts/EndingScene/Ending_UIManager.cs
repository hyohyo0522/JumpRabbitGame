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

    private static Ending_UIManager m_instance; // �̱����� �Ҵ�� ����


    // ���� ���� ��� ���� ����
    [SerializeField] string winnerNick;
    [SerializeField] Text houseOwner; // �ν����Ϳ��� �Ҵ��Ѵ�.

    //����Ϲ�ư �÷��̾��̵����� 

    float movePower;
    [SerializeField] float moveSpeed = 5f;



    private void Start()
    {
        winnerNick = PlayerPrefs.GetString("Winner"); // ����� �̸� ����
        houseOwner.text = winnerNick;  // ����� �̸��� ���� ǥ��

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


    public void QuitApp()
    {
        Application.Quit();
    }

    public void PressRestatBtn()
    {
        SceneManager.LoadScene("StartScene");
    }


}
