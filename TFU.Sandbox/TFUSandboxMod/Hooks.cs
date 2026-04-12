using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using static TFUSandboxMod.Mod;

namespace TFUSandboxMod
{
    internal unsafe static class Hooks
    {
        static Memory memory;
        static ILogger logger;
        
        [Function(CallingConventions.Cdecl)]
        delegate uint stricrc32_Delegate(string str);
        static stricrc32_Delegate stricrc32;

        [Function(CallingConventions.Cdecl)]
        delegate int AddDiskAssetToCatalog_Delegate(string filename);
        static AddDiskAssetToCatalog_Delegate AddDiskAssetToCatalog;

        [Function(CallingConventions.Cdecl)]
        delegate int LoadAsset_Delegate(uint param_1, bool param_2);
        static LoadAsset_Delegate LoadAsset;

        [Function(CallingConventions.Cdecl)]
        delegate uint GetAsset_Delegate(uint filenamCRC, uint assettypeCRC, bool param_3);
        static GetAsset_Delegate GetAsset;

        [Function(CallingConventions.Cdecl)]
        delegate uint CacheAsset_Delegate(string param_1);
        static CacheAsset_Delegate CacheAsset;


        [Function(CallingConventions.Cdecl)]
        delegate void AssetCatalogInit_Delegate(int* param_1, char* param_2, char* param_3, bool SandboxMode, bool param_5);
        static IHook<AssetCatalogInit_Delegate> AssetCatalogInit_Hook;
        [Function(CallingConventions.Cdecl)]
        delegate void AssetPathToSandboxPath_Delegate(char* param_1, char* param_2, uint param_3, bool param_4);
        static IHook<AssetPathToSandboxPath_Delegate> AssetPathToSandboxPath_Hook;

        public static void Init(IReloadedHooks hooks, ILogger _logger)
        {
            memory = Memory.Instance;
            logger = _logger;
            logger.WriteLine("[Sandbox] Initialising Game Hooks");

            nuint stricrc32_Address = memory.Read<nuint>(0xf348dc);
            nuint GetSandboxMode_Address = memory.Read<nuint>(0xF34B08);
            nuint TryLoadAssetFromSandbox_Address = memory.Read<nuint>(0xf348f4);
            nuint AssetPathToSandboxPath_Address = memory.Read<nuint>(0xf34948);
            nuint AddDiskAssetToCatalog_Address = memory.Read<nuint>(0xf349ec);
            nuint AssetCatalogInit_Address = memory.Read<nuint>(0xf347f8);
            nuint CacheAsset_Address = memory.Read<nuint>(0xf34ba8);

            nuint LoadAsset_Address = memory.Read<nuint>(0xf34ce4);
            nuint GetAsset_Address = memory.Read<nuint>(0xf34914);

            AssetCatalogInit_Hook = hooks.CreateHook<AssetCatalogInit_Delegate>(AssetCatalogInit, (long)AssetCatalogInit_Address).Activate();
            AssetPathToSandboxPath_Hook = hooks.CreateHook<AssetPathToSandboxPath_Delegate>(AssetPathToSandboxPath, (long)AssetPathToSandboxPath_Address).Activate();
            stricrc32 = hooks.CreateWrapper<stricrc32_Delegate>((long)stricrc32_Address, out _);
            CacheAsset = hooks.CreateWrapper<CacheAsset_Delegate>((long)CacheAsset_Address, out _);

            LoadAsset = hooks.CreateWrapper<LoadAsset_Delegate>((long)LoadAsset_Address, out _);
            GetAsset = hooks.CreateWrapper<GetAsset_Delegate>((long)GetAsset_Address, out _);


            AddDiskAssetToCatalog = hooks.CreateWrapper<AddDiskAssetToCatalog_Delegate>((long)AddDiskAssetToCatalog_Address, out _);

            //Patching out a check for if tabbed out, experimental fix for loads failing after a tab out
            //memory.SafeWrite(0x6a3d9f, new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
            //memory.SafeWrite(0x6a3ece, new byte[] { 0x90, 0x90 });
        }

