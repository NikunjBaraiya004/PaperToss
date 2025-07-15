using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{
    public class MotivationText : MonoBehaviour
    {
        [SerializeField] SpriteRenderer MotiTxtImg;
        [SerializeField] List<Sprite> motisprite;
        [SerializeField] float moveUpDistance = 1f;  // Adjustable upward distance
        [SerializeField] float moveUpDuration = 2f;
        [SerializeField] Transform Parentpos;
        private Vector3 initialPosition;
        private Color initialColor;

        private void Awake()
        {
            initialPosition = MotiTxtImg.transform.position;
            initialColor = MotiTxtImg.color;
        }

        private void OnEnable()
        {
            MotiTxtImg.transform.position = initialPosition;
            MotiTxtImg.color = initialColor;
            SetRandomImg();
        }

        public void SetRandomImg()
        {
            int randomselectImg = Random.Range(0, motisprite.Count);
            MotiTxtImg.sprite = motisprite[randomselectImg];
            EnableAnimation();
        }

        void EnableAnimation()
        {
            Sequence seq = DOTween.Sequence();

            MotiTxtImg.transform.localPosition = Vector3.zero;
            MotiTxtImg.transform.localScale = Vector3.zero;
            MotiTxtImg.color = initialColor;

            MotiTxtImg.transform.SetParent(null);

            seq.Append(MotiTxtImg.transform.DOScale(Vector3.one * 1f, 1.5f).SetEase(Ease.OutBack))
               .Join(MotiTxtImg.transform.DOMoveY(initialPosition.y + moveUpDistance, moveUpDuration).SetEase(Ease.OutSine))
               .Append(MotiTxtImg.DOFade(0f, 0.25f))
               .Join(MotiTxtImg.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear))
               .OnComplete(() => {
               
                   gameObject.SetActive(false);
                   
                   MotiTxtImg.color = initialColor;
                   
                   MotiTxtImg.transform.SetParent(Parentpos);
                   MotiTxtImg.transform.localPosition = new Vector3(0,1,0); 
               
               });
        }
    }
}
