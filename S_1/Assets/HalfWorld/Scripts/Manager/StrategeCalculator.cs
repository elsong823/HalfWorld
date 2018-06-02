using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//策略计算器
namespace ELGame
{
    public class StrategeCalculator : MonoBehaviour
    {
        private struct Ladder
        {
            public int key;
            public float value;
            public Ladder(int key, float value)
            {
                this.key = key;
                this.value = value;
            }
        }
        

        private static StrategeCalculator instance;
        public static StrategeCalculator Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
            Init();
        }


        [SerializeField] TextAsset diffToLvAndStr;
        [SerializeField] TextAsset lvToExp;
        [SerializeField] TextAsset lvToFame;
        [SerializeField] TextAsset strToTime;

        [SerializeField] Dictionary<int, Ladder> fieldDiff; //野外难度对应的等级和所需力量
        [SerializeField] Ladder[] ladderExp;    //经验(等级)阶梯
        [SerializeField] Ladder[] ladderFame;   //声望(等级)阶梯
        [SerializeField] Ladder[] ladderTime;   //时间(力量)阶梯


        private bool inited = false;
        private void Init()
        {
            if (inited)
                return;
            inited = true;

            //读取csv文件
            //处理等级 -> 经验
            string str = string.Empty;
            if (lvToExp)
            {
                str = lvToExp.text;
                ProcessLadder(str, ref ladderExp);
            }
            if (lvToFame)
            {
                str = lvToFame.text;
                ProcessLadder(str, ref ladderFame);
            }
            if (strToTime)
            {
                str = strToTime.text;
                ProcessLadder(str, ref ladderTime);
            }
            if (diffToLvAndStr)
            {
                str = diffToLvAndStr.text.TrimEnd();
                fieldDiff = new Dictionary<int, Ladder>();
                string[] vals = str.TrimEnd().Split('\n');
                for (int i = 1; i < vals.Length; ++i)
                {
                    if (vals[i].Contains(","))
                    {
                        //分割.csv的每一行
                        string[] args = vals[i].Split(',');
                        if (args.Length == 3)
                        {
                            fieldDiff.Add(int.Parse(args[0]), new Ladder(int.Parse(args[1]), float.Parse(args[2])));
                        }
                    }
                }
            }
            Desc();
        }

        private void ProcessLadder(string str, ref Ladder[] ladder)
        {
            string[] vals = str.TrimEnd().Split('\n');
            ladder = new Ladder[vals.Length - 1];
            for (int i = 1; i < vals.Length; ++i)
            {
                if (vals[i].Contains(","))
                {
                    //分割.csv的每一行
                    string[] args = vals[i].Split(',');
                    if (args.Length == 2)
                    {
                        ladder[i - 1] = new Ladder(int.Parse(args[0]), float.Parse(args[1]));
                    }
                }
            }
        }

        /// <summary>
        /// 根据探索量计算经验获取
        /// </summary>
        /// <param name="heroData">英雄信息</param>
        /// <param name="fieldData">野外信息</param>
        /// <param name="explored">英雄对野外的总探索量</param>
        /// <returns>经验</returns>
        public float CalculateExp(HeroData heroData, FieldData fieldData, float explored)
        {
            if (explored <= 0f)
                return 0f;

            int needLevel = 1;
            if (!fieldDiff.ContainsKey(fieldData.difficulty))
            {
                EUtilityHelperL.LogWarning("错误的野外难度！");
            }
            else
            {
                needLevel = Mathf.FloorToInt(fieldDiff[fieldData.difficulty].key);
            }
            //等级差距
            float lvGrep = heroData.level - needLevel;
            float multiple = 1f;
            for (int i = 0; i < ladderExp.Length; ++i)
            {
                if (lvGrep <= ladderExp[i].key)
                {
                    multiple = ladderExp[i].value;
                    break;
                }
            }
            return explored * multiple * fieldData.expRate;
        }

