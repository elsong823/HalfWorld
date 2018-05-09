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
                if (!m_motionComponent)
                {
                    m_motionComponent = GetComponent<MotionComponent>();
                }

                return m_motionComponent;
            }
        }

        [SerializeField, Range(1f, 10f)] private float m_heroMoveSpeed;

        [SerializeField, Range(1f, 5f)] private float m_viewRadius;

        public void MoveTo(Vector3 target)
        {
            MotionComponent.MoveTo(target, m_heroMoveSpeed, delegate ()
            {
                Debug.LogError("到达目的地");
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_viewRadius);
        }


        //英雄状态
        enum HeroState
        {
            IDLE,       //空闲
            MOVING,     //向目的地移动中
            EXPLORING,  //当前正在探索某个野外
            RECOVER,    //恢复中
        }
        //当前英雄的状态
        [SerializeField] private HeroState m_heroState = HeroState.IDLE;

        private void Update()
        {
            switch (m_heroState)
            {
                case HeroState.IDLE:
                    //时间就是生命，怎么能够闲着？
                    //找下一个目标吧
                    SearchForTarget();
                    break;
                case HeroState.MOVING:
                    //行动起来，向着目标前进吧
                    MoveToTarget();
                    break;
                case HeroState.EXPLORING:
                    //看看有什么好东西呢
                    ExploreTarget();
                    break;
                case HeroState.RECOVER:
                    //劳逸结合，让爷们休息一下
                    RecoverStrength();
                    break;
                default:
                    break;
            }
        }

        //搜索目标
        private void SearchForTarget()
        {

        }

        //向目标移动
        private void MoveToTarget()
        {

        }

        //探索目标
        private void ExploreTarget()
        {

        }

        //恢复战斗力
        private void RecoverStrength()
        {

        }
    }
}