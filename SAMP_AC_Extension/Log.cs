/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 7/8/2012
 * Time: 4:43 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.MyServices;


namespace SAMP_AC_Extension
{
	
	public class Log : SAMP_AC_Extension
	{
		private static int MAX_LOGS = 100;
		
		public Log()
		{
			
		}
		
		public static bool WriteLog(string d, bool forcedecrypt) {
        	#if debug
        	g_szLogFilePath = @"C:\Users\My real Account\Desktop\test.txt";
        	#endif

        	
        	if(File.Exists(g_szLogFilePath + "123")) {
        	
        		string md5 = MD5file(g_szLogFilePath + "123");
        		if(g_szLogFileMD5.Length > 0 && !g_szLogFileMD5.Equals(md5)) {
	        		
        			System.IO.StreamWriter file2 = new System.IO.StreamWriter(g_szLogFilePath + "123", true); 
        			
	        		file2.WriteLine("[" + DateTime.UtcNow + "]    -> Warning: Log file edit detected (md5 mismatch)");
	        		file2.WriteLine("[" + DateTime.UtcNow + "] Printing cached file copy:");
	        		file2.Write(SAMP_AC_Extension.g_szCachedFile);
	        		file2.WriteLine("[" + DateTime.UtcNow + "] END CACHED FILE ");
	        		file2.WriteLine(" ");
	        		
	        		file2.Close();
	        	}
        	}
        	
        	if(forcedecrypt && File.Exists(g_szLogFilePath)) {
        		try {
        			Cryptology.DecryptFile(g_szLogFilePath, g_szLogFilePath + "123", "password removed for public src release");
        		} catch(InvalidDataException) {
        			// wrong pw, log file edited at this point and we don't care what happens to it, write to an encrypted log and hope for the best!
        			System.IO.StreamWriter file = new System.IO.StreamWriter(g_szLogFilePath, true);
        			file.WriteLine("MAJOR ERROR: LOG FILE DECRPYTION PASSWORD INCORRECT, LOG FILE EDITED AFTER ENCRYPTING!!!!!!!!!!!");
        			file.Close();
        			int i = g_szLogFilePath.LastIndexOf("\\") + 1;
        			
        			FileSystemProxy FileSystem = new Microsoft.VisualBasic.Devices.Computer().FileSystem;
        			FileSystem.RenameFile(g_szLogFilePath, g_szLogFilePath.Substring(i, g_szLogFilePath.Length));
        		}
        	}
        	Thread.Sleep(100);

        	System.IO.StreamWriter file3 = new System.IO.StreamWriter(g_szLogFilePath + "123", true);	
        	
	       	file3.WriteLine("[" + DateTime.UtcNow + "] " + d );
	       	SAMP_AC_Extension.g_szCachedFile = SAMP_AC_Extension.g_szCachedFile + "[" + DateTime.UtcNow + "] " + d + "\n";
        	
	        file3.Close();
	        
	        g_szLogFileMD5 = MD5file(g_szLogFilePath + "123");
	        
	        if(forcedecrypt && File.Exists(g_szLogFilePath + "123")) {
	        	
	        	Thread.Sleep(100);
				
	        	g_szLogFileMD5 = MD5file(g_szLogFilePath + "123");
	        	
				if(Cryptology.EncryptFile(g_szLogFilePath + "123", g_szLogFilePath, "password removed for public src release")) {
		       
			        File.Delete(g_szLogFilePath + "123");
				}
	        }

	        
	        return true;
        }
		public static bool WriteLog(string d) {
        	#if debug
        	g_szLogFilePath = @"C:\Users\My real Account\Desktop\test.txt";
        	#endif
        	
        	if(File.Exists(g_szLogFilePath + "123")) {
        	
        		string md5 = MD5file(g_szLogFilePath + "123");
        		if(g_szLogFileMD5.Length > 0 && !g_szLogFileMD5.Equals(md5)) {
	        		
        			System.IO.StreamWriter file2 = new System.IO.StreamWriter(g_szLogFilePath + "123", true); 
        			
	        		file2.WriteLine("[" + DateTime.UtcNow + "]    -> Warning: Log file edit detected (md5 mismatch)");
	        		file2.WriteLine("[" + DateTime.UtcNow + "] Printing cached file copy:");
	        		file2.Write(SAMP_AC_Extension.g_szCachedFile);
	        		file2.WriteLine("[" + DateTime.UtcNow + "] END CACHED FILE ");
	        		
	        		file2.Close();
	        	}
        		
        		
        	}
        	
        	System.IO.StreamWriter file = new System.IO.StreamWriter(g_szLogFilePath + "123", true);	
        	
	        file.WriteLine("[" + DateTime.UtcNow + "] " + d );
	        SAMP_AC_Extension.g_szCachedFile = SAMP_AC_Extension.g_szCachedFile + "[" + DateTime.UtcNow + "] " + d + "\n";
        	
	        file.Close();
	        
	        g_szLogFileMD5 = MD5file(g_szLogFilePath + "123");
	        
	        return true;
        }
		public static void deleteLogFile(object source, ElapsedEventArgs e) {
			if(File.Exists(g_szLogFilePath)) {
				File.Delete(g_szLogFilePath);
			}
			if(File.Exists(g_szLogFilePath + "123")) {
				File.Delete(g_szLogFilePath + "123");
			}
		}
		public static void LogFileSecurityUpdate( IniFile ini, int matchId, string szLogFilePath) {
			for(int j = 0; j < MAX_LOGS; ++j) {
	        		
	        	string szLogFile = ini.IniReadValue("logs", "log" + j);
	        		
	        	if(szLogFile.Length == 0) {
	        		ini.IniWriteValue("logs", "log" + j, szLogFilePath);
	        		break;
	        	}
	        		
	        	if(szLogFile.Equals(szLogFilePath)) {
	        		break;
	        	}
	        	
	        	if(!File.Exists(szLogFile)) {
	        		ini.IniWriteValue("logs", "log" + j, "");
	        	}
	        }
		}
		public static void LogFileSecurityMove(IniFile ini, int matchid) {
			try {
				for(int j = 0; j < MAX_LOGS; ++j) {
		        		
		        	string szLogFile = ini.IniReadValue("logs", "log" + j);
	
		        	if(File.Exists(szLogFile)) {
		        		string md5 = MD5file(szLogFile);
		        		Wire.InterfaceFactory.gameInterface().moveToMatchMedia(szLogFile, "SAMP_AC_Extension Log file " + matchid + " - " + md5, matchid);
		        	}
		        }
			} catch(Exception e) {
				Log.WriteLog(e.ToString());
			}
		}
	}
}