        static void AssetPathToSandboxPath(char* param_1, char* param_2, uint param_3, bool param_4)
        {
            AssetPathToSandboxPath_Hook.OriginalFunction(param_1, param_2, param_3, param_4);

            string file = ReadInternalString(param_1);
            //logger.WriteLine($"{file}");

            if(FileDictionary.TryGetValue(file, out string filePath))
            {
                memory.WriteRaw((nuint)param_2, ASCIIEncoding.ASCII.GetBytes(filePath));
                logger.WriteLine($"Redirected file {file} to {filePath}");
            }

        }


        static void AssetCatalogInit(int* param_1, char* param_2, char* param_3, bool SandboxMode, bool param_5)
        {
            AssetCatalogInit_Hook.OriginalFunction(param_1, param_2, param_3, true, param_5);
        }




        //THANK YOU X360 DEMO FOR HAVING DEV FILES HOLY SHIT YOU MAKE MY JOB SO MUCH EASIER
        //girl who could generate a new crc dict in like 30 minutes probs (5 if just using files the game already loads)
        enum AssetTypes : uint
        {
            ASSET_TYPE_SCALED_PHYSICS_CACHE = 0x4029fb66,
            ASSET_TYPE_IBL = 0x33a4dfed,
            ASSET_TYPE_CINEMATIC = 0x6305e8db,
            ASSET_TYPE_CINE_BLOCK = 0x68f24c2e,
            ASSET_TYPE_CINE_EDL = 0xc435d348,
            ASSET_TYPE_UNICODETEXT = 0xe8c4ebcd,
            ASSET_TYPE_SHADERVARIATIONFILE_P = 0x8b92d467,
            ASSET_TYPE_SHADERVARIATIONFILE_X = 0x85495c55,
            ASSET_TYPE_SHADERVARIATIONFILE_G = 0x084151a0,
            ASSET_TYPE_SHADERVARIATIONFILE_D = 0x9148001a,
            ASSET_TYPE_LIGHTNINGVFX_DEFINITION = 0xee97d748,
            ASSET_TYPE_VISUALFX_POOL = 0x4c263d7b,
            ASSET_TYPE_SHADER = 0x134eb2e4,
            ASSET_TYPE_SCHEMA = 0x54336d9e,
            ASSET_TYPE_SHADERNETWORK = 0xc72f88fb,
            ASSET_TYPE_CAMERA = 0xd7a1c2c9,
            ASSET_TYPE_JOINT_EXPRESSIONS = 0x124004cb,
            ASSET_TYPE_XML = 0xa9d80a65,
            ASSET_TYPE_WAV = 0xf353fb2e,
            ASSET_TYPE_EUPHORIA_BIN_PS3 = 0x65ea58b0,
            ASSET_TYPE_EUPHORIA_BIN_X360 = 0x1eea18da,
            ASSET_TYPE_EUPHORIA_BIN_PC = 0x4736662f,
            ASSET_TYPE_SCRIPT = 0xf03cabf6,
            ASSET_TYPE_INPUTMAP = 0x790a7bd4,
            ASSET_TYPE_MATERIAL = 0xc61a7b1c,
            ASSET_TYPE_ANARKFONT = 0x61db1906,
            ASSET_TYPE_ANARKSCENE = 0xb11a7943,
            ASSET_TYPE_FONT = 0xd0034e70,
            ASSET_TYPE_TEXTURE = 0xf934f180,
            ASSET_TYPE_ANIMATION = 0xf0e0d413,
            ASSET_TYPE_PHYSICS = 0xa3631b11,
            ASSET_TYPE_GTO_MODEL = 0x1d9e31ba,
            ASSET_TYPE_MODEL = 0x0621e81e
        }


        public static string ReadInternalString(char* str)
        {
            nuint stringAddress = (nuint)str;
            List<byte> strList = new List<byte>();
            byte character = Memory.Instance.Read<byte>(stringAddress);
            int i = 0;
            while (character != 0)
            {
                i++;
                strList.Add(character);
                character = Memory.Instance.Read<byte>(stringAddress + (nuint)i);
            }
            return ASCIIEncoding.ASCII.GetString(strList.ToArray());
        }

    }
}
