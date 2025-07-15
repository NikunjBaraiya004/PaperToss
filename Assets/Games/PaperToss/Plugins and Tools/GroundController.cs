using System.Collections.Generic;
using UnityEngine;

namespace nostra.booboogames.PaperToss
{

    public class GroundController : MonoBehaviour
    {
        [SerializeField] List<GameObject> groundObj;
        [SerializeField] GoalHandler gh;
        [SerializeField] Color SpaceColor, NormalColor;

        private void Start()
        {
            ChangeRandomGround();

        }

        public void ChangeRandomGround()
        {
            DisableGround();
            EnableRandomGround();

        }

        void DisableGround()
        {
            for (int i = 0; i < groundObj.Count; i++)
            {
                groundObj[i].gameObject.SetActive(false);
            }
        }


        void EnableRandomGround()
        {
            int SelectRanGround = Random.Range(0, groundObj.Count);
            groundObj[SelectRanGround].gameObject.SetActive(true);
            gh.ChangeLande(SelectRanGround, NormalColor, SpaceColor);
        }

    }
    
}