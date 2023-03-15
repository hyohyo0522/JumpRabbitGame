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
    private static FoxMonSpawner m_instance; // �̱����� �Ҵ�� ����



    public GameObject foxMonPrefab;
    public Collider2D groundCollider; // ���ݶ��̴��� �Ҵ��ؼ� foxMonPrefab�� �Ҵ����ֵ��� �Ѵ�.
    [SerializeField] List<AIFoxController> foxMonsEnabled = new List<AIFoxController>(); //������ �������� ���� ����Ʈ

    float timeBetFoxInstantiate = 10f; // �����ֱ�
    public int maxFoxNum; // �ִ� ���� ���� ���� ��
    private int currentNumOfFox; // ���� �������ͻ�����


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

    private void CreateFoxMon(Vector2 spot) // ���찡 �����Ǵ� ��ġ�� �ö�� ���͵��� ������ġ�� �ϰڴ�.
    {
        GameObject newfox = Instantiate(foxMonPrefab, spot, Quaternion.identity);
        AIFoxController newFoxMonSC = newfox.GetComponent<AIFoxController>();
        newFoxMonSC.groundCollider = this.groundCollider;// �� ����
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
