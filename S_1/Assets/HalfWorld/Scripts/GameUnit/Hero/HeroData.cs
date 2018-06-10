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
        public int fame;            //声望
        public int gold;            //金币
        public int hpMax;           //最大生命值
        public int hpCur;           //当前生命值
        public int strength;        //力量，影响探索
        public int baseStrGrowth;   //力量增长
        public int moveSpeed;       //地图移动速度

        //在对野外进行决策计算时，不同资源类型的向往值会影响计算结果
        public int fameFavour;      //声望向
        public int goldFavour;      //金币向

        //力量成长随等级提升而下降
        public float strGrowth
        {
            get
            {
                float growth = baseStrGrowth;
                if (level >= 30)
                    growth -= 150f;
                else if (level > 20)
                    growth -= 100f;
                else if (level > 10)
                    growth -= 50f;

                return growth * 0.001f;
            }
        }
    }
}