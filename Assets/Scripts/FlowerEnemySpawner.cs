using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    
    private int maxFlower; // �ִ� �� ���� ���� ��
    public float percentForFlowers; // ���������� �ö�� ���� ��������
    private int currentNum; // ���� �� ���� ���� �� 
    float timeBetFlowerInstantiate = 4.5f; 
    // Start is called before the first frame update
    void Start()
    {
        percentForFlowers = 0.8f;
        updateMaxFlowers(); // �ִ� �� ���� ������ ����

    }

    // Update is called once per frame
    void Update()
    {
        timeBetFlowerInstantiate -= Time.deltaTime;

        // �����ֱ⶧���� �ִ���� �������� �ö�����Ͱ� ������ ���͸� �����Ѵ�. 
        if (timeBetFlowerInstantiate <= 0)
        {
            //�ִ� �ɸ��� �� ���Ŀ� ���� �����Ѵ�. 
            Flowerfield monsterSpot = randomEmptyFileds();
            if(currentNum< maxFlower)
            {
                CreateFlowers(monsterSpot);
            }

            timeBetFlowerInstantiate = 4.5f;
        }

    }



    //�ö�� ���� �޼��� 
    private void CreateFlowers(Flowerfield spot)
    {
        Transform flowerSpot = spot.flowerSpot;
        if(flowerSpot != null)
        {
            GameObject enemyObject = Instantiate(flowerPrefab, flowerSpot.position, Quaternion.identity);
            FlowerEnemy flowrEnemy = enemyObject.GetComponent<FlowerEnemy>();
            enemies.Add(flowrEnemy);

            spot.hasFlowerMon = true;
            //Flowerfield thisFlowerSpot = spot.GetComponent<Flowerfield>();
            //thisFlowerSpot.hasFlowerMon = true;

            //���� ��, ����Ʈ���� ����
            enemyObject.GetComponent<LivingEntity>().OnDeath += () => enemies.Remove(flowrEnemy);
            //�ö�����͸� ���� ������ �ö���� ���� ���ٴ� ���� ǥ��
            enemyObject.GetComponent<LivingEntity>().OnDeath += () => spot.hasFlowerMon = false;
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
