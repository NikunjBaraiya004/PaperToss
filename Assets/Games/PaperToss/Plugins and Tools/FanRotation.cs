using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    
    public class FanRotation : MonoBehaviour
    {
        [SerializeField] Transform fanCenter;
        [SerializeField] float EffectSpeed;
        [SerializeField] float childspeed;
        [SerializeField] GameObject WindParticle;
        [SerializeField] ParticleSystem _BreezeEffect,_PaperEffect;
        [SerializeField] Texture2D _LeafSprite, _PaperSprite;
        [SerializeField] Color _Leaf1,_Leaf2,_Paper1, _Paper2;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            setFanSpeed(0.5f);
        }
        private void OnEnable()
        {
            transform.DOScale(Vector3.one * 0.6f,0.2f).SetEase(Ease.OutBounce);
        }

        public void SetLeafAndPaperParticle(int num)
        {
            var main = _PaperEffect.main;
            var renderer = _PaperEffect.GetComponent<ParticleSystemRenderer>();

            if (num == 0)
            {
                main.startColor = new ParticleSystem.MinMaxGradient(_Paper1, _Paper2);
                renderer.material.mainTexture = _PaperSprite;
            }
            else if (num == 1)
            {
                main.startColor = new ParticleSystem.MinMaxGradient(_Leaf1, _Leaf2);
                renderer.material.mainTexture = _LeafSprite;
            }
        }






        public void setFanSpeed(float speed,string Dir = null)
        {
          
            float duration = 1.5f / speed;

            var emi = _BreezeEffect.emission;
            var emi2 = _PaperEffect.emission;

            emi.rateOverTime = speed * EffectSpeed;
            emi2.rateOverTime = speed * childspeed;

            fanCenter.DOKill();
            
            fanCenter.transform.localRotation = Quaternion.Euler(Vector3.zero);

            if (Dir != null) 
            {
                if (Dir == "Right")
                {
                    WindParticle.transform.localPosition = new Vector3(30, 0, 20);
                    WindParticle.transform.DORotate(new Vector3(0, -150, 0), 0);
                }

                else if (Dir == "Left")
                {
                    WindParticle.transform.localPosition = new Vector3(-30, 0, 20);
                    WindParticle.transform.DORotate(new Vector3(0, -30, 0), 0);
                }
            }


            fanCenter.DOLocalRotate(
                new Vector3(0, 0, 360f),
                duration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        }

        private void OnDisable()
        {
            transform.DOScale(Vector3.zero, 0f);
        }
    }
    
}