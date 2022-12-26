using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveXButtons : MonoBehaviour,IPointerUpHandler,IPointerExitHandler,IPointerDownHandler
{
    private Image thisImg;
    public GameObject checkPosition;
    public GameObject rectChecek; // 빨강
    public GameObject normalTransform; // 초록
    private Vector3 inputVector;

  


    [SerializeField] bool isMoving;
    [SerializeField] float moveSpeed=2f;
    int moveDirection;
    static float movePower;
    


    // Start is called before the first frame update
    void Start()
    {
        thisImg = GetComponent<Image>();
        Debug.Log(thisImg.rectTransform.position.x);
        Debug.Log(thisImg.rectTransform.position.y);

        Vector2 recrPosition = new Vector2();
        recrPosition.x = thisImg.rectTransform.position.x;
        recrPosition.y = thisImg.rectTransform.position.y;
        checkPosition.GetComponent<RectTransform>().transform.position = recrPosition;
        normalTransform.transform.position = recrPosition;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving) //속도가 점점 올라가야함
        {
            switch (moveDirection)
            {
                case -1: //왼쪽
                    if (movePower > moveDirection)
                    {
                        movePower -= Time.deltaTime*moveSpeed;
                    }
                    break;
                case 1: // 오른쪽
                    if (movePower < moveDirection)
                    {
                        movePower += Time.deltaTime*moveSpeed;
                    }
                    break;

            }
            Debug.Log(movePower);
        }

    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Vector2 pos;
    //    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(thisImg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
    //    {

    //    }

    //    inputVector = eventData.position;
    //    ////temptCamera = eventData.pressEventCamera.gameObject;

    //    Debug.Log(eventData.position);



    //}

    public void OnClickButton(int index) // 눌린 버튼이 오른쪽인지 왼쪽인지 판가름 하기 
    {
        if (!isMoving)
        {
            isMoving = true;
            movePower = 0;

        }
  
        moveDirection = index; // 방향값 저장.
        movePower = 0;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMoving = false;
        movePower = 0f;


    }

    public static float getHorizontalValue()
    {
        return movePower;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (!isMoving)
    //    {
    //        isMoving = true;
    //    }
    //    //Vector2 pos;
    //    //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(thisImg.rectTransform, eventData.position,
    //    //    eventData.pressEventCamera, out pos))
    //    //{
    //    //    pos.x = (pos.x / thisImg.rectTransform.sizeDelta.x);
    //    //    pos.y = (pos.y / thisImg.rectTransform.sizeDelta.y);

    //    //    inputVector = new Vector3(pos.x * 2 + 1, pos.ys * 2 - 1, 0);
    //    //    inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

    //    //}

    //    //Debug.Log(eventData.position);
    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        isMoving = false;
        movePower = 0f;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isMoving)
        {
            isMoving = true;
            

        }
    }
}
