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

    private static InputManager m_instance; // �̱����� �Ҵ�� ����


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
                // Debug.Log("���ڰ� Ŭ���Ǿ���. HASITouche ã��");
                if (!touchOn) // �ٸ� ITouchedObj�� ���õǾ��ִ� ��Ȳ�̶�� �ٸ� ITouchedObj�ۿ��� �Ͼ�� �ʵ��� �Ѵ�.
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
                    case eWhatTouched.chest: // ü��Ʈ�� ���õǾ��� �� 
                        
                        break;
                }

                }

            }
            //setClickObj(toucedObj); //Ŭ���� ��ü�� �±׿� ���� ����� �����Ѵ�.


            //switch (toucedObj.tag)
            //{
            //    case "chest": // ���⼭ eWhatTouched�� ���� �ٷ� ��Ʈ������ ���� ���� ������??
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


        // ����� Ŭ�� ����� ���⼭ ��������???? >>> �� Touch�� �ؾ��� �ʿ䰡 ������?
        //if (Input.touchCount > 0)
        //{
        //    //�̱���ġ�� ��������, �̵��ϸ鼭 ���ڸ� ���� ���� ����!

        //    hasTouch = Input.GetTouch(0);


        //    switch (hasTouch.phase)
        //    {
        //        case TouchPhase.Began:
        //            touchPos = Camera.main.ScreenToWorldPoint(hasTouch.position);
        //            RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.forward, maxDisatance);
        //            if (hit.collider.CompareTag("Chest"))
        //            {
        //                //Debug.Log("���ڰ� Ŭ���Ǿ���!");
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
