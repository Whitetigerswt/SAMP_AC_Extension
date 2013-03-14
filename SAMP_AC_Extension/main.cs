/*
 * Created by SharpDevelop.
 * User: My Account
 * Date: 3/19/2012
 * Time: 3:54 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 * 
 * SA-MP Anti-Cheat Extension plugin for ESL Wire Anti-Cheat.
 * 
 * Things this plugin does:
 * Verifies game files
 * Checks memory tampering that could allow modified game files
 * 
 */
 
 /*
  * Problems
  * 	- fix Log.DeleteLog(); from deleting files before moving to match media
  * 	- if bored, remove the massive try { } catch(Exception e)'s, and utilize } finally { too
  *		- check for game id in onmatchended
  * 	- fix system.win32component exception (only part of readprocessmemory or writeprocessmemory was completed ...)
  * 
  * */
 
 
 /*
  * v1.1
  * 	- Fixed getting a log file edit warning every time the checks where performed again every 15 minutes.
  * 	- Fixed a typo in file name.
  * 	- Fixed wire-plugin.exe crashing under certain circumstances
  * 	- Added more md5s for gta3.img
  * 
  * v1.2
  * 	- Adds exception to windows firewall for wire-plugin.exe automaticly.
  * 	- Added timestamp to log files
  * 	- Added protection against lieing about the number of log files generated
  * 	- Fixed obscure bug that could potentially overrite previous logs
  * 
  * v1.3
  * 	- Fixes a bug with decompressed sniper.txd's (with or without sniper mod) imported in gta3.img would get flagged for modified files
  * 	- Fixed a bug with "Match Media files:" showing even if there wasn't any
  * 	- Fixes a bug with System.NullReferenceException getting thrown
  * 	- Fixes a bug with System.ComponentModel.Win32Exception getting thrown (needs further testing)
  * 	- Fixes a bug where the plugin would crash and not generate log files due to an ESL Wire bug causing matchStarted event to not get called.
  * 	- Fixes a bug if a file didn't exist in your GTA directory, the scanning would stop
  *		- Fixes a bug with auto-update where if you didn't have owner permission in the /plugins/ folder then auto update would fail 	
  *		- Adds file size to modified files on log files
  * 	- Added last modified date on modified filesgame
  * 	- Small optimizations
  *
  * v1.4
  * 	- Fixes a security hole since previously nothing was obfuscated.
  * 	- Changes AES encryption password (just in case)
  * 	- Adds ESL profile instead of sa-mp name
  * 	- Adds a couple more files to memory scan
  * 	- Adds server connected IP information, to help combat 2 PC trick.
  * 	- Fixes a problem in logs where the address shown of a memory modifcation wasn't shown in hex.
  * 
  * v1.5 
  * 	- Fixed a problem if the game had not been initialized the server was not retrievable
  * 	- Updates the updater to u-army.com instead of yu-ki-ko.com, which goes offline in December 2012.
  * 
  * */
 
//#define debug

 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualBasic.MyServices;
using Microsoft.Win32;   

namespace SAMP_AC_Extension
{
	
