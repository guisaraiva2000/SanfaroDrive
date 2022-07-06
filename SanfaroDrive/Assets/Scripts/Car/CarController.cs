/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 * 
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using System.Runtime.CompilerServices;
using UnityEngine;

#if MULTIOSCONTROLS
    using MOSC;
#endif

[assembly: InternalsVisibleTo("VehicleBehaviour.Dots")]
namespace VehicleBehaviour {
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour {
        
        [Header("Inputs")]
        // Input names to read using GetAxis
        [SerializeField] internal VehicleInputs m_Inputs;
        string throttleInput => m_Inputs.ThrottleInput;
        string brakeInput => m_Inputs.BrakeInput;
        string turnInput => m_Inputs.TurnInput;
        string jumpInput => m_Inputs.JumpInput;
        string driftInput => m_Inputs.DriftInput;
	    string boostInput => m_Inputs.BoostInput;
        string rotateInput => m_Inputs.RotateInput;
        /* 
         *  Turn input curve: x real input, y value used
         *  My advice (-1, -1) tangent x, (0, 0) tangent 0 and (1, 1) tangent x
         */
        [SerializeField] AnimationCurve turnInputCurve = AnimationCurve.Linear(-1.0f, -1.0f, 1.0f, 1.0f);

        [Header("Wheels")]
        [SerializeField] WheelCollider[] driveWheel = new WheelCollider[0];
        public WheelCollider[] DriveWheel => driveWheel;
        [SerializeField] WheelCollider[] turnWheel = new WheelCollider[0];

        public WheelCollider[] TurnWheel => turnWheel;

        // This code checks if the car is grounded only when needed and the data is old enough
        bool isGrounded = false;
        int lastGroundCheck = 0;
        public bool IsGrounded { get {
            if (lastGroundCheck == Time.frameCount)
                return isGrounded;

            lastGroundCheck = Time.frameCount;
            isGrounded = true;
            foreach (WheelCollider wheel in wheels)
            {
                if (!wheel.gameObject.activeSelf || !wheel.isGrounded)
                    isGrounded = false;
            }
            return isGrounded;
        }}

        [Header("Behaviour")]
        /*
         *  Motor torque represent the torque sent to the wheels by the motor with x: speed in km/h and y: torque
         *  The curve should start at x=0 and y>0 and should end with x>topspeed and y<0
         *  The higher the torque the faster it accelerate
         *  the longer the curve the faster it gets
         */
        [SerializeField] AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));

        // Differential gearing ratio
        [Range(2, 16)]
        [SerializeField] float diffGearing = 4.0f;
        public float DiffGearing { get => diffGearing;
            set => diffGearing = value;
        }

        // Basicaly how hard it brakes
        [SerializeField] float brakeForce = 1500.0f;
        public float BrakeForce { get => brakeForce;
            set => brakeForce = value;
        }

        // Max steering hangle, usualy higher for drift car
        [Range(0f, 50.0f)]
        [SerializeField] float steerAngle = 30.0f;
        public float SteerAngle { get => steerAngle;
            set => steerAngle = Mathf.Clamp(value, 0.0f, 50.0f);
        }

        // The value used in the steering Lerp, 1 is instant (Strong power steering), and 0 is not turning at all
        [Range(0.001f, 1.0f)]
        [SerializeField] float steerSpeed = 0.2f;
        public float SteerSpeed { get => steerSpeed;
            set => steerSpeed = Mathf.Clamp(value, 0.001f, 1.0f);
        }

        // How hight do you want to jump?
        [Range(1f, 4.0f)] 
        [SerializeField] private float jumpVel = 1f;
        public float JumpVel { get => jumpVel;
            set => jumpVel = Mathf.Clamp(value, 1.0f, 4.0f);
        }

        // How hard do you want to drift?
        [Range(0.0f, 2f)]
        [SerializeField] float driftIntensity = 1f;
        public float DriftIntensity { get => driftIntensity;
            set => driftIntensity = Mathf.Clamp(value, 0.0f, 2.0f);
        }

        // Reset Values
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        /*
         *  The center of mass is set at the start and changes the car behavior A LOT
         *  I recomment having it between the center of the wheels and the bottom of the car's body
         *  Move it a bit to the from or bottom according to where the engine is
         */
        [SerializeField] Transform centerOfMass = null;

        // Force aplied downwards on the car, proportional to the car speed
        [Range(0.5f, 10f)]
        [SerializeField] float downforce = 1.0f;

        public float Downforce
        {
            get => downforce;
            set => downforce = Mathf.Clamp(value, 0, 5);
        }     

        // When IsPlayer is false you can use this to control the steering
        float steering;
        public float Steering { get => steering;
            set => steering = Mathf.Clamp(value, -1f, 1f);
        } 

        // When IsPlayer is false you can use this to control the throttle
        float throttle;
        public float Throttle { get => throttle;
            set => throttle = Mathf.Clamp(value, -1f, 1f);
        } 
        
        bool drift;
        public bool Drift { get => drift;
            set => drift = value;
        }         

        // Use this to read the current car speed (you'll need this to make a speedometer)
        [SerializeField] float speed = 0.0f;
        //public float Speed => speed;
        public float Speed { get => speed;
            set => speed = value;
        }

        [Header("Particles")]
        // Exhaust fumes
        [SerializeField] ParticleSystem[] gasParticles = new ParticleSystem[0];

        [Header("Boost")]
        // Disable boost
        [HideInInspector] public bool allowBoost = true;

        // Use this to boost when IsPlayer is set to false
        private bool boosting = false;
        // Use this to jump when IsPlayer is set to false
        private bool jumping = false;

        private EngineSoundManager engineSoundManager;

        // Boost particles and sound
        [SerializeField] ParticleSystem[] boostParticles = new ParticleSystem[0];
        [SerializeField] AudioClip boostClip = default;
        private AudioSource boostSource;

        [Header("Canvas")]
        [SerializeField] private GameObject canvas;
        public GameObject Canvas => canvas;

        private AnalyticsController analyticsController;
        
        // Private variables set at the start
        Rigidbody rb = default;
        public Rigidbody Rb
        {
            get => rb;
            set => rb = value;
        }

        internal WheelCollider[] wheels = new WheelCollider[0];

        // Init rigidbody, center of mass, wheels and more
        void Start() {
            boostSource = gameObject.AddComponent<AudioSource>();
            if (boostClip != null) {
                boostSource.clip = boostClip;
            }
            boostSource.playOnAwake = false;
            boostSource.volume = 0.3f;

		    //boost = maxBoost;

            rb = GetComponent<Rigidbody>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (rb != null && centerOfMass != null)
            {
                rb.centerOfMass = centerOfMass.localPosition;
            }

            wheels = GetComponentsInChildren<WheelCollider>();

            // Set the motor torque to a non null value because 0 means the wheels won't turn no matter what
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0f;
            }

            engineSoundManager = transform.GetComponentInParent<EngineSoundManager>();
            analyticsController = GameObject.Find("GameManager").GetComponent<AnalyticsController>();
        }

        // Visual feedbacks and boost regen
        void Update()
        {
            foreach (ParticleSystem gasParticle in gasParticles)
            {
                gasParticle.Play();
                ParticleSystem.EmissionModule em = gasParticle.emission;
                em.rateOverTime = Mathf.Lerp(em.rateOverTime.constant, Mathf.Clamp(150.0f * throttle, 30.0f, 100.0f), 0.1f);
            }

            if (allowBoost) {
                /*boost += Time.deltaTime * boostRegen;
                if (boost > maxBoost) { boost = maxBoost; }*/
                GetComponent<BoostSystem>().UpdateUI();
            }
        }
        
        // Update everything
        void FixedUpdate () {
            // Mesure current speed
            speed = transform.InverseTransformDirection(rb.velocity).z * 3.6f;
            
            // Accelerate & brake
            if (throttleInput != "" && throttleInput != null)
            {
                throttle = GetInput(throttleInput) - GetInput(brakeInput)*2;
                if (throttle !=0){
                    engineSoundManager.MuteSounds(false);
                    GetComponent<FuelSystem>().ReduceFuel();
                }
            }
            // Boost
            boosting = GetInput(boostInput) > 0.5f;
            // Turn
            steering = turnInputCurve.Evaluate(GetInput(turnInput)) * steerAngle;
            // Dirft
            drift = GetInput(driftInput) > 0 && rb.velocity.sqrMagnitude > 100;
            // Jump
            jumping = GetInput(jumpInput) != 0;
            
            if ( GetInput(rotateInput) != 0)
            {
                transform.Rotate(new Vector3(0, 0, GetInput(rotateInput)));
            }

            // Direction
            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, steering, steerSpeed);
            }

            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = 0f;
                wheel.brakeTorque = 0f;
            }

            if (throttle != 0 && (Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(throttle)))
            {
                foreach (WheelCollider wheel in driveWheel)
                {
                    float biTorque = motorTorque.Evaluate(speed);
                    if ( !GetComponent<FuelSystem>().HasFuel && biTorque > 470){
                        biTorque = biTorque / 12;
                        analyticsController.NoFuelCounter();
                    }
                    wheel.motorTorque = throttle * biTorque * diffGearing / driveWheel.Length;
                }
            }
            else if (throttle != 0)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = Mathf.Abs(throttle) * brakeForce;
                }
                
                engineSoundManager.BrakeSound();
            }

            // Jump
            if (jumping) {
                if (!IsGrounded)
                    return;
                
                analyticsController.TotalJumps();
                engineSoundManager.JumpSound();
                rb.velocity += transform.up * jumpVel;
            }

            // Boost
            if (boosting && allowBoost && GetComponent<BoostSystem>().Boost > 0.1f) {
                rb.AddForce(transform.forward * GetComponent<BoostSystem>().BoostForce);

                GetComponent<BoostSystem>().UseBoost();
                //if (!GetComponent<BoostSystem>().HasBoost) { boost = 0f; }

                if (boostParticles.Length > 0/* && !boostParticles[0].isPlaying*/) {
                    foreach (ParticleSystem boostParticle in boostParticles) {
                        boostParticle.Play();
                    }
                }

                if (boostSource && !boostSource.isPlaying) {
                    boostSource.Play();
                }
            } else {
                if (boostParticles.Length > 0 && boostParticles[0].isPlaying) {
                    foreach (ParticleSystem boostParticle in boostParticles) {
                        boostParticle.Stop();
                    }
                }

                if (boostSource && boostSource.isPlaying) {
                    boostSource.Stop();
                }
            }

            // Drift
            if (drift) {
                Vector3 driftForce = -transform.right;
                driftForce.y = 0.0f;
                driftForce.Normalize();

                if (steering != 0)
                    driftForce *= rb.mass * speed/7f * throttle * steering/steerAngle;
                Vector3 driftTorque = transform.up * 0.1f * steering/steerAngle;
                
                rb.AddForce(driftForce * driftIntensity, ForceMode.Force);
                rb.AddTorque(driftTorque * driftIntensity, ForceMode.VelocityChange);
                
                analyticsController.TotalDrifts();
                engineSoundManager.BrakeSound();
            }
            
            // Downforce
            rb.AddForce(-transform.up * speed * downforce);
        }

        // Reposition the car to the start position
        public void ResetPos() {
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Use this method if you want to use your own input manager
        private float GetInput(string input) {
            return Input.GetAxis(input);
        }
    }
}
