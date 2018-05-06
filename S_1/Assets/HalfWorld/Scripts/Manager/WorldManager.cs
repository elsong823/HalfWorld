using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class WorldManager
        : MonoBehaviour
    {
        #region

        private static WorldManager worldManager = null;

        //游戏配置
        [SerializeField] private EGameConfigL m_gameConfig;

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
        //clone城市和野外的模型
        [SerializeField] private CityUnit m_cityModel;

        //clone出的城市
        private List<CityUnit> m_cityClones = new List<CityUnit>();
        #endregion

        public static WorldManager Instance
        {
            get
            {
                if (!worldManager)
                {
                    var temp = GameObject.FindGameObjectWithTag(EGameConstL.TAG_WORLD_MANAGER);
                    if (!temp)
                    {
                        EUtilityHelperL.LogError("Can not find out world manager instance!");
                        return null;
                    }
                    worldManager = temp.GetComponent<WorldManager>();
                }
                return worldManager;
            }
        }

        public static EGameConfigL GameConfig
        {
            get
            {
                return Instance.m_gameConfig;
            }
        }

        //重置并根据参数生成新的世界
        public void ResetWorld()
        {
            //旧的不去
            RemoveCities();
            //新的不来
            CreateCities();
            CreateFields();
        }

        //根据城市总数，随机城市
        private void CreateCities()
        {
            GameObject cityNode = GameObject.FindGameObjectWithTag(EGameConstL.TAG_CITY_NODE);

            if (!cityNode || !m_cityModel)
            {
                EUtilityHelperL.LogError("None city node or model!");
                return;
            }

            //生成预定数量的城市
            for (int i = 0; i < m_citiesCount; ++i)
            {
                CityUnit clone = Instantiate<CityUnit>(m_cityModel);
                clone.name = string.Format("City_{0}", i);
                clone.UnitName = clone.name;

                clone.transform.SetParent(cityNode.transform);
                clone.transform.localPosition = GetCityPos();
                clone.transform.localScale = Vector3.one;
                clone.transform.localRotation = Quaternion.identity;
                clone.transform.gameObject.SetActive(true);

                //初始化城市
                clone.Init();

                m_cityClones.Add(clone);
            }
        }

        //移除所有城市
        private void RemoveCities()
        {
            foreach (var item in m_cityClones)
            {
                item.Destroy();
            }
            m_cityClones.Clear();
        }

        //根据每个城市周围的野外数量，随机生成野外
        private void CreateFields()
        {
            foreach (var item in m_cityClones)
            {
                item.CreateFields(m_fieldsAroundCity, m_fieldDensity, m_cityRadiusRange.x, m_cityRadiusRange.y);
            }
        }

        //移除所有野外
        private void RemoveFields()
        {
            foreach (var item in m_cityClones)
            {
                item.RemoveAllRoundFields();
            }
        }

        //获取一个随机的城市位置
        private Vector3 GetCityPos()
        {
            Vector3 randPos = new Vector3(0f, 1.01f, 0f);

            //每次随机查找位置可以随机的机会，越大则越可能让世界摆放的相对分散，计算时间也越长
            int chance = EGameConstL.CITY_RAND_CHANCE;
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
                Debug.LogWarning("Can not find out optimum random position for city...");

            return randPos;
        }
        
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Reset all"))
            {
                ResetWorld();
            }
            else if (GUI.Button(new Rect(0f, 100f, 100f, 100f), "Reset city"))
            {
                RemoveCities();
                CreateCities();
            }
            else if (GUI.Button(new Rect(0f, 200f, 100f, 100f), "Reset field"))
            {
                RemoveFields();
                CreateFields();
            }
        }
    }
}