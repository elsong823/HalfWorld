using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
	[System.Serializable]
    public struct FieldData
    {
		public float resVolume;		//总共资源量
		public float resRemain; 	//剩余资源量
		public int difficulty;		//难度值
		public int exp;				//给予的经验
		public int gold;			//给予的金钱
		public int fame;			//给予的声望

		public FieldData(float timeCost, int exp, int gold, int fame, int difficulty)
		{
			this.resVolume = timeCost;
			this.resRemain = this.resVolume;
			this.exp = exp;
            this.gold = gold;
            this.fame = fame;
            this.difficulty = difficulty;
		}
    }
}