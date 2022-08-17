using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public struct Air_Surface
    {
        public float    Mass;
        public Vector3  Coords;
        public float    Area;
        public float    Chord;
        public float    Span;
        public float    AR;
        public float    FlapFraction;
        public Vector3  Normal;
        public Vector3  CGCoords;
        public Vector3  LastPosition;
        public Vector3  RelativeVelocity;
        public Vector3  Drag;
        public Vector3  Lift;
    }



public class Flying_Model : MonoBehaviour
{
    private Rigidbody Airplane;
    public GameObject[] Elements;
    //float pi = 3.14159f;
    private float rho = 1.225f;
    private float tmp;
    private float ThrustPercent;
    public Flight_Controls Flight_Controls_script;
    public Air_Surface[] Surface = new Air_Surface[12];
    private float TotalMass = 0;
    private Vector3 CG = Vector3.zero; 
    private float Ixx;
    private float Iyy;
    private float Izz;
    private Vector3 WorldAirForce = new Vector3(0, 0, 0);
    private Vector3 Gravity;
    private Vector3 LiftResultant;
    private Vector3 DragResultant;
    private Vector3 MomentResultant;
    private float AirplaneSpeedKnots;
    private float LiftSlope = 2 * Mathf.PI;
    private float SkinFriction0 = 0.02f;
    private float ZeroLiftAoABase = 0;         //deg
    private float StallAngleHighBase = 15; //deg
    private float StallAngleLowBase = -15; //deg
    private float CL, CD, CM, CT, CN; //Lift, Drag, Moment, Tangential and Normal Coeffs

    void Awake()
    {
        Airplane = gameObject.GetComponent<Rigidbody>();    
        
    }

