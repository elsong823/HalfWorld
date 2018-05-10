﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class FieldUnit
        : GameUnit
    {
        //所属城市
        private CityUnit m_cityUnit = null;

        //是否已经被探索过了
        [SerializeField] private bool m_explored = false;

        public CityUnit CityUnit
        {
            set { m_cityUnit = value; }
        }

        public bool Explored
        {
            get { return m_explored; }
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