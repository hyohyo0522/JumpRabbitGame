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

    Button myButton;
    [SerializeField] GameObject CompletedStamp;

    //빙고 아이템 인포
    string whatItem;
    int ItemNumber;

    // 빙고 아이템수 인덱스, 여기에 *n해서 아이템수를 정할 것이다.
    int itemValueIndex;


    private void OnEnable()
    {
        CompletedStamp.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();

        //Sprite imageFile = Resources.Load<Sprite>("flower") as Sprite;
        //itemImage.sprite = imageFile;
    }

    // Update is called once per frame
    void Update()
    {
        SetBingoItem();
    }
    

    void SetBingoItem()
    {
        if (hasbingoItem) { return; }

        string itemName = "N";


        int eBingoIndex = Random.Range(0, 5);
        myBingoItem = (eBingoItem)eBingoIndex;

        itemName = myBingoItem.ToString();
        Debug.Log(itemName + "아이템 네임이 적용되었다.");


        if (itemName != "N")
        {
            //int ItemRandomNumber = Random.Range(5, 10);


            SetImage(itemName);
            setInfoByEnum();
            SetBingoInfoMessage(itemName, itemValueIndex);
            SetBingoItemInfo(itemName, itemValueIndex);

            hasbingoItem = true;

        }


    }


    void ShowBingoCardUI(eBingoItem whatItem, int ItemNum, bool completed)
    {
        string Name = whatItem.ToString();
        SetImage(Name); // 빙고 이미지 생성
        SetBingoInfoMessage(Name, ItemNum);
        SetBingoItemInfo(name, ItemNum);

        hasbingoItem = true;

    }


    public void SetCompletedStamp()
    {

        CompletedStamp.SetActive(true);
    }

    void setInfoByEnum()
    {
        itemValueIndex = Random.RandomRange(1, 6);
        switch (myBingoItem)
        {
            case eBingoItem.carrot:
                itemValueIndex *= 5;
                break;
            case eBingoItem.flower:
                itemValueIndex *= 3;
                break;
            case eBingoItem.slug:
                itemValueIndex *= 2;
                break;
            case eBingoItem.star:
                itemValueIndex *= 2;
                break;
            case eBingoItem.player:
                break;
        }
    }
    


    void SetImage(string imageName) // 빙고 이미지 
    {

        Sprite imageFile = Resources.Load<Sprite>(imageName) as Sprite;
        itemImage.sprite = imageFile;

    }

    void SetBingoInfoMessage(string itemName, int value)
    {
        bingoInfo.text = "x " + value + "개";
    }

    void SetBingoItemInfo(string name, int value)
    {
        whatItem = name;
        ItemNumber = value;

    }

    
}