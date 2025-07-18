using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    public class GoalHandler : MonoBehaviour
    {
        [Header("-- Ref --")]
        public FootballTossGameManager gameManager;

        [Header("-- Star Particle --")]
        [SerializeField] ParticleSystem GoalParticle;

        [Header("-- Goal Info --")]
        public Transform goalTarget;
        public Vector2 zLimit = new Vector2(0f, 10f);
        public Vector2 movingLimit = new Vector2(-5f, 5f); // relative to startingPos
        public float movingSpeed;
        private float currentSpeed;
        private Vector3 startingPos;
        private int direction = 1;

      //  [SerializeField] Transform SeaPanel, SpacePanel, SteamCityPanel;

        public bool isMoving = false;

        private Coroutine moveRoutine;
        [SerializeField] Animator BucketAni;
     //   [SerializeField] GameObject SeaView,StaemCityView, SpaceView;
        [SerializeField] Material BucketMaterial,SoccerMaterial,VollyMaterial,GolfMaterial;
        [SerializeField] Color SoccerColor, VollyColor, GolfColor;
        [SerializeField] ParticleSystem BuckerRingParticle;
        [SerializeField] Color SeaViewColor, SteamViewColor, SpaceViewColor;

        [SerializeField] ParticleSystem TargetParticle;
        [SerializeField] int goalcount;
        [SerializeField] TextMeshPro ComboText;

        private void Start()
        {
            var emission = TargetParticle.emission;
            emission.enabled = false;
            ComboText.text = "";
        }
        public void ChangeLande(int obj,Color NormalColor, Color SpaceColor)
        {
            var main = BuckerRingParticle.main;

            if (obj == 0) // Enable sea view
            {
              
               /* SeaView.SetActive(true);
                StaemCityView.SetActive(false);
                SpaceView.SetActive(false);*/
              
                main.startColor = SeaViewColor;

                BucketMaterial.color = NormalColor;
                SoccerMaterial.color = NormalColor;
                VollyMaterial.color = NormalColor;
                GolfMaterial.color = NormalColor;
                gameManager.footBall.SetTrail(0);
            }
            else if (obj == 1) // Enable SpaceView
            {

               /* SeaView.SetActive(false);
                StaemCityView.SetActive(false);
                SpaceView.SetActive(true);*/

                main.startColor = SpaceViewColor;

                BucketMaterial.color = SpaceColor;
                SoccerMaterial.color = NormalColor;
                VollyMaterial.color = NormalColor;
                GolfMaterial.color = NormalColor;
                gameManager.footBall.SetTrail(1);


            }
            else if (obj == 2) // Enable SteamCity
            {
               /* StaemCityView.SetActive(true);
                SpaceView.SetActive(false);
                SeaView.SetActive(false);*/

                main.startColor = SteamViewColor;

                BucketMaterial.color = NormalColor;
                SoccerMaterial.color = SoccerColor;
                VollyMaterial.color = VollyColor;
                GolfMaterial.color = GolfColor;
                gameManager.footBall.SetTrail(2);

            }
        }



        private void OnTriggerEnter(Collider other)
        {
            var fh = other.gameObject.GetComponent<FootballHandler>();
            if (fh && !fh.isReset)
            {
                goalcount++;
                fh.Setinparent(transform);
                fh.isReset = true;
                fh.transform.position = transform.position;
                GoalParticle.Play();
                fh.selectedmesh.enabled = false;
                other.GetComponent<MeshRenderer>().enabled = false;
                other.GetComponent<SphereCollider>().enabled = true;
                fh.Goal();
                BucketAni.Play("GoalAnimation");
                gameManager.AddBall();
                gameManager.IncLvl(true);
                gameManager.audioSource.GoalTossPlay();

                if (goalcount > 1)
                {
                    if (ComboText.transform.localScale == Vector3.zero)
                        ComboText.transform.DOScale(Vector3.one, 0.1f);

                    else
                    {
                        ComboText.transform.DOShakeScale(0.3f, new Vector3(1f, 1f, 1f), 1, 1f, false);
                    }


                    ComboText.text = "x" + goalcount.ToString();
                }
                



                if (goalcount >= 2)
                {
                    var emission = TargetParticle.emission;
                    emission.enabled = true;
                }
            }
        }

        public void StopEmission()
        {

            if (ComboText.transform.localScale == Vector3.one)
            {
                ComboText.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => 
                {
                    ComboText.text = "";
                });
            }

            goalcount = 0;
            var emission = TargetParticle.emission;
            emission.enabled = false;
        }

        public void AdjustBucketPosition(bool move)
        {
            var cam = Camera.main;
            if (!cam) return;

            float z = transform.position.z;
            float dist = Mathf.Abs(z - cam.transform.position.z);

            float minX = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, dist)).x;
            float maxX = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, dist)).x;

            if (move)
            {
                minX = Mathf.Max(minX, movingLimit.x);
                maxX = Mathf.Min(maxX, movingLimit.y);
            }

            float newX = Random.Range(minX, maxX);
           
            var pos = new Vector3(newX, transform.position.y, z);
            
            pos.x = Mathf.Clamp(pos.x, -4 + movingLimit.y, 4 + movingLimit.x);

            transform.DOMove(pos, 0.15f);

            if (moveRoutine != null)
            {
            
                StopCoroutine(moveRoutine);
            
                moveRoutine = null;
            
            }


         /*   SeaPanel.DOMove(new Vector3(pos.x, SeaPanel.transform.position.y, pos.z), 0.15f).OnComplete(() =>
            {
                if (move)
                {
                    StartSideToSideWithPause(SeaPanel.transform);
                }
            });

            SteamCityPanel.DOMove(new Vector3(pos.x, SteamCityPanel.transform.position.y, pos.z), 0.15f).OnComplete(() =>
            {
                if (move)
                {
                    StartSideToSideWithPause(SteamCityPanel.transform);
                }
            });

            SpacePanel.DOMove(new Vector3(pos.x, SpacePanel.transform.position.y, pos.z), 0.15f).OnComplete(() =>
            {
                if (move)
                {
                    StartSideToSideWithPause(SpacePanel.transform);
                }
            });*/

        }

        private void StartSideToSideWithPause(Transform trans)
        {
        
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            startingPos = transform.position;
            direction = 1;
            currentSpeed = Random.Range(movingSpeed, movingSpeed + 2f);
            moveRoutine = StartCoroutine(SideToSideRoutine(trans));
        
        }

        private IEnumerator SideToSideRoutine(Transform trans)
        {
            isMoving = true;

            while (true)
            {
                while (!ReachedLimit(trans))
                {
                    if (!isMoving)
                    {
                        yield return null;
                        continue;
                    }

                    transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;
                    // Example: move panel together if needed
                    // trans.position += Vector3.right * direction * currentSpeed * Time.deltaTime;

                    yield return null;
                }

                isMoving = false;
                yield return new WaitForSeconds(1f);
                isMoving = true;

                direction *= -1;
            }
        }

        private bool ReachedLimit(Transform trans)
        {
            float offsetX = transform.position.x - startingPos.x;

            if (direction == 1 && offsetX >= movingLimit.y)
                return true;
            if (direction == -1 && offsetX <= movingLimit.x)
                return true;

            return false;
        }
    }
}
