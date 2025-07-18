using System.Collections.Generic;
using DG.Tweening;
using nostra.booboogames.slapcastle;
using NUnit.Framework;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    
    public class StaticBallScript : MonoBehaviour
    {

        [SerializeField] List<GameObject> staticball = new List<GameObject>();
        bool Visible = true;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] float ResetBallTimer = 1.5f;
        [SerializeField] Rigidbody rb;
        [SerializeField] int BallNum;
        public AudioManagerPaperToss audioSource;

        public void StaticballEnable(int visiblenum)
        {
            for (int i = 0; i < staticball.Count; i++) 
            {
                if (visiblenum == i)
                {
                    staticball[i].SetActive(true);
                    meshRenderer = staticball[i].GetComponent<MeshRenderer>();
                    BallNum = i;

                }
                else
                    staticball[i].SetActive(false);
            
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals("QP_Tag_04"))
            {
                audioSource.BallDropSound(BallNum);
            }
        }


        public bool GetVisible()
        { 
            return Visible;
        }


        bool isUnderForce = false;

        public void Setposition(Transform temppos)
        {
            transform.position = temppos.position;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            Visible = false;

            // Reset mesh alpha
            if (meshRenderer != null && meshRenderer.material.HasProperty("_Color"))
            {
                Color color = meshRenderer.material.color;
                color.a = 1f;
                meshRenderer.material.color = color;
            }

            rb.isKinematic = false;

            // STOP previous force
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Prevent overlapping force application
            if (isUnderForce)
                DOTween.Kill(rb); // Optional: stops any tweens on rigidbody

            isUnderForce = true;

            // Apply new force
            Vector3 forceDir = temppos.up + temppos.forward;
            float forcePower = 25f;

            rb.AddForce(forceDir.normalized * forcePower, ForceMode.Impulse);

            // Schedule reset
            DOVirtual.DelayedCall(ResetBallTimer, ResetPos);
        }




        void ResetPos()
        {
            if (meshRenderer != null && meshRenderer.material.HasProperty("_Color"))
            {
                Material mat = meshRenderer.material;

                // Fade out
                mat.DOFade(0f, 1f).OnComplete(() =>
                {
                    transform.localScale = Vector3.zero;
                    Visible = true;
                    isUnderForce = false;
                    rb.isKinematic = true;

                    // Reset alpha
                    Color resetColor = mat.color;
                    resetColor.a = 1f;
                    mat.color = resetColor;
                });
            }
        }


    }

}