        /// <summary>
        /// 根据探索量计算金币获取
        /// </summary>
        /// <param name="heroData">英雄信息</param>
        /// <param name="fieldData">野外信息</param>
        /// <param name="explored">英雄对野外的总探索量</param>
        /// <returns>金币</returns>
        public float CalculateGold(HeroData heroData, FieldData fieldData, float explored)
        {
            if (explored <= 0f)
                return 0f;

            return explored * fieldData.goldRate;
        }

        /// <summary>
        /// 根据探索量计算声望获取
        /// </summary>
        /// <param name="heroData">英雄信息</param>
        /// <param name="fieldData">野外信息</param>
        /// <param name="explored">英雄对野外的总探索量</param>
        /// <returns>声望</returns>
        public float CalculateFame(HeroData heroData, FieldData fieldData, float explored)
        {
            if (explored <= 0f)
                return 0f;

            int needLevel = 1;
            if (!fieldDiff.ContainsKey(fieldData.difficulty))
            {
                EUtilityHelperL.LogWarning("错误的野外难度！");
            }
            else
            {
                needLevel = Mathf.FloorToInt(fieldDiff[fieldData.difficulty].key);
            }
            //等级差距
            float lvGrep = heroData.level - needLevel;
            float multiple = 1f;
            for (int i = 0; i < ladderFame.Length; ++i)
            {
                if (lvGrep <= ladderFame[i].key)
                {
                    multiple = ladderFame[i].value;
                    break;
                }
            }
            return explored * multiple * fieldData.fameRate;
        }

        /// <summary>
        /// 根据探索量计算探索时间
        /// </summary>
        /// <param name="heroData">英雄信息</param>
        /// <param name="fieldData">野外信息</param>
        /// <param name="explored">探索量</param>
        /// <returns>探索时间</returns>
        public float CalculateExploreTime(HeroData heroData, FieldData fieldData, float volume)
        {
            return 0f;
        }

        /// <summary>
        /// //计算英雄探索某一野外的收益
        /// </summary>
        /// <param name="heroData">英雄数据</param>
        /// <param name="heroPos">英雄位置</param>
        /// <param name="fieldData">野外数据</param>
        /// <param name="fieldPos">野外位置</param>
        /// <returns></returns>
        public float Calculate(HeroData heroData, Vector3 heroPos, FieldData fieldData, Vector3 fieldPos)
        {
            //收益
            //经验 + 金钱 + 声望
            float exp = CalculateExp(heroData, fieldData, fieldData.resRemain);
            float gold = CalculateGold(heroData, fieldData, fieldData.resRemain);
            float fame = CalculateFame(heroData, fieldData, fieldData.resRemain);

            //时间
            float time = CalculateExploreTime(heroData, fieldData, fieldData.resRemain);
            //路途时间
            float travelTime = EUtilityHelperL.CalcDistanceIn2D(heroPos, fieldPos) / heroData.moveSpeed;
            return (exp + gold + fame) / (time + travelTime + Mathf.Epsilon);
        }

        public void Desc()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ladderExp");
            builder.AppendLine();
            for (int i = 0; i < ladderExp.Length; ++i)
            {
                builder.AppendFormat("{0}->{1}\n", ladderExp[i].key, ladderExp[i].value);
            }
            builder.Append("ladderFame");
            builder.AppendLine();
            for (int i = 0; i < ladderFame.Length; ++i)
            {
                builder.AppendFormat("{0}->{1}\n", ladderFame[i].key, ladderFame[i].value);
            }
            builder.Append("ladderTime");
            builder.AppendLine();
            for (int i = 0; i < ladderTime.Length; ++i)
            {
                builder.AppendFormat("{0}->{1}\n", ladderTime[i].key, ladderTime[i].value);
            }
            builder.Append("fieldDiff");
            builder.AppendLine();
            foreach (var item in fieldDiff)
            {
                builder.AppendFormat("{0}->{1}, {2}\n", item.Key, item.Value.key, item.Value.value);
            }
            Debug.Log(builder.ToString());
        }

    }
}