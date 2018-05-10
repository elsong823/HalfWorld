using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    //策略组件
    public class StrategyComponent 
        : UnitComponent
    {
        //已知的城市
        private HashSet<CityUnit> m_knowCities = new HashSet<CityUnit>();
        //未探索过的野外
        private HashSet<FieldUnit> m_unexploredFields = new HashSet<FieldUnit>();
        //已经探索过的野外
        private HashSet<FieldUnit> m_exploredFields = new HashSet<FieldUnit>();

        /// <summary>
        /// 扫描周围城市
        /// </summary>
        /// <param name="radius">扫描半径</param>
        public void ScanRound(float radius)
        {
            //扫描一下周围的城市
            var citys = WorldManager.Instance.AllCities;
            while (citys.MoveNext())
            {
                CityUnit city = citys.Current;
                if (m_knowCities.Contains(city))
                {
                    //已经知道这个城市
                    continue;
                }
                else
                {
                    //判断距离
                    if (EUtilityHelperL.CalcDistanceIn2D(city.transform.position, transform.position) <= radius)
                    {
                        m_knowCities.Add(city);
                    }
                }
            }

            //扫描一下周围的野外
            var fields = WorldManager.Instance.AllFields;
            while (fields.MoveNext())
            {
                FieldUnit field = fields.Current;
                if (m_exploredFields.Contains(field))
                {
                    //已经探索过了
                    continue;
                }
                else if (m_unexploredFields.Contains(field))
                {
                    //如果这个野外已经被探索过了，则移除
                    if (field.Explored)
                        m_unexploredFields.Remove(field);
                }
                else
                {
                    if (field.Explored)
                    {
                        //这个野外已经被探索过了
                        //TODO:等待后期自动刷新
                        continue;
                    }
                    else
                    {
                        //判断距离
                        if (EUtilityHelperL.CalcDistanceIn2D(field.transform.position, transform.position) <= radius)
                        {
                            m_unexploredFields.Add(field);
                        }
                    }
                }
            }
            Debug.Log(m_knowCities.Count);
            Debug.Log(m_unexploredFields.Count);
            Debug.Log(m_exploredFields.Count);
        }
    }
}