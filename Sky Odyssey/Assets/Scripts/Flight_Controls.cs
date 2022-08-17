using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flight_Controls : MonoBehaviour
{
    private Rigidbody Airplane;
    public GameObject[] Elements;
    float RotationSpeed = 120;
    public float ThrustPercent;
    private float FlapsReturnSpeed = 1;

    void Awake()
    {
        Airplane = gameObject.GetComponent<Rigidbody>();
        ThrustPercent = 0;
    }

    // Start is called before the first frame update
    void Start()
    {  

    }

    // Update is called once per frame
    void Update()
    {
        //Roll Input
        if(Input.GetKey(KeyCode.D))
        {   
            Elements[5].transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            Elements[6].transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            if(Elements[5].transform.localRotation.eulerAngles.x >= 20 &&
               Elements[5].transform.localRotation.eulerAngles.x <= 340)
            {
                Elements[5].transform.localRotation = Quaternion.Euler(-20,0,0);
                Elements[6].transform.localRotation = Quaternion.Euler(20,0,0);
            }                           
        }

        if(Input.GetKey(KeyCode.A))
        {      
            Elements[5].transform.Rotate(RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self );
            Elements[6].transform.Rotate(-RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self);
            if(Elements[5].transform.localRotation.eulerAngles.x >= 20 &&
               Elements[5].transform.localRotation.eulerAngles.x <= 340)
            {
                Elements[5].transform.localRotation = Quaternion.Euler(20,0,0);
                Elements[6].transform.localRotation = Quaternion.Euler(-20,0,0);
            }        
        }

        if(Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false)
        {
            if(Elements[5].transform.localRotation.eulerAngles.x > 1 && 
               Elements[5].transform.localRotation.eulerAngles.x <= 55)
            {
                Elements[5].transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
                Elements[6].transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            }                     
            else if(Elements[5].transform.localRotation.eulerAngles.x < 359 &&
                    Elements[5].transform.localRotation.eulerAngles.x >= 305)
            {
                Elements[5].transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
                Elements[6].transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            }       
            else
            {
                Elements[5].transform.localRotation = Quaternion.Euler(0,0,0);
                Elements[6].transform.localRotation = Quaternion.Euler(0,0,0);
            } 
            
        }

        //Pitch Input
        if(Input.GetKey(KeyCode.S))
        {    
            Elements[7].transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            Elements[8].transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            if(Elements[7].transform.localRotation.eulerAngles.x >= 10 &&
               Elements[7].transform.localRotation.eulerAngles.x <= 350)
            {
                Elements[7].transform.localRotation = Quaternion.Euler(10,0,0);
                Elements[8].transform.localRotation = Quaternion.Euler(10,0,0);
            }               
        }

        if(Input.GetKey(KeyCode.W))
        {      
            Elements[7].transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            Elements[8].transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            if(Elements[7].transform.localRotation.eulerAngles.x >= 10 &&
               Elements[7].transform.localRotation.eulerAngles.x <= 350)
            {
                Elements[7].transform.localRotation = Quaternion.Euler(-10,0,0);
                Elements[8].transform.localRotation = Quaternion.Euler(-10,0,0);
            }          
        }

        if(Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
        {
            if(Elements[7].transform.localRotation.eulerAngles.x > 1 && 
               Elements[7].transform.localRotation.eulerAngles.x <= 55)
            {
                Elements[7].transform.Rotate(-RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self);
                Elements[8].transform.Rotate(-RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self);
            }
                      
            else if(Elements[7].transform.localRotation.eulerAngles.x < 359 &&
                    Elements[7].transform.localRotation.eulerAngles.x >= 305)
            {
                Elements[7].transform.Rotate(RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self);
                Elements[8].transform.Rotate(RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, 0, Space.Self);
            }       
            else
            {
                Elements[7].transform.localRotation = Quaternion.Euler(0,0,0);
                Elements[8].transform.localRotation = Quaternion.Euler(0,0,0);
            }               
        }

        //Yaw Input
        if(Input.GetKey(KeyCode.Q))
        {    
            Elements[9].transform.Rotate(0, RotationSpeed * Time.deltaTime, 0, Space.Self);
            if(Elements[9].transform.localRotation.eulerAngles.y >= 20 &&
               Elements[9].transform.localRotation.eulerAngles.y <= 340)
                Elements[9].transform.localRotation = Quaternion.Euler(0,20,0);
        }

        if(Input.GetKey(KeyCode.E))
        {      
            Elements[9].transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0, Space.Self);
            if(Elements[9].transform.localRotation.eulerAngles.y >= 20 &&
               Elements[9].transform.localRotation.eulerAngles.y <= 340)
                Elements[9].transform.localRotation = Quaternion.Euler(0,-20,0);
        }

        if(Input.GetKey(KeyCode.Q) == false && Input.GetKey(KeyCode.E) == false)
        {
            if(Elements[9].transform.localRotation.eulerAngles.y > 1 && 
               Elements[9].transform.localRotation.eulerAngles.y <= 55)
                Elements[9].transform.Rotate(0, -RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, Space.Self);      
            else if(Elements[9].transform.localRotation.eulerAngles.y < 359 &&
                    Elements[9].transform.localRotation.eulerAngles.y >= 305)
                Elements[9].transform.Rotate(0, RotationSpeed * FlapsReturnSpeed * Time.deltaTime, 0, Space.Self);
            else
                Elements[9].transform.localRotation = Quaternion.Euler(0,0,0);
        }

        //Thrust
        if(Input.GetKey(KeyCode.I))
        {
            ThrustPercent += 0.45f * Time.deltaTime;
            if(ThrustPercent >= 1)
                ThrustPercent = 1;
        }

        if(Input.GetKey(KeyCode.K))
        {
            ThrustPercent -= 0.45f * Time.deltaTime;
            if(ThrustPercent <= 0)
                ThrustPercent = 0;
        }


    }
}
