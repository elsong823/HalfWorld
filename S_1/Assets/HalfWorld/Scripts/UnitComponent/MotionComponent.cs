using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ELGame
{
    public class MotionComponent
        : UnitComponent
    {
        //移动目标
        private Vector3 m_moveTarget;
        //移动速度
        private float m_moveSpeed = 0f;
        //是否移动中
        private bool m_moving = false;
        //到位后回调
        private Action m_arrivalCallback;
        
        /// <summary>
        /// 移动到目的地
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="moveSpeed">移动速度</param>
        /// <param name="arrivalCallback">到达后回调</param>
        public void MoveTo(Vector3 target, float moveSpeed, Action arrivalCallback)
        {
            //设置移动目标
            m_moveTarget = target;
            m_moveTarget.y = transform.position.y;

            //设置移动速度
            m_moveSpeed = Mathf.Max(moveSpeed, 0.1f); 

            //设置到达后的回调
            m_arrivalCallback = arrivalCallback;
            m_moving = true;
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void Stop()
        {
            m_moving = false;
            m_arrivalCallback = null;
        }

        //MotionComponent
        void Update()
        {
            if (m_moving)
            {
                //距离近则通知并停止移动
                if (Vector3.Distance(transform.position, m_moveTarget) <= 0.1f)
                {
                    transform.position = m_moveTarget;
                    if (m_arrivalCallback != null)
                    {
                        m_arrivalCallback();
                    }
                    Stop();
                }
                //向目标位置移动
                else
                {
                    Vector3 moveDir = (m_moveTarget - transform.position).normalized;
                    transform.Translate(m_moveSpeed * Time.deltaTime * moveDir);
                    //把移动路线画出来
                    Debug.DrawLine(transform.position, m_moveTarget);
                }
            }
        }
    }
}