using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    
    public class StaticBallScript : MonoBehaviour
    {

        [SerializeField] List<GameObject> staticball = new List<GameObject>();

        private void Awake()
        {
            StaticballDisable();
        }

        void StaticballDisable()
        {

            for (int i = 0; i < staticball.Count; i++) 
            {
                staticball[i].SetActive(false);
            }
        }

        public void SetStaticBallEnables(int enableball)
        {
            staticball[enableball].SetActive(true);
        }
    }
    
}