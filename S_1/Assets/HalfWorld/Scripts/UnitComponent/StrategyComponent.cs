using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class StrategyComponent 
        : UnitComponent
    {
        //已知的野外
        [SerializeField] private List<FieldUnit> m_knownFields = new List<FieldUnit>();
        //已经探索过的野外
        [SerializeField] private List<FieldUnit> m_exploredFields = new List<FieldUnit>();


    }
}