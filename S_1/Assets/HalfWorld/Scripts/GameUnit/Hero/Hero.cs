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
            Move,       //向野外移动
            Explore,    //探索中
            Recover     //恢复中
        }

        [SerializeField] private HeroState m_heroState = HeroState.Idle;
        [SerializeField] private Field m_fieldTarget;
        [SerializeField] private City m_cityTarget;

        public HeroData heroData;

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
                return m_cityTarget != null ? (GameUnit)m_cityTarget : m_fieldTarget; 
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

        /// <summary>
        /// 选择目的地
        /// </summary>
        void SearchTarget()
        {
            float highest = -Mathf.Infinity;
            Field target = null;

            //便利所有野外，计算目标
            var fields = WorldManager.Instance.AllFields;
            while (fields.MoveNext())
            {
                var field = fields.Current;
                //获取得分最高的野外为移动目标
                float curWeight = CalcFieldWeight(field);
                EUtilityHelperL.LogError(string.Format("{0}->{1:0.00}", field.name, curWeight));
                if (curWeight > highest)
                {
                    target = field;
                    highest = curWeight;
                }
            }

            //设置目标
            if (target != null)
            {
                m_fieldTarget = target;
                //切换状态
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
                if(dis.sqrMagnitude <= 0.5f)
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
                    transform.Translate(dis.normalized * Time.deltaTime * heroData.moveSpeed);
                }
            }
        }

        //扣减hp
        float damageTimer = 0f;
        void Damage()
        {
            damageTimer += Time.deltaTime;
            if(damageTimer >= 0.1f)
            {
                UpdateHpBar(--heroData.hpCur * 1f / heroData.hpMax);
                damageTimer = 0f;
                heroData.hpCur = Mathf.Clamp(heroData.hpCur, 0, heroData.hpMax);
                if(heroData.hpCur <= 0)
                {
                    //回到离自己最近的城市
                    SearchCity();
                }
            }
        }

        void SearchCity()
        {
            float dis = Mathf.Infinity;
            float tempDis = 0f;
            City target = null;
            var citys = WorldManager.Instance.AllCities;
            while (citys.MoveNext())
            {
                tempDis = EUtilityHelperL.CalcDistanceIn2D_SQR(citys.Current.transform.position, transform.position);
                if(tempDis < dis)
                {
                    target = citys.Current;
                    dis = tempDis;
                }
            }

            if(target)
            {
                m_cityTarget = target;
                State = HeroState.Move;
            }
        }

        //探索目标
        void ExploreTarget()
        {
            if(!m_fieldTarget)
            {
                State = HeroState.Idle;
                return;
            }

            //如果当前野外还有剩余资源
            if(m_fieldTarget.fieldData.resRemain > 0f)
            {
                //探索
                m_fieldTarget.Explore(this);
                //探索时扣减英雄生命
                Damage();
            }
            else
            {
                //尝试切换状态
                State = HeroState.Idle;
            }
        }

#region Recover
        float m_recoverTimer = 0f;
        void RecoverHP()
        {
            m_recoverTimer += Time.deltaTime;
            if(m_recoverTimer >= 0.1f)
            {
                UpdateHpBar(++heroData.hpCur* 1f / heroData.hpMax);
                m_recoverTimer = 0f;
                heroData.hpCur = Mathf.Clamp(heroData.hpCur, 0, heroData.hpMax);
                if (heroData.hpCur == heroData.hpMax)
                {
                    //恢复好了继续返回探索
                    m_cityTarget = null;
                    State = HeroState.Move;
                }
            }
        }

        #endregion

        private void InitHeroData()
        {
            heroData.level = 1;
            heroData.exp = 0;
            heroData.hpMax = 50;
            heroData.hpCur = heroData.hpMax;
            heroData.strength = Random.Range(10, 20);
            heroData.moveSpeed = 5;
        }

        //计算野外对于此英雄的权重
        private float CalcFieldWeight(Field field)
        {
            if(!StrategeCalculator.Instance)
            {
                return 0f;
            }

            if(!field)
                return 0f;

            return StrategeCalculator.Instance.Calculate(
                heroData, transform.position, 
                field.fieldData, field.transform.position);
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

        //更新血条
        #region HP Bar

        [SerializeField] GameObject m_objHP;

        [SerializeField] Transform m_tranHpRemain;
        private void UpdateHpBar(float remain)
        {
            float rm = Mathf.Clamp01(remain);
            if (m_tranHpRemain)
            {
                m_tranHpRemain.localPosition = new Vector3(0f, 1f - rm, 0f);
                m_tranHpRemain.localScale = new Vector3(1.2f, rm + 0.01f, 1.2f);
            }
        }

#endregion
    }
}