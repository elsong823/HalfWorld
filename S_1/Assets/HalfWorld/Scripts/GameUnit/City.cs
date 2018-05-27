using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class City 
        : GameUnit
    {
        [Header("野外相关")]
        //野外的Prefab
        [SerializeField] private Field m_fieldModel = null;

        //围绕城市生成的野外都有哪些呢？
        [SerializeField] private List<Field> m_roundFields = new List<Field>();

        public override void Init(params object[] args)
        {
            if (m_inited)
                return;

            if (!m_fieldModel)
            {
                EUtilityHelperL.LogError("City unit error: none file model");
                return;
            }
            
            //向世界管理器注册
            WorldManager.Instance.OperateCity(this, true);

            m_inited = true;
        }

        /// <summary>
        /// 移除所有周围野外
        /// </summary>
        public void RemoveAllRoundFields()
        {
            foreach (var item in m_roundFields)
            {
                item.Destroy();
            }
            m_roundFields.Clear();
        }

        /// <summary>
        /// 创建野外
        /// </summary>
        /// <param name="count">创建数量</param>
        /// <param name="density">密度系数</param>
        /// <param name="minRadius">最小半径</param>
        /// <param name="maxRadius">最大半径</param>
        public void CreateFields(int count, float density, float minRadius, float maxRadius)
        {
            GameObject fieldNode = GameObject.FindGameObjectWithTag(EGameConstL.TAG_FIELD_NODE); 

            if (!fieldNode || !m_fieldModel)
            {
                EUtilityHelperL.LogError("None filed node or model!");
                return;
            }

            int created = 0;
            //生成预定数量的野外
            for (int i = 0; i < count; ++i)
            {
                Field clone = Instantiate<Field>(m_fieldModel);
                clone.transform.SetParent(fieldNode.transform);

                //设置名字
                clone.name = string.Format("Field_{0}_{1}", m_roundFields.Count, UnitName);
                clone.UnitName = clone.name;

                //设置所属城市
                clone.CityUnit = this;

                //必须要做的转换
                clone.transform.position = transform.TransformPoint(GetFieldPos(density, minRadius, maxRadius));
                clone.transform.localScale = Vector3.one;
                clone.transform.localRotation = Quaternion.identity;
                clone.gameObject.SetActive(true);
                m_roundFields.Add(clone);

                clone.Init();

                ++created;
            }
        }

        /// <summary>
        /// 获得随机的野外位置
        /// </summary>
        /// <param name="density">密度系数</param>
        /// <param name="minRadius">最小半径</param>
        /// <param name="maxRadius">最大半径</param>
        /// <returns></returns>
        private Vector3 GetFieldPos(float density, float minRadius, float maxRadius)
        {
            Vector3 randPos = Vector3.zero;
            //每次随机查找位置可以随机的机会，越大则越可能让世界摆放的相对分散，计算时间也越长
            int chance = EGameConstL.FIELD_RAND_CHANCE;

            //是否找到了合适的位置
            bool findOut = false;

            while (chance > 0 && !findOut)
            {
                findOut = true;
                --chance;
                //在m_cityRadiusRange.x ~ m_cityRadiusRange.y半径范围内随机一个位置
                //注意这里随机的位置是相对于当前city空间的
                var randV2 = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);

                randPos.x = randV2.x;
                randPos.z = randV2.y;

                //计算与其他野外的位置关系
                foreach (var item in m_roundFields)
                {
                    //距离有点近呢
                    //需要将空间位置进行一次转换
                    if (EUtilityHelperL.CalcDistanceIn2D(transform.InverseTransformPoint(item.transform.position), randPos) < density)
                    {
                        findOut = false;
                        break;
                    }
                }
            }

            //提醒一下
            if (!findOut)
                Debug.LogWarning("Can not find out optimum random position for field...");

            return randPos;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var item in m_roundFields)
            {
                Gizmos.DrawLine(transform.position, item.transform.position);
            }
        }

        public override void Destroy()
        {
            RemoveAllRoundFields();
            base.Destroy();

            //从世界管理器中移除
            WorldManager.Instance.OperateCity(this, false);
        }

        public override void Reset(params object[] args)
        {
        }

        public override string Desc()
        {
            return string.Empty;
        }
    }
}
