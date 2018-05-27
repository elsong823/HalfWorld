using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SObject = System.Object;
using UObject = UnityEngine.Object;

namespace ELGame
{
    public enum GameUnitType
    {
        NONE,
        CITY,
        FIELD,
        HERO,
    }

    public abstract class GameUnit
        : MonoBehaviour, IELBase
    {
        //游戏单位类型
        [SerializeField] protected GameUnitType m_gameUnitType = GameUnitType.NONE;
        
        protected bool m_inited = false;
        
        //游戏对象的名字
        [SerializeField] protected string m_unitName = "GameUnit";

        public GameUnitType GameUnitType
        {
            get { return m_gameUnitType; }
        }

        public string UnitName
        {
            get { return m_unitName; }
            set { m_unitName = value; }
        }

        public abstract void Init(params SObject[] args);

        public abstract void Reset(params SObject[] args);

        public abstract string Desc();

        public virtual void Destroy()
        {
            GameObject.Destroy(gameObject);
        }

        public override bool Equals(object other)
        {
            GameUnit unit = other as GameUnit;
            if (!unit)
                return false;

            return unit.GetInstanceID() == this.GetInstanceID();
        }
    }
}