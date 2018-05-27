using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{   
    public class Hero
        : GameUnit
    {
        public enum HeroState
        {
            Idle,       //空闲
            Move,       //移动中
            Explore,    //探索中
            Recover     //恢复中
        }


        [SerializeField] private HeroState m_heroState = HeroState.Idle;
        [SerializeField] private HeroData m_heroData;
        [SerializeField] private Field m_fieldTarget;
        [SerializeField] private City m_cityTarget;

        public int HeroStr
        {
            get
            {
                return m_heroData.strength;
            }
        }

        private HeroState State
        {
            set
            {
                if(m_heroState != value)
                    m_heroState = value;
            }
        }

        private GameUnit MoveTarget
        {
            get
            {
                return m_fieldTarget != null ? (GameUnit)m_fieldTarget : m_cityTarget; 
            }
        }

        void Awake()
        {
            Init();
        }

        public override void Init(params object[] args)
        {
            if(m_inited)
                return;

            //初始化英雄状态
            InitHeroData();

            m_inited = true;
        }

        void Update()
        {
            switch (m_heroState)
            {
                case HeroState.Idle:
                    //寻找下一个野外目标
                    SearchTarget();
                break;

                case HeroState.Move:
                    //向目标位置移动
                    MoveToTarget();
                break;

                case HeroState.Explore:
                    //探索目标
                    ExploreTarget();
                break;

                case HeroState.Recover:
                    //恢复体力
                    RecoverHP();
                break;
            }
        }

        void SearchTarget()
        {
            float highest = -Mathf.Infinity;
            Field target = null;
            
            //便利所有野外，计算目标
            var fields = WorldManager.Instance.AllFields;
            while (fields.MoveNext())
            {
                var field = fields.Current;
                float curWeight = CalcFieldWeight(field);
                if(curWeight > highest)
                {
                    target = field;
                    highest = curWeight;
                }
            }

            if(target != null)
            {
                m_fieldTarget = target;
                State = HeroState.Move;
            }
        }

        void MoveToTarget()
        {
            if(MoveTarget != null)
            {
                Vector3 dis = MoveTarget.transform.position - transform.position;
                dis.y = 0f;
                //到了
                if(dis.sqrMagnitude <= 4f)
                {
                    if(MoveTarget.GameUnitType == GameUnitType.CITY)
                    {
                        //城市
                        State = HeroState.Recover;
                    }
                    else if(MoveTarget.GameUnitType == GameUnitType.FIELD)
                    {
                        //野外
                        State = HeroState.Explore;
                    }
                    else
                    {
                        Debug.LogError("到达了奇怪的地方～");
                        State = HeroState.Idle;
                    }
                }
                else
                {
                    //向目标移动
                    transform.Translate(dis.normalized * Time.deltaTime * m_heroData.moveSpeed);
                }
            }
        }

        void ExploreTarget()
        {
            if(!m_fieldTarget)
            {
                State = HeroState.Idle;
                return;
            }

            if(m_fieldTarget.fieldData.timeRemain > 0f)
            {
                m_fieldTarget.Explore(this);
            }
            else
            {
                State = HeroState.Idle;
            }
        }

        void RecoverHP()
        {

        }

        private void InitHeroData()
        {
            m_heroData.level = 0;
            m_heroData.exp = 0;
            m_heroData.hpMax = 100;
            m_heroData.hpCur = m_heroData.hpMax;
            m_heroData.strength = Random.Range(10, 20);
            m_heroData.moveSpeed = 1;
        }

        //计算野外对于此英雄的权重
        private float CalcFieldWeight(Field field)
        {
            //test
            return Random.Range(100f, 200f);

            if(!field)
                return 0f;

            //距离的平方
            float dis = (field.transform.position - transform.position).sqrMagnitude;
            
            return Random.Range(100f, 200f);
            return 0f;
        }

        ///根据英雄情况计算探索时间
        private float CalcTimeCost(float baseTime, int difficulty, int heroStr)
        {
            return baseTime / CalcExploreSpd(difficulty, heroStr);
        }

        private float CalcExploreSpd(int difficulty, int heroStr)
        {
            int needStr = 10;
            switch (Mathf.Clamp(difficulty, 1, 5))
            {
                case 1:
                    needStr = 10;
                    break;
                case 2:
                    needStr = 20;
                    break;
                case 3:
                    needStr = 40;
                    break;
                case 4:
                    needStr = 65;
                    break;
                case 5:
                    needStr = 100;
                    break;
            }
            float exploreSpd = 1f - (heroStr - needStr * 1f) / needStr;
            //探索速度
            exploreSpd = Mathf.Clamp(exploreSpd, 0.5f, 2f);

            return exploreSpd;
        }


#region override
        public override void Reset(params object[] args)
        {
            
        }

        public override string Desc()
        {
            return string.Empty;
        }
#endregion
    }
}