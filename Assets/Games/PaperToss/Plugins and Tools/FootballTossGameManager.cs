using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using nostra.booboogames.slapcastle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nostra.booboogames.PaperToss
{ 
    public class FootballTossGameManager : MonoBehaviour
    {
        [Header("-- Lvl Info --")]
        public int windIntroLvl;
        public int movingGoalIntroLvl;

        [Header("-- Score Info --")]
        public TextMeshProUGUI scoreTxt;
        public TextMeshProUGUI LifeTxt;
        public int MaxLife = 10;
        
        int scoreVal = 0;
        int currLife = 0;

        [Header("-- ref --")]
        public FootballHandler footBall;
        public GoalHandler goalBucket;
        public Camera gameMainCam;
        [SerializeField] GroundController groundController;
       
        [Header("-- Wind Info --")]
        public Vector2 windMinMax;
        public FanRotation fanrotaion;
        bool hasWind;

        public WindIndicator windIndicator;
      
        [HideInInspector] public Vector3 windDirection;
        [HideInInspector] public float windMul;
        public bool isAutoPlay;
        public bool isPlaying;

        [Header("-- Motivate Text --")]
        [SerializeField] GameObject motivateTextObj;
       // [SerializeField] string ballText;

        [Header("-- GameOver --")]
        [SerializeField] GameObject gameOverObj;
        [SerializeField] Button RestartBtn, RestartBtn2;

        [Header("AudioSource")]
        public AudioManagerPaperToss audioSource;

        #region Game Control

        public void OnLoaded()
        {
            // Game load
        }
        public void OnFocussed()
        {
            ChangeGroundandBall();
            fanrotaion.gameObject.SetActive(false); 
            //auto play
            isAutoPlay = true;
            footBall.ResetBall(0);
            StartLevel();
        }

        void ChangeGroundandBall()
        {
            //footBall.SetRandomBall();
            groundController.ChangeRandomGround();
        }



        public void OnStart()
        {
            
            //start player controll
            isAutoPlay = false;
            StopCoroutine(AutoPlay());
            
            footBall.ResetBall(0);

            currLife = MaxLife;
            scoreVal = 0;
            
            UpdateData();
            
            isPlaying = true;
            gameOverObj.SetActive(false);
            
            StartLevel();
            
        }

        public void AddBall()
        { 
            currLife++; 
            UpdateData();
        }

        public void OnPause()
        {
            //pause game play - stop player controll
            isPlaying = false;
            footBall.ResetBall(0);
        
            OnFocussed(); //start auto play
        }

        public void onRestart()
        {
            goalBucket.StopEmission();
            // reset all as initial
            ChangeGroundandBall();
            OnStart();
        }

        #endregion

        private void Start()
        {
            Application.targetFrameRate = 90;

            //score set
            currLife = MaxLife;
            scoreVal = 0;
            UpdateData();
            gameOverObj.SetActive(false);
            //Start Lvl
            //StartLevel();
            RestartBtn.onClick.AddListener(onRestart);
            RestartBtn2.onClick.AddListener(onRestart);
        }


        public void setRandomWind()
        {
            float x = UnityEngine.Random.value <= 0.5f ? -1 : 1;
            float y = UnityEngine.Random.Range(-0.5f, 0.4f);
            windDirection = new Vector3(x, y, 0);

            windMul = GetWindStrengthByScore(scoreVal);

            hasWind = windMul > 0 && (windDirection.x != 0 || windDirection.y != 0);

            windIndicator.SetIndicator(windMul, windDirection, hasWind, goalBucket.transform);
        }

        private float GetWindStrengthByScore(int score)
        {
            if (score == 7) return 0.1f;
            if (score == 8) return 0.3f;
            if (score == 9) return 0.5f;
            if (score == 10) return 0.7f;
            if (score == 11) return 0.9f;

            if (score > 20)
            {
                float[] windCycle = new float[] { 1.0f, 1.2f, 1.5f, 1.8f, 2.1f, 2.4f, 2.7f };
                int index = UnityEngine.Random.Range(0, windCycle.Length);
                return Mathf.Clamp(windCycle[index], windMinMax.x, windMinMax.y);
            }

            return UnityEngine.Random.Range(windMinMax.x, windMinMax.y);
        }



        //lvl start
        public void StartLevel(bool changePos = true)
        {
            if(isAutoPlay)
            {    
                if(footBall != null) 
                   footBall.ApplyWind(Vector3.zero, 0);
               
                StartCoroutine(AutoPlay());
                goalBucket.AdjustBucketPosition(false);
            }
            else
            {
                /* footBall.ApplyWind(windDirection, windMul);
                 goalBucket.AdjustBucketPosition(true);*/


                if (changePos)
                {
                   /* if (scoreVal < 7 || tries >= 2)
                    {
                        footBall.ApplyWind(Vector3.zero, 0);
                        footBall.DisableWind();
                        goalBucket.AdjustBucketPosition(false);
                        tries = 0;
                        return;
                    }*/
                    setRandomWind();
                    if (scoreVal >= 7 && scoreVal < 12)
                    {
                        footBall.ApplyWind(windDirection, windMul);
                        goalBucket.AdjustBucketPosition(false);
                    }
                    else if (scoreVal >= 12 && scoreVal < 20)
                    {
                        footBall.ApplyWind(Vector3.zero, 0);
                        footBall.DisableWind();
                        goalBucket.AdjustBucketPosition(true);
                    }
                    else
                    {
                        footBall.ApplyWind(windDirection, windMul);
                        goalBucket.AdjustBucketPosition(true);
                    }

                }
                else
                {
                    tries++;
                }
            }
        }


        int tries;
        public void IncLvl(bool isClear)
        {
            if (!isAutoPlay)
            {
                if (isClear)
                {
                    motivateTextObj.gameObject.SetActive(true);
                    scoreVal++;

                   // scoreTxt.DOColor(Color.green, 0.15f);                    
                    scoreTxt.transform.DOScale(new Vector3(1.8f,1.8f,1.8f), 0.5f).OnComplete(() => 
                    {
                        scoreTxt.transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
                        {
                     //       scoreTxt.DOColor(Color.black, 0.15f);
                        });
                    });
                }
                else
                {
                    currLife--;

                    if (currLife != 0)
                    {
                        LifeTxt.DOColor(Color.red, 0.15f);

                        LifeTxt.transform.DOShakePosition(0.15f, 1, 10, 90, false).OnComplete(() =>
                        {
                            LifeTxt.DOColor(Color.white, 0.15f);
                        });
                    }

                    if (currLife == 0)
                    {
                        GameOver();
                    }
                }
            }
            UpdateData();
            
        }

        /*public void IncCombo(bool IscomboContinue)
        {
            if (!isAutoPlay)
            {
                if (IscomboContinue)
                {
                    currCombo++;
                }
                else
                {
                    currCombo = 0;
                }
            }
            UpdateData();
        }*/


        void UpdateData()
        {
            scoreTxt.text = scoreVal.ToString();
            LifeTxt.text = /*ballText + "  " +*/ currLife.ToString();
        }

        IEnumerator AutoPlay()
        {
            yield return new WaitForSeconds(1f);
     
            if(isAutoPlay) footBall.AutoFlick();
        }


        void GameOver()
        {
          //scoreVal = 0;
          
          
            gameOverObj.SetActive(true);
            isPlaying = false;

          

            /* if (!isAutoPlay)
             {

                 GameOverLeaderboard leaderboard = new GameOverLeaderboard();

                 GameOverRank rank = new GameOverRank();

                 rank.playerName = "You";
                 rank.playerScore = 0;
                 rank.isPlayer = true;
                 leaderboard.lb.Add(rank);


                 rank = new GameOverRank();
                 rank.playerName = "Enemy";
                 rank.playerScore = 1;
                 rank.isPlayer = false;
                 leaderboard.lb.Add(rank);


                 controller.GameOverScreen(leaderboard);

             }*/

        }


    }
}
