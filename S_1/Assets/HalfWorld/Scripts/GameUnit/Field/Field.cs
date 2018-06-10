using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    
    [RequireComponent(typeof(MeshRenderer))]
    public class Field
        : GameUnit
    {
        //所属城市
        [Header("城市相关")]
        private City m_cityUnit = null;
        public City CityUnit
        {
            set { m_cityUnit = value; }
        }

        //野外信息
        public FieldData fieldData;

        //野外名字
        public string FieldName
        {
            get
            {
                return string.Format("{0}({1})", name, m_resetTimes);   
            }
        }

        public override void Init(params object[] args)
        {
            if (m_inited)
                return;

            //起始默认难度为1
            InitWithDiff(1);
            //向世界管理器注册
            WorldManager.Instance.OperateField(this, true);
            m_inited = true;
        }

        public override void Reset(params object[] args)
        {
        }

        public override void Destroy()
        {
            base.Destroy();
            //通知世界管理器移除
            WorldManager.Instance.OperateField(this, false);
        }

        #region 探索相关
        //探索信息
        public class ExploreData
        {
            public float exploreTimeMultiple;   //探索时间倍数
            public float expMultiple;           //经验倍数
            public float goldMultiple;          //金币倍数
            public float fameMultiple;          //声望倍数

            public float expUpdater;
            public float goldUpdater;
            public float fameUpdater;
        }

        //探索英雄对应的探索信息(可同时被多个英雄探索)
        public Dictionary<int, ExploreData> m_exploringHeros;
        
        //重置英雄，当被一个新英雄探索，或英雄的属性发生了变化时调用
        public void ResetHero(Hero hero, bool resetAll)
        {
            if (!hero || !m_exploringHeros.ContainsKey(hero.GetInstanceID()))
                return;

            //主要为了重置计算公式呢~
            ExploreData ed = m_exploringHeros[hero.GetInstanceID()];
            //float oldExpMultiple = ed.exploreTimeMultiple;
            //float oldFameMultiple = ed.exploreTimeMultiple;
            //float oldGoldMultiple = ed.exploreTimeMultiple;
            ed.exploreTimeMultiple = StrategeCalculator.Instance.CalculateExploreTimeMultiple(hero.heroData, fieldData);
            ed.expMultiple = StrategeCalculator.Instance.CalculateExpMultiple(hero.heroData, fieldData);
            ed.fameMultiple = StrategeCalculator.Instance.CalculateFameMultiple(hero.heroData, fieldData);
            ed.goldMultiple = StrategeCalculator.Instance.CalculateGoldMultiple(hero.heroData, fieldData);
            //Debug.LogError("y英雄升级，重置数据");
            //Debug.LogError(string.Format("exp m:{0:0.0} -> {1:0.0}", oldExpMultiple, ed.exploreTimeMultiple));
            //Debug.LogError(string.Format("fame m:{0:0.0} -> {1:0.0}", oldFameMultiple, ed.expMultiple));
            //Debug.LogError(string.Format("gold m:{0:0.0} -> {1:0.0}", oldGoldMultiple, ed.goldMultiple));
            if (resetAll)
            {
                ed.expUpdater = 0f;
                ed.goldUpdater = 0f;
                ed.fameUpdater = 0f;
            }
        }

        //英雄探索时帧调用
        public void Explore(Hero hero)
        {
            if(!hero)
                return;

            if (m_exploringHeros == null)
                m_exploringHeros = new Dictionary<int, ExploreData>();

            //保存英雄的记录
            int heroID = hero.GetInstanceID();
            if (!m_exploringHeros.ContainsKey(heroID))
            {
                m_exploringHeros.Add(heroID, new ExploreData());
                //重置英雄，重置时间倍数值
                ResetHero(hero, true);
            }

            ExploreData ed = m_exploringHeros[heroID];

            float validTime = Time.deltaTime / ed.exploreTimeMultiple;
            //更新经验
            ed.expUpdater += (validTime * ed.expMultiple * fieldData.expRate * 0.01f);
            if (ed.expUpdater >= 1f)
            {
                hero.AddExp(1);
                ed.expUpdater = 0f;
            }
            //更新声望
            ed.fameUpdater += (validTime * ed.fameMultiple * fieldData.fameRate * 0.01f);
            if (ed.fameUpdater >= 1f)
            {
                hero.AddFame(1);
                ed.fameUpdater = 0f;
            }
            //更新金币
            ed.goldUpdater += (validTime * ed.goldMultiple * fieldData.goldRate * 0.01f);
            if (ed.goldUpdater >= 1f)
            {
                hero.AddGold(1);
                ed.goldUpdater = 0f;
            }

            //资源量减少
            fieldData.resRemain -= validTime;
            //没有剩余资源了
            if (fieldData.resRemain <= 0f)
            {
                FieldResOver();
            }
            else
            {
                //显示资源剩余(血条)
                UpdateRemainBar(fieldData.resRemain / fieldData.resVolume);
            }
        }

        //根据难度进行初始化
        private void InitWithDiff(int diff)
        {
            int resVolume = Random.Range(10, 30);
            //生成的野外所带金币、声望比例在一定范围内浮动
            int fameRate = Random.Range(50, 151);
            int goldRate = 200 - fameRate;

            //重新创建一个新的野外数据
            fieldData = new FieldData(
                resVolume,
                100,
                goldRate,
                fameRate,
                diff);

            //重置颜色
            ResetDifficultyColor();

            //只在被探索时显示资源剩余量
            m_objTime.SetActive(false);
        }

        //没有资源了
        private void FieldResOver()
        {
            ShowField(false);
            //清空探索中的英雄列表
            m_exploringHeros.Clear();
            //隐藏资源条
            m_objTime.SetActive(false);
            //准备迎来第二春
            StartCoroutine(WaitForReset());
        }

        //显示及隐藏
        private void ShowField(bool show)
        {
            ColorRender.enabled = show;
        }
        #endregion


        #region 颜色及显示
        [SerializeField] Color[] m_colors;
        private MeshRenderer m_colorRenderer;
        private Material m_colorMaterial;
        private MeshRenderer ColorRender
        {
            get
            {
                if (m_colorRenderer == null)
                    m_colorRenderer = GetComponent<MeshRenderer>();

                return m_colorRenderer;
            }
        }
        private Material ColorMat
        {
            get
            {
                if (m_colorMaterial == null)
                {
                    m_colorMaterial = ColorRender.material;
                }
                return m_colorMaterial;
            }
        }
        private void ResetDifficultyColor()
        {
            //根据难度等级设置不同的颜色
            if (m_colors != null && fieldData.difficulty <= m_colors.Length)
            {
                ColorMat.color = m_colors[fieldData.difficulty - 1];
            }
            else
            {
                //出现问题的设置成洋红色，很直观
                ColorMat.color = new Color(1, 0, 1);
            }
        }
        #endregion

        #region 野外重置
        //重置时间
        [SerializeField] private float m_resetTime = 10f;  
        //重置次数
        private int m_resetTimes = 0;   
        IEnumerator WaitForReset()
        {
            yield return new WaitForSeconds(m_resetTime);

            //记录重启次数
            ++m_resetTimes;
            //获取新的难度
            int newDiff = WorldManager.Instance.GetReasonableFieldDifficulty();
            //重置数值
            InitWithDiff(newDiff);
            //激活显示
            ShowField(true);
        }
        #endregion

        #region 血条及更新
        [SerializeField]
        Transform m_tranTimeRemain;
        [SerializeField]
        GameObject m_objTime;
        private void UpdateRemainBar(float remain)
        {
            if (!m_objTime.activeSelf)
                m_objTime.SetActive(true);

            float rm = Mathf.Clamp01(remain);
            if(m_tranTimeRemain)
            {
                m_tranTimeRemain.localPosition = new Vector3(0f, 1f - rm, 0f);
                m_tranTimeRemain.localScale = new Vector3(1.2f, rm + 0.01f, 1.2f);
            }
        }
#endregion

        public override string Desc()
        {
            return string.Empty;
        }

    }
}