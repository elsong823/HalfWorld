using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public class EUtilityHelperL
    {
        //计算3d空间下的两点平面距离
        public static float CalcDistanceIn2D(Vector3 p1, Vector3 p2)
        {
            p1.y = 0f;
            p2.y = 0f;
            return Vector3.Distance(p1, p2);
        }
    }
}