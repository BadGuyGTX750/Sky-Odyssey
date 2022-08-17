using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    float bias = 0.9f;
    public GameObject Camera;
    public GameObject Airplane;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movCamTo = Airplane.transform.position - Airplane.transform.forward * 10f + Vector3.up * 5f;
        Camera.transform.position = Camera.transform.position * bias + (1 - bias) * movCamTo;
        Camera.transform.LookAt(Airplane.transform.position + Airplane.transform.forward * 30f); 
    }
}
