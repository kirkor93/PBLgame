using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PBLgame.Engine.Components;

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


        private static float CalculateAngle(float x, float y)
        {
            return (float) (Math.Atan2(y, x));
        }

        /// <summary>
        /// Calculates angle of vector on 2D XY plane.
        /// </summary>
        /// <param name="vector">2D vector</param>
        /// <returns>Angle in degrees [0 .. 360) </returns>
        public static float CalculateDegrees(Vector2 vector)
        {
            float angle = MathHelper.ToDegrees(CalculateAngle(vector.X, vector.Y));
            if (angle < 0f) angle += 360f;
            return angle;
        }

        public static string GetString(this Renderer.Technique technique)
        {
            switch (technique)
            {
                case Renderer.Technique.ShadowsPoint: return "Shadows";
                case Renderer.Technique.ShadowsDirectional: return "ShadowsDir";
                case Renderer.Technique.Reflection: return "Reflection";
                case Renderer.Technique.Glow: return "Glow";
                default: return "PhongBlinn";
            }
        }

        public static string ToShortString(this Vector3 v, String separator = ", ")
        {
            return String.Format("{0}{3}{1}{3}{2}", v.X, v.Y, v.Z, separator);
        }

        public static Vector3 GetCenter(this BoundingBox bb)
        {
            return (bb.Min + bb.Max) / 2;
        }

        public static Vector3 GetSize(this BoundingBox bb)
        {
            return bb.Max - bb.Min;
        }

        public static BoundingBox Translate(this BoundingBox bb, Vector3 t)
        {
            BoundingBox newbb = bb;
            newbb.Min += t;
            newbb.Max += t;
            return newbb;
        }

        public static float GetMin(this Vector3 v)
        {
            return Math.Min(v.X,  Math.Min(v.Y, v.Z) );
        }

        public static float GetMax(this Vector3 v)
        {
            return Math.Max(v.X,  Math.Max(v.Y, v.Z) );
        }
        
        /// <summary>
        /// Normalizes angle into range (-180 .. 180] degrees.
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns></returns>
        public static float NormalizeAngle180(float angle)
        {
            while (angle <= -180f) angle += 360f;
            while (angle >   180f) angle -= 360f;
            return angle;
        }
    }
}