	public class SAMP_AC_Extension 
		: Wire.Plugin
    {
		static string[,] Files = new string[,] {
		{
				
			// MAIN DIRECTORY //
			
			"eax.dll",
			"ogg.dll",
			"vorbis.dll",
			"vorbisfile.dll",
			"stream.ini",
			"bass.dll",
			"rcon.exe",
			"samp.exe",
			"samp.dll",
			"samp.saa",
			"samp_debug.exe",
			"sampaux3.ttf",
			"gtaweap3.ttf",
			
			// SAMP FOLDER //
			
			"SAMP/blanktex.txd",
			"SAMP/CUSTOM.ide", // (EMPTY FILE!)
			"SAMP/custom.img",
			"SAMP/samaps.txd",
			"SAMP/SAMP.ide",
			"SAMP/SAMP.img",
			"SAMP/SAMPCOL.img",
			
			// DATA FOLDER //
			
			"data/animgrp.dat",
			"data/animviewer.dat",
			"data/ar_stats.dat",
			"data/carcols.dat",
			"data/cargrp.dat",
			"data/carmods.dat",
			"data/clothes.dat",
			"data/default.dat",
			"data/default.ide",
			"data/fonts.dat",
			"data/furnitur.dat",
			"data/gridref.dat",
			"data/gta.dat",
			"data/gta_quick.dat",
			"data/handling.cfg",
			"data/info.zon",
			"data/map.zon",
			"data/melee.dat",
			"data/numplate.dat",
			"data/object.dat",
			"data/ped.dat",
			"data/pedgrp.dat",
			"data/peds.ide",
			"data/pedstats.dat",
			"data/plants.dat",
			"data/polydensity.dat",
			"data/popcycle.dat",
			"data/procobj.dat",
			"data/shopping.dat",
			"data/statdisp.dat",
			"data/surface.dat",
			"data/surfaud.dat",
			"data/surfinfo.dat",
			"data/timecyc.dat",
			"data/timecycp.dat",
			"data/txdcut.ide",
			"data/vehicles.ide",
			"data/water1.dat",
			"data/water.dat",
			"data/weapon.dat",	
			"data/AudioEvents.txt",	
			"data/main.sc",
				
				// MAPS FOLDER //
				
					"data/maps/Audiozon.ipl",
					"data/maps/cull.ipl",
					"data/maps/occlu.ipl",
					"data/maps/occluint.ipl",
					"data/maps/occluLA.ipl",
					"data/maps/occlusf.ipl",
					"data/maps/occluveg.ipl",
					"data/maps/paths2.ipl",
					"data/maps/paths3.ipl",
					"data/maps/paths4.ipl",
					"data/maps/paths5.ipl",
					"data/maps/paths.ipl",
					"data/maps/tunnels.ipl",
					"data/maps/txd.ide",
					
					// COUNTRY FOLDER //
					
						"data/maps/country/countn2.ide",
						"data/maps/country/countn2.ipl",
						"data/maps/country/countrye.ide",
						"data/maps/country/countrye.ipl",
						"data/maps/country/countryN.ide",
						"data/maps/country/countryN.ipl",
						"data/maps/country/countryS.ide",
						"data/maps/country/countryS.ipl",
						"data/maps/country/countryW.ide",
						"data/maps/country/countryw.ipl",
						"data/maps/country/counxref.ide",
						
					// GENERIC FOLDER //
					
						"data/maps/generic/barriers.ide",
						"data/maps/generic/dynamic2.ide",
						"data/maps/generic/dynamic.ide",
						"data/maps/generic/multiobj.ide",
						"data/maps/generic/procobj.ide",
						"data/maps/generic/vegepart.ide",
						
					// INTERIOR FOLDER //
						
						"data/maps/interior/gen_int1.ide",
						"data/maps/interior/gen_int1.ipl",
						"data/maps/interior/gen_int2.ide",
						"data/maps/interior/gen_int2.ipl",
						"data/maps/interior/gen_int3.ide",
						"data/maps/interior/gen_int3.ipl",
						"data/maps/interior/gen_int4.ide",
						"data/maps/interior/gen_int4.ipl",
						"data/maps/interior/gen_int5.ide",
						"data/maps/interior/gen_int5.ipl",
						"data/maps/interior/gen_intb.ide",
						"data/maps/interior/gen_intb.ipl",
						"data/maps/interior/int_cont.ide",
						"data/maps/interior/int_cont.ipl",
						"data/maps/interior/int_LA.ide",
						"data/maps/interior/int_LA.ipl",
						"data/maps/interior/int_SF.ide",
						"data/maps/interior/int_SF.ipl",
						"data/maps/interior/int_veg.ide",
						"data/maps/interior/int_veg.ipl",
						"data/maps/interior/propext.ide",
						"data/maps/interior/props2.ide",
						"data/maps/interior/props.ide",
						"data/maps/interior/savehous.ide",
						"data/maps/interior/savehous.ipl",
						"data/maps/interior/stadint.ide",
						"data/maps/interior/stadint.ipl",
						
					// LA FOLDER //
					
						"data/maps/LA/LAe2.ide",
						"data/maps/LA/LAe2.ipl",
						"data/maps/LA/LAe.ide",
						"data/maps/LA/LAe.ipl",
						"data/maps/LA/LAhills.ide",
						"data/maps/LA/LAhills.ipl",
						"data/maps/LA/LAn2.ide",
						"data/maps/LA/LAn2.ipl",
						"data/maps/LA/LAn.ide",
						"data/maps/LA/LAn.ipl",
						"data/maps/LA/LAs2.ide",
						"data/maps/LA/LAs2.ipl",
						"data/maps/LA/LAs.ide",
						"data/maps/LA/LAs.ipl",
						"data/maps/LA/LAw2.ide",
						"data/maps/LA/LAw2.ipl",
						"data/maps/LA/LAw.ide",
						"data/maps/LA/LAw.ipl",
						"data/maps/LA/LaWn.ide",
						"data/maps/LA/LaWn.ipl",
						"data/maps/LA/LAxref.ide",
						
					// LEVELDES FOLDER //
					
						"data/maps/leveldes/leveldes.ide",
						"data/maps/leveldes/leveldes.ipl",
						"data/maps/leveldes/levelmap.ide",
						"data/maps/leveldes/levelmap.ipl",
						"data/maps/leveldes/levelxre.ide",
						"data/maps/leveldes/seabed.ide",
						"data/maps/leveldes/seabed.ipl",
						
					// SF FOLDER //
					
						"data/maps/SF/SFe.ide",		
						"data/maps/SF/SFe.ipl",
						"data/maps/SF/SFn.ide",
						"data/maps/SF/SFn.ipl",
						"data/maps/SF/SFs.ide",
						"data/maps/SF/SFs.ipl",
						"data/maps/SF/SFSe.ide",
						"data/maps/SF/SFSe.ipl",
						"data/maps/SF/SFw.ide",
						"data/maps/SF/SFw.ipl",
						"data/maps/SF/SFxref.ide",
						
					// VEGAS FOLDER //

						"data/maps/vegas/vegasE.ide",
						"data/maps/vegas/vegasE.ipl",
						"data/maps/vegas/VegasN.ide",
						"data/maps/vegas/VegasN.ipl",
						"data/maps/vegas/VegasS.ide",
						"data/maps/vegas/VegasS.ipl",
						"data/maps/vegas/VegasW.ide",
						"data/maps/vegas/VegasW.ipl",
						"data/maps/vegas/vegaxref.ide",
						"data/maps/vegas/vegaxref.ipl",
						
					// VEH_MODS FOLDER //
					
						"data/maps/veh_mods/veh_mods.ide",	

						
				// DECISIONS FOLDER //
				
					"data/Decision/BLANK.ped", 
					"data/Decision/Cop.ped", 
					"data/Decision/FLAT.ped",  
					"data/Decision/GangMbr.ped",  
					"data/Decision/GROVE.ped",  
					"data/Decision/Indoors.ped",  
					"data/Decision/m_empty.ped",  
					"data/Decision/m_infrm.ped",  
					"data/Decision/m_norm.ped",  
					"data/Decision/m_std.ped",  
					"data/Decision/m_tough.ped", 
					"data/Decision/m_weak.ped", 
					"data/Decision/MISSION.grp", 
					"data/Decision/MISSION.ped", 
					"data/Decision/PedEvent.txt", 
					
					// DAVID FOLDER // 
					
						"data/Decision/david/dam_sec.ped", 
						"data/Decision/david/hei2_sc.ped", 
						
					// IMRAN FOLDER // 
						
						"data/Decision/Imran/sci1_is.ped",
						"data/Decision/Imran/std1_is.ped",
						"data/Decision/Imran/std2_is.ped",
						
					// CRAIG FOLDER //
					
						"data/Decision/Craig/crack1.ped",					
											
					// CHRISM FOLDER  //	

						"data/Decision/ChrisM/CMblnk.ped",
						"data/Decision/ChrisM/m_std_cm.ped",
						
					// CHRIS FOLDER //
					
						"data/Decision/chris/maf5.ped",
						"data/Decision/chris/ryder3.ped",
						
					// ANDYD FOLDER //	

						"data/Decision/andyd/ADgrp.grp",
						"data/Decision/andyd/ADtemp.ped",
					
					// ALLOWED FOLDER //
					
						"data/Decision/Allowed/Cop.ped",
						"data/Decision/Allowed/Fireman.ped",
						"data/Decision/Allowed/GangMbr.ped",
						"data/Decision/Allowed/Indoors.ped",
						"data/Decision/Allowed/m_empty.ped",
						"data/Decision/Allowed/m_norm.ped",
						"data/Decision/Allowed/m_plyr.ped",
						"data/Decision/Allowed/m_steal.ped",
						"data/Decision/Allowed/m_tough.ped",
						"data/Decision/Allowed/m_weak.ped",
						"data/Decision/Allowed/MISSION.grp",
						"data/Decision/Allowed/R_Norm.ped",
						"data/Decision/Allowed/R_Tough.ped",
						"data/Decision/Allowed/R_Weak.ped",
						"data/Decision/Allowed/RANDOM.grp",
						"data/Decision/Allowed/RANDOM.ped",
						"data/Decision/Allowed/RANDOM2.grp",
			
			// ANIM FOLDER //
			
			"anim/cuts.img",
			"anim/ped.ifp",
			//"anim/anim.img",
			
			// AUDIO FOLDER //
			
				// SFX FOLDER //
				
					"audio/SFX/FEET",
					"audio/SFX/GENRL",
					"audio/SFX/PAIN_A",
					
				// CONFIG FOLDER //
				
				"audio/CONFIG/AudioEventHistory.txt",
				"audio/CONFIG/BankLkup.dat",
				"audio/CONFIG/BankSlot.dat",
				"audio/CONFIG/EventVol.dat",
				"audio/CONFIG/PakFiles.dat",
				"audio/CONFIG/StrmPaks.dat",
				"audio/CONFIG/TrakLkup.dat",
			

			// TEXT FOLDER //
			
			"text/american.gxt",			
			
			// MODELS FOLDER // 
			
			"models/effects.fxp",
			"models/effectsPC.txd",
			"models/fonts.txd",
			"models/fronten1.txd",
			"models/fronten2.txd",
			"models/fronten3.txd",
			"models/fronten_pc.txd",
			//"models/gta3.img",
			//"models/gta_int.img",
			"models/hud.txd",
			"models/misc.txd",
			"models/particle.txd",
			"models/player.img",
			"models/cutscene.img",
			"models/pcbtns.txd",
			
				// COLL FOLDER //
				
				"models/coll/peds.col",
				"models/coll/vehicles.col",
				"models/coll/weapons.col",
				
				// GRASS FOLDER // 
				
				"models/grass/grass0_1.dff",
				"models/grass/grass0_2.dff",
				"models/grass/grass0_3.dff",
				"models/grass/grass0_4.dff",
				"models/grass/grass1_1.dff",
				"models/grass/grass1_2.dff",
				"models/grass/grass1_3.dff",
				"models/grass/grass1_4.dff",
				"models/grass/grass2_1.dff",
				"models/grass/grass2_2.dff",
				"models/grass/grass2_3.dff",
				"models/grass/grass2_4.dff",
				"models/grass/grass3_1.dff",
				"models/grass/grass3_2.dfF",
				"models/grass/grass3_3.dff",
				"models/grass/grass3_4.dff",
				"models/grass/plant1.dff",
				"models/grass/plant1.txd"
				
			
		},
		{
			"309d860fc8137e5fe9e7056c33b4b8be",	// eax.dll
			"0602f672ba595716e64ec4040e6de376", // ogg.dll
			"2840f08dd9753a5b13c60d6d1c165c9a", // vorbis.dll
			"2b7b803311d2b228f065c45d13e1aeb2", // vorbisfile.dll
			"05b6fdb1ff98a4ec75a58536a0c47b5e", // stream.ini
			"8f5b9b73d33e8c99202b5058cb6dce51", // bass.dll
			"3f4821cda1de6d7d10654e5537b4df6e", // rcon.exe
			"c06568d52fecd2f97b7aaab911a83959", // samp.exe (0.3e)
			"07012dc468f2b89db2584e358a048e39", // samp.dll (0.3e)
			"693b1497e7ce89869c24a43a3ff8e836", // samp.saa (0.3e)
			"2c00c60a5511c3a41a70296fd1879067", // samp_debug.exe (0.3e)
			"6a03a32076e76f6c1720cad6c6ea6915", // sampaux3.ttf
			"59cbae9fd42a9a4eea90af7f81e5e734", // gtaweap3.ttf
			
			"00dc42d499f5ca6059e4683fd761f032", // SAMP/blanktex.txd
			"d41d8cd98f00b204e9800998ecf8427e", // SAMP/CUSTOM.ide (EMPTY FILE!)
			"8fc7f2ec79402a952d5b896b710b3a41", // SAMP/CUSTOM.img
			"e0fdfd9fbe272baa9284e275fb426610", // SAMP/samaps.txd
			"26bfe17e8213d06f9bbeefe5635f24f2", // SAMP/SAMP.ide
			"7228658fa7884bedf87576d6d04f7517", // SAMP/SAMP.img
			"7edefcf60a1c3852e978dce1ea0a61ed", // SAMP/SAMPCOL.img
				
			"6a484b0b2356c524207d939487f1bff1", // data/animgrp.dat
			"f856ba3a4ba25ae10b561aa764fba0c4", // data/animviewer.dat
			"a98936b0f3523f23cad2eacc0eaf7a9b", // data/ar_stats.dat
			"2b33843e79bd113873a5d0fb02157749", // data/carcols.dat
			"63138ab62a10428a7c88f0be8ece094d", // data/cargrp.dat
			"6cbe845361e76aae35ddca300867cadf", // data/carmods.dat
			"8762637e580eb936111859ffa29bddb4", // data/clothes.dat
			"8e133355396761bd5cd16bf873154b30", // data/default.dat
			"5b6d75bae827e2d88f24f2be66a037bb", // data/default.ide
			"eb30c2a90d66d6f0bf5e3a7d5447ac01", // data/fonts.dat
			"3199fc8b81a4c5334a497508fe408afd", // data/furnitur.dat
			"795a9c013ee683e286768e06e4a5e2d7", // data/gridref.dat
			"2d2e4f7f05e2d82b25c88707096d3393", // data/gta.dat
			"012841ec691f84de4606ddcbff89e997", // data/gta_quick.dat
			"6868accef933f1855ec28ce193a78159", // data/handling.cfg
			"7df10bed5404a2f7669cdfaa47b8b81b", // data/info.zon
			"79d255c7a27bb49b50d680390e908e5a", // data/map.zon
			"b2f05657980e4a693f8ff5eadcbad8f8", // data/melee.dat
			"f152559cdaba5573e9f8aa78bf1d0fc2", // data/numplate.dat
			"46a5e7dff90078842e24d9de5e92cc3e", // data/object.dat
			"67d960dde13228d4818e0f144adafe4e", // data/ped.dat
			"fa1731066423ba0c584e757eda946f15", // data/pedgrp.dat
			"f7dea69fa6ab973479b9ef0cf05d3d98", // data/peds.ide
			"d722c90c92f3ad5c1b531596769f61cd", // data/pedstats.dat
			"a2713338dbbd55898a4195e4464c6b06", // data/plants.dat
			"48676fe82312f8f4a1bdf65c76719425", // data/polydensity.dat
			"a43f90361d1034c819a602171d8d66cb", // data/popcycle.dat
			"7229fa03d65f135bd569c3692d67c4b3", // data/procobj.dat
			"c1086eb6c0bfa36845f2026b68519f14", // data/shopping.dat
			"2ee5d9c1abb281f26f8cd00e9eefd65e", // data/statdisp.dat
			"9eb4e4e474abd5da2f3961a5ef549f9e", // data/surface.dat
			"c32c586e8ba35742e356e6525619f7c3", // data/surfaud.dat
			"605dd0beabccc797ce94a51a3e4a09eb", // data/surfinfo.dat
			"d66a121bc8f17a5b69e34b841744956c", // data/timecyc.dat
			"c91ce6b9f69578dc0fcd890f6147224c", // data/timecycp.dat
			"e3c231039048a30680b8f13fb51cc4ac", // data/txdcut.ide
			"bdc3a0fced2402c5bc61585714457d4b", // data/vehicles.ide
			"16fe5a3e8c57d02eb62a44a96d8b9d39", // data/water1.dat
			"690400ecc92169d9eaddaaa948903efb", // data/water.dat
			"0a9bb49003680364f9f9768e9bcea982", // data/weapon.dat
			"f638fae1023422aef37b22b336e7fdc6", // data/AudioEvents.txt
			"0b78b0b080b05d2de9228e0d23196aed", // data/main.sc
			
			"bc3d7fc5a6927b61c10acda92e7e20c0", // data/maps/Audiozon.ipl
			"7d723b80560f956bddb8d97ed66086b8", // data/maps/cull.ipl
			"fbe5264b558576cff738291bc17a9c51", // data/maps/occlu.ipl
			"e89a5ae5ee074086862664b50f6881f5", // data/maps/occluint.ipl
			"a355e96c0102c3187fe75da90572b3f2", // data/maps/occluLA.ipl
			"9ac2fb7ddddfe7f71a4faad8f71a7b98", // data/maps/occlusf.ipl
			"394dc4170c928d4227f9c0e185d51261", // data/maps/occluveg.ipl
			"b38219724ec5eaa1dba1df4331389509", // data/maps/paths2.ipl
			"1f5f7a824575552057fe7001c54c51a9", // data/maps/paths3.ipl
			"73c948a8d373623524b4fca8a2b9c25a", // data/maps/paths4.ipl
			"8f3d100baff8ee2088d0b74474175250", // data/maps/paths5.ipl
			"37426c7d5218aa13aaec2f582aaaabc4", // data/maps/paths.ipl
			"1438622c076f6122ff6cdd03241b638c", // data/maps/tunnels.ipl
			"46f2df900d7d79a68ac3eac499cb6f35", // data/maps/txd.ide
			
			"77fc33d796a96c6d3b680e2f74e6739a", // data/maps/country/countn2.ide
			"0727df5077b9a6bb7e3e23c1c3990da9", // data/maps/country/countn2.ipl
			"d0883386721ef7dfd9322069f2bddd9a", // data/maps/country/countrye.ide
			"5033cf4354baaad2a521031a1b318df0", // data/maps/country/countrye.ipl
			"ad384494c4d2d94683d2c51cef390395", // data/maps/country/countryN.ide
			"443490bda181e7f87e74fcc7704f2500", // data/maps/country/countryN.ipl
			"85792d5a12621e879cc59ed87db82480", // data/maps/country/countryS.ide
			"8d1bd4b5d337139ff3a953d44fc2b0e3", // data/maps/country/countryS.ipl
			"b8d0fdd9f7223ded76f4b5f6700fcb6f", // data/maps/country/countryW.ide
			"a46e0560ee61446cbb4d531d99dec553", // data/maps/country/countryw.ipl
			"661831485cc61d2b99dca07431cf08e4", // data/maps/country/counxref.ide
			
			"d22010cd9522b19bf07efbc421872add", // data/maps/generic/barriers.ide
			"46f6e2bcfaed43f10885961408691c4e", // data/maps/generic/dynamic2.ide
			"319e6aea03de0c05e075a1f15ed1ce8c", // data/maps/generic/dynamic.ide
			"63d672310c0cf0efaa8b96c584dd407a", // data/maps/generic/multiobj.ide
			"bf592e31a663405116a68b63e7d2c49f", // data/maps/generic/procobj.ide
			"b9c84559de97b49ce2036498b3d504d5", // data/maps/generic/vegepart.ide
			
			"8cdc36bf580a82bf281c9d3a257d4742", // data/maps/interior/gen_int1.ide
			"eb13aff288ed3876354a326a22f29d93", // data/maps/interior/gen_int1.ipl
			"9b55e0b126ef22f3703b5ccf5bb1b174", // data/maps/interior/gen_int2.ide
			"5efb82b23a9462cbf4c1d0ba6dbc9fd4", // data/maps/interior/gen_int2.ipl
			"5119e846419e50bbc35bd57415d4376e", // data/maps/interior/gen_int3.ide
			"e14d644ed3a26d5711704f875f2c11e1", // data/maps/interior/gen_int3.ipl
			"16c7dba5af8ae61172599f29ed9dd6c0", // data/maps/interior/gen_int4.ide
			"339028d9c3aac53a8a28212179fced01", // data/maps/interior/gen_int4.ipl
			"1f183baa44e0b759c2917c34ac23d3b5", // data/maps/interior/gen_int5.ide
			"0acd219bca22d7cb8c8b50d738c3275c", // data/maps/interior/gen_int5.ipl
			"48a554b28c8045c21b6ed0905a76768f", // data/maps/interior/gen_intb.ide
			"663b75b0898db03687d5e6edb1d3b7f8", // data/maps/interior/gen_intb.ipl
			"f811cdbcfd62ad7c8bf4be61c2d89855", // data/maps/interior/int_cont.ide
			"5331b6ee9cbf7a976f98dc2cce6992e3", // data/maps/interior/int_cont.ipl
			"b46f52a4e205996c24791b3e9ad012de", // data/maps/interior/int_LA.ide
			"2ae3c352e5de6e290dc631da053c5cfc", // data/maps/interior/int_LA.ipl
			"cdfdb64d5254a0fc689604d000d0e29c", // data/maps/interior/int_SF.ide
			"3e8cf0138f81f0c505f245e7bbff8b28", // data/maps/interior/int_SF.ipl
			"3ffe6a366fcadba1a2fb3fb8166ceb31", // data/maps/interior/int_veg.ide
			"8d8030efa3a493324016a395fd180926", // data/maps/interior/int_veg.ipl
			"be0f534711073a19378cd30231b9d094", // data/maps/interior/propext.ide
			"043a304b604db2e22d7464bcc36a41ce", // data/maps/interior/props2.ide
			"b7ca66885d4fc34fe2f2083f8ed5d725", // data/maps/interior/props.ide
			"19af45eb13708b6b3ed9434a42e6a929", // data/maps/interior/savehous.ide
			"2e79a68217244d7d99f3790b63fe3267", // data/maps/interior/savehous.ipl
			"10953f74890c554dd368ff20bdbeac3d", // data/maps/interior/stadint.ide
			"242ece3c9c070faf6751e66bfb17531c", // data/maps/interior/stadint.ipl
			
			"99d656e3f7b484cf7231c01e42b02c8e", // data/maps/LA/LAe2.ide
			"ffa2c90e439f5bd7da317c1ad29a9495", // data/maps/LA/LAe2.ipl
			"6f046fe75467807c612c7f5f0d9bc90f", // data/maps/LA/LAe.ide
			"fa95160d9826c195e1b9d5128c20b35f", // data/maps/LA/LAe.ipl
			"7b9289601c4961f461dd1f2d8b2cc0fa", // data/maps/LA/LAhills.ide
			"4c30d0186a0c65d5be31f5a45965003a", // data/maps/LA/LAhills.ipl
			"a0e347f662168d4397e2bc1140ba4b2e", // data/maps/LA/LAn2.ide
			"dbcd933c5857dab676715b930f3eef1f", // data/maps/LA/LAn2.ipl
			"b24ce30c982c7bc5b8fafc58b97c0dd7", // data/maps/LA/LAn.ide
			"3f3a9e01ca47388ed62e7bd10527572f", // data/maps/LA/LAn.ipl
			"d6f69e02992be26e5606fe1b3d6c8be5", // data/maps/LA/LAs2.ide
			"cf63399c3f61e8a93e0e8dc7a09e766b", // data/maps/LA/LAs2.ipl
			"f73f76648dc35e2b7eed800bcf699757", // data/maps/LA/LAs.ide
			"4540df6c3a170ab9b4ff3acce5a9674f", // data/maps/LA/LAs.ipl
			"91ce9890027ca87158a5328417689004", // data/maps/LA/LAw2.ide
			"f220195b871833163f5c04856b03ae8d", // data/maps/LA/LAw2.ipl
			"e3c998eb61ff77a86aeee8ac804d1a7e", // data/maps/LA/LAw.ide
			"1bfe3e1fabd610dc76ab0b44e26dacc6", // data/maps/LA/LAw.ipl
			"235545b8eb93dcffcb70c1dc2ff6d5fe", // data/maps/LA/LaWn.ide
			"0720120f5c025757ece1344df0d85a30", // data/maps/LA/LaWn.ipl
			"ae01aa97caf5741d240a301ee2770915", // data/maps/LA/LAxref.ide
			
			"521496c7b8e148bf65e15a2eb9cffdba", // data/maps/leveldes/leveldes.ide
			"4276ccba517ef022c45b39608699a8af", // data/maps/leveldes/leveldes.ipl
			"8ea3bf7c907763418a32fd7b42249d96", // data/maps/leveldes/levelmap.ide
			"4fb15ec9a9bd76e47842e01cdefa2585", // data/maps/leveldes/levelmap.ipl
			"4dda3ffcee83f83ca554d85817e52198", // data/maps/leveldes/levelxre.ide
			"0779f538229decee62071e53f41d4b93", // data/maps/leveldes/seabed.ide
			"fbedb38d8860a71d63d30e4c0f458c86", // data/maps/leveldes/seabed.ipl
			
			"a45f71b74a0dc25d83892dc4ba5fac3c", // data/maps/SF/SFe.ide
			"da55b24ac305b2779f46cc1509f444d8", // data/maps/SF/SFe.ipl
			"27566b5a6dce82513cfdcf65c82ca958", // data/maps/SF/SFn.ide
			"a2133c436d46396b4e1511a33bcb8870", // data/maps/SF/SFn.ipl
			"8b6341f47887183fbe475c14722d8e9f", // data/maps/SF/SFs.ide
			"518077ea0974332c8352eb9af931d197", // data/maps/SF/SFs.ipl
			"a245de4449fe41d4ed7440b80c193d06", // data/maps/SF/SFSe.ide
			"b275b70372a6d91643e3f585325f8ddc", // data/maps/SF/SFSe.ipl
			"4103fe4fffa18c4845027fb3d7929296", // data/maps/SF/SFw.ide
			"e435f3d83ef4abcc4ce56c41767601ee", // data/maps/SF/SFw.ipl
			"23b00cc73b9564f739730c61a232353d", // data/maps/SF/SFxref.ide
			
			"eea176558132ff026e2e5dac68ff9e5a", // data/maps/vegas/vegasE.ide
			"ea279d9a9d6e3addb981b5186da91424", // data/maps/vegas/vegasE.ipl
			"6a35f16ac1b76f151be42c3860c81ffd", // data/maps/vegas/VegasN.ide
			"e6b6cc52ad19a5e93a45d659019f7b41", // data/maps/vegas/VegasN.ipl
			"ed0cca34a8fdc556a7ca835ee0923b58", // data/maps/vegas/VegasS.ide
			"ce4cb524d5bee74bab77029e5541b1ae", // data/maps/vegas/VegasS.ipl
			"593313a65eb9f8905c61bd02296e4468", // data/maps/vegas/VegasW.ide
			"43989ba645119a9b287fb0e3782245e7", // data/maps/vegas/VegasW.ipl
			"61c9cd72a43ca6f34788c4bde736431e", // data/maps/vegas/vegaxref.ide
			"789520fd41a60f0067c802c6f00d021b", // data/maps/vegas/vegaxref.ipl
			
			"e5f05eea1d6fb145bfa0d5f9950ddd54", // data/maps/veh_mods/veh_mods.ide
			
			"4383184825f1613669ca3355e315f1e9", // data/Decision/BLANK.ped
			"e9eec8d526895a406b574a078de613ba", // data/Decision/Cop.ped
			"b38e087d8b77152a984cf8a5164d6e97", // data/Decision/FLAT.ped
			"2ed56525f52ee06e96fe05599bb6fab1", // data/Decision/GangMbr.ped
			"38dfb77dc7343d470688014b4eabce27", // data/Decision/GROVE.ped
			"2f413f8cf94aa074d4d2b35984c1b1fe", // data/Decision/Indoors.ped
			"03ee2db935fe87152b9e3540f1ac509e", // data/Decision/m_empty.ped
			"f3d578db43e1148e657211cb392b35cd", // data/Decision/m_infrm.ped
			"cf979d9712f478d0deb92fbb11c6ff2e", // data/Decision/m_norm.ped
			"8296a96da8498d8848a191d47ea75ab5", // data/Decision/m_std.ped
			"cf979d9712f478d0deb92fbb11c6ff2e", // data/Decision/m_tough.ped (Identical MD5s with m_weak.ped.)
			"cf979d9712f478d0deb92fbb11c6ff2e", // data/Decision/m_weak.ped
			"ecfce8c43aa3e27eaa063543b2c68891", // data/Decision/MISSION.grp
			"4ebc62f3c473d949cecac27f98bb87aa", // data/Decision/MISSION.ped
			"e4fa5caa1558f2945294a3652e6f0cde", // data/Decision/PedEvent.txt
			"36e16f72d8be78bb8628478d5642860d", // data/Decision/david/dam_sec.ped
			"bf932fd285c05f708171b2e7cf0abe35", // data/Decision/david/hei2_sc.ped
			
			"fbefb46d14ba4dd939c3781d8ebdc2b8", // data/Decision/Imran/sci1_is.ped
			"5ac444f731e87c911d5f6469c98a6684", // data/Decision/Imran/std1_is.ped
			"07b03be54f98eae5e60674f77f9e9b45", // data/Decision/Imran/std2_is.ped
			"81c527d932e4949a3f0dce77caab1b5b", // data/Decision/Craig/crack1.ped
			"cb2fdafd51c78baed7d2a60470007401", // data/Decision/ChrisM/CMblnk.ped
			"cc4bce60ef1aac211340bd54ad08b2e1", // data/Decision/ChrisM/m_std_cm.ped
			"4f7aa59ad04a276f28211fe3780bd4da", // data/Decision/chris/maf5.ped
			"da741c471b42859c99b3468bde1dc621", // data/Decision/chris/ryder3.ped
			"de53187f1c9ba8b1efcb240e7c01a4e9", // data/Decision/andyd/ADgrp.grp
			"cfefbc0fdc988cafcd4a3bf6b13be064", // data/Decision/andyd/ADtemp.ped
			"2b2456482e8719e8c64877070fecee7f", // data/Decision/Allowed/Cop.ped
			"99ef637f82455921c9572fede370e33b", // data/Decision/Allowed/Fireman.ped
			"cd6e25e2a07fa5e1cc2f0d952c51f3af", // data/Decision/Allowed/GangMbr.ped
			"5071731f4fd49d79b0562c7dec9a673b", // data/Decision/Allowed/Indoors.ped
			"149d0c778cab34b995d3958f44eeb18b", // data/Decision/Allowed/m_empty.ped
			"d6ed517e1e6809c6ad0e9e2c163f410e", // data/Decision/Allowed/m_norm.ped
			"50ec4c398f482bbe9428e1011b4bc0b2", // data/Decision/Allowed/m_plyr.ped
			"ea0e40c00071a4a9446c19b12bf5a035", // data/Decision/Allowed/m_steal.ped
			"be9ea5daacc227b8383b9c84a5b3fa9b", // data/Decision/Allowed/m_tough.ped
			"03dc3abafa80abec285adc0bddee6777", // data/Decision/Allowed/m_weak.ped
			"837dd1e06da29bf5d7210a9074164cf2", // data/Decision/Allowed/MISSION.grp
			"75d670db732344ec3f90e7db71b1e3a6", // data/Decision/Allowed/R_Norm.ped
			"b3e4ca143c1bbcbf99ebf70ef95e7343", // data/Decision/Allowed/R_Tough.ped
			"3cddd65754ad3c6ee8aec71b8a69b6c3", // data/Decision/Allowed/R_Weak.ped
			"cda22c3bed5dd3742542084461082d24", // data/Decision/Allowed/RANDOM.grp
			"d6ed517e1e6809c6ad0e9e2c163f410e", // data/Decision/Allowed/RANDOM.ped
			"25cfafb3b7da432277bfa1291df4d58d", // data/Decision/Allowed/RANDOM2.grp
		
			"2afcb246fe97406b47f4c59deaf5b716", // anim/cuts.img
			"4736b2c90b00981255f9507308ee9174", // anim/ped.ifp
			//"3359ba8cb820299161199ee7ef3f1c02", // anim/anim.img
			
			"1717fe0644d83f7464665808b4b71b80", // audio/SFX/FEET
			"7813ccc099987ff9e51c136ed919f665", // audio/SFX/GENRL
			"80629d549f026ef5d27b6ac9fa453f90", // audio/SFX/PAIN_A
			

			"4f15962340d51394d47a60e11cdbb608", // audio/CONFIG/AudioEventHistory.txt
			"b45905c794677467644240aa9abc2f60", // audio/CONFIG/BankLkup.dat
			"da40c568a349b58c78c2a8faf8da95a9", // audio/CONFIG/BankSlot.dat
			"d676adc31b1d0a95631451344892ddd2", // audio/CONFIG/EventVol.dat
			"db1e657a3baafbb86cd1b715c5282c66", // audio/CONFIG/PakFiles.dat
			"6e65fd943ad6b0bbbc032e1f081ce699", // audio/CONFIG/StrmPaks.dat
			"528e75d663b8bae072a01351081a2145", // audio/CONFIG/TrakLkup.dat
			"6791e6e0ffa6317af8a0dff648c9633d", // text/american.gxt
			
			"6143a72e8ff2974db14f65df65d952b0", // models/effects.fxp
			"0802650dfea37ed516e1c0f12ccb77d7", // models/effectsPC.txd
			"3ea286fb7d7086d353b42a8e2b021cea", // models/fonts.txd
			"7414ee5a8fa7a906f1c49b8897805e07", // models/fronten1.txd
			"386dae2e9f205ed2c75c4499503466f7", // models/fronten2.txd
			"af42eee4d2d71a83039eaee3f602be9a", // models/fronten3.txd
			"aa7ba893d292c6bf2aa5e16e0e6c8c1b", // models/fronten_pc.txd
			//"9282e0df8d7eee3c4a49b44758dd694d", // models/gta3.img
			//"dbe7e372d55914c39eb1d565e8707c8c", // models/gta_int.img
			"18d2abd58e28c06b721197a0458d4405", // models/hud.txd
			"5ba1aa955cf55240b6dd6e0a25d28b57", // models/misc.txd
			"585f47abb0a6ea6c17d5a7638a1a07d9", // models/particle.txd
			"b06073200f58e220dcd5915ea646b468", // models/player.img
			"6b1047ae046e2697cec326610ec07a1a", // models/cutscene.img
			"9ff145d936961fd37915c6ae186f6775", // models/pcbtns.txd
			
			"74288cbdd843c3cfb77b036a5614ae9d", // models/coll/peds.col
			"c84c1a1b67d5fad3df75dd8d45fc576b", // models/coll/vehicles.col
			"510e74e32b323eee54dd7a243b073808", // models/coll/weapons.col
			
			"be8763269599e91dcc596f13056d58dc", // models/grass/grass0_1.dff
			"fe3b316979b03509278268b7479614f1", // models/grass/grass0_2.dff
			"51d72ecadea1da6b5c4e1272b77d79fb", // models/grass/grass0_3.dff
			"07a37a4e069aafb2eeeab56125ee21ed", // models/grass/grass0_4.dff
			// notice these MD5's/files+ are identical
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass1_1.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass1_2.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass1_3.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass1_4.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass2_1.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass2_2.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass2_3.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass2_4.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass3_1.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass3_2.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass3_3.dff
			"84e3cdac0050a7ea9a87395728b99ac3", // models/grass/grass3_4.dff
		
			"e88432f1e188a4cfc6959ae645a4329f", // models/grass/plant1.dff
			"15552e439a8daf86a6da252ba575381f"  // models/grass/plant1.txd
			} 
		};
		
		static string[,] gta_sa = new string[,] {
		{
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe",
			"gta_sa.exe"
		},
		{	
			"e7697a085336f974a4a6102a51223960", // original gta_sa.exe
			
			"170b3a9108687b26da2d8901c6948a18", // cracked gta_sa.exe's
			"6c6160da9b175b66cf9127c86be57bf7", // ^
            "68ba9ec43813ad58e39d64f88ebdc6a6", // ^
            "c8a31567f7279889cff09e49f3b8ce7a", // ^
            "842c61a45ace7638ff1e85d1f7a38545", // ^
            "8a3ff2c40e64ecb8ebe9a51d36b18b21", // ^
            "25405921d1c47747fd01fd0bfe0a05ae", // euro v1.01 cracked DEViANCE
            "bf25c28e9f6c13bd2d9e28f151899373"  // v2.0 American non-cracked
		} }; 
		
		static string[,] gta3img = new string[,] {
		{
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img",
			"models/gta3.img"
		},
		{	
			"9282e0df8d7eee3c4a49b44758dd694d", // euro gta3.img
			"42a1224aa88741c4f6b40d3d5191b7f0", // american gta3.img
			"4814e71b26ac60c611bca1ca86e6926c", // euro gta3.img with sniper mod
			"0fa42a523cbc651bc0d483c8e1daa6c7", // american gta3.img with sniper mod
			"7d6474795fe3829bbb93543af7e9084b", // krider sniper.txd
			"2a60a61c65c1fb46372994bffe2b319b",
			"02c3ab427b4924470e06a6a77b4f875f",
			"d1584540828e439fd406633626af3bd1",
			"afd02aaba77630051629ec917d915b83", // phaze 1 mb sniper.txd
			"b120f2acd8f6b67a11a54bee39ba23c6", // jakez
			"38ed6ce356a65cbd75fa545553457955", // me, 1 mb sniper.txd
			"bb2efcf3caca725da691d7cc4997dd07" // phaze
		} };
		
		static string[,] animimg = new string[,] {
		{
			"anim/anim.img",
			"anim/anim.img"
		},
		{	
			"3359ba8cb820299161199ee7ef3f1c02", // pacino anim.img
			"93efd3c1dbedcf77fdb6cc34f8cb98f5"  // me anim.img
		} };
		
		static string[,] gtaintimg = new string[,] {
		{
			"models/gta_int.img",
			"models/gta_int.img"
		},
		{	
			"dbe7e372d55914c39eb1d565e8707c8c", // euro gta_int.img
			"04791842f55aeb3d2c40cd82f98c82fe", // american gta_int.img
		} };
		
		public static string g_szCachedFile = "";
		public static bool g_bGTASAStarted = false;
		public static int g_iMatchId = -1;
		public static string g_szMatchMediaPath;
		public static string g_szLogFilePath;
		public static int g_iCleanGame = 0;
		public static string g_szGTASaPath;
		public static string g_szLogFileMD5;
		public static string g_szWireGamePath;
		public static string g_szLastSavedPath;
		
		public static double g_dVersion = 1.5;
		
		public static System.Timers.Timer aTimer = new System.Timers.Timer();
		
		
        public static void Main() 
        {
        }

        public override string Author
        {
            get { 
                return "Whitetiger";
            }
        }
        public override string  Title {
	        get { 
                return "SAMP_AC_Extension"; 
            }
        }
        public override string Version
        {
            get { 
        		return g_dVersion.ToString();
            }
        }
        
        public override void init()
        {
        	setIcon("gtasa.gif");
        	setTooltip("SA-MP Anti-Cheat Extension plugin.");
        	
            Wire.GameInterface gi = Wire.InterfaceFactory.gameInterface();
            
	        gi.GameStarted += new Wire.GameInterface.GameStartedHandler(checkGame);
	        gi.GameStopped += new Wire.GameInterface.GameStoppedHandler(whenGameStopped);
	            
	        gi.MatchStarted += new Wire.GameInterface.MatchStartedHandler(onMatchStarted);
	        gi.MatchEnded += new Wire.GameInterface.MatchEndedHandler(onMatchEnded);
	        
	        // Set our repeating timer to re-check files after 10 minutes.
	        aTimer.Elapsed +=new ElapsedEventHandler(checkGameRepeat);

			aTimer.Interval = 600000;
			
			// Get our random file path name.
	        g_szLogFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
	        
	        try {
	        	// Add the wire-plugin.exe to the Windows Firewall allowed list	
	        	Process p = Process.GetCurrentProcess();
	        	if(p != null) {
					if(Misc.GetOSName().Contains("Windows XP")) {
						Misc.runCmdLine("netsh firewall delete allowedprogram \"" + p.MainModule.FileName + "\"");
						Misc.runCmdLine("netsh firewall add allowedprogram program=\"" + p.MainModule.FileName  + "\" name=\"ESL Wire Plugin\" mode=ENABLE scope=ALL profile=ALL");
					} else {
						Misc.runCmdLine("netsh advfirewall firewall delete rule name=\"ESL Wire Plugin\"");
						Misc.runCmdLine("netsh advfirewall firewall add rule name=\"ESL Wire Plugin\" dir=in action=allow program=\"" + p.MainModule.FileName + "\" enable=yes remoteip=any profile=public,private");
					}
	        	}
			} catch(Exception e) { Log.WriteLog(e.ToString()); }

	        try {
	        	
	        	checkForUpdate();
	        } catch(Exception e) { Log.WriteLog(e.ToString()); }
	    
	        try {
	        	if(!UacHelper.IsProcessElevated) {
	        		Misc.EndProcessAdmin();
	        	}
	        } catch(Exception e) { Log.WriteLog(e.ToString()); }
	        
	        // Check if SA-MP is open already
	        ProcessMemory Mem = new ProcessMemory("samp"); 
	        
	        if(Mem.CheckProcess()) {
	        	
	        	Log.WriteLog("Warning: SA-MP Was already started when ESL Wire was launched.");
	        	
	        	Process[] p = Process.GetProcessesByName("samp");
	        	
	        	foreach(Process proc in p) {
	        		if(proc != null) {
	        			checkGame(4039, proc.MainModule.FileName);
	        		}        			
	        	}
	        }	        
        }
        
        public override void onExit() {        
        	Log.WriteLog("SAMP_AC_Extension is closing.");
        }
        
        public static void checkForUpdate() {

	        FileSystemProxy FileSystem = new Microsoft.VisualBasic.Devices.Computer().FileSystem;
	        
	        // Get the current SAMP_AC_Extension.dll location.
	        string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
	        int idx = loc.LastIndexOf("\\");
	        
	     	// Get the file name, relevant to the current directory (prog files/eslwire/plugins)        
	        string loc_name = loc.Substring(idx+1);  
	        
	        // Take a hash of it to compare to the new version, if one is needed.
	        string md5 = MD5file(loc);
	        
			try {
				if(!loc_name.Equals("SAMP_AC_Extension.dll")) {
					
	        		// If the file isn't named "SAMP_AC_Extension.dll", rename it to that.
					FileSystem.RenameFile(loc, "SAMP_AC_Extension.dll");
					
					// Force them to restart Wire.
					ThreadStart job = new ThreadStart(Form1.ShowTheWindow);
			        Thread thread = new Thread(job);
			        thread.Start();
			        
					return;
				} 
			} catch(Exception) { }
	     	        
	        loc_name = loc.Substring(0, idx+1);  
	        
	        // If we found an old update version, delete it.
	        if(File.Exists(loc_name + "SAMP_AC_Extension.dll_")) {
	        	File.Delete(loc_name + "SAMP_AC_Extension.dll_");
	        } 
	        
			
	        try {
	        	// Query for update
	        	WebRequest request = WebRequest.Create("http://u-army.com/tiger/esl/vernumber.txt");
		        
		        request.Credentials = CredentialCache.DefaultCredentials;
		        
		        // get our response to the webpage query
				WebResponse response = request.GetResponse();
				
				System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
				double newver = Double.Parse(reader.ReadToEnd().Trim(), CultureInfo.InvariantCulture);
				
				// Free
				reader.Close();
				response.Close();
				
				// If current version doesn't equal newest version, update.
				if(newver != g_dVersion) {
					
					// get the URL of the download.
					request = WebRequest.Create("http://u-army.com/tiger/esl/addr.txt");
		        
					request.Credentials = CredentialCache.DefaultCredentials;
					
					// Get response
					response = request.GetResponse();
					reader = new System.IO.StreamReader(response.GetResponseStream());

					// Take our file location string, for the currently loaded plugin, and get the last index of "\" 
					int idx_ = System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\");
					try {
						// Make the /plugins/ folder's owner our currently loaded user. (Note: only works when process is elevated.)
						Misc.TakeOwnership(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, idx_));
					} catch(Exception) { } 
					
					// Rename the currently loaded plugin, not the update, to have an _ at the end of it (Wire will only load plugins that end in .dll and not .dll_.)
					FileSystem.RenameFile(System.Reflection.Assembly.GetExecutingAssembly().Location, "SAMP_AC_Extension.dll_");					
								
					System.Net.WebClient download = new System.Net.WebClient();
					// Download the new version to the exact location of the executing .dll, even though we've renamed it, it still returns the old file location.
					download.DownloadFile(reader.ReadToEnd(), System.Reflection.Assembly.GetExecutingAssembly().Location);
					
					reader.Close();
			        response.Close();
					
			        // Compare MD5 of old version to new version, if they match, delete this update and restore old .dll
					if(File.Exists(System.Reflection.Assembly.GetExecutingAssembly().Location + "_") && MD5file(System.Reflection.Assembly.GetExecutingAssembly().Location) == md5) {
					
			        	File.Delete(System.Reflection.Assembly.GetExecutingAssembly().Location);
			        	FileSystem.RenameFile(System.Reflection.Assembly.GetExecutingAssembly().Location + "_", "SAMP_AC_Extension.dll");	
						return;
					}
			        
					// else, force a Wire restart.
					ThreadStart job = new ThreadStart(Form1.ShowTheWindow);
			        Thread thread = new Thread(job);
			        thread.Start();

			        return;
				}
	        } catch(System.Net.WebException) {
	        	Log.WriteLog("A firewall(?) has blocked auto-update.");
	        } catch(Exception e) {
	        	Log.WriteLog(e.ToString());
	        }  
	   }
        // NOTE: this function does not get called on some PCs (QWERTs), for most things, don't rely on this function!!!!
        public static void onMatchStarted(int matchId, string matchMediaPath) {
        	
        	try {
        		
        		// Set current matchId, and match media path to our equivalent global variables.
	        	g_iMatchId = matchId;
	        	g_szMatchMediaPath = matchMediaPath;
	        	
	        	
	        	// Check the match media path for any files there might already be in there, and print them in our log.
	        	string[] f = Directory.GetFiles(g_szMatchMediaPath);
	        	
	        	if(f.Length > 0) {
	        		Log.WriteLog("Match Media Path files: ");
		        	foreach(string ff in f) {
		        		Log.WriteLog(ff);
		        	}
	        	}
	        	
	        	// When players do an incorrect logout from ESL Wire, This code will make it so
	        	// if they do 5 incorrect log outs in a row, and do 1 correct logout after that
	        	// then all 6 logs will be uploaded - this file keeps track of all the created logs
	   			// incase of an incorrect logout, plugin crash, or Wire crash.
	   			// {
		        	int i = 0;
		        	IniFile ini = new IniFile(System.IO.Path.GetTempPath() + matchId + ".txt");
		        	string str = ini.IniReadValue("misc", "logfilenum");
		        	
		        	if(str.Length > 0) {
		        		i = int.Parse(str)+1;
		        	} else {
		        		i = 1;
		        	}
		        	
		        	ini.IniWriteValue("misc", "logfilenum", i.ToString());
		        	
		        	Log.LogFileSecurityMove(ini, matchId);
		        	Log.LogFileSecurityUpdate(ini, matchId, g_szLogFilePath);
		        //}	
				
		        // Get the ESL Wire profile of the user, and print it in the log.
				Dictionary<string, object> d = new Dictionary<string, object>();
				Wire.SessionInterface si = Wire.InterfaceFactory.sessionInterface();
				
				d = si.user();
					
				string prof = "Unknown";
				foreach (KeyValuePair<string, object> pair in d) {
					if(pair.Key.Equals("profileURL")) {
						prof = (string)pair.Value;
					}
				}

	        	Log.WriteLog("    -> SA-MP AC Extension Plugin v" + g_dVersion.ToString() + " for use in ESL loaded.");
	        	Log.WriteLog("	-> Number of log files: " + i);
	        	Log.WriteLog("	-> ESL Profile: " + prof);
	        	Log.WriteLog("Match ID: " + matchId);
        	} catch(Exception e) {
        		Log.WriteLog(e.ToString());
        	}
        }
        public static void onMatchEnded(int matchId) {
        	
        	Log.WriteLog("Match ID: " + matchId + " has ended.", true);
        	
        	// Move our plugin log to the match media folder.
        	if(File.Exists(g_szLogFilePath)) {
        		string md5 = MD5file(g_szLogFilePath);
        		Wire.InterfaceFactory.gameInterface().moveToMatchMedia(g_szLogFilePath, "SAMP_AC_Extension Log file " + matchId + " - " + md5, matchId);
        	}
        	
        	// Check for old Wire logs from an incorrect logout, or crash.
        	IniFile ini = new IniFile(System.IO.Path.GetTempPath() + matchId + ".txt");
        	Log.LogFileSecurityMove(ini, matchId);
        	
        	// Reset our variables.
        	g_iMatchId = -1;
        	g_szMatchMediaPath = "";
        	g_szCachedFile = "";
        	// Get a new log file here, no file is created now, but a new file path is generated in case onMatchStarted is not called on the user. See comment above onMatchStarted.
        	g_szLogFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
        	g_szLogFileMD5 = "";
        }
        public static void checkGame(int gameId, string gamePath)
        {  
        	try {
        		      	
        		// If we didn't start SA-MP, return.
	        	if(gameId != 4039) return;
	        	
	        	// Initiatre our ProcessMemory objects for address checking
	        	ProcessMemory Mem = new ProcessMemory("gta_sa");
	        	ProcessMemory Mem2 = new ProcessMemory("samp");  
	
	        	// if GTASA and sa-mp browser are started then
	        	if(Mem != null && Mem2 != null) {
	        	
			       	do {		
			        		
			       		if(!Mem2.CheckProcess()) {
			       			// SA-MP browser is closed, end our checking.
			       			whenGameStopped(4039);
			       			return;
			       			
			       		}
			       		Thread.Sleep(5000);
			       		// Wait for GTA SA to be launched. (only sa-mp server browser is open at this time.)
	        		} while(!Mem.CheckProcess());
	        	}
	        	
	        	#if !debug
	        	try {
		        	if(File.Exists(g_szLogFilePath) && !File.Exists(g_szLogFilePath + "123")) {
			        	if(Cryptology.DecryptFile(g_szLogFilePath, g_szLogFilePath + "123", "password removed for public src release")) {
		        			if(File.Exists(g_szLogFilePath)) {
		        				File.Delete(g_szLogFilePath);
		        			}
			        	}
		        	}
	        	} catch(Exception) { }
	        	#endif
	        	
	        	string file = "";
	        	
	        	// Remove samp.exe from our path, so we can get the GTA directory, might cause problems if for some reason players have "samp.exe" in the path
	        	file = gamePath.Replace("samp.exe", ""); 
	        	
	        	g_szWireGamePath = file;
	        	  		
				string path = "";
				// check what the registry says about the GTA Directory.				
		        var path2 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\SAMP", "gta_sa_exe", "");
		        		
		        // NOTE: if path2 is null, then there are 3 things that could be true, either sa-mp isn't installed, the Registry is disabled, or a firewall blocked it
		        if(path2 != null) {
		        	path = path2.ToString();
		        	// check if the path is valid.
			        if(path.Length > 3) {
		        		// Remove "gta_sa.exe" from our file path.
			        	if(path.LastIndexOf("\\") > 0) {
			        		int index = path.LastIndexOf("\\");
			        		path = path.Substring(0, index + 1);
			        	}
		        		// the sa-mp.exe launch path doesn't match the one found in registry, the one in registry is the one actually used by the SA-MP browser, so ignore ESL Wire path.
			        	if(!path.Equals(file)) {
			        		file = path;
			        		Log.WriteLog("    -> Warning: game path from ESL Wire doesn't match SA-MP GTA Path from registry, checking registry path and ignoring ESL Wire path");
			        		Log.WriteLog(" ESL Wire Path: " + gamePath);
			        		Log.WriteLog(" Path in registry: " + file);
			        		Log.WriteLog(" ");
			        	}
			        }
		        }
		        
		        int bAddr = -1;
		        do {
		        
			        Process[] p = Process.GetProcessesByName("gta_sa");
			        int idx = 0;
			        
			        // hopefully there is only 1 gta_sa.exe started!
			        foreach(Process proc in p) {
			       		idx++;
			        	if(proc != null) {
			       			// weird restart loop incase getting process filename fails below. 
			       			// if it fails it will usually work the 2nd time, and if not the 2nd time, the 3rd.
			       			// if not the 3rd, then the 4th, if not the 4th, then etc..
			       			
			        		bool restart = false;
			        		do {
								restart = false;
				        		try {
				        			string s = Misc.getProcessPath(proc);
						        	if(File.Exists(s)) {
						        		
						        		path = s;
				        			} 
				        		} catch(Exception e) {
			        				Log.WriteLog(e.ToString());
				        			try {
					        			if(File.Exists(proc.MainModule.FileName)) {
							        		path = proc.MainModule.FileName;
					        			} 
				        			} catch(Exception ee) {
				        				// really now?
				        				Log.WriteLog(ee.ToString());
				        				restart = true;
				        				Thread.Sleep(500);
				        			}
			        			}
			        		} while(restart);
			        		
			        		// Get the directory path, remove gta_sa.exe.
				        	if(path.Length > 3) {
							   	if(path.LastIndexOf("\\") > 0) {
							   		int index = path.LastIndexOf("\\");
							   		path = path.Substring(0, index + 1);
							   	}
							} 
				        		
				        	g_bGTASAStarted = true;
				        	Log.WriteLog("gta_sa.exe launched from: " + path2);
							Log.WriteLog(" ");
										
							g_szGTASaPath = path;
								
							do {	
								restart = false;
								try {
									// get base address for samp.dll
									bAddr = Modules.GetModuleBaseAddress(proc, "samp.dll");
								} catch(Exception e) {
									restart = true;
									Log.WriteLog("Getting samp.dll offset error:");
									Log.WriteLog(e.ToString());
									Thread.Sleep(500);
								}
							}
							while(restart);		
				        }
			       		// it's ok, we've prepared for more than 1 gta.
			       		if(idx > 1) {
			       			proc.Kill();
			       			g_bGTASAStarted = false;
				        }
		        	 }
			        Thread.Sleep(500);
		        } while(!g_bGTASAStarted);
        		        		
	       		g_bGTASAStarted = true;
	        		
	        	g_iCleanGame = 0;
	        	
	        	// samp.dll + 
	        	/*
	        	 * 0x20D77D - ip
	        	 * 0x20D87E - port
	        	 * 0x20D97F - name
	        	 * 
	        	 * (it'd be better to just read command line, and would be compatable with all sa-mp versions then, these are 0.3e addresses.)
	        	 * */
	        	if(Mem.StartProcess()) {
	        		
	        		// get connected server IP and player name.
		        	string ip = Mem.ReadStringAscii(bAddr + 0x020D77D, 30);
		        	string port = Mem.ReadStringAscii(bAddr + 0x020D87E, 10);
		        	string name = Mem.ReadStringAscii(bAddr + 0x020D97F, 24);
		        	
		        	Log.WriteLog("Connected Server: " + ip + ":" + port + " as " + name);
		        	Log.WriteLog("Attempting to Query server...");
		        	
		        	// get time stamp to calculate our ping.
		        	DateTime p = DateTime.Now;
		        	bool restart = false;
		        	do {
		        		restart = false;
			        	try {
			        	
		        			// use sa-mp server query mechanism
			        		Query sQuery = new Query(ip, int.Parse(port));
		
							sQuery.Send('i');
							
							int count = sQuery.Recieve();
							
							string[] info = sQuery.Store(count);
							
							DateTime pp = DateTime.Now;
							
							TimeSpan ts = pp - p;
							
							Log.WriteLog("Successfully contacted server. (ping: " + ts.Milliseconds + ")");
							
							Log.WriteLog("Hostname: " + info[3]);
							Log.WriteLog("Gamemode: " + info[4]);
							Log.WriteLog("Players: " + info[1]);
							
							Log.WriteLog(" ");
							
							sQuery.Send('d');
							
							count = sQuery.Recieve();
							
							info = sQuery.Store(count);
							
							int i = 0;
							
							
							for(int j = 0; j < info.Length-2; ++j) {
								// still don't understand how this works, but it does!
								if(i == 0) {
									Log.WriteLog("PlayerID: " + info[j] + " || PlayerName: " + info[j+1]);
								}
								i++;
								if(i == 4) i = 0;
							}
							
							Log.WriteLog(" ");
							
							
			        	} catch(System.IndexOutOfRangeException) {
			        		Log.WriteLog("** Failed to get player list.");
			        	} catch(System.FormatException) {
			        		Log.WriteLog("Failed to contact SA-MP server");
			        		Log.WriteLog("** Game not initialized, retrying...");
			        		restart = true;
			        		Thread.Sleep(1000);
			        	} catch(Exception e) {
			        		Log.WriteLog("Failed to contact SA-MP server - " + ip + ":" + port + " as " + name);
			        		Log.WriteLog(e.ToString());
			        	}
		        	} while(restart);
	        	}
	        	
	        	// Check game integrity
	       		checkGameFiles( g_szGTASaPath );
	       		// check some memory addresses to make sure the file path for the data files hasn't been changed.
			 	Memory.VerifySomeMemoryStuff();
        

		        // Show results.
		        Log.WriteLog("Strange files in GTA SA Path: ");
		        Log.WriteLog(" ");
		        
		        // paste all files in GTA SA path that aren't part of the original game.
		        gtadir(g_szGTASaPath);
		        
		        Log.WriteLog(" ");
		        	
		     	if(g_iCleanGame == 0) {
		       		Log.WriteLog("VERDICT: Game is clean!");
		       	} else {
		         	if(g_iCleanGame > 1) {
		       			Log.WriteLog("VERDICT: Detected " + g_iCleanGame + " inconsistencies");
		         	} else {
		         		Log.WriteLog("VERDICT: Detected " + g_iCleanGame + " inconsistency");
		         	}
		       	}
		         Log.WriteLog(" ");
		        		
		        // check again in 15 minutes
				
		        aTimer.Enabled = true;
		        
		        //#if !debug
		        // encrypt our log file and delete the original .txt we where writing plain text too.
		        if(File.Exists(g_szLogFilePath + "123") && !File.Exists(g_szLogFilePath)) {
		        	
		        
			        g_szLogFileMD5 = MD5file(g_szLogFilePath + "123");
					if(Cryptology.EncryptFile(g_szLogFilePath + "123", g_szLogFilePath, "password removed for public src release")) {
			        	if(File.Exists(g_szLogFilePath + "123")) {
				        	File.Delete(g_szLogFilePath + "123"); 
			        	} 
					}
		        }
		        //#endif
		        
		        return;
        	} catch(Exception e) { Log.WriteLog(e.ToString()); }
        }
	
