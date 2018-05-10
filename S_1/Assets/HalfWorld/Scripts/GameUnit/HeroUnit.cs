using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(HeroUnit))]
    public class HeroUnitInspector
        :Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("测试:扫描"))
                {
                    HeroUnit unit = target as HeroUnit;
                    if (unit)
                    {
                        unit.ScanRound();
                    }
                }
            }
        }
    }
#endif
    

    public class HeroUnit
        : GameUnit
    {
        [Space(10)]
        //移动组件
        [SerializeField] private MotionComponent m_motionComponent;
        //策略组件
        [SerializeField] private StrategyComponent m_strategyComponent;

        [Space(10)]

        [SerializeField, Range(1f, 10f)] private float m_heroMoveSpeed = 5f;

        [SerializeField, Range(1f, 15f)] private float m_viewRadius = 5f;

        public void MoveTo(Vector3 target)
        {
            m_motionComponent.MoveTo(target, m_heroMoveSpeed, delegate ()
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

        public void ScanRound()
        {
            m_strategyComponent.ScanRound(m_viewRadius);
        }
    }
}