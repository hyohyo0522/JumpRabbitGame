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


        }


        // 모바일 클릭 기능을 여기서 구현하자.
        if (Input.touchCount > 0)
        {
            //싱글터치로 구현하자, 이동하면서 상자를 열수 없게 하자!

            hasTouch = Input.GetTouch(0);


            switch (hasTouch.phase)
            {
                case TouchPhase.Began:
                    touchPos = Camera.main.ScreenToWorldPoint(hasTouch.position);
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.forward, maxDisatance);
                    if (hit.collider.CompareTag("Chest"))
                    {
                        //Debug.Log("상자가 클릭되었다!");
                    }


                    break;
            }
        }

        //switch()
    }




    // switch문 이용해서 tag에 따라 기능을 실행하자.
    void setClickObj(GameObject touchedObj)
    {

        string touchedTag = touchedObj.tag;
        //etouchedObj = eWhatTouched.touchedTag;

        switch (touchedTag)
        {
            case "chest": // 여기서 eWhatTouched의 값을 바로 스트링으로 넣을 수는 없을까??
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
