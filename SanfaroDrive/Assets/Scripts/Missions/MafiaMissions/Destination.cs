using System;
using UnityEngine;

namespace Missions.MafiaMissions
{
    public class Destination : MonoBehaviour
    {
        [SerializeField] private GameObject npc;

        private void Update()
        {
            var distance = Vector3.Distance(transform.position, npc.transform.position);
            if (distance <= 2)
            {
                npc.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}