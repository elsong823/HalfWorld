using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    [System.Serializable]
    public struct HeroData
    {
        public int level;           //等级
        public int exp;             //经验值
        public int fame;
        public int gold;
        public int hpMax;           //最大生命值
        public int hpCur;           //当前生命值
        public int strength;        //力量，影响探索
        public float baseStrGrowth; //力量增长
        public int moveSpeed;       //地图移动速度

        //力量成长随等级提升而下降
        public float strGrowth
        {
            get
            {
                if (level >= 30)
                    return baseStrGrowth - 1.05f;
                else if (level > 20)
                    return baseStrGrowth - 0.1f;
                else if (level > 10)
                    return baseStrGrowth - 0.05f;
                return baseStrGrowth;
            }
        }
    }
}