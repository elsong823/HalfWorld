using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class WorldManager
        : MonoBehaviour
    {
        //地图尺寸:x:地图宽 y:地图高
        [SerializeField] private Vector2 m_mapSize;

        [Space(7)]
        //城市总数
        [SerializeField, Range(1, 50)] private int m_citiesCount = 0;
        //每个城市周围的野外
        [SerializeField, Range(1, 5)] private int m_fieldsAroundCity = 0;

        [Space(7)]
        //城市密度，越大城市间越分散
        [SerializeField, Range(0.1f, 10f)] private float m_cityDensity = 0.1f;
        //野外密度，越大野外越分散
        [SerializeField, Range(0.1f, 10f)] private float m_fieldDensity = 0.1f;
        //城市半径，围绕城市生成的野外都在这个半径内随机摆放(min~max)
        [SerializeField] private Vector2 m_cityRadiusRange;


        [Space(7)]
        //生成的城市和野外后所在的父节点
        [SerializeField] private Transform m_cityNode;
        [SerializeField] private Transform m_fieldNode;

        [Space(7)]
        //clone城市和野外的模型
        [SerializeField] private GameObject m_cityModel;
        [SerializeField] private GameObject m_fieldModel;

        //clone出的城市和野外存放位置
        private List<Transform> m_cityClones = new List<Transform>();
        private List<Transform> m_fieldClones = new List<Transform>();
        //围绕某个城市的野外会被装到这个dic中
        private Dictionary<Transform, List<Transform>> m_fieldClonesAroundCity = new Dictionary<Transform, List<Transform>>();

        //获取一个随机的城市位置
        private Vector3 GetCityPos()
        {
            Vector3 randPos = new Vector3(0f, 1.01f, 0f);

            //每次随机查找位置可以随机的机会，越大则越可能让世界摆放的相对分散，计算时间也越长
            int chance = 10;
            //是否找到了合适的位置
            bool findOut = false;
            while (chance > 0 && !findOut)
            {
                findOut = true;
                --chance;
                //随机一个位置
                randPos.x = Random.Range(-m_mapSize.x * 0.5f, m_mapSize.x * 0.5f);
                randPos.z = Random.Range(-m_mapSize.y * 0.5f, m_mapSize.y * 0.5f);
                //计算与其他城市间的位置关系
                foreach (var item in m_cityClones)
                {
                    //距离有点近呢
                    if (EUtilityHelperL.CalcDistanceIn2D(item.transform.localPosition, randPos) < m_cityDensity)
                    {
                        findOut = false;
                        break;
                    }
                }
            }

            //提醒一下
            if (!findOut)
                Debug.LogWarning("没有找到合适的位置，凑合一下吧~");

            return randPos;
        }

        //获取一个随机的野外位置
        private Vector3 GetFieldPos(Transform city)
        {
            Vector3 randPos = Vector3.zero;
            //每次随机查找位置可以随机的机会，越大则越可能让世界摆放的相对分散，计算时间也越长
            int chance = 10;
            //是否找到了合适的位置
            bool findOut = false;
            List<Transform> temp = null;
            m_fieldClonesAroundCity.TryGetValue(city, out temp);
            while (chance > 0 && !findOut)
            {
                findOut = true;
                --chance;
                //在m_cityRadiusRange.x ~ m_cityRadiusRange.y半径范围内随机一个位置
                //注意这里随机的位置是相对于当前city空间的
                var randV2 = Random.insideUnitCircle.normalized * Random.Range(m_cityRadiusRange.x, m_cityRadiusRange.y);
                randPos.x = randV2.x;
                randPos.z = randV2.y;
                if (temp != null)
                {
                    //计算与其他野外的位置关系
                    foreach (var item in temp)
                    {
                        //距离有点近呢
                        //需要将空间位置进行一次转换
                        if (EUtilityHelperL.CalcDistanceIn2D(city.InverseTransformPoint(item.transform.position), randPos) < m_fieldDensity)
                        {
                            findOut = false;
                            break;
                        }
                    }
                }
            }

            //提醒一下
            if (!findOut)
                Debug.LogWarning("没有找到合适的野外位置，凑合一下吧~");

            return randPos;
        }

        //根据城市总数，随机城市
        private void CreateCities()
        {
            if (!m_cityNode || !m_cityModel)
            {
                Debug.LogError("弄啥咧！人家还没准备好就开始么？");
                return;
            }

            //生成预定数量的城市
            for (int i = 0; i < m_citiesCount; ++i)
            {
                Transform clone = Instantiate(m_cityModel).transform;
                clone.name = string.Format("City_{0}", i);
                clone.SetParent(m_cityNode);
                clone.localPosition = GetCityPos();
                clone.localScale = Vector3.one;
                clone.localRotation = Quaternion.identity;
                clone.gameObject.SetActive(true);
                m_cityClones.Add(clone);
            }
        }

        //移除所有城市
        private void RemoveCities()
        {
            foreach (var item in m_cityClones)
            {
                GameObject.Destroy(item.gameObject);
            }
            m_cityClones.Clear();
        }

        //根据每个城市周围的野外数量，随机生成野外
        private void CreateFields()
        {
            if (!m_fieldNode || !m_fieldModel)
            {
                Debug.LogError("弄啥咧！人家还没准备好就开始么？");
                return;
            }

            int created = 0;
            //围绕城市建立野外
            foreach (var item in m_cityClones)
            {
                //生成预定数量的野外
                for (int i = 0; i < m_fieldsAroundCity; ++i)
                {
                    Transform clone = Instantiate(m_fieldModel).transform;
                    clone.name = string.Format("Field_{0}", i);
                    clone.SetParent(m_fieldNode);
                    //必须要做的转换
                    clone.position = item.TransformPoint(GetFieldPos(item));
                    clone.localScale = Vector3.one;
                    clone.localRotation = Quaternion.identity;
                    clone.gameObject.SetActive(true);
                    m_fieldClones.Add(clone);

                    //用于加入到围绕城市的野外列表中，用作野外的密度检测
                    List<Transform> aroundCity = null;
                    if (!m_fieldClonesAroundCity.TryGetValue(item, out aroundCity))
                    {
                        aroundCity = new List<Transform>();
                        m_fieldClonesAroundCity.Add(item, aroundCity);
                    }
                    aroundCity.Add(clone);
                    ++created;
                }
            }
        }

        //移除所有野外
        private void RemoveFields()
        {
            foreach (var item in m_fieldClones)
            {
                GameObject.Destroy(item.gameObject);
            }
            m_fieldClones.Clear();
            m_fieldClonesAroundCity.Clear();
        }

        //重置并根据参数生成新的世界
        public void ResetWorld()
        {
            //旧的不去
            RemoveFields();
            RemoveCities();
            //新的不来
            CreateCities();
            CreateFields();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Reset all"))
            {
                ResetWorld();
            }
            else if (GUI.Button(new Rect(0f, 100f, 100f, 100f), "Reset city"))
            {
                RemoveFields();
                RemoveCities();
                CreateCities();
            }
            else if (GUI.Button(new Rect(0f, 200f, 100f, 100f), "Reset field"))
            {
                RemoveFields();
                CreateFields();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var item in m_fieldClonesAroundCity)
            {
                //将据有关系的城市和野外用线连接起来，方便显示
                foreach (var field in item.Value)
                {
                    Gizmos.DrawLine(item.Key.position, field.position);
                }
            }
        }
    }
}