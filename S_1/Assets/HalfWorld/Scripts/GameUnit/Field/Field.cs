using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    
    [RequireComponent(typeof(MeshRenderer))]
    public class Field
        : GameUnit
    {
        //探索信息
        public struct ExploreData
        {
            public float exploreSpeedMultiple;  //探索速度
            public float exploredTime;          //总探索时间
            public float validExploredTime;     //有效探索时间
        }

        //所属城市
        private City m_cityUnit = null;

        public City CityUnit
        {
            set { m_cityUnit = value; }
        }

        public FieldData fieldData;

        public Dictionary<int, >

        public override void Init(params object[] args)
        {
            if(m_inited)
                return;

            InitWithDiff(1);
            WorldManager.Instance.OperateField(this, true);
            m_inited = true;
        }

        public override void Reset(params object[] args)
        {
        }

        public override void Destroy()
        {
            base.Destroy();
            WorldManager.Instance.OperateField(this, false);
        }
        
        public void Explore(Hero hero)
        {
            if(!hero)
                return;

            //临时处理探索速度
            float explored = Time.deltaTime;
            //资源量减少
            fieldData.resRemain -= explored;
            //没有剩余资源了
            if (fieldData.resRemain <= 0f)
            {
                FieldResOver();
            }

            //显示资源剩余(血条)
            UpdateRemainBar(fieldData.resRemain / fieldData.resVolume);
        }

        //没有资源了
        private void FieldResOver()
        {
            //隐藏单位
            gameObject.SetActive(false);
            //世界管理器移除这个野外
            WorldManager.Instance.OperateField(this, false);
        }

        #region 难度颜色
        private Material m_colorMaterial;
        Material ColorMat
        {
            get
            {
                if (m_colorMaterial == null)
                {
                    MeshRenderer render = GetComponent<MeshRenderer>();
                    m_colorMaterial = render.material;
                }
                return m_colorMaterial;
            }
        }
        //难度对应的颜色
        [SerializeField] Color[] m_colors;
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

        public override string Desc()
        {
            return string.Empty;
        }

        //重启时间
        [SerializeField] private float m_resetTime = 10f;
        IEnumerator WaitForReset()
        {
            yield return new WaitForSeconds(m_resetTime);
            int newDiff = WorldManager.Instance.GetReasonableFieldDifficulty();
        }

        private void InitWithDiff(int diff)
        {
            //重新创建一个新的野外数据
            fieldData = new FieldData(
                Random.Range(50f, 100f),
                0.2f,
                0.5f,
                0.3f,
                diff);

            ResetDifficultyColor();

            m_objTime.SetActive(false);
        }

        //更新时间剩余（血条）
#region TimeBar
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
    }
}