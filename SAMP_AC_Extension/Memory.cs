/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 7/8/2012
 * Time: 5:00 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SAMP_AC_Extension
{
	/// <summary>
	/// Description of Memory.
	/// </summary>
	public class Memory : SAMP_AC_Extension
	{
		public Memory()
		{
		}
		
		
		public static void VerifySomeMemoryStuff() {
        	try {
	        	ProcessMemory Mem = new ProcessMemory("gta_sa");
	        	
	        	if(!g_bGTASAStarted) return;
	        		
	        	while(!Mem.CheckProcess() && g_bGTASAStarted) {
	        		
	        		ProcessMemory Mem2 = new ProcessMemory("samp");
	        		
	        		if(!Mem.CheckProcess()) {
	        			// SA-MP browser is closed.
	        			whenGameStopped(4039);
	        		}
	        		Thread.Sleep(5000);
	        		// Wait for GTA SA to be launched. (only sa-mp server browser is open at this time.)
	        	} 
	        	
	        	
	        	// Wait a second just in case the game isn't initialized yet.
	        	Thread.Sleep(1000);
	        	
	        	// YAY!
	        	// now we need to read some memory addresses!       	
	        	if(Mem.StartProcess()) {
	         		
		        	// public static void checkMemoryAddr(ProcessMemory Mem, int addr, string tomatch)
		        	checkMemoryAddr(Mem, 0x085C718, @"ANIM\PED.IFP");
		        	checkMemoryAddr(Mem, 0x086AA28, @"DATA\WEAPON.DAT");
		        	checkMemoryAddr(Mem, 0x0869668, @"DATA\CARMODS.DAT");
		        	checkMemoryAddr(Mem, 0x086A7F4, @"DATA\ANIMGRP.DAT");
		        	checkMemoryAddr(Mem, 0x086AAB4, @"DATA\melee.dat");
		        	checkMemoryAddr(Mem, 0x08671F8, @"DATA\CLOTHES.DAT");
		        	checkMemoryAddr(Mem, 0x0869B20, @"DATA\OBJECT.DAT");
		        	checkMemoryAddr(Mem, 0x0863A90, @"DATA\DEFAULT.DAT");
		        	checkMemoryAddr(Mem, 0x0864318, @"data\surface.dat");
		        	checkMemoryAddr(Mem, 0x0863B10, @"DATA\GTA.DAT");
		        	checkMemoryAddr(Mem, 0x0872148, @"DATA\water.dat");
		        	checkMemoryAddr(Mem, 0x0872158, @"DATA\water1.dat");
		        	checkMemoryAddr(Mem, 0x086AF80, @"data\furnitur.dat");
		        	checkMemoryAddr(Mem, 0x0867014, @"data\procobj.dat");
		        	checkMemoryAddr(Mem, 0x086A964, @"HANDLING.CFG");     	
		        	checkMemoryAddr(Mem, 0x086A778, @"TIMECYC.DAT");
		        	checkMemoryAddr(Mem, 0x086A698, @"DATA\PEDSTATS.DAT");        	
		        	checkMemoryAddr(Mem, 0x086A648, @"MODELS\FONTS.TXD");
		        	checkMemoryAddr(Mem, 0x086A51C, @"models\coll\peds.col");
		        	checkMemoryAddr(Mem, 0x0863F80, @"DATA\STATDISP.DAT");
		        	checkMemoryAddr(Mem, 0x0863FA0, @"DATA\AR_STATS.DAT");
		        	checkMemoryAddr(Mem, 0x0864DB4, @"data\surfinfo.dat");
		        	
		        	// added v1.4
		        	
		        	checkMemoryAddr(Mem, 0x08E4318, @"SAMP\CUSTOM.IMG");
		        	checkMemoryAddr(Mem, 0x08E4398, @"MODELS\GTA3.IMG");
		        	checkMemoryAddr(Mem, 0x08E43D8, @"MODELS\GTA_INT.IMG");
		        	checkMemoryAddr(Mem, 0x08E44D8, @"MODELS\PLAYER.IMG");
		        	checkMemoryAddr(Mem, 0x08E40D8, @"AUDIO\SFX\FEET");
		        	checkMemoryAddr(Mem, 0x08E4118, @"AUDIO\SFX\GENRL");
		        	checkMemoryAddr(Mem, 0x08E4158, @"AUDIO\SFX\PAIN_A");
		        	checkMemoryAddr(Mem, 0x08E4198, @"AUDIO\SFX\SCRIPT");
		        	checkMemoryAddr(Mem, 0x08E41D8, @"AUDIO\SFX\SPC_EA");
		        	checkMemoryAddr(Mem, 0x08E4218, @"AUDIO\SFX\SPC_FA");
		        	checkMemoryAddr(Mem, 0x08E4258, @"AUDIO\SFX\SPC_GA");
		        	checkMemoryAddr(Mem, 0x08E4298, @"AUDIO\SFX\SPC_NA");
		        	checkMemoryAddr(Mem, 0x08E42D8, @"AUDIO\SFX\SPC_PA");
		        	checkMemoryAddr(Mem, 0x08E4358, @"SAMP\SAMP.IMG");
		        	checkMemoryAddr(Mem, 0x08E4418, @"SAMP\SAMPCOL.IMG");
		        	checkMemoryAddr(Mem, 0x08E4458, @"DATA\SCRIPT\SCRIPT.IMG");
		        	checkMemoryAddr(Mem, 0x08E4498, @"MODELS\CUTSCENE.IMG");
		        	checkMemoryAddr(Mem, 0x08E48D8, @"SAMP\CUSTOM.IMG");
		        	checkMemoryAddr(Mem, 0x08E4908, @"SAMP\SAMP.IMG");
		        	checkMemoryAddr(Mem, 0x08E4938, @"MODELS\GTA3.IMG");
		        	checkMemoryAddr(Mem, 0x08E4968, @"MODELS\GTA_INT.IMG");
		        	checkMemoryAddr(Mem, 0x08E4998, @"SAMP\SAMPCOL.IMG");
		        	checkMemoryAddr(Mem, 0x08E49C8, @"DATA\SCRIPT\SCRIPT.IMG");
		        	checkMemoryAddr(Mem, 0x08E49F8, @"MODELS\CUTSCENE.IMG");
		        	checkMemoryAddr(Mem, 0x08E4A28, @"MODELS\PLAYER.IMG");
	        	}

        	} catch(Exception e) {
        		Log.WriteLog(e.ToString());
        	}
        }
		private static void checkMemoryAddr(ProcessMemory Mem, int addr, string tomatch) {
        	
			int size = tomatch.Length;
			
        	if(!Mem.CheckProcess()) { 
        		Log.WriteLog("ERROR: Failed to read memory. addr=0x0" + String.Format("{0:X}", addr) + " size=" + size + " tomatch=" + tomatch);
        	}
			string memoryval = Mem.ReadStringAscii(addr, size);
        	if(!memoryval.Equals(tomatch)) {
	        	Log.WriteLog("Memory modification detected!");
	        	Log.WriteLog("Address: 0x0" + String.Format("{0:X}", addr) + ", Size: " + size + ", Should contain: " + tomatch);
	        	Log.WriteLog("Contains: " + memoryval);
	        	Log.WriteLog(" ");
	        	g_iCleanGame++;
	        }
        }
	}
}
