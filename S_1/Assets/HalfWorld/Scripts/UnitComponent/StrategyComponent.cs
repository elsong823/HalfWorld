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
        [SerializeField] private List<CityUnit> m_knowCities = new List<CityUnit>();
        //未探索过的野外
        [SerializeField] private List<FieldUnit> m_unexploredFields = new List<FieldUnit>();
        //已经探索过的野外
        [SerializeField] private List<FieldUnit> m_exploredFields = new List<FieldUnit>();

        /// <summary>
        /// 扫描周围城市
        /// </summary>
        /// <param name="radius">扫描半径</param>
        public void ScanRound(float radius)
        {

        }
    }
}