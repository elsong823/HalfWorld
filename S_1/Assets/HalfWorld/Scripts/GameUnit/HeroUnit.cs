using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class HeroUnit
        : GameUnit
    {
        private MotionComponent m_motionComponent;
        
        private MotionComponent MotionComponent
        {
            get
            {
                if(!m_motionComponent)
                {   
                    m_motionComponent = GetComponent<MotionComponent>();
                }

                return m_motionComponent;
            }
        }

        [SerializeField, Range(1f, 10f)] private float m_heroMoveSpeed;

        public void MoveTo(Vector3 target)
        {
            MotionComponent.MoveTo(target, m_heroMoveSpeed, delegate()
            {
                Debug.LogError("到达目的地");
            });
        }
    }
}