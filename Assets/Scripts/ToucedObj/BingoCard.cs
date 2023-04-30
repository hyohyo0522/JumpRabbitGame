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

    public bool completed; // ����ĭ�� �޼��Ǿ��°�?

    [SerializeField] GameObject CompletedStamp;
    [SerializeField] BingoPanel myParentBigoPanel;
    [SerializeField] PlayerLife myPlayerInfo;
    

    //���� ������ ����
    int ItemNumber;
    eBingoItem myBingoItem;



    private void OnEnable()
    {
        CompletedStamp.SetActive(false);
        myPlayerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        myParentBigoPanel = GetComponentInParent<BingoPanel>();
    }



    //BingoPanel(�θ�)���� ���� �ִ°�.
    public void ShowBingoCardUI(eBingoItem whatItem, int ItemNum, bool isCompleted) 
    {
        string Name = whatItem.ToString();
        Debug.Log("�����ؾ��ϴ� ����������� : " + Name);

        myBingoItem = whatItem;
        ItemNumber = ItemNum;
        SetImage(Name); // ���� �̹��� ����
        SetBingoInfoMessage(Name, ItemNum);



        if (isCompleted)
        {
            SetCompletedStamp();
        }
        completed = isCompleted;


    }


    public void OnClick(int n)
    {
        // �ڳ��߿� ��Ƽ�÷��̾� ��� �߰��� ��,
        // ���⼭ ��ư�� Ŭ���� �÷��̾��� ������ �����ؼ� BingoPanel�� ����
        // Chset�� �����ϴ� �޼ҵ带 �߰��ؾ��� �� ����. 

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
    


    void SetImage(string imageName) // ���� �̹��� 
    {
        if(imageName == "player")  // �̱۸��� ��Ƽ����϶� �ʿ��� ������ �̹��� �ٸ��� �Ѵ�. 
        {
            string gameMode = PlayerPrefs.GetString("Mode");
            if (gameMode == "Single") // �̱۰��Ӹ��  : ������� �̹��� ����
            {
                Sprite imageFile = Resources.Load<Sprite>("fox") as Sprite;
                itemImage.sprite = imageFile;
            }
            if(gameMode == "Multi") //��Ƽ���Ӹ�� : �÷��̾� �̹��� ���� ����
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
        bingoInfo.text = "x " + value + "��";
    }


    
}