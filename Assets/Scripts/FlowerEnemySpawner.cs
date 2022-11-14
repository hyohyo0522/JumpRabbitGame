using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class FlowerEnemySpawner : MonoBehaviour
{

     // �� �̰� �̱��� �������� �������� 

    public GameObject flowerPrefab;
    public static List<Flowerfield> allflowerFields = new List<Flowerfield>(); // ��� �ö������ �ʵ� ���� ������
    private static List<Flowerfield> emptyFileds = new List<Flowerfield>(); // �ö�� ���Ͱ� ���� �ȵ� �ö�� �ʵ带 ���� ����Ʈ
    private static List<FlowerEnemy> enemies = new List<FlowerEnemy>(); // ������ �ö�� ���͵��� ��� ����Ʈ
    
    private int maxFlower; // �ִ� �� ���� ���� ��
    public float percentForFlowers; // ���������� �ö�� ���� ��������
    private int currentNum; // ���� �� ���� ���� �� 
    float timeBetFlowerInstantiate = 4.5f; 
    // Start is called before the first frame update
    void Start()
    {
        percentForFlowers = 0.6f;
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
        //GameObject enemyObject = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name, enemypoint.position, enemypoint.rotation);
        GameObject enemyObject = Instantiate(flowerPrefab, flowerSpot.position, Quaternion.identity);
        FlowerEnemy flowrEnemy = enemyObject.GetComponent<FlowerEnemy>();
        enemies.Add(flowrEnemy);

        spot.hasFlowerMon = true;
        Debug.Log(spot.hasFlowerMon);
        //Flowerfield thisFlowerSpot = spot.GetComponent<Flowerfield>();
        //thisFlowerSpot.hasFlowerMon = true;

        //���� ��, ����Ʈ���� ����
        enemyObject.GetComponent<LivingEntity>().OnDeath += () => enemies.Remove(flowrEnemy);
        //�ö�����͸� ���� ������ �ö���� ���� ���ٴ� ���� ǥ��
        enemyObject.GetComponent<LivingEntity>().OnDeath += () => spot.hasFlowerMon = false;



    }

    private Flowerfield randomEmptyFileds()    // �ö�� ���Ͱ� ���� �ʵ带 �������� �ϳ� �̴´�.
    {
        emptyFileds.Clear();
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

    //�ִ� �ö�� ���� �� ����
    private void updateMaxFlowers()
    {
        maxFlower = Mathf.RoundToInt(percentForFlowers * allflowerFields.Count);
    }



}
