using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class FieldUnit
        : GameUnit
    {
        //所属城市
        private CityUnit m_cityUnit = null;

        public CityUnit CityUnit
        {
            set { m_cityUnit = value; }
        }
    }
}