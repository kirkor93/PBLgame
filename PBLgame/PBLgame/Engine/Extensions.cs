using System;
using System.Collections.Generic;

namespace PBLgame.Engine
{
    public static class Extensions
    {
        /// <summary>
        /// Inserts item on specified position or at the end when index == -1
        /// </summary>
        /// <typeparam name="T">type of item</typeparam>
        /// <param name="list">list collection</param>
        /// <param name="index">index to insert to</param>
        /// <param name="item">item to insert</param>
        public static void AddInsert<T>(this List<T> list, int index, T item)
        {
            if (index == -1) list.Add(item);
            else list.Insert(index, item);
        }


        public static float CalculateAngle(float x, float y)
        {
            return (float) (Math.Atan2(y, x));
        }
    }
}