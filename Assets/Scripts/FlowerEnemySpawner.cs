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

    private static FlowerEnemySpawner m_instance; // �̱����� �Ҵ�� ����

    // �� �̰� �̱��� �������� �������� 

    public GameObject flowerPrefab;
    private static List<Flowerfield> allflowerFields = new List<Flowerfield>(); // ��� �ö������ �ʵ� ���� ������
    private static List<Flowerfield> emptyFileds = new List<Flowerfield>(); // �ö�� ���Ͱ� ���� �ȵ� �ö�� �ʵ带 ���� ����Ʈ
    private static List<FlowerEnemy> enemies = new List<FlowerEnemy>(); // ������ �ö�� ���͵��� ��� ����Ʈ
    
    private int maxFlower=0; // �ִ� �� ���� ���� ��
    private float percentForFlowers= 0.7f; // ���������� �ö�� ���� ��������
    private int currentNum=0; // ���� �� ���� ���� �� 
    float timeBetFlowerInstantiate = 4.5f;

    // ��������

    // C#, C++
    // C++ -> C++ (��ȯ���� �˰� �ִ� ���� 100% �Ȱ��ٰ� ���� X)

    // C++ ���� �����ϰ� �ʱ�ȭ X, ������ ��
    // int asd = 0; 0�� �ȵ� random

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
        updateMaxFlowers(); // �ִ� �� ���� ������ ����

    }

    // Update is called once per frame
    void Update()
    {

        timeBetFlowerInstantiate -= Time.deltaTime;

        // �����ֱ⶧���� �ִ���� �������� �ö�����Ͱ� ������ ���͸� �����Ѵ�. 
        if (timeBetFlowerInstantiate <= 0)
        {
            Debug.Log("Check Random Flower Start");

            //�ִ� �ɸ��� �� ���Ŀ� ���� �����Ѵ�. 
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



    //�ö�� ���� �޼��� 
    private void CreateFlowers(Flowerfield spot)
    {
        if (spot == null)
        {
            Debug.Log("CreateFlower spot null!!");
        }

        Transform flowerSpot = spot.flowerSpot;
        if(flowerSpot != null)
        {
            // ��¥ �� ����־�������???
            if (flowerPrefab == null)
            {
                Debug.Log("CreateFlower flowerPrefab null!!");
            }

            // position�� ȭ�� �� �߳����� ��ġ�� �³�?
            GameObject enemyObject = Instantiate(flowerPrefab, flowerSpot.position, Quaternion.identity);
            FlowerEnemy flowrEnemy = enemyObject.GetComponent<FlowerEnemy>();
            enemies.Add(flowrEnemy);

            Debug.Log($"CreateFlower Position : {flowrEnemy.transform.position}");

            spot.DoeeHaveFlowerMon(true);
            //Flowerfield thisFlowerSpot = spot.GetComponent<Flowerfield>();
            //thisFlowerSpot.hasFlowerMon = true;

            // GetComponent <-- ���� ���� ������
            //���� ��, ����Ʈ���� ����

            var entity = enemyObject.GetComponent<LivingEntity>();
            entity.OnDeath += () => enemies.Remove(flowrEnemy);
            entity.OnDeath += () => spot.DoeeHaveFlowerMon(false);
            //�ö�����͸� ���� ������ �ö���� ���� ���ٴ� ���� ǥ��
        }
        else
        {
            Debug.LogError("CreateFlower object null!!");
        }
    }

    public Flowerfield randomEmptyFileds()    // �ö�� ���Ͱ� ���� �ʵ带 �������� �ϳ� �̴´�.
    {
        emptyFileds.Clear(); // �ʱ�ȭ
        currentNum = 0; //  ���� �ö�� ���� ���� 0���� �ʱ�ȭ�Ѵ�. 

        for (int n = 0; n < allflowerFields.Count; n++)
        {
            //�ö�� ���Ͱ� ���� ��� ����Ʈ�� �߰�
            if (!allflowerFields[n].hasFlowerMon)
            {
                emptyFileds.Add(allflowerFields[n]);
            }
            //�ö�� ���Ͱ� ������ ���� �ö�� ���� �� +1 �� �Ѵ�. 
            if (allflowerFields[n].hasFlowerMon)
            {
                currentNum++; 
            }
        }

        int index = Random.Range(0, emptyFileds.Count);
        Debug.Log($"Random Empty Fields : {index}");
        return emptyFileds[index];

    }

    //�ö�������ʵ� �迭�� �ö�� �ʵ带 �߰��Ѵ�
    public void AddFlowerFields(GameObject newObj)
    {
        Flowerfield newFlowerField = newObj.GetComponent<Flowerfield>();

        allflowerFields.Add(newFlowerField);
    }


    //�ִ� �ö�� ���� �� ����
    private void updateMaxFlowers()
    {
        maxFlower = Mathf.RoundToInt(percentForFlowers * allflowerFields.Count);
    }

    



}
