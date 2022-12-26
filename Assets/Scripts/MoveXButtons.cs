using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveXButtons : MonoBehaviour,IPointerUpHandler,IPointerExitHandler,IPointerDownHandler
{
    private Image thisImg;
    public GameObject checkPosition;
    public GameObject rectChecek; // ����
    public GameObject normalTransform; // �ʷ�
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
        if (isMoving) //�ӵ��� ���� �ö󰡾���
        {
            switch (moveDirection)
            {
                case -1: //����
                    if (movePower > moveDirection)
                    {
                        movePower -= Time.deltaTime*moveSpeed;
                    }
                    break;
                case 1: // ������
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

    public void OnClickButton(int index) // ���� ��ư�� ���������� �������� �ǰ��� �ϱ� 
    {
        if (!isMoving)
        {
            isMoving = true;
            movePower = 0;

        }
  
        moveDirection = index; // ���Ⱚ ����.
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
