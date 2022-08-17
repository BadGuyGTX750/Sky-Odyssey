using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel_Physics : MonoBehaviour
{
    private float minLength = 0.2f;
    private float maxLength = 1f;
    private float springStiffness = 2000;
    private float damperStiffness = 600;
    private float restLength;
    private float springLength;
    private float lastLength;
    private float springVelocity;
    private float wheelRadius;
    private float springForce;
    private float damperForce;
    private Vector3 suspensionForce;

    public Rigidbody Airplane;
    public LayerMask IgnoreMe;

    // Start is called before the first frame update
    void Start()
    {
        wheelRadius = transform.localScale.x / 2;
        restLength = (minLength + maxLength) / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position + Vector3.up * 0.6f, -Vector3.up, out RaycastHit hit, wheelRadius + maxLength, ~IgnoreMe))
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.6f, -Vector3.up * hit.distance, Color.yellow);
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springVelocity = (springLength - lastLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;
            suspensionForce = (springForce + damperForce) * transform.right;

            Airplane.AddForceAtPosition(suspensionForce, hit.point);
        }
    }
}
