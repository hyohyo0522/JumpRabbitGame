using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContinueBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //ColorBlock OnEnterColor = new Color(0f, 253f, 3f, 253f);

    Color ButtonColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeButtonColor("#00FD03");
        this.gameObject.GetComponent<Image>().color = ButtonColor;

    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void ChangeButtonColor(string hexcod)
    {

        ColorUtility.TryParseHtmlString(hexcod, out ButtonColor);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeButtonColor("#FFFFFF");
        this.gameObject.GetComponent<Image>().color = ButtonColor;
    }
}
