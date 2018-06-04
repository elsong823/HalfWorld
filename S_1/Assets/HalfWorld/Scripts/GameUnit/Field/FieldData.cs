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
		public float expRate;				//给予的经验
		public float goldRate;			//给予的金钱
		public float fameRate;			//给予的声望

		public FieldData(float resVolume, float expRate, float goldRate, float fameRate, int difficulty)
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