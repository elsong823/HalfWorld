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
    }

    public class GameUnit
        : MonoBehaviour, IELBase
    {
        //游戏单位类型
        [SerializeField] private GameUnitType m_gameUnitType = GameUnitType.NONE;

        //自身携带的组件
        private List<UnitComponent> m_unitComponets = new List<UnitComponent>();
        
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

        public virtual void Init(params SObject[] args)
        {
            if (m_gameUnitType == GameUnitType.NONE)
            {
                EUtilityHelperL.LogError(string.Format("Game unity type error:" + m_unitName));
                return;
            }

            //获取所有组件
            UnitComponent[] components = gameObject.GetComponentsInChildren<UnitComponent>();
            foreach (var item in components)
            {
                m_unitComponets.Add(item);
            }

            //初始化所有组件
            foreach (var item in m_unitComponets)
            {
                item.Init(args);
            }
        }

        public virtual void Reset(params SObject[] args)
        {
            foreach (var item in m_unitComponets)
            {
                item.Reset(args);
            }
        }

        public virtual string Desc()
        {
            return string.Empty;
        }

        public virtual void Destroy()
        {
            foreach (var item in m_unitComponets)
            {
                item.Destroy();
            }

            GameObject.Destroy(gameObject);
        }
    }
}