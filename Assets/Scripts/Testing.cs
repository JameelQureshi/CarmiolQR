using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject cube;
    public float scal;
    public float var=0.001f;
    // Start is called before the first frame update
    void Start()
    {
        scal = ((Camera.main.transform.position - transform.position).magnitude)*var;
        cube.transform.localScale = new Vector3(scal, scal, scal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
