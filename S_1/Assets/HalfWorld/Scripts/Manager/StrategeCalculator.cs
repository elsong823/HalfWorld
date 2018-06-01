using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//策略计算器
namespace ELGame
{
    public class StrategeCalculator : MonoBehaviour
    {
        private struct Ladder
        {
            public float key;
            public float value;
            public Ladder(float key, float value)
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

        [SerializeField] TextAsset lvToExp;
        [SerializeField] TextAsset lvToFame;
        [SerializeField] TextAsset strToTime;

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
            int a = 1;
        }

        private void ProcessLadder(string str, ref Ladder[] ladder)
        {
            string[] vals = str.Split('\n');
            ladder = new Ladder[vals.Length - 1];
            for (int i = 1; i < vals.Length; ++i)
            {
                if (vals[i].Length > 3)
                {
                    string[] args = vals[i].Split(',');
                    if (args.Length == 2)
                    {
                        ladder[i - 1] = new Ladder(float.Parse(args[0]), float.Parse(args[1]));
                    }
                }
            }
        }
    }
}