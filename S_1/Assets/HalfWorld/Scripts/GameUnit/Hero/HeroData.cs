using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    [System.Serializable]
    public struct HeroData
    {
        public int level;       //等级
        public int exp;         //经验值
        public int hpMax;       //最大生命值
        public int hpCur;       //当前生命值
        public int strength;    //力量，影响探索
        public int moveSpeed;   //地图移动速度
    }
}