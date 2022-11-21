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
    bool hasbingoItem; // ��� �������� ���� �Ǿ��°�?
    bool completed; // ����ĭ�� �޼��Ǿ��°�?

    //���� ������ ����
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

    void SetImage(string imageName) // ���� �̹��� 
    {
        Debug.Log(imageName+ "SetImage �۵��Ѵ�!");
        Sprite imageFile = Resources.Load<Sprite>("Resources/" + imageName) as Sprite;
        itemImage.sprite = imageFile;
    }

    void SetBingoInfoMessage(string itemName, int value)
    {
        bingoInfo.text = itemName + value + "��";
    }

    void SetBingoItemInfo(string name, int value)
    {
        whatItem = name;
        ItemNumber = value;

    }

    
}