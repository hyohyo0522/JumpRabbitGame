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

    bool hasbingoItem; // ��� �������� ���� �Ǿ��°�?
    public bool completed; // ����ĭ�� �޼��Ǿ��°�?

    Button myButton;
    [SerializeField] GameObject CompletedStamp;
    [SerializeField] BingoPanel myParentBigoPanel;
    [SerializeField] PlayerLife myPlayerInfo;
    

    //���� ������ ����
    string whatItem;
    int ItemNumber;
    eBingoItem myBingoItem;

    // ���� �����ۼ� �ε���, ���⿡ *n�ؼ� �����ۼ��� ���� ���̴�.
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
    //    Debug.Log(itemName + "������ ������ ����Ǿ���.");


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

    //BingoPanel(�θ�)���� ���� �ִ°�.
    public void ShowBingoCardUI(eBingoItem whatItem, int ItemNum, bool isCompleted) 
    {
        string Name = whatItem.ToString();
        Debug.Log("�����ؾ��ϴ� ����������� : " + Name);

        myBingoItem = whatItem;
        SetImage(Name); // ���� �̹��� ����
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
        // �ڳ��߿� ��Ƽ�÷��̾� ��� �߰��� ��,
        // ���⼭ ��ư�� Ŭ���� �÷��̾��� ������ �����ؼ� BingoPanel�� ����
        // Chset�� �����ϴ� �޼ҵ带 �߰��ؾ��� �� ����. 

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
    


    void SetImage(string imageName) // ���� �̹��� 
    {
        if(imageName == "player") 
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

    void SetBingoItemInfo(string name, int value)
    {
        whatItem = name;
        ItemNumber = value;

    }

    
}