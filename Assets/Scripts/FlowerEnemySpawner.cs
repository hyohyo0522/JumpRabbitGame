using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlowerEnemySpawner : MonoBehaviour
{

    public static FlowerEnemySpawner instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<FlowerEnemySpawner>();
            }

            return m_instance;
        }
    }

    private static FlowerEnemySpawner m_instance; // 싱글톤이 할당될 변수

    // ★ 이거 싱글톤 패턴으로 만들어야함 

    public GameObject flowerPrefab;
    private static List<Flowerfield> allflowerFields = new List<Flowerfield>(); // 모든 플라워몬스터 필드 담을 리스터
    private static List<Flowerfield> emptyFileds = new List<Flowerfield>(); // 플라워 몬스터가 생성 안된 플라워 필드를 담을 리스트
    private static List<FlowerEnemy> enemies = new List<FlowerEnemy>(); // 생성된 플라워 몬스터들을 담는 리스트
    
    private int maxFlower=0; // 최대 꽃 몬스터 생성 수
    private float percentForFlowers= 0.7f; // 생성지역의 플라워 몬스터 생성비율
    private int currentNum=0; // 현재 꽃 몬스터 생성 수 
    float timeBetFlowerInstantiate = 4.5f;

    // 정보은닉

    // C#, C++
    // C++ -> C++ (변환과정 알고 있는 동작 100% 똑같다고 보장 X)

    // C++ 변수 선언하고 초기화 X, 쓰레기 값
    // int asd = 0; 0이 안들어감 random

    // [Window - Unity Editor]
    // C# -> dll -> Unity (dll)

    // [Android - Unity]
    // C++ 
    // C#  -> il2cpp -> C++ -> NDK, ~ (Unity) -> so -> APK

    // [Android - Native]
    // Java
    // Kotlin



    // Start is called before the first frame update
    void Start()
    {
        updateMaxFlowers(); // 최대 꽃 몬스터 생성수 갱신

    }

    // Update is called once per frame
    void Update()
    {

        timeBetFlowerInstantiate -= Time.deltaTime;

        // 생성주기때마다 최대몬스터 갯수보다 플라워몬스터가 없으면 몬스터를 생성한다. 
        if (timeBetFlowerInstantiate <= 0)
        {
            Debug.Log("Check Random Flower Start");

            //최대 꽃몬스터 수 비교후에 몬스터 생성한다. 
            Flowerfield monsterSpot = randomEmptyFileds();

            Debug.Log($"Check Random Flower End [Current : {currentNum}] [MaxFlower : {maxFlower}]");

            if (currentNum< maxFlower)
            {
                Debug.Log("Create Flower Begin");
                CreateFlowers(monsterSpot);
                Debug.Log("Create Flower End");
            }

            timeBetFlowerInstantiate = 4.5f;
        }

    }



    //플라워 생성 메서드 
    private void CreateFlowers(Flowerfield spot)
    {
        if (spot == null)
        {
            Debug.Log("CreateFlower spot null!!");
        }

        Transform flowerSpot = spot.flowerSpot;
        if(flowerSpot != null)
        {
            // 진짜 잘 집어넣어졌을까???
            if (flowerPrefab == null)
            {
                Debug.Log("CreateFlower flowerPrefab null!!");
            }

            // position이 화면 내 잘나오는 위치가 맞나?
            GameObject enemyObject = Instantiate(flowerPrefab, flowerSpot.position, Quaternion.identity);
            FlowerEnemy flowrEnemy = enemyObject.GetComponent<FlowerEnemy>();
            enemies.Add(flowrEnemy);

            Debug.Log($"CreateFlower Position : {flowrEnemy.transform.position}");

            spot.DoeeHaveFlowerMon(true);
            //Flowerfield thisFlowerSpot = spot.GetComponent<Flowerfield>();
            //thisFlowerSpot.hasFlowerMon = true;

            // GetComponent <-- 많이 쓰면 느려짐
            //죽을 때, 리스트에서 삭제

            var entity = enemyObject.GetComponent<LivingEntity>();
            entity.OnDeath += () => enemies.Remove(flowrEnemy);
            entity.OnDeath += () => spot.DoeeHaveFlowerMon(false);
            //플라워몬스터를 만든 공간에 플라워가 지금 없다는 것을 표시
        }
        else
        {
            Debug.LogError("CreateFlower object null!!");
        }
    }

    public Flowerfield randomEmptyFileds()    // 플라워 몬스터가 없는 필드를 랜덤으로 하나 뽑는다.
    {
        emptyFileds.Clear(); // 초기화
        currentNum = 0; //  현재 플라워 몬스터 수를 0으로 초기화한다. 

        for (int n = 0; n < allflowerFields.Count; n++)
        {
            //플라워 몬스터가 없을 경우 리스트에 추가
            if (!allflowerFields[n].hasFlowerMon)
            {
                emptyFileds.Add(allflowerFields[n]);
            }
            //플라워 몬스터가 있으면 현재 플라워 몬스터 수 +1 을 한다. 
            if (allflowerFields[n].hasFlowerMon)
            {
                currentNum++; 
            }
        }

        int index = Random.Range(0, emptyFileds.Count);
        Debug.Log($"Random Empty Fields : {index}");
        return emptyFileds[index];

    }

    //플라워몬스터필드 배열에 플라워 필드를 추가한다
    public void AddFlowerFields(GameObject newObj)
    {
        Flowerfield newFlowerField = newObj.GetComponent<Flowerfield>();

        allflowerFields.Add(newFlowerField);
    }


    //최대 플라워 몬스터 수 갱신
    private void updateMaxFlowers()
    {
        maxFlower = Mathf.RoundToInt(percentForFlowers * allflowerFields.Count);
    }

    



}
