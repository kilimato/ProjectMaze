using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{

    private SpriteRenderer sr;
    private float timer;
    public float floatSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {

        sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        timer = floatSpeed * Time.time;
        sr.transform.position = new Vector3(sr.transform.position.x, sr.transform.position.y + Mathf.Cos(timer)/5f * Time.deltaTime, 0);
        
    }
}
