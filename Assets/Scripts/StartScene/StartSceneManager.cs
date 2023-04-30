using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public InputField myNickname;
    string _myNick;

    public Text _guideMsg; //인스펙터 할당
    string firstGuide = "Please input your nickname";
    string noNickNameMsg = "닉네임이 입력되지 않았습니다.";

    public void InputEnded()
    {
        if (checkstringNull(myNickname.text.ToString()))
        {
            _guideMsg.text = firstGuide;
            return;

        }
        _myNick = myNickname.text;
        _guideMsg.text = "안녕! " + _myNick.ToString()+ ", Start를 눌러요!";
           
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
        PlayerPrefs.SetString("Mode", "Single"); //게임 모드는 싱글로 설정됨.

        SceneManager.LoadScene("Map_1");

    }

}
