using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SObject = System.Object;
using UObject = UnityEngine.Object;

namespace ELGame
{
    //单位组件必须依附在游戏单位上才能工作
    [RequireComponent(typeof(GameUnit))]
    public class UnitComponent 
        : MonoBehaviour, IELBase
    {
        //所属游戏单位
        private GameUnit m_host = null;
        
        //获取绑定的游戏对象
        public T GetGameUnit<T>()
            where T:GameUnit
        {
            if (m_host == null)
            {
                m_host = GetComponent<T>();
            }
            return m_host as T;
        }

        public void Init(params SObject[] args)
        {

        }
        
        public void Reset(params SObject[] args)
        {
        }

        public void Destroy()
        {
        }

        public string Desc()
        {
            return string.Empty;
        }
    }
}