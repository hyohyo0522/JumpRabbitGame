using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class FlowerEnemySpawner : MonoBehaviour
{

     // ★ 이거 싱글톤 패턴으로 만들어야함 

    public GameObject flowerPrefab;
    public static List<Flowerfield> allflowerFields = new List<Flowerfield>(); // 모든 플라워몬스터 필드 담을 리스터
    private static List<Flowerfield> emptyFileds = new List<Flowerfield>(); // 플라워 몬스터가 생성 안된 플라워 필드를 담을 리스트
    private static List<FlowerEnemy> enemies = new List<FlowerEnemy>(); // 생성된 플라워 몬스터들을 담는 리스트
    
    private int maxFlower; // 최대 꽃 몬스터 생성 수
    public float percentForFlowers; // 생성지역의 플라워 몬스터 생성비율
    private int currentNum; // 현재 꽃 몬스터 생성 수 
    float timeBetFlowerInstantiate = 4.5f; 
    // Start is called before the first frame update
    void Start()
    {
        percentForFlowers = 0.6f;
        updateMaxFlowers(); // 최대 꽃 몬스터 생성수 갱신

    }

    // Update is called once per frame
    void Update()
    {
        timeBetFlowerInstantiate -= Time.deltaTime;

        // 생성주기때마다 최대몬스터 갯수보다 플라워몬스터가 없으면 몬스터를 생성한다. 
        if (timeBetFlowerInstantiate <= 0)
        {
            //최대 꽃몬스터 수 비교후에 몬스터 생성한다. 
            Flowerfield monsterSpot = randomEmptyFileds();
            if(currentNum< maxFlower)
            {
                CreateFlowers(monsterSpot);
            }

            timeBetFlowerInstantiate = 4.5f;
        }

    }



    //플라워 생성 메서드 
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

        //죽을 때, 리스트에서 삭제
        enemyObject.GetComponent<LivingEntity>().OnDeath += () => enemies.Remove(flowrEnemy);
        //플라워몬스터를 만든 공간에 플라워가 지금 없다는 것을 표시
        enemyObject.GetComponent<LivingEntity>().OnDeath += () => spot.hasFlowerMon = false;



    }

    private Flowerfield randomEmptyFileds()    // 플라워 몬스터가 없는 필드를 랜덤으로 하나 뽑는다.
    {
        emptyFileds.Clear();
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
        return emptyFileds[index];

    }

    //최대 플라워 몬스터 수 갱신
    private void updateMaxFlowers()
    {
        maxFlower = Mathf.RoundToInt(percentForFlowers * allflowerFields.Count);
    }



}
