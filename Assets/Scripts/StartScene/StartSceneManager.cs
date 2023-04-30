using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public InputField myNickname;
    string _myNick;

    public Text _guideMsg; //�ν����� �Ҵ�
    string firstGuide = "Please input your nickname";
    string noNickNameMsg = "�г����� �Էµ��� �ʾҽ��ϴ�.";

    public void InputEnded()
    {
        if (checkstringNull(myNickname.text.ToString()))
        {
            _guideMsg.text = firstGuide;
            return;

        }
        _myNick = myNickname.text;
        _guideMsg.text = "�ȳ�! " + _myNick.ToString()+ ", Start�� ������!";
           
    }

    private bool checkstringNull(string txt)
    {
        bool isNull = false;
        if(txt.Length == 0)
        {
            isNull = true;
        }
        return isNull;
    }

    public void PressStartForSinglePlay()
    {
        if (checkstringNull(myNickname.text.ToString()))
        {
            _guideMsg.text = noNickNameMsg;
            return;

        }

        PlayerPrefs.SetString("_myNick", _myNick);
        PlayerPrefs.SetString("Mode", "Single"); //���� ���� �̱۷� ������.

        SceneManager.LoadScene("Map_1");

    }

}
