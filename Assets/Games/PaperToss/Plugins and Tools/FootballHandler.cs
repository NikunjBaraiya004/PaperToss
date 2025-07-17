using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nostra.booboogames.PaperToss
{
    public class FootballHandler : MonoBehaviour
    {
        [Header("-- Reference --")]
        public FootballTossGameManager gamemanager;
      
        public GameObject WindObj;
        public FanRotation FanObject;

        [Header("-- Swipe Settings --")]
        [SerializeField] private float minSwipeDistance = 10f;
        [SerializeField] private float maxSwipeDistance = 100f;

        [Header("-- Ball Launch Setting --")]
        public float launchHeightMul = 5f;
        [SerializeField] private float flightDuration = 1.2f;

        public GameObject staticBall, WaterSplash, SpaceEx;

        private float launchHeight = 0f;
        private Vector2 startPos;
        private Vector2 endPos;
        private bool isSwiping = false;
        public bool isFlicked = false;
        private bool windApplied = false;

        private Rigidbody rb;
        [SerializeField] Vector3 initialPos;

        [SerializeField] GameObject SetTrailObj;
        [SerializeField] GameObject DefaultTrail;
        [SerializeField] GameObject SpaceTrail;
        [SerializeField] GameObject SteamCity;

        private float flightTimer;
        private Vector3 arcStart;
        private Vector3 arcEnd;
        private Vector3 lastPosition;
        private Vector3 windDirection = Vector3.zero;
        private float windMultiplier = 0f;
        private Vector3 windOffset = Vector3.zero;
        public bool isReset = true;
        [SerializeField] bool Istouch = true;
      //  [SerializeField] List<GameObject> dummyBallList = new List<GameObject>();
        [SerializeField] Transform PreFabParent;

        [Header("Tutorial Obj"), Space(5)]
        [SerializeField] GameObject TutorialObj;
        [SerializeField] private float idleTimeThreshold = 5f; // Time in seconds to show tutorial
        private float idleTimer = 0f;
        private bool tutorialShown = false;


        [Header("-- Set Random Ball --")]
        [SerializeField] List<GameObject> RandomBall = new List<GameObject>();
        private int Randomballselect;
        public MeshRenderer selectedmesh;
        bool isStopped;
        [SerializeField] Transform MainCam;


        [SerializeField] StaticBallGenerate staticBallGenerate;



        #region Unity Events

        private void Awake()
        {
            FanObject = gamemanager.fanrotaion;
            Application.targetFrameRate = 90;
            QualitySettings.vSyncCount = 0;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            initialPos = transform.position;


            SetRandomBall();
        }

       public void SetRandomBall()
       {
            DisableBall();
            Randomballselect = UnityEngine.Random.Range(0, RandomBall.Count);
            RandomBall[Randomballselect].SetActive(true);
            selectedmesh = RandomBall[Randomballselect].GetComponent<MeshRenderer>();
            staticBallGenerate.GenerateStaticBall(Randomballselect);
        }

        public void DisableWind()
        {

            FanObject.gameObject.SetActive(false);
            WindObj.gameObject.SetActive(false);
        }
       

        void DisableBall()
        {
            for (int i = 0; i < RandomBall.Count; i++) 
            {
                RandomBall[i].SetActive(false);
            }
        }

        [SerializeField] private float swipeHoldThreshold = 0.5f; // Time in seconds before auto-flick
        private float swipeTimer = 0f;
        private Queue<Vector2> swipePositionBuffer = new Queue<Vector2>();
        [SerializeField] private int bufferSize = 5; // Number of positions to average

        void Update()
        {
            if (!gamemanager.isPlaying || !Istouch)
                return;

            Vector2 currentPosition = Vector2.zero;
            bool inputDown = false;
            bool inputUp = false;
            bool inputHeld = false;

            // Input: Mouse
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    inputDown = true;
                    currentPosition = Mouse.current.position.ReadValue();
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    inputUp = true;
                    currentPosition = Mouse.current.position.ReadValue();
                }
                else if (Mouse.current.leftButton.isPressed)
                {
                    inputHeld = true;
                    currentPosition = Mouse.current.position.ReadValue();
                }
            }

#if UNITY_ANDROID || UNITY_IOS
            // Input: Touch
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;

                if (touch.press.wasPressedThisFrame)
                {
                    inputDown = true;
                    currentPosition = touch.position.ReadValue();
                }
                else if (touch.press.wasReleasedThisFrame)
                {
                    inputUp = true;
                    currentPosition = touch.position.ReadValue();
                }
                else if (touch.press.isPressed)
                {
                    inputHeld = true;
                    currentPosition = touch.position.ReadValue();
                }
            }
