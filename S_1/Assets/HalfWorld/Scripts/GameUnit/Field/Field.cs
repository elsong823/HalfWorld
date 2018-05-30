using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class Field
        : GameUnit
    {
        //所属城市
        private City m_cityUnit = null;

        public City CityUnit
        {
            set { m_cityUnit = value; }
        }

        public FieldData fieldData;

        public override void Init(params object[] args)
        {
            if(m_inited)
                return;
            
            Reset();
            WorldManager.Instance.OperateField(this, true);
            m_inited = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            WorldManager.Instance.OperateField(this, false);
        }

        public override void Reset(params object[] args)
        {
            //随机一些数据
            float res = 2f;
            int difficulty = Random.Range(1, 5);
            int gold = Mathf.CeilToInt(res * Random.Range(10, 12f));
            int fame = Mathf.CeilToInt(difficulty * Random.Range(10, 12f));
            int exp = Mathf.CeilToInt(res * Random.Range(1f, 1.2f) * difficulty);

            //重新创建一个新的野外数据
            fieldData = new FieldData(
                res,
                exp,
                gold,
                fame,
                difficulty
            );

            m_objTime.SetActive(false);
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

        public override string Desc()
        {
            return string.Empty;
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