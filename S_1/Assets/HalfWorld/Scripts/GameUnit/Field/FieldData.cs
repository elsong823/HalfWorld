using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
	[System.Serializable]
    public struct FieldData
    {
		public float resVolume;		    //总共资源量
		public float resRemain; 	    //剩余资源量
		public int difficulty;		    //难度值
        //产出的比例：
        //回报 = 资源量 * 比例
        //比例影响探索时某一收益的获得量，比例越高，获取越高
		public int expRate;             //给予经验
        public int goldRate;			//给予金钱
		public int fameRate;			//给予声望

		public FieldData(float resVolume, int expRate, int goldRate, int fameRate, int difficulty)
		{
			this.resVolume = resVolume;
			this.resRemain = this.resVolume;
            this.expRate = expRate;
            this.goldRate = goldRate;
            this.fameRate = fameRate;
            this.difficulty = difficulty;
		}
    }
}