using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    // �� �̰� �̱��� �������� �������� 


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
    private static PlayerManager m_instance; // �̱����� �Ҵ�� ����




    // �÷��̾� ��Ȱ ����Ʈ ����Ʈ
    public List<ReviveSpotForPlayer> allReviveSpots = new List<ReviveSpotForPlayer>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�ʵ忡 �ִ� ��Ȱ ������ �����ϰ�, ���������� �̴´�.
     public Transform randomReviveSpot()
    {
        //�ʵ忡 �ִ� ��Ȱ ������ ����, null���� �����Ѵ�. 
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
