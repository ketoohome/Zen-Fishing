using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOOL;
using Random = System.Random;

namespace TOOL
{
    /// <summary>
    /// 数值工具
    /// </summary>
    public static class NumericalTool
    {
        /// <summary>
        /// Randoms the bool.
        /// </summary>
        /// <returns><c>true</c>, if bool was randomed, <c>false</c> otherwise.</returns>
        /// <param name="value">Value.</param>
        /// <param name="max">Max.</param>
        public static bool RandomBool(uint value,uint max = 100) {
            if (max < value) Debug.LogError("max必须小于value");
            uint[] elements = { value, max-value};
            return RandomChoose(elements) == 0;
        }

        /// <summary>
        /// 随机选择
        /// </summary>
        /// <returns>随机数组的结果</returns>
        /// <param name="elements"> 每一种选择的几率</param>
        public static int RandomChoose(uint[] elements)
        {
            uint sum = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                sum += elements[i];
            }
            uint temp1 = (uint)UnityEngine.Random.Range(0, (int)sum);
            uint temp2 = 0;
            int result = -1;
            for (int i = 0; i < elements.Length; i++)
            {
                temp2 += elements[i];
                if (temp1 < temp2)
                {
                    result = i;
                    break;
                }
            }
            Debug.LogWarning("随机");
            return result;
        }

        /*
        public static void RandomIDTest() {
            uint[] test = { 1, 0, 0, 1 };
            int id0 = 0, id1 = 0, id2 = 0, id3 = 0;
            for (int i = 0; i < 100000; i++)
            {
                switch (NumericalTool.RandomChoose(test))
                {
                    case 0: id0++; break;
                    case 1: id1++; break;
                    case 2: id2++; break;
                    case 3: id3++; break;
                }
            }
            Debug.LogError("0:[" + id0 / 1000.0f + "%]");
            Debug.LogError("1:[" + id1 / 1000.0f + "%]");
            Debug.LogError("2:[" + id2 / 1000.0f + "%]");
            Debug.LogError("3:[" + id3 / 1000.0f + "%]");
        }
        */

        /// <summary>
        /// 正态分布发生器
        /// </summary>
        public class NormalDistribution
        {
            public double Mu { get; set; } // μ 期望值
            public double Sigma { get; set; } //σ 标准差

            private Random rand;

            public double NextDouble()
            {
                double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles 
                double u2 = rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1) 
                double randNormal = Mu + Sigma * randStdNormal; //random normal(mean,stdDev^2)

                return randNormal;
            }

            public NormalDistribution()
            {
                rand = new Random((int)DateTime.UtcNow.Ticks);
            }
        }

        /// <summary>
        /// 根据以知的结果推算的正太分布，根据概率给出一个随机结果
        /// </summary>
        /// <returns>The normal distribution.</returns>
        /// <param name="args">Arguments.已经存在的采样结果</param>
        public static double StandardNormalDistribution(double[] args) 
        {
            NormalDistribution a = new NormalDistribution();
            double mu = 0, sigma = 0;
            for(int i = 0; i < args.Length; i++) {
                mu+=args[i];
            }
            mu /= args.Length;
            for(int i = 0; i < args.Length; i++) {
                sigma += (mu - args[i]) * (mu - args[i]);
            }
            sigma = Math.Sqrt(sigma/args.Length);

            a.Mu = mu;
            a.Sigma = sigma;
            return a.NextDouble();
        }

        public static double StandardNormalDistributionQ(params double[] args)
        {
            NormalDistribution a = new NormalDistribution();
            double mu = 0, sigma = 0;
            for (int i = 0; i < args.Length; i++)
            {
                mu += args[i];
            }
            mu /= args.Length;
            for (int i = 0; i < args.Length; i++)
            {
                sigma += (mu - args[i]) * (mu - args[i]);
            }
            sigma = Math.Sqrt(sigma / args.Length);

            a.Mu = mu;
            a.Sigma = sigma;
            return a.NextDouble();
        }

        /*
        /// <summary>
        /// 正太分布测
        /// </summary>
        public static void StandardNormalDistributionTest() {
            int count = 100000; 
            int a=0, b=0, c=0, d=0, e=0;
            double[] values = { 40, 50, 60 };
            for(int i = 0;i< count; i++) 
            {
                double value = StandardNormalDistribution(values);
                if (value < 20 && value >= 0) a++;
                if (value >= 20 && value < 40) b++;
                if (value >= 40 && value < 60) c++; 
                if (value >= 60 && value < 80) d++; 
                if (value >= 80 && value < 100) e++;
            }
            Debug.Log("a["+a+"]\tb["+b+"]\tc["+c+"]\td["+d+"]\te["+e+"]");
        }
        */

        /// <summary>
        /// Randoms the choose.
        /// </summary>
        /// <returns>The choose.</returns>
        /// <param name="datas">Datas.</param>
        /// <param name="attribute">Attribute.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RandomChoose<T>(T datas, string attribute) where T : class, IDataNode, new()
        {
            uint sum = 0;
            foreach(T data in datas.ChildrenIDataNode) 
            {
                IDataNode temp = data.FindChildIDataNode(attribute);
                sum += data.FindChildIDataNode(attribute).GetValue<uint>();
            }
            uint temp1 = (uint)UnityEngine.Random.Range(0,sum), temp2 = 0;
            foreach (T data in datas.ChildrenIDataNode)
            {
                temp2 += data.FindChildIDataNode(attribute).GetValue<uint>();
                if (temp1 < temp2) {
                    return data;
                }
            }
            return null;
        }
    }
}