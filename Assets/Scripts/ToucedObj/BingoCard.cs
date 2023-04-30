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

    public bool completed; // 빙고칸이 달성되었는가?

    [SerializeField] GameObject CompletedStamp;
    [SerializeField] BingoPanel myParentBigoPanel;
    [SerializeField] PlayerLife myPlayerInfo;
    

    //빙고 아이템 인포
    int ItemNumber;
    eBingoItem myBingoItem;



    private void OnEnable()
    {
        CompletedStamp.SetActive(false);
        myPlayerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        myParentBigoPanel = GetComponentInParent<BingoPanel>();
    }



    //BingoPanel(부모)에서 쓰고 있는것.
    public void ShowBingoCardUI(eBingoItem whatItem, int ItemNum, bool isCompleted) 
    {
        string Name = whatItem.ToString();
        Debug.Log("생성해야하는 빙고아이템은 : " + Name);

        myBingoItem = whatItem;
        ItemNumber = ItemNum;
        SetImage(Name); // 빙고 이미지 생성
        SetBingoInfoMessage(Name, ItemNum);



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

        if (!completed)
        {
            if(myPlayerInfo.HasEnuoughItem(myBingoItem, ItemNumber))
            {
                SetCompletedStamp();
                myParentBigoPanel.GetNewClick(n);
            }
            else
            {
                AudioManager.instance.PlaySFX("BingoDisableClick");
                return;
            }
        }
       
       


    }

    public void SetCompletedStamp()
    {
        
        CompletedStamp.SetActive(true);
        completed = true;
    }
    


    void SetImage(string imageName) // 빙고 이미지 
    {
        if(imageName == "player")  // 싱글모드와 멀티모드일때 필요한 아이템 이미지 다르게 한다. 
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


    
}