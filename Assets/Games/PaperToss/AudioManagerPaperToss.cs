using UnityEngine;

namespace nostra.booboogames.slapcastle
{
    
    public class AudioManagerPaperToss : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        [SerializeField] AudioClip ThrowToss, GoalToss, WaterSplash, spaceSparking, balldrop;


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

        void PlayAudio(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    
}