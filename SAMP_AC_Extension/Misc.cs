/*
 * Created by SharpDevelop.
 * User: My PC
 * Date: 7/8/2012
 * Time: 5:04 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Management;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SAMP_AC_Extension
{
	/// <summary>
	/// Description of Misc.
	/// </summary>
	public class Misc
	{
		public Misc()
		{
		}
		
        public static void runCmdLine(string arg) {
			
        	System.Diagnostics.Process process = new System.Diagnostics.Process();
	        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
	        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
	        startInfo.FileName = "cmd.exe";
	        startInfo.Arguments = "/c " + arg;
	        process.StartInfo = startInfo;
	        process.Start();
	        
        }
		
		public static string GetOSName()
		{
        	try {
			    string result = string.Empty;
			    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
			    foreach (ManagementObject os in searcher.Get())
			    {
			        result = os["Caption"].ToString();
			        break;
			    }
			    return result;
        	} catch(Exception e) { 
        		Log.WriteLog("Failed at Get OS Name, here is the error:");
        		Log.WriteLog(e.ToString());
        	}
        	return string.Empty;
		}
		public static void TakeOwnership(string path) {
			try {
				using (var user = WindowsIdentity.GetCurrent())
				{
				    var ownerSecurity = new FileSecurity();
				    ownerSecurity.SetOwner(user.User);
				    File.SetAccessControl(path, ownerSecurity);
				
				    var accessSecurity = new FileSecurity();
				    accessSecurity.AddAccessRule(new FileSystemAccessRule(user.User, FileSystemRights.FullControl, AccessControlType.Allow));
				    File.SetAccessControl(path, accessSecurity);
				}
			} catch(UnauthorizedAccessException) {
				EndProcessAdmin();
			}
		}
		public static void EndProcessAdmin() {
			RunAsAdmin.showWindow();
			Thread.Sleep(5000);
				
			Process[] prs = Process.GetProcessesByName("wire");

			foreach (Process pr in prs)
			{				
				pr.Kill();
			}
		}
		public static string getProcessPath(Process p) {
			var wmiQueryString = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + p.Id;
			using (var searcher = new ManagementObjectSearcher(wmiQueryString))
			foreach (ManagementObject disk in searcher.Get()) 
	        {
				return disk.ToString();
	        }
			return "";
		}
	}
	public static class UacHelper
	{
	    private const string uacRegistryKey = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
	    private const string uacRegistryValue = "EnableLUA";
	
	    private static uint STANDARD_RIGHTS_READ = 0x00020000;
	    private static uint TOKEN_QUERY = 0x0008;
	    private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
	
	    [DllImport("advapi32.dll", SetLastError = true)]
	    [return: MarshalAs(UnmanagedType.Bool)]
	    static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);
	
	    [DllImport("advapi32.dll", SetLastError = true)]
	    public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);
	
	    public enum TOKEN_INFORMATION_CLASS
	    {
	        TokenUser = 1,
	        TokenGroups,
	        TokenPrivileges,
	        TokenOwner,
	        TokenPrimaryGroup,
	        TokenDefaultDacl,
	        TokenSource,
	        TokenType,
	        TokenImpersonationLevel,
	        TokenStatistics,
	        TokenRestrictedSids,
	        TokenSessionId,
	        TokenGroupsAndPrivileges,
	        TokenSessionReference,
	        TokenSandBoxInert,
	        TokenAuditPolicy,
	        TokenOrigin,
	        TokenElevationType,
	        TokenLinkedToken,
	        TokenElevation,
	        TokenHasRestrictions,
	        TokenAccessInformation,
	        TokenVirtualizationAllowed,
	        TokenVirtualizationEnabled,
	        TokenIntegrityLevel,
	        TokenUIAccess,
	        TokenMandatoryPolicy,
	        TokenLogonSid,
	        MaxTokenInfoClass
	    }
	
	    public enum TOKEN_ELEVATION_TYPE
	    {
	        TokenElevationTypeDefault = 1,
	        TokenElevationTypeFull,
	        TokenElevationTypeLimited
	    }
	
	    public static bool IsUacEnabled
	    {
	        get
	        {
	        	string str = Registry.GetValue(uacRegistryKey, uacRegistryValue, "").ToString();
	        	if(str.Length > 0) {
	        		return str.Equals("1");
	        	}
	        	// if we get to this point the user must be running windows XP or a serious error has occured.
	        	return false;
	        }
	    }
	
	    public static bool IsProcessElevated
	    {
	        get
	        {
	            if (IsUacEnabled)
	            {
	                IntPtr tokenHandle;
	                if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
	                {
	                    throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
	                }
	
	                TOKEN_ELEVATION_TYPE elevationResult = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
	
	                int elevationResultSize = Marshal.SizeOf((int)elevationResult);
	                uint returnedSize = 0;
	                IntPtr elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);
	
	                bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
	                if (success)
	                {
	                    elevationResult = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevationTypePtr);
	                    bool isProcessAdmin = elevationResult == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
	                    return isProcessAdmin;
	                }
	                else
	                {
	                    throw new ApplicationException("Unable to determine the current elevation.");
	                }
	            }
	            else
	            {
	                WindowsIdentity identity = WindowsIdentity.GetCurrent();
	                WindowsPrincipal principal = new WindowsPrincipal(identity);
	                bool result = principal.IsInRole(WindowsBuiltInRole.Administrator);
	                return result;
	            }
	        }
	    }
	}
}
