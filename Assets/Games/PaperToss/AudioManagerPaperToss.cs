using UnityEngine;

namespace nostra.booboogames.slapcastle
{
    
    public class AudioManagerPaperToss : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        [SerializeField] AudioClip ThrowToss, GoalToss, WaterSplash, spaceSparking, balldrop, FootballSound, BasketBallSound, GolfSound;


        public void BallDropTossPlay()
        {
            PlayAudio(balldrop);
        }

        public void ThrowTossPlay()
        { 
            PlayAudio(ThrowToss);
        }

        public void GoalTossPlay()
        { 
            PlayAudio(GoalToss);
        }

        public void WaterTossPlay()
        { 
            PlayAudio(WaterSplash);
        }

        public void SpaceTossPlay()
        {
            PlayAudio(spaceSparking);
        }

        public void BallDropSound(int BallName)
        {
            if (BallName == 0)
                PlayAudio(BasketBallSound);

            else if (BallName == 1)
                PlayAudio(FootballSound);

            else if(BallName == 2)
                PlayAudio(FootballSound);

            else if(BallName == 3)
                PlayAudio(GolfSound);
        }

        void PlayAudio(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }


    }

    
}