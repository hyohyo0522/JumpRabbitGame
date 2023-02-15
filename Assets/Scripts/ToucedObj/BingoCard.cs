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

    bool hasbingoItem; // 방고 아이템이 세팅 되었는가?
    public bool completed; // 빙고칸이 달성되었는가?

    Button myButton;
    [SerializeField] GameObject CompletedStamp;
    [SerializeField] BingoPanel myParentBigoPanel;
    [SerializeField] PlayerLife myPlayerInfo;
    

    //빙고 아이템 인포
    string whatItem;
    int ItemNumber;
    eBingoItem myBingoItem;

    // 빙고 아이템수 인덱스, 여기에 *n해서 아이템수를 정할 것이다.
    int itemValueIndex;


    private void OnEnable()
    {
        CompletedStamp.SetActive(false);
        myPlayerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        myParentBigoPanel = GetComponentInParent<BingoPanel>();
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
        //SetBingoItem();
    }


    //void SetBingoItem( )
    //{
    //    if (hasbingoItem) { return; }

    //    string itemName = "N";


    //    int eBingoIndex = Random.Range(0, 5);
    //    myBingoItem = (eBingoItem)eBingoIndex;

    //    itemName = myBingoItem.ToString();
    //    Debug.Log(itemName + "아이템 네임이 적용되었다.");


    //    if (itemName != "N")
    //    {
    //        //int ItemRandomNumber = Random.Range(5, 10);


    //        SetImage(itemName);
    //        setInfoByEnum();
    //        SetBingoInfoMessage(itemName, itemValueIndex);
    //        SetBingoItemInfo(itemName, itemValueIndex);

    //        hasbingoItem = true;

    //    }


    //}

    //BingoPanel(부모)에서 쓰고 있는것.
    public void ShowBingoCardUI(eBingoItem whatItem, int ItemNum, bool isCompleted) 
    {
        string Name = whatItem.ToString();
        Debug.Log("생성해야하는 빙고아이템은 : " + Name);

        myBingoItem = whatItem;
        SetImage(Name); // 빙고 이미지 생성
        SetBingoInfoMessage(Name, ItemNum);
        SetBingoItemInfo(name, ItemNum);

        hasbingoItem = true;


        if (isCompleted)
        {
            SetCompletedStamp();
        }
        completed = isCompleted;


    }


    public void OnClick(int n)
    {
        // ★나중에 멀티플레이어 기능 추가될 때,
        // 여기서 버튼을 클릭한 플레이어의 정보를 저장해서 BingoPanel을 통해
        // Chset에 저장하는 메소드를 추가해야할 것 같다. 

        if (myPlayerInfo.HasEnuoughItem(myBingoItem, ItemNumber) == false)
        {
            AudioManager.instance.PlaySFX("BingoDisableClick");
            return;
        }


        if (!completed)
        {
            SetCompletedStamp();
            myParentBigoPanel.GetNewClick(n);

        }

       


    }

    public void SetCompletedStamp()
    {
        
        CompletedStamp.SetActive(true);
        completed = true;
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
        if(imageName == "player") 
        {
            string gameMode = PlayerPrefs.GetString("Mode");
            if (gameMode == "Single") // 싱글게임모드  : 여우몬스터 이미지 빙고
            {
                Sprite imageFile = Resources.Load<Sprite>("fox") as Sprite;
                itemImage.sprite = imageFile;
            }
            if(gameMode == "Multi") //멀티게임모드 : 플레이어 이미지 몬스터 빙고
            {
                Sprite imageFile = Resources.Load<Sprite>("player") as Sprite;
                itemImage.sprite = imageFile;
            }

        }
        else
        {

            Sprite imageFile = Resources.Load<Sprite>(imageName) as Sprite;
            itemImage.sprite = imageFile;
        }

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