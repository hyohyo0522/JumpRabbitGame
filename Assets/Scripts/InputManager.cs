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
    public bool touchOn;
    float maxDisatance = 15f;

    eWhatTouched now_etouchedObj;


    //����â
    [SerializeField] GameObject ChestPanelUI; // ����â
    [SerializeField] Image[] normalCoinButtons = new Image[3];
    [SerializeField] GameObject ComboCoinButton;


    //InputManager���� �����ϴ� â���� ����Ʈ�� ��Ƴ��´�.
    [SerializeField] GameObject myCanvas; // ������Ʈ â���� �Ҵ��� ���̴�. 
    [SerializeField] List<GameObject> CancleButtonFam = new List<GameObject>(); // ������Ʈ â���� �Ҵ��� ���̴�. 

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
                // Debug.Log("���ڰ� Ŭ���Ǿ���. HASITouche ã��");
                if (!touchOn) // �ٸ� ITouchedObj�� ���õǾ��ִ� ��Ȳ�̶�� �ٸ� ITouchedObj�ۿ��� �Ͼ�� �ʵ��� �Ѵ�.
                {
                    IhasTouchedObj.Touch();
                    touchOn = true;
                    now_etouchedObj = IhasTouchedObj.Return_WhatTouched();
                    return;
                }
                else
                {
                    // �� Cancle ��ư�� ������(=�ش� ����â�� ������) 
                    // ��� touchOn�� True�� �����ִ� ��찡 �־ 
                    // �̷� ��쵵 â�� ��� ���� �� �ֵ��� �ڵ带 �߰�.

                    // ActiveSelf�� �ش��ϴ� ���� â���� ���� �˻��Ѵ�.
                    if (hit.collider.CompareTag("Chest")) // ����â�� �������� �̰� Enum���ε� ���� �� ���� �� ����. 
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