#endif

            // Swipe logic with smoothing
            if (inputDown)
            {
                startPos = currentPosition;
                isSwiping = true;
                swipeTimer = 0f;
                swipePositionBuffer.Clear();
                swipePositionBuffer.Enqueue(currentPosition);
            }
            else if (isSwiping && inputHeld)
            {
                swipeTimer += Time.deltaTime;

                // Add to buffer
                swipePositionBuffer.Enqueue(currentPosition);
                if (swipePositionBuffer.Count > bufferSize)
                    swipePositionBuffer.Dequeue();

                if (swipeTimer >= swipeHoldThreshold)
                {
                    endPos = GetAverageSwipePosition();
                    isSwiping = false;
                    HandleFlick();
                }
            }
            else if (inputUp && isSwiping)
            {
                endPos = GetAverageSwipePosition();
                isSwiping = false;
                HandleFlick();
            }

            // --- Tutorial Timer Logic ---
            if (inputDown || inputHeld || inputUp)
            {
                idleTimer = 0f;

                if (TutorialObj.activeSelf)
                    TutorialObj.SetActive(false);

                tutorialShown = false;
            }
            else
            {
                idleTimer += Time.deltaTime;

                if (!tutorialShown && idleTimer >= idleTimeThreshold)
                {
                    TutorialObj.SetActive(true);
                    tutorialShown = true;
                }
            }
        }


        private Vector2 GetAverageSwipePosition()
        {
            if (swipePositionBuffer.Count == 0)
                return startPos; // fallback

            Vector2 sum = Vector2.zero;
            foreach (var pos in swipePositionBuffer)
                sum += pos;

            return sum / swipePositionBuffer.Count;
        }



        void FixedUpdate()
        {
            if (!isFlicked) 
                return;

            LaunchBall();
        }

        public void Setinparent(Transform parenttransform)
        {
            rb.isKinematic = true;
            transform.SetParent(parenttransform);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (isFlicked && !isReset)
            {
                string manageDummyArg = "Dust";

               

                if (collision.gameObject.CompareTag("QP_Tag_01"))
                {
                    manageDummyArg = "Dust";
                   
                }
                else if (collision.gameObject.CompareTag("QP_Tag_02"))
                {
                    manageDummyArg = "Water";
                }
                else if (collision.gameObject.CompareTag("QP_Tag_03"))
                {
                    isStopped = true;
                    return;
                }
                else if (collision.gameObject.CompareTag("QP_Tag_04"))
                {
                    manageDummyArg = "Space";
                }
                else
                {
                    return;
                }

                isReset = true;
                gamemanager.IncLvl(false);
                rb.isKinematic = true;
                // gamemanager.IncCombo(false);
                GetComponent<SphereCollider>().enabled = false;
                selectedmesh.enabled = false;
                ManageDummyBall(manageDummyArg);
                ResetBall(2, false);

            }
        }


        #endregion

        #region Core Methods

        #region Ball launch
        public float winfInfPer = 0;
        private void LaunchBall()
        {
            flightTimer += Time.fixedDeltaTime;
            float t = flightTimer / flightDuration;

            if (isFlicked && !isStopped)
            {
                Vector3 newPosition = GetParabolaPoint(arcStart, arcEnd, launchHeight, t);
                Vector3 moveDirection = newPosition - lastPosition;

                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                    rb.MoveRotation(targetRotation);
                }

                if (windApplied)
                {
                    if (isReset && t >= 0.5f)
                    {
                        isReset = false;
                    }

                    float windInfluenceTime = Mathf.Clamp01((t - 0.45f) / 0.55f) * winfInfPer;
                    windOffset = new Vector3(windDirection.x, 0f, 0f) * windMultiplier * 50f * windInfluenceTime * Time.fixedDeltaTime * Mathf.Sin(t);
                }

                rb.MovePosition(newPosition);
                lastPosition = newPosition;
                //Vector3 scale = transform.localScale;
                //if (scale.x >= 0.75f && scale.y >= 0.75f && scale.z >= 0.75f)
                //{
                //    transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                //}
                
            }
           
        }


        #endregion

        #region User Input Flick
        private void HandleFlick()
        {
            Vector2 swipe = endPos - startPos;

            if (swipe.magnitude < minSwipeDistance)
                return;

            gamemanager.audioSource.ThrowTossPlay();

            Istouch = false;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Get camera vectors
            Vector3 camForward = MainCam.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = MainCam.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Apply horizontal dampening factor (reduce sensitivity)
            float horizontalSensitivity = 0.3f; // <--- Adjust this value as needed
            float verticalSensitivity = 1.0f;

            swipe = Vector2.ClampMagnitude(swipe, maxSwipeDistance);

            Vector3 swipeDir = (swipe.x * horizontalSensitivity * camRight) + (swipe.y * verticalSensitivity * camForward);
            swipeDir.Normalize();

            // Compute arc start/end
            Vector3 start = transform.position;
            Vector3 end = gamemanager.goalBucket.goalTarget.position;
            float distanceZ = end.z - start.z;
            float targetX = start.x + swipeDir.x * Mathf.Abs(distanceZ);

            arcStart = start;
            arcEnd = new Vector3(targetX, end.y, end.z);

            // Set arc height
            float distance = Vector3.Distance(arcStart, arcEnd);
            float baseLaunchHeight = distance * 0.25f;
            launchHeight = baseLaunchHeight * launchHeightMul;

            flightTimer = 0f;
            lastPosition = start;
            windOffset = Vector3.zero;
            windApplied = true;
            isFlicked = true;

            // Optional: play sound or VFX
            // gamemanager.audioManager.ThrowTossPlay();
        }


        private Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
        {
            float peakT = 0.5f;
            float parabola = height * (1 - Mathf.Pow((t - peakT) / peakT, 2));
            Vector3 point = Vector3.Lerp(start, end, t);
            point.y += parabola;
            point += windOffset;

            return point;
        }

        #endregion

        #region Wind
        public void ApplyWind(Vector3 direction, float multiplier)
        {
            string dir = "";
            
            if(!FanObject.gameObject.activeSelf && !gamemanager.isAutoPlay)
                FanObject.gameObject.SetActive(true);

            if(!WindObj.gameObject.activeSelf && !gamemanager.isAutoPlay)
                WindObj.gameObject.SetActive(true);

            if (direction.x > 0)
            {
                FanObject.transform.position = new Vector3 (transform.position.x - 1.7f, transform.position.y, transform.position.z+0.5f);
                FanObject.transform.rotation = Quaternion.Euler(0, 30, 0);
                dir = "Left";
                Debug.Log("Left");
            }
            else
            {
                FanObject.transform.position = new Vector3(transform.position.x + 1.7f, transform.position.y, transform.position.z+0.5f);
                FanObject.transform.rotation = Quaternion.Euler(0, -30, 0);
                dir = "Right";
                Debug.Log("Right");
            }

            windDirection = new Vector3(direction.x, direction.y, 0f);
            windMultiplier = multiplier;
            FanObject.setFanSpeed(multiplier, dir);
        }


        #endregion

        #region Reset Ball
        public void ResetBall(float waitTime = 1, bool isGoal = false)
        {
            if (!isFlicked) 
                return;

            isStopped = false;
            isReset = true;
            windApplied = false;
            //rb.linearVelocity = Vector3.zero;
            StartCoroutine(ResetDelay(waitTime, isGoal));
        }


        public void Goal()
        {
            Istouch = true;
            isReset = true;
            ResetBall(1, true);
        }

        private IEnumerator ResetDelay(float waitTime, bool isGoal)
        {
            SetTrailObj.SetActive(false);
            
            yield return new WaitForSeconds(waitTime);

            selectedmesh.enabled = true;
            transform.SetParent(PreFabParent);
            GetComponent<SphereCollider>().enabled = true;
            rb.isKinematic = true;
            transform.position = initialPos;
            
            transform.localScale = Vector3.one;
            flightTimer = 0f;
            isFlicked = false;
            Istouch = true;

            gamemanager.StartLevel(isGoal);

            SetTrailObj.SetActive(true);
        }

        public void SetTrail(int levelnum)
        {
            if (levelnum == 0)
            {
                SetTrailObj = DefaultTrail;
                DefaultTrail.SetActive(true);
                SpaceTrail.gameObject.SetActive(false);
                SteamCity.gameObject.SetActive(false);
            }
            else if (levelnum == 1)
            {
                SetTrailObj = SpaceTrail;
                SpaceTrail.gameObject.SetActive(true);
                SetTrailObj.GetComponent<ParticleSystem>().Play();
                DefaultTrail.gameObject.SetActive(false);
                SteamCity.gameObject.SetActive(false);
            }
            else if (levelnum == 2)
            {
                SetTrailObj = SteamCity;
                SteamCity.SetActive(true);
                SetTrailObj.GetComponent<ParticleSystem>().Play();
                DefaultTrail.SetActive(false);
                SpaceTrail.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Auto FLick
        public void AutoFlick()
        {
            arcStart = transform.position;
            arcEnd = gamemanager.goalBucket.goalTarget.position;

            float distance = Vector3.Distance(arcStart, arcEnd);
            float baseLaunchHeight = distance * 0.25f;
            launchHeight = baseLaunchHeight * launchHeightMul;
            flightTimer = 0f;
            lastPosition = arcStart;
            windOffset = Vector3.zero;
            windApplied = true;
            isFlicked = true;

            Vector3 dir = (arcEnd - arcStart).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            rb.MoveRotation(targetRotation);
        }
        #endregion

        #region DummyBall manager
        void ManageDummyBall(string IsSplash)
        {
            if (IsSplash == "Water")
            {
                gamemanager.goalBucket.StopEmission();
                staticBallGenerate.SetBallTransform(transform);
              //  Instantiate(WaterSplash, transform.position, Quaternion.Euler(-90, 0, 0), gamemanager.transform);
                gamemanager.audioSource.WaterTossPlay();
            }
            else if (IsSplash == "Dust")
            {
                gamemanager.goalBucket.StopEmission();
                staticBallGenerate.SetBallTransform(transform);

                //  Instantiate(staticBall, transform.position, Quaternion.Euler(-90, 0, 0), gamemanager.transform);
                gamemanager.audioSource.BallDropTossPlay();
            }
            else if (IsSplash == "Space")
            {
                gamemanager.goalBucket.StopEmission();
                staticBallGenerate.SetBallTransform(transform);

                //Instantiate(SpaceEx, transform.position, Quaternion.Euler(-90, 0, 0), gamemanager.transform);
                gamemanager.audioSource.SpaceTossPlay();
            }

        }
        #endregion


        #endregion
    }
}
