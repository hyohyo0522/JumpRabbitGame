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
    bool hasbingoItem;
    bool completed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetBingoItem(eBingoItem myItem)
    {
        if (!completed) { return; }

        switch (myItem)
        {
            case eBingoItem.carrot:
                break;
            case eBingoItem.star:
                break;
            case eBingoItem.flower:
                break;
            case eBingoItem.slug:
                break;
            case eBingoItem.player:
                break;
        }
    }

    void setImage(string imageName)
    {
        itemImage = Resources.Load("Resources/" + imageName) as Image;
    }
}