    // Start is called before the first frame update
    void Start()
    {     
        CalcAirplaneProprieties();
        Gravity = new Vector3(0, -9.81f * TotalMass, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        LiftResultant = Vector3.zero;
        DragResultant = Vector3.zero;
        MomentResultant = Vector3.zero;

        Ixx = 0;
        Iyy = 0;
        Izz = 0;
        for (int j = 0; j < 12; j++)
        {
            Vector3 LocalAngularVelocity = Airplane.transform.InverseTransformDirection(Airplane.angularVelocity);
            Ixx += LocalAngularVelocity.x + Surface[j].Mass * (Surface[j].CGCoords.y * Surface[j].CGCoords.y +
                                            Surface[j].CGCoords.z * Surface[j].CGCoords.z);
            Iyy += LocalAngularVelocity.y + Surface[j].Mass * (Surface[j].CGCoords.x * Surface[j].CGCoords.x +
                                            Surface[j].CGCoords.z * Surface[j].CGCoords.z);
            Izz += LocalAngularVelocity.z + Surface[j].Mass * (Surface[j].CGCoords.x * Surface[j].CGCoords.x +
                                            Surface[j].CGCoords.y * Surface[j].CGCoords.y);
        }
        Airplane.inertiaTensor = new Vector3(Ixx, Iyy, Izz);
        //Debug.Log(Airplane.inertiaTensor);

        for (int i = 0; i < 5; i++)
        {    
            Surface[i].Normal = Vector3.Cross(Elements[i].transform.GetChild(0).position-Elements[i].transform.position, Elements[i].transform.GetChild(1).position-Elements[i].transform.position);
            Surface[i].Normal = Vector3.Normalize(Surface[i].Normal);

            Surface[i].CGCoords = Elements[i].transform.position - Airplane.transform.position;
            Vector3 LocAngVel = Airplane.transform.InverseTransformDirection(Airplane.angularVelocity);
            Vector3 AirVelocity = (Elements[i].transform.position - Surface[i].LastPosition) / Time.fixedDeltaTime + Vector3.Cross(Airplane.angularVelocity, Surface[i].CGCoords);
            Surface[i].LastPosition = Elements[i].transform.position;
            
            Surface[i].Drag = Vector3.Normalize(-AirVelocity);
            Surface[i].Lift = Vector3.Normalize(Vector3.Cross(Vector3.Cross(Surface[i].Drag, Surface[i].Normal), Surface[i].Drag));

            float AngleOfAttack = Vector3.Angle(Surface[i].Drag, Surface[i].Lift) - Vector3.Angle(Surface[i].Drag, Surface[i].Normal);
            float DynamicPressure = 0.5f * AirVelocity.sqrMagnitude * rho;

            if (i != 4)
            {
                CalculateCoefficients(Surface[i], AngleOfAttack, -Flight_Controls_script.Elements[i + 5].transform.localRotation.x * Mathf.Rad2Deg,
                                         LiftSlope, SkinFriction0, ZeroLiftAoABase, StallAngleHighBase, StallAngleLowBase);

                Surface[i].Lift = Surface[i].Lift * CL * DynamicPressure * Surface[i].Area;
                Surface[i].Drag = Surface[i].Drag * CD * DynamicPressure * Surface[i].Area;

                LiftResultant += Surface[i].Lift;
                DragResultant += Surface[i].Drag;

                Vector3 CurrentMoment = Vector3.Cross(Surface[i].CGCoords, Surface[i].Lift + Surface[i].Drag);
                CurrentMoment = Airplane.transform.InverseTransformDirection(CurrentMoment);
                if (i == 0 || i == 1)
                { CurrentMoment.x /= 1; CurrentMoment.y /= 10; }
                else
                { CurrentMoment.y /= 10; CurrentMoment.z /= 10; }
                CurrentMoment = Airplane.transform.TransformDirection(CurrentMoment);
                MomentResultant += CurrentMoment;
            }
            else
            {
                CalculateCoefficients(Surface[i], -Flight_Controls_script.Elements[i + 5].transform.localRotation.y * Mathf.Rad2Deg, AngleOfAttack,
                                         LiftSlope, SkinFriction0, ZeroLiftAoABase, StallAngleHighBase, StallAngleLowBase);

                Surface[i].Lift = Surface[i].Lift * CL * DynamicPressure * Surface[i].Area;
                Surface[i].Drag = Surface[i].Drag * CD * DynamicPressure * Surface[i].Area;

                LiftResultant += Surface[i].Lift;
                DragResultant += Surface[i].Drag;

                Vector3 CurrentMoment = Vector3.Cross(Surface[i].CGCoords, Surface[i].Lift + Surface[i].Drag);
                CurrentMoment = Airplane.transform.InverseTransformDirection(CurrentMoment);
                CurrentMoment.x /= 10; 
                CurrentMoment.z /= 10;
                CurrentMoment = Airplane.transform.TransformDirection(CurrentMoment);
                MomentResultant += 10 * CurrentMoment; 
            }
            Debug.DrawLine(Elements[i].transform.position, Elements[i].transform.position + AirVelocity / 100, Color.blue, 0f);
            Debug.DrawLine(Elements[i].transform.position, Elements[i].transform.position + Surface[i].Normal, Color.green, 0f);
            Debug.DrawLine(Elements[i].transform.position, Elements[i].transform.position + Surface[i].Lift / 100, Color.yellow, 0f);
            Debug.DrawLine(Elements[i].transform.position, Elements[i].transform.position + Surface[i].Drag / 100, Color.red, 0f);

        }

        float ThrustPercent = Flight_Controls_script.ThrustPercent;
        Airplane.AddForce(5f * ThrustPercent * TotalMass * transform.forward);

        AirplaneSpeedKnots = Airplane.transform.InverseTransformDirection(Airplane.velocity).z * 1.944f;
        if (Input.GetKey(KeyCode.Space) && AirplaneSpeedKnots > 70)
            Airplane.AddForce(-10f * TotalMass * transform.forward);

        Airplane.AddForce(DragResultant + LiftResultant + Gravity + WorldAirForce);
        Airplane.AddTorque(MomentResultant);

        Debug.Log(AirplaneSpeedKnots);  
    }



    void CalcAirplaneProprieties()
    {
        //Wings
        Surface[0].Mass = 25;
        Surface[1].Mass = 25;
        Surface[5].Mass = 7;
        Surface[6].Mass = 7;

        //Tail
        Surface[2].Mass = 12;
        Surface[3].Mass = 12;
        Surface[4].Mass = 9;
        Surface[7].Mass = 4;
        Surface[8].Mass = 4;
        Surface[9].Mass = 3;

        //Fuselage
        Surface[10].Mass = 250;
        Surface[11].Mass = 40;

        //Initialize InertiaTensor
        Ixx = 0; 
        Iyy = 0; 
        Izz = 0;

        //Area, Normals, Length of the aero-surface
        for(int i = 0; i < 12; i++)
        {       
            Surface[i].Coords = Elements[i].transform.position;
            if(i == 4 || i == 9 || i == 10 || i == 11)
                Surface[i].Area = Elements[i].transform.localScale.y * Elements[i].transform.localScale.z;
            else
                Surface[i].Area = Elements[i].transform.localScale.x * Elements[i].transform.localScale.z;

            Surface[i].Normal = Vector3.Cross(Elements[i].transform.GetChild(0).position-Elements[i].transform.position, Elements[i].transform.GetChild(1).position-Elements[i].transform.position);
            Surface[i].Normal = Vector3.Normalize(Surface[i].Normal);

            if(i == 4 || i == 10 || i == 11)
            {
                Surface[i].Chord = Elements[i].transform.localScale.z;
                Surface[i].Span = Elements[i].transform.localScale.y;
                Surface[i].AR = Surface[i].Span / Surface[i].Chord;
            }
            else if(i == 9)
            {
                Surface[i].Chord = Elements[i].transform.GetChild(2).localScale.z;
                Surface[i].Span = Elements[i].transform.GetChild(2).localScale.y;
                Surface[i].AR = Surface[i].Span / Surface[i].Chord;
            }
            else if(i == 5 || i == 6 || i == 7 || i == 8)
            {
                Surface[i].Chord = Elements[i].transform.GetChild(2).localScale.z;
                Surface[i].Span = Elements[i].transform.GetChild(2).localScale.x;
                Surface[i].AR = Surface[i].Span / Surface[i].Chord;
                
            }
            else
            {
                Surface[i].Chord = Elements[i].transform.localScale.z;
                Surface[i].Span = Elements[i].transform.localScale.x;
                Surface[i].AR = Surface[i].Span / Surface[i].Chord;
            }
        }

        //Flapt Fraction = cf/c
        for(int i = 0; i < 5; i++)
        {
            if(i == 0 || i == 1 || i == 2 || i == 3 || i == 4)
            {
                Surface[i].FlapFraction = Surface[i+5].Chord / (Surface[i].Chord + Surface[i+5].Chord);      
            }
            else 
                Surface[i].FlapFraction = 0;
        }
        
        //Calculate the total mass of the plane 
        for(int i = 0; i < 12; i++)
        {
            TotalMass += Surface[i].Mass;
        }

        //Calculate CG
        for(int i = 0; i < 12; i++)
        {
            CG += Surface[i].Coords * Surface[i].Mass;
        }
        CG /= TotalMass;

        //Move Airplane RigidBody to CG without moving the children
        for(int i = 0; i < 12; i++)
        {
            Elements[i].transform.parent = null;
        }
        Airplane.transform.position = CG;

        for(int i = 0; i < 12; i++)
        {
            Elements[i].transform.parent = Airplane.transform;
        }

        //Calculate Airplane Coordinates relative to center of gravity
        for(int i = 0; i < 12; i++)
        {
            Surface[i].CGCoords = Surface[i].Coords - CG;
        }
        
        //Calculate Airplane last position in order to calculate velocity for non RB components
        for(int i = 0; i < 12; i++)
        {
            Surface[i].LastPosition = Elements[i].transform.position;
        }

        // Compute Inertia Tensor
        for(int i = 0; i < 12; i++)
        {
            Vector3 LocalAngularVelocity = Airplane.transform.InverseTransformDirection(Airplane.angularVelocity);
            Ixx += LocalAngularVelocity.x + Surface[i].Mass * (Surface[i].CGCoords.y * Surface[i].CGCoords.y +
                                            Surface[i].CGCoords.z * Surface[i].CGCoords.z);
            Iyy += LocalAngularVelocity.y + Surface[i].Mass * (Surface[i].CGCoords.x * Surface[i].CGCoords.x +
                                            Surface[i].CGCoords.z * Surface[i].CGCoords.z);
            Izz += LocalAngularVelocity.z + Surface[i].Mass * (Surface[i].CGCoords.x * Surface[i].CGCoords.x +
                                            Surface[i].CGCoords.y * Surface[i].CGCoords.y);            
        }
        Airplane.inertiaTensor = new Vector3(Ixx, Iyy, Izz);
        Debug.Log(Airplane.inertiaTensor);
        //Airplane.inertiaTensor = 15 * Airplane.inertiaTensor;

        //Apply mass to rigidbody
        Airplane.mass = TotalMass;
    }



    void CalculateCoefficients(Air_Surface Surface,float AngleOfAttack, float FlapAngle, float LiftSlope, float SkinFriction0, float ZeroLiftAoABase, float StallAngleHighBase, float StallAngleLowBase)
    {
        AngleOfAttack = AngleOfAttack * Mathf.Deg2Rad;
        FlapAngle = FlapAngle * Mathf.Deg2Rad;
        ZeroLiftAoABase = ZeroLiftAoABase * Mathf.Deg2Rad;
        StallAngleHighBase = StallAngleHighBase * Mathf.Deg2Rad;
        StallAngleLowBase = StallAngleLowBase * Mathf.Deg2Rad;

        float CorrectedLiftSlope = LiftSlope * Surface.AR/(Surface.AR+2*(Surface.AR+4)/(Surface.AR+2));

        float theta = Mathf.Acos(2 * Surface.FlapFraction - 1);
        float FlapEffectiveness = 1 - (theta - Mathf.Sin(theta)) / Mathf.PI; //thau
        //Flap Effectiveness correction
        float eta = Mathf.Lerp(0.8f, 0.4f, Mathf.Abs(FlapAngle) * Mathf.Rad2Deg / 50);
        float DeltaLift = CorrectedLiftSlope * FlapEffectiveness * eta * FlapAngle;

        float ZeroLiftAoA = ZeroLiftAoABase - DeltaLift / CorrectedLiftSlope;

        float clMaxHigh = CorrectedLiftSlope * (StallAngleHighBase - ZeroLiftAoABase) + 
                                                    DeltaLift * Surface.FlapFraction;
        float clMaxLow = CorrectedLiftSlope * (StallAngleLowBase - ZeroLiftAoABase) + 
                                                    DeltaLift * Surface.FlapFraction;

        float StallAngleHigh = ZeroLiftAoA + clMaxHigh/CorrectedLiftSlope;
        float StallAngleLow = ZeroLiftAoA + clMaxLow/CorrectedLiftSlope;

        if(StallAngleLow < AngleOfAttack && AngleOfAttack < StallAngleHigh)
        {
            CL = CorrectedLiftSlope * (AngleOfAttack - ZeroLiftAoA);
            float InducedAngle = CL / (Mathf.PI * Surface.AR);
            float EffectiveAngle = AngleOfAttack - ZeroLiftAoA - InducedAngle;
            CT = SkinFriction0 * Mathf.Cos(EffectiveAngle);
            CN = (CL + CT * Mathf.Sin(EffectiveAngle)) / Mathf.Cos(EffectiveAngle);
            CD = CN * Mathf.Sin(EffectiveAngle) + CT * Mathf.Cos(EffectiveAngle);
            CM = -CN * (0.25f - 0.175f * (1 - 2 * Mathf.Abs(EffectiveAngle) / Mathf.PI));
        }
        else
        {
            float LiftCoefficientLowAoA;
            float LerpParam;
            float SkinFriction90 = -4.26f * 0.01f * Mathf.Pow(AngleOfAttack,2) +
                                    2.1f * 0.1f * AngleOfAttack + 1.98f;
            if(AngleOfAttack > StallAngleHigh)
                LiftCoefficientLowAoA = CorrectedLiftSlope * (StallAngleHigh - ZeroLiftAoA);
            else
                LiftCoefficientLowAoA = CorrectedLiftSlope * (StallAngleLow - ZeroLiftAoA);
            
            float InducedAngle = LiftCoefficientLowAoA / (Mathf.PI * Surface.AR);

            if(AngleOfAttack > StallAngleHigh)
                LerpParam = (Mathf.PI / 2 - Mathf.Clamp(AngleOfAttack, -Mathf.PI / 2, Mathf.PI / 2)) /
                                    (Mathf.PI / 2 - StallAngleHigh);
            else
                LerpParam = (-Mathf.PI / 2 - Mathf.Clamp(AngleOfAttack, -Mathf.PI / 2, Mathf.PI / 2)) /
                                    (-Mathf.PI / 2 - StallAngleLow);

            InducedAngle = Mathf.Lerp(0, InducedAngle, LerpParam);
            float EffectiveAngle = AngleOfAttack - ZeroLiftAoA -InducedAngle;
            CN = SkinFriction90 * Mathf.Sin(EffectiveAngle) * (1/(0.56f + 0.44f * Mathf.Abs(Mathf.Sin(EffectiveAngle))) -
                        0.41f * (1 - Mathf.Exp(-17/Surface.AR)));
            CT = 0.5f * SkinFriction0 * Mathf.Cos(EffectiveAngle);  
            CL = CN * Mathf.Cos(EffectiveAngle) - CT * Mathf.Sin(EffectiveAngle);
            CD = CN * Mathf.Sin(EffectiveAngle) + CT * Mathf.Cos(EffectiveAngle);
            CM = -CN * (0.25f - 0.175f * (1 - 2 * Mathf.Abs(EffectiveAngle)/Mathf.PI));
        }

    }

}

