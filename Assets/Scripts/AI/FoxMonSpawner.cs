using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMonSpawner : MonoBehaviour
{

    public static FoxMonSpawner instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<FoxMonSpawner>();
            }

            return m_instance;
        }
    }
    private static FoxMonSpawner m_instance; // 싱글톤이 할당될 변수



    public GameObject foxMonPrefab;
    public Collider2D groundCollider; // 땅콜라이더를 할당해서 foxMonPrefab에 할당해주도록 한다.
    [SerializeField] List<AIFoxController> foxMonsEnabled = new List<AIFoxController>(); //생성된 여우몬들을 담을 리스트

    float timeBetFoxInstantiate = 10f; // 생성주기
    public int maxFoxNum; // 최대 여우 몬스터 생성 수
    private int currentNumOfFox; // 현재 폭스몬스터생성수


    // Start is called before the first frame update
    void Start()
    {
        maxFoxNum = 5;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeBetFoxInstantiate -= Time.deltaTime;

        if (timeBetFoxInstantiate <=0)
        {
            ResetCurrentNumOfFox();
            if (currentNumOfFox < maxFoxNum)
            {
                Vector2 newSpot = FlowerEnemySpawner.instance.randomEmptyFileds().flowerSpot.position;
                CreateFoxMon(newSpot);
            }
            timeBetFoxInstantiate = 4.5f;

        }
    }

    private void CreateFoxMon(Vector2 spot) // 여우가 생성되는 위치는 플라워 몬스터들의 생성위치로 하겠다.
    {
        GameObject newfox = Instantiate(foxMonPrefab, spot, Quaternion.identity);
        AIFoxController newFoxMonSC = newfox.GetComponent<AIFoxController>();
        newFoxMonSC.groundCollider = this.groundCollider;// 땅 지정
        foxMonsEnabled.Add(newFoxMonSC);

        newFoxMonSC.OnDeath += () => foxMonsEnabled.Remove(newFoxMonSC);


    }


    private int ResetCurrentNumOfFox()
    {
        currentNumOfFox = 0;

        for(int n=0; n < foxMonsEnabled.Count; n++)
        {
            if (foxMonsEnabled[n] != null)
            {
                currentNumOfFox++;
            }
        }

        return currentNumOfFox;
        
    }




}
