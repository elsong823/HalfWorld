using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ELGame
{

public class DataRecorder : MonoBehaviour
    {
        private static DataRecorder instance;
        public static DataRecorder Instance
        {
            get
            {
                return instance;
            }
        }

        private StringBuilder heroDataStr;

        private void InitHeroData()
        {
            heroDataStr = new StringBuilder();
            heroDataStr.Append("Name,BaseStrGrowth,Str,FameFavour,GoldFavour,Level,FieldName,Diff,FameRate,GoldRate,ExploreTime,Exp,Fame,Gold\n");

        }

		private void Awake()
		{
            instance = this;
            InitHeroData();
		}


        public void RecordHero(string name, HeroData hero, HeroRecord record)
        {
            //name, baseStrGrowth, str, fameFavor, goldFavlor, lv, fieldName, diff, expRate, goldRate, exploreTime, exp, fame, gold,
            heroDataStr.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10:0.0},{11},{12},{13}\n",
                                    name, hero.baseStrGrowth, hero.strength, hero.fameFavour, hero.goldFavour, hero.level, 
                                    record.fieldName, record.diff, record.fameRate, record.goldRate, record.exploreTime, record.exp, record.fame, record.gold);
        }

        private void SaveHeroRecord()
        {
            //保存英雄探索记录到磁盘
            File.WriteAllText(string.Format("{0}/{1}/{2}/{3}.csv", Application.dataPath, "HalfWorld", "DataRecord", "record"), heroDataStr.ToString());
        }

		private void OnDestroy()
		{
            //结束后保存英雄数据
            SaveHeroRecord();
        }
	}
}