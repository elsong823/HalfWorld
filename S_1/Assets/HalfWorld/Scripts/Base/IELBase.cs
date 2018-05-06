using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELGame
{
    public interface IELBase
    {
        //初始化
        void Init(params System.Object[] args);

        //重置
        void Reset(params System.Object[] args);

        //显示详情
        string Desc();
    }
}