		private static void checkGameRepeat(object source, ElapsedEventArgs e)
	    {
			aTimer.Enabled = false;
			
			if(g_bGTASAStarted) {
				checkGame(4039, g_szWireGamePath);
			}
			
		}
		
        
        
        public static void whenGameStopped(int gameId)
        {       
        	try {
        		
	        	if(gameId == 4039) {
					g_bGTASAStarted = false;    
	        	}	
	
	        	Log.WriteLog( "    -> SA-MP Stopped.", true);
        	} catch(Exception e) {
        		Log.WriteLog(e.ToString());
        	}
        }
       
        
        
        public static bool checkGameFiles( string path ) {
        	int size = (Files.Length / 2);
        	bool[] res = new bool[size];
        	
        	string newfile; 
        	
        	// since for these files, we have multiple hashes for allowed files, they need to be
        	// in a seperate function.
        	WriteFileInfo(path + gtaintimg[0, 0], gtaintimg);
        	WriteFileInfo(path + gta_sa[0, 0], gta_sa);
        	WriteFileInfo(path + gta3img[0, 0], gta3img);
        	WriteFileInfo(path + animimg[0, 0], animimg);

        	// use Files.Length / 2 because the first parameter of the Files array represents the files at a given index.
        	// the Files[1, i] represents the hash of the file at that given idx. and Files.length will return the total length of both dimensions
        	for(int i=0; i < (Files.Length / 2); ++i) {
        		try {
        			
	        		newfile = path + Files[0, i];
	        		
	        		// make sure it exists before we check it's md5 to fix a crash.
	        		if(!File.Exists(newfile)) {
	        			Log.WriteLog("WARNING: " + newfile + " does not exist.");
	        			Log.WriteLog(" ");
	        			continue;
	        		}
	        		
	        		// check if the md5 matches the one in our array, if it does, the file is clean.
	        		if(MD5file(newfile) == Files[1, i]) {
	        			res[i] = true;        				
	        		} else res[i] = false;
	        		
	        		// keep the file open so it can't be modified (by people who don't know what they're doing XD) after the checks!
	        		new StreamReader(newfile);
	        		
	        		
	        		if(res[i] == false) {
		        		Log.WriteLog("File: " + Files[0, i]);
		        		Log.WriteLog( "is Default?: " + res[i]);
		        		Log.WriteLog("MD5: " + MD5file(newfile));
		        		Log.WriteLog("Should be: " + Files[1, i]);
		        		Log.WriteLog("File Size: " + new FileInfo(newfile).Length + " bytes");
		        		Log.WriteLog("Last Modified: " + File.GetLastWriteTimeUtc(newfile) + " GMT +0");
		        		Log.WriteLog(" ");
		        		g_iCleanGame++;
	        		}
	        		
       			} catch(Exception e) {
      				Log.WriteLog( e.ToString() );
       			}
       		}       	
        	return true;
        }
        public static string MD5file(string fileName)
		{
			String md5Result;
			StringBuilder sb = new StringBuilder();
			MD5 md5Hasher = MD5.Create();
			
			using (FileStream fs = File.OpenRead(fileName))
			{
			    foreach(Byte b in md5Hasher.ComputeHash(fs))
			        sb.Append(b.ToString("x2").ToLower());
			}
			
			md5Result = sb.ToString();
			return md5Result;
		} // Credits: http://stackoverflow.com/questions/827527/c-sharp-md5-hasher-example

        public static void gtadir(string sDir) {
        	
        	// print all files in GTA directory that aren't part of the original game
        	//int idx;
		    string str;
		    bool found = false;
		    
		    string[] ignoreFiles = {
		    	
		    	// but ignore these files anyway:
		    	@"anim\anim.img",
				@"audio\SFX\SCRIPT",
				@"audio\SFX\SPC_EA",
				@"audio\SFX\SPC_FA",
				@"audio\SFX\SPC_GA",
				@"audio\SFX\SPC_NA",
				@"audio\SFX\SPC_PA",
				@"audio\streams\AA",
				@"audio\streams\ADVERTS",
				@"audio\streams\AMBIENCE",
				@"audio\streams\BEATS",
				@"audio\streams\CH",
				@"audio\streams\CO",
				@"audio\streams\CR",
				@"audio\streams\CUTSCENE",
				@"audio\streams\DS",
				@"audio\streams\HC",
				@"audio\streams\MH",
				@"audio\streams\MR",
				@"audio\streams\NJ",
				@"audio\streams\RE",
				@"audio\streams\RG",
				@"audio\streams\TK",
				@"data\Icons\bin.ico",
				@"data\Icons\saicon.ICN",
				@"data\Icons\saicon2.ICN",
				@"data\Icons\saicon3.ICN",
				@"data\maps\vegas\vegasN.ipl",
				@"data\maps\vegas\vegasS.ipl",
				@"data\maps\vegas\vegasW.ipl",
				@"data\Paths\carrec.img",
				@"data\Paths\NODES0.DAT",
				@"data\Paths\NODES1.DAT",
				@"data\Paths\NODES10.DAT",
				@"data\Paths\NODES11.DAT",
				@"data\Paths\NODES12.DAT",
				@"data\Paths\NODES13.DAT",
				@"data\Paths\NODES14.DAT",
				@"data\Paths\NODES15.DAT",
				@"data\Paths\NODES16.DAT",
				@"data\Paths\NODES17.DAT",
				@"data\Paths\NODES18.DAT",
				@"data\Paths\NODES19.DAT",
				@"data\Paths\NODES2.DAT",
				@"data\Paths\NODES20.DAT",
				@"data\Paths\NODES21.DAT",
				@"data\Paths\NODES22.DAT",
				@"data\Paths\NODES23.DAT",
				@"data\Paths\NODES24.DAT",
				@"data\Paths\NODES25.DAT",
				@"data\Paths\NODES26.DAT",
				@"data\Paths\NODES27.DAT",
				@"data\Paths\NODES28.DAT",
				@"data\Paths\NODES29.DAT",
				@"data\Paths\NODES3.DAT",
				@"data\Paths\NODES30.DAT",
				@"data\Paths\NODES31.DAT",
				@"data\Paths\NODES32.DAT",
				@"data\Paths\NODES33.DAT",
				@"data\Paths\NODES34.DAT",
				@"data\Paths\NODES35.DAT",
				@"data\Paths\NODES36.DAT",
				@"data\Paths\NODES37.DAT",
				@"data\Paths\NODES38.DAT",
				@"data\Paths\NODES39.DAT",
				@"data\Paths\NODES4.DAT",
				@"data\Paths\NODES40.DAT",
				@"data\Paths\NODES41.DAT",
				@"data\Paths\NODES42.DAT",
				@"data\Paths\NODES43.DAT",
				@"data\Paths\NODES44.DAT",
				@"data\Paths\NODES45.DAT",
				@"data\Paths\NODES46.DAT",
				@"data\Paths\NODES47.DAT",
				@"data\Paths\NODES48.DAT",
				@"data\Paths\NODES49.DAT",
				@"data\Paths\NODES5.DAT",
				@"data\Paths\NODES50.DAT",
				@"data\Paths\NODES51.DAT",
				@"data\Paths\NODES52.DAT",
				@"data\Paths\NODES53.DAT",
				@"data\Paths\NODES54.DAT",
				@"data\Paths\NODES55.DAT",
				@"data\Paths\NODES56.DAT",
				@"data\Paths\NODES57.DAT",
				@"data\Paths\NODES58.DAT",
				@"data\Paths\NODES59.DAT",
				@"data\Paths\NODES6.DAT",
				@"data\Paths\NODES60.DAT",
				@"data\Paths\NODES61.DAT",
				@"data\Paths\NODES62.DAT",
				@"data\Paths\NODES63.DAT",
				@"data\Paths\NODES7.DAT",
				@"data\Paths\NODES8.DAT",
				@"data\Paths\NODES9.DAT",
				@"data\Paths\ROADBLOX.DAT",
				@"data\Paths\spath0.dat",
				@"data\Paths\tracks.dat",
				@"data\Paths\tracks2.dat",
				@"data\Paths\tracks3.dat",
				@"data\Paths\tracks4.dat",
				@"data\Paths\train.dat",
				@"data\Paths\train2.dat",
				@"data\script\main.scm",
				@"data\script\script.img",
				@"models\gta3.img",
				@"models\backup\gta3.img",
				@"models\generic\air_vlo.DFF",
				@"models\generic\arrow.DFF",
				@"models\generic\hoop.dff",
				@"models\generic\vehicle.txd",
				@"models\generic\wheels.DFF",
				@"models\generic\wheels.txd",
				@"models\generic\zonecylb.DFF",
				@"models\grass\grass3_2.dff",
				@"models\txd\intro1.txd",
				@"models\txd\intro2.txd",
				@"models\txd\INTRO3.TXD",
				@"models\txd\intro4.txd",
				@"models\txd\LD_BEAT.txd",
				@"models\txd\LD_BUM.txd",
				@"models\txd\LD_CARD.txd",
				@"models\txd\LD_CHAT.txd",
				@"models\txd\LD_DRV.txd",
				@"models\txd\LD_DUAL.txd",
				@"models\txd\ld_grav.txd",
				@"models\txd\LD_NONE.txd",
				@"models\txd\LD_OTB.txd",
				@"models\txd\LD_OTB2.txd",
				@"models\txd\LD_PLAN.txd",
				@"models\txd\LD_POKE.txd",
				@"models\txd\LD_POOL.txd",
				@"models\txd\LD_RACE.txd",
				@"models\txd\LD_RCE1.txd",
				@"models\txd\LD_RCE2.txd",
				@"models\txd\LD_RCE3.txd",
				@"models\txd\LD_RCE4.txd",
				@"models\txd\LD_RCE5.txd",
				@"models\txd\LD_ROUL.txd",
				@"models\txd\ld_shtr.txd",
				@"models\txd\LD_SLOT.txd",
				@"models\txd\LD_SPAC.txd",
				@"models\txd\LD_TATT.txd",
				@"models\txd\load0uk.txd",
				@"models\txd\loadsc0.txd",
				@"models\txd\loadsc1.txd",
				@"models\txd\loadsc10.txd",
				@"models\txd\loadsc11.txd",
				@"models\txd\loadsc12.txd",
				@"models\txd\loadsc13.txd",
				@"models\txd\loadsc14.txd",
				@"models\txd\loadsc2.txd",
				@"models\txd\loadsc3.txd",
				@"models\txd\loadsc4.txd",
				@"models\txd\loadsc5.txd",
				@"models\txd\loadsc6.txd",
				@"models\txd\loadsc7.txd",
				@"models\txd\loadsc8.txd",
				@"models\txd\loadsc9.txd",
				@"models\txd\LOADSCS.txd",
				@"models\txd\LOADSUK.txd",
				@"models\txd\outro.txd",
				@"models\txd\splash1.txd",
				@"models\txd\splash2.txd",
				@"models\txd\splash3.txd",
				@"movies\GTAtitles.mpg",
				@"movies\Logo.mpg",
				@"ReadMe\Readme.txt",
				@"text\spanish.gxt",
				@"models\gta_int.img",
				@"data\Icons\app.ico",
				@"text\french.gxt",
				@"text\german.gxt",
				@"text\italian.gxt"
		    };
		        
        	foreach (string d in Directory.GetDirectories(sDir)) // recursive through all sub directories in gTA SA folder
			{
				foreach (string f in Directory.GetFiles(d)) // all files in specific folder
				{
					// replace directories path with nothing so we only get the file NAME and not it's entire path
			       	str = f.Replace(g_szGTASaPath, "");
			       	
			       	// check if it exists from our Files array
			       	for(int j=0; j < (Files.Length / 2); ++j) {
			       		
			       		string tmp = Files[0, j]; 
			       		
			       		// remove GTA SA path
			       		tmp = tmp.Replace(g_szGTASaPath, "");
			       		tmp = tmp.Replace("/", @"\");
			       		
			       		//Log.WriteLog(tmp + " " + str);
			        		
			       		// check if it equals from our Files array
			       		if(str.Equals(tmp)) {
			       			found = true;
			       			break;
			       		}
			       	}
			       	
			       	// Do the same thing we did for Files array, but for the ignoredFiles array above 
			       	for(int j=0; j < ignoreFiles.Length; ++j) {
			       		string tmp = ignoreFiles[j]; 
			       		tmp = tmp.Replace(g_szGTASaPath, "");
			       		
			       		if(str.Equals(tmp)) {
			       			found = true;
			       			break;
			       		}
			       	}
			       	
		
			       	// ignore the file if it's in either the Files array, or the ignoredFiles array
			       	if(found == true) {
				        		
			       		found = false;
			       		continue;
			       	}

			       	// else write it to our log
			       	Log.WriteLog( str );
				}
				gtadir(d);
        	}
        
        }
        
        public static void WriteFileInfo(string file, string[,] arr) {
        	
        	if(!File.Exists(file)) {
        		Log.WriteLog("WARNING: " + file + " does not exist.");
        		return;
        	}
        	
        	string md5 = MD5file(file);
        	new StreamReader(file);
        	
        	bool res = false;
        	for(int i=0; i < (arr.Length / 2); ++i) {
        		try {	
	        		
        			if(md5.Equals(arr[1, i])) {
	        			res = true; 
						break;	        			
	        		} else { 
	        			res = false;
	        		}
       			} catch(Exception e) {
      				Log.WriteLog( e.ToString() );
       			}
       		} 

        	if(res == false) {
		    	Log.WriteLog("File: " + file);
		        Log.WriteLog( "is Default?: " + res);
		        Log.WriteLog("MD5: " + md5);
		        Log.WriteLog("File Size: " + new FileInfo(file).Length + " bytes");
		        Log.WriteLog("Last Modified: " + File.GetLastWriteTimeUtc(file) + " GMT +0");
		        Log.WriteLog(" ");
		        g_iCleanGame++;
	 
        	}
        	
        	return;
        }
	}
}