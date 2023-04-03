using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // ★ 이거 싱글톤 패턴으로 만들어야함 


    public static PlayerManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PlayerManager>();
            }

            return m_instance;
        }
    }
    private static PlayerManager m_instance; // 싱글톤이 할당될 변수




    // 플레이어 부활 포인트 리스트
    public List<ReviveSpotForPlayer> allReviveSpots = new List<ReviveSpotForPlayer>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //필드에 있는 부활 스팟을 갱신하고, 랜덤스팟을 뽑는다.
     public Transform randomReviveSpot()
    {
        //필드에 있는 부활 스팟을 갱신, null값은 삭제한다. 
        for (int n = 0; n < allReviveSpots.Count; n++)
        {
            if (allReviveSpots[n] == null)
            {
                allReviveSpots.RemoveAt(n);
            }
        }

        int index = Random.Range(0, allReviveSpots.Count);
        Transform newRevivePosition = allReviveSpots[index].reviveSpot;

        return newRevivePosition;
    }
}
