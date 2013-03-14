/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 8/16/2012
 * Time: 4:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Diagnostics;

namespace SAMP_AC_Extension
{
	/// <summary>
	/// Description of Modules.
	/// </summary>
	public class Modules
	{
		public Modules()
		{
		}
		public static int GetModuleBaseAddress(Process p, string modname) {
				
			foreach(ProcessModule pm in p.Modules) {
				if(pm.ModuleName.Equals(modname)) {
					return pm.BaseAddress.ToInt32();
				}
			}
			return -1;
		}
	}
}
