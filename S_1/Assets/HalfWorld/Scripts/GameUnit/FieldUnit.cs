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

        public override void Init(params object[] args)
        {
            base.Init(args);
            WorldManager.Instance.OperateField(this, true);
        }

        public override void Destroy()
        {
            base.Destroy();
            WorldManager.Instance.OperateField(this, false);
        }
    }
}