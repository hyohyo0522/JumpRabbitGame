using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyCollider : MonoBehaviour
{
    PlatformEffector2D myPlatformer;

    public LayerMask Collider;

    // Start is called before the first frame update
    void Start()
    {
        myPlatformer = GetComponent<PlatformEffector2D>();
        setCollider(Collider);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Dollar))
        {
            subtractCollider(Collider);
        }
    }

    public void setCollider(int value)
    {
        myPlatformer.colliderMask = value;
        Debug.Log(myPlatformer.colliderMask.ToString());
    }

    public void subtractCollider(int value)
    {
        myPlatformer.colliderMask -= value;
    }

}
