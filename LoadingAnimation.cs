/**
 * Created by SharpDevelop.
 * User: omggomb
 * Date: 05.06.2014
 * Time: 13:53
 */
using System;

namespace ExplorerTreeView
{
	/// <summary>
	/// Simply animates a "Loading..." string
	/// </summary>
	public class LoadingAnimation
	{
		/// <summary>
		/// Current count of dots behind "Loading";
		/// </summary>
		static int m_nDotCount;
		
		public LoadingAnimation()
		{
		}
		
		public static string GetString()
		{
			if (m_nDotCount > 3)
				m_nDotCount = 0;
			
			var loadString = "Loading";
			
			for (int i = 0; i < m_nDotCount; ++i)
			{
				loadString += ".";
			}
			
			++m_nDotCount;
			
			return loadString;
		}
		
		
	}
}
