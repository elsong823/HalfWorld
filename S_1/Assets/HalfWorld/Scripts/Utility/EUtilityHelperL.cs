using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class EUtilityHelperL
    {
        private static string ConvertColor(string str, int color)
        {
            if (color == 100)
                return string.Format("<color=#ff0000>{0}</color>", str);
            else if (color == 110)
                return string.Format("<color=#ffff00>{0}</color>", str);
            else if (color == 111)
                return string.Format("<color=#ffffff>{0}</color>", str);
            else if (color == 010)
                return string.Format("<color=#00ff00>{0}</color>", str);
            else if (color == 011)
                return string.Format("<color=#00ffff>{0}</color>", str);
            else if (color == 001)
                return string.Format("<color=#0000ff>{0}</color>", str);
            else if (color == 000)
                return string.Format("<color=#000000>{0}</color>", str);

            return str;
        }

        public static void Log(System.Object obj, int color = 111)
        {
            Debug.Log(ConvertColor(obj.ToString(), color));
        }

        public static void LogWarning(System.Object obj, int color = 110)
        {
            Debug.LogWarning(ConvertColor(obj.ToString(), color));
        }

        public static void LogError(System.Object obj, int color = 100)
        {
            Debug.LogError(ConvertColor(obj.ToString(), color));
        }

        //计算3d空间下的两点平面距离
        public static float CalcDistanceIn2D(Vector3 p1, Vector3 p2)
        {
            p1.y = 0f;
            p2.y = 0f;
            return Vector3.Distance(p1, p2);
        }
    }
}