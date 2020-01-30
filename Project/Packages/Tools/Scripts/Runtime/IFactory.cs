/*******************************************************************************************************************
 * 作者：杜凯
 * 时间：2016.10.17
 * *****************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace TOOL 
{
    /// <summary>
    /// 工厂泛型
    /// </summary>
    /// <typeparam name="T">产品</typeparam>
    public class IFactory<T>
    {
		private IFactory() { }

        static readonly Dictionary<int, Type> _dict = new Dictionary<int, Type>();

        /// <summary>
        /// 创建产品
        /// </summary>
        /// <param name="id">产品类型</param>
        /// <param name="args"></param>
        /// <returns>产品实例</returns>
        public static T Create(int id, params object[] args) { 
            Type type = null;
            if (_dict.TryGetValue(id, out type))
                return (T)Activator.CreateInstance(type, args);

            throw new ArgumentException("No type registered for this id");
        }

        /// <summary>
        /// 注册产品
        /// </summary>
        /// <typeparam name="Tderived"></typeparam>
        /// <param name="id"></param>
        public static void Register<Tderived>(int id) where Tderived : T
        {
            var type = typeof(Tderived);
            // 不允许是抽象类或借口
            if (type.IsInterface || type.IsAbstract)
                throw new ArgumentException("...");

            _dict.Add(id,type);
        }
    }
}

