using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public InputField myNickname;
    string _myNick;

    public Text _guideMsg;
    string firstGuide;

    private void Start()
    {
        firstGuide = _guideMsg.text;
    }



    public void InputEnded()
    {
        if (checkstringNull(myNickname.text.ToString()))
        {
            _guideMsg.text = firstGuide;
            return;

        }
        _myNick = myNickname.text;
        _guideMsg.text = "Is " + _myNick.ToString()+ " yours, Press Start";
           
        //PlayerPrefs.SetString("MyNick", myNickname);
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
            _guideMsg.text = "�г����� �Էµ��� �ʾҽ��ϴ�.";
            return;

        }

        PlayerPrefs.SetString("_myNick", _myNick);
        PlayerPrefs.SetString("Mode", "Single"); //���� ���� �̱۷� ������.

        SceneManager.LoadScene("Stage_1");

    }

}
