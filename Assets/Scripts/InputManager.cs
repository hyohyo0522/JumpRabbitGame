using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum eWhatTouched
{
    chest                   =0,


    othersNoMeaning        =99
}



public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update


    private Touch hasTouch;
    private Vector2 touchPos;
    private bool touchOn;
    float maxDisatance = 15f;

    eWhatTouched etouchedObj;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, transform.forward, maxDisatance);
            ITouchedObj hasTouchedObj = hit.collider.GetComponent<ITouchedObj>(); 

            if(hasTouchedObj != null)
            {
                hasTouchedObj.Touch();
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


        }


        // ����� Ŭ�� ����� ���⼭ ��������.
        if (Input.touchCount > 0)
        {
            //�̱���ġ�� ��������, �̵��ϸ鼭 ���ڸ� ���� ���� ����!

            hasTouch = Input.GetTouch(0);


            switch (hasTouch.phase)
            {
                case TouchPhase.Began:
                    touchPos = Camera.main.ScreenToWorldPoint(hasTouch.position);
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.forward, maxDisatance);
                    if (hit.collider.CompareTag("Chest"))
                    {
                        //Debug.Log("���ڰ� Ŭ���Ǿ���!");
                    }


                    break;
            }
        }

        //switch()
    }




    // switch�� �̿��ؼ� tag�� ���� ����� ��������.
    void setClickObj(GameObject touchedObj)
    {

        string touchedTag = touchedObj.tag;
        //etouchedObj = eWhatTouched.touchedTag;

        switch (touchedTag)
        {
            case "chest": // ���⼭ eWhatTouched�� ���� �ٷ� ��Ʈ������ ���� ���� ������??
                etouchedObj = eWhatTouched.chest;
                break;
            //case "chest":
            //    etouchedObj = eWhatTouched.chest;
            //    break;
            default:
                etouchedObj = eWhatTouched.othersNoMeaning;
                break;
        }

    }





}
