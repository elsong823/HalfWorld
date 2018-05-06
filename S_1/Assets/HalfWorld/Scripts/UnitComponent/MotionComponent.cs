using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class MotionComponent
        : UnitComponent
    {
        private bool m_moving = false;
        private Vector3 m_moveTarget;

        public void MoveTo(Vector3 target)
        {
            m_moveTarget = target;
						m_moveTarget.y = transform.position.y;
						m_moving = true;
        }

        void Update()
        {
            if (m_moving)
            {
                if (Vector3.Distance(transform.position, m_moveTarget) <= 0.1f)
                {
                    transform.position = m_moveTarget;
                    m_moving = false;
										Debug.LogError("到达目的地");
                }
                else
                {
                    Vector3 moveDir = (m_moveTarget - transform.position).normalized;
                    transform.Translate(10f * Time.deltaTime * moveDir);
                }
            }
        }
    }
}