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
    private bool touchOn;
    float maxDisatance = 15f;

    eWhatTouched now_etouchedObj;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, transform.forward, maxDisatance);
            ITouchedObj IhasTouchedObj = hit.collider.GetComponent<ITouchedObj>(); 
            

            if(IhasTouchedObj != null)
            {
                // Debug.Log("상자가 클릭되었다. HASITouche 찾음");
                if (!touchOn) // 다른 ITouchedObj가 선택되어있는 상황이라면 다른 ITouchedObj작용이 일어나지 않도록 한다.
                {
                    IhasTouchedObj.Touch();
                    touchOn = true;
                    now_etouchedObj = IhasTouchedObj.Return_WhatTouched();
                    return;
                }

            }



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
        if (touchOn)
        {
            touchOn = false;
            now_etouchedObj = eWhatTouched.othersNoMeaning;

        
        }

    }




}
