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

    Button myButton;
    [SerializeField] GameObject CompletedStamp;

    //���� ������ ����
    string whatItem;
    int ItemNumber;

    // ���� �����ۼ� �ε���, ���⿡ *n�ؼ� �����ۼ��� ���� ���̴�.
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
        Debug.Log(itemName + "������ ������ ����Ǿ���.");


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
        SetImage(Name); // ���� �̹��� ����
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
    


    void SetImage(string imageName) // ���� �̹��� 
    {

        Sprite imageFile = Resources.Load<Sprite>(imageName) as Sprite;
        itemImage.sprite = imageFile;

    }

    void SetBingoInfoMessage(string itemName, int value)
    {
        bingoInfo.text = "x " + value + "��";
    }

    void SetBingoItemInfo(string name, int value)
    {
        whatItem = name;
        ItemNumber = value;

    }

    
}