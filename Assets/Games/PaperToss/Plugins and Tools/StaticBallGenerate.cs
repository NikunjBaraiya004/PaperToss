using System.Collections.Generic;
using nostra.booboogames.PaperToss;
using NUnit.Framework;
using UnityEngine;


namespace nostra.booboogames.PaperToss
{
    public class StaticBallGenerate : MonoBehaviour
    {

        [SerializeField] GameObject StaticBallPrefab;
        [SerializeField] List<StaticBallScript> StaticBallScripts = new List<StaticBallScript>();
        [SerializeField] Transform prefabparent;
        [SerializeField] int staticballcount;

        public void GenerateStaticBall(int visibleballnumber)
        {
            for (int i = 0; i < staticballcount; i++)
            {
                var ball = Instantiate(StaticBallPrefab, new Vector3(0, 0, -10), Quaternion.identity, prefabparent);
                var ballTemp = ball.GetComponent<StaticBallScript>();
                StaticBallScripts.Add(ballTemp);
                ballTemp.StaticballEnable(visibleballnumber);
            }
        }

        public void SetBallTransform(Transform balltransform)
        {
            for (int i = 0; i < StaticBallScripts.Count; i++)
            {
                if (StaticBallScripts[i].GetVisible())
                {
                    StaticBallScripts[i].Setposition(balltransform);
                    break;
                }

            }
        }
    } 
}
