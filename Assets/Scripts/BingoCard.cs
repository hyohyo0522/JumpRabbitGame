using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eBingoItem
{
    carrot      = 0,
    star        = 1,
    flower      = 2,
    slug        = 3,
    player      = 4

}

public class BingoCard : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Text bingoInfo;
    [SerializeField] Image completedImage;
    eBingoItem myBingoItem;
    bool hasbingoItem; // 방고 아이템이 세팅 되었는가?
    bool completed; // 빙고칸이 달성되었는가?

    //빙고 아이템 인포
    string whatItem;
    int ItemNumber;
    

    // Start is called before the first frame update
    void Start()
    {
        Sprite imageFile = Resources.Load<Sprite>("flower") as Sprite;
        itemImage.sprite = imageFile;
    }

    // Update is called once per frame
    void Update()
    {
        SetBingoItem();
    }

    void SetBingoItem()
    {
        if (!hasbingoItem) { return; }

        string itemName = "N";


        int eBingoIndex = Random.Range(0, 5);
        myBingoItem = (eBingoItem)eBingoIndex;
        itemName = myBingoItem.ToString();


        if (itemName != "N")
        {
            int ItemRandomNumber = Random.Range(5, 10);


            SetImage(itemName);
            SetBingoInfoMessage(itemName, ItemRandomNumber);
            SetBingoItemInfo(itemName, ItemRandomNumber);

            hasbingoItem = true;

        }


    }

    void SetImage(string imageName) // 빙고 이미지 
    {
        Debug.Log(imageName+ "SetImage 작동한다!");
        Sprite imageFile = Resources.Load<Sprite>("Resources/" + imageName) as Sprite;
        itemImage.sprite = imageFile;
    }

    void SetBingoInfoMessage(string itemName, int value)
    {
        bingoInfo.text = itemName + value + "개";
    }

    void SetBingoItemInfo(string name, int value)
    {
        whatItem = name;
        ItemNumber = value;

    }

    
}