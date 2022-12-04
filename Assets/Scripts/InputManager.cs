using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;



public enum eWhatTouched
{
    chest                   =0,


    othersNoMeaning        =99
}



public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static InputManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<InputManager>();
            }

            return m_instance;
        }
    }

    private static InputManager m_instance; // 싱글톤이 할당될 변수


    private Touch hasTouch;
    private Vector2 touchPos;
    public bool touchOn;
    float maxDisatance = 15f;

    eWhatTouched now_etouchedObj;


    //게임창
    [SerializeField] GameObject ChestPanelUI; // 빙고창
    [SerializeField] Image[] normalCoinButtons = new Image[3];
    [SerializeField] GameObject ComboCoinButton;


    //InputManager에서 관리하는 창들의 리스트로 담아놓는다.
    [SerializeField] GameObject myCanvas; // 컴포넌트 창에서 할당할 것이다. 
    [SerializeField] List<GameObject> CancleButtonFam = new List<GameObject>(); // 컴포넌트 창에서 할당할 것이다. 

    private void Awake()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, transform.forward, maxDisatance);


            if (hit)
            {
                ITouchedObj IhasTouchedObj = hit.collider.GetComponent<ITouchedObj>();
                // Debug.Log("상자가 클릭되었다. HASITouche 찾음");
                if (!touchOn) // 다른 ITouchedObj가 선택되어있는 상황이라면 다른 ITouchedObj작용이 일어나지 않도록 한다.
                {
                    IhasTouchedObj.Touch();
                    touchOn = true;
                    now_etouchedObj = IhasTouchedObj.Return_WhatTouched();
                    return;
                }
                else
                {
                    // ★ Cancle 버튼을 눌러도(=해당 게임창이 꺼져도) 
                    // 계속 touchOn이 True로 남아있는 경우가 있어서 
                    // 이런 경우도 창이 계속 켜질 수 있도록 코드를 추가.

                    // ActiveSelf로 해당하는 게임 창들을 직접 검사한다.
                    if (hit.collider.CompareTag("Chest")) // 게임창이 많아지면 이걸 Enum으로도 만들 수 있을 것 같다. 
                    {
                        if (!ChestPanelUI.activeSelf)
                        {
                            touchOn = false;
                        }
                    }
                    

                }




            }

            // 



            if (touchOn) 
            {

                switch (now_etouchedObj)
                {
                    case eWhatTouched.chest: // 체스트가 선택되었을 때 

                        break;
                }

                }

            }
            //setClickObj(toucedObj); //클릭한 물체의 태그에 따라 기능을 실현한다.


            //switch (toucedObj.tag)
            //{
            //    case "chest": // 여기서 eWhatTouched의 값을 바로 스트링으로 넣을 수는 없을까??
            //        etouchedObj = eWhatTouched.chest;
            //        Chest touchedChest = toucedObj.GetComponent<Chest>();

            //        break;
            //    //case "chest":
            //    //    etouchedObj = eWhatTouched.chest;
            //    //    break;
            //    default:
            //        etouchedObj = eWhatTouched.othersNoMeaning;
            //        break;
            //}


        //}


        // 모바일 클릭 기능을 여기서 구현하자???? >>> 꼭 Touch로 해야할 필요가 있을까?
        //if (Input.touchCount > 0)
        //{
        //    //싱글터치로 구현하자, 이동하면서 상자를 열수 없게 하자!

        //    hasTouch = Input.GetTouch(0);


        //    switch (hasTouch.phase)
        //    {
        //        case TouchPhase.Began:
        //            touchPos = Camera.main.ScreenToWorldPoint(hasTouch.position);
        //            RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.forward, maxDisatance);
        //            if (hit.collider.CompareTag("Chest"))
        //            {
        //                //Debug.Log("상자가 클릭되었다!");
        //            }


        //            break;
        //    }
        //}

        //switch()
    }

    public void ExitITouchedObjPanel()
    {

            touchOn = false;
            now_etouchedObj = eWhatTouched.othersNoMeaning;


    }


    public void ClickAllCancleFamButton()
    {
        for(int i=0; i< CancleButtonFam.Count; i++)
        {

                CancleButtonFam[i].GetComponent<Button>().onClick.Invoke();

        }

        if (touchOn)
        {
            touchOn = false;
        }
    }


}
