/*******************************************************************************************************************
 * 作者：杜凯
 * 时间：2016.10.17
 * *****************************************************************************************************************/
using UnityEngine;

namespace TOOL
{
	/// <summary>
	/// Unity专属单件工具
	/// </summary>
	public class U3DSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		/**
		Returns the instance of this singleton.
		*/
		public static T Instance
		{
			get
			{
				if(instance == null)
				{
					instance = (T) FindObjectOfType(typeof(T));
					if (instance == null)
					{
						Debug.LogWarning("An instance of " + typeof(T) +
						               " is needed in the scene, but there is none.");
					}
				}
				return instance;
			}
		}

		// 判断当前是否是单独的
		private bool m_IsSingle = false;
		public bool IsSingle{
			get{
				if(instance == null){
					instance = (T) FindObjectOfType(typeof(T));
					m_IsSingle = true;
				}
				return m_IsSingle;
			}
		}
	}


	/// <summary>
	/// 单件工具
	/// </summary>
	public class Singleton<T> where T : new()
	{
	    static readonly object padlock = new object();

	    private static T it;

	    public static T It
	    {
	        get
	        {
	            if (it == null)
	            {
	                lock (padlock)
	                {
	                    it = new T();
	                }
	            }
	            return it;
	        }
	    }
	}
}