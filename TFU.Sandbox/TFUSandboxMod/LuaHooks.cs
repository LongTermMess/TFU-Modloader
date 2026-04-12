using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using Reloaded.Mod.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFUSandboxMod
{
    static internal unsafe class LuaHooks
    {
        static Memory memory;
        static ILogger logger;

        [Function(CallingConventions.Cdecl)]
        delegate void LuaRegister_Delegate(string category, string name, void* function, string description);
        static IHook<LuaRegister_Delegate> LuaRegister_Hook;

        [Function(CallingConventions.Stdcall)]
        delegate int LuaGetStringParam_Delegate(int param_1, int paramIdx, int param_3);
        static IHook<LuaGetStringParam_Delegate> LuaGetStringParam_Hook;

        [Function(CallingConventions.Stdcall)]
        delegate void Scripts_SendMsg_Delegate(int param_1);
        static IHook<Scripts_SendMsg_Delegate> Scripts_SendMsg_Hook;

        public static void Init(IReloadedHooks hooks, ILogger _logger)
        {
            //luaexec 0x4d2550
            //print? 0x628470

            memory = Memory.Instance;
            logger = _logger;
            logger.WriteLine("[Sandbox] Initialising Lua Hooks");

            LuaRegister_Hook = hooks.CreateHook<LuaRegister_Delegate>(LuaRegister, 0x4d2120).Activate();
            //LuaGetStringParam_Hook = hooks.CreateHook<LuaGetStringParam_Delegate>(LuaGetStringParam, 0x616ca0).Activate();
            Scripts_SendMsg_Hook = hooks.CreateHook<Scripts_SendMsg_Delegate>(Scripts_SendMsg, 0x4f46a0).Activate();


        }


        static void LuaRegister(string category, string name, void* function, string description)
        {
            //logger.WriteLine($"{category}.{name}, {(long)function:X8}");
            LuaRegister_Hook.OriginalFunction(category, name, function, description);
        }
        static int LuaGetStringParam(int param_1, int paramIdx, int param_3)
        {
            return LuaGetStringParam_Hook.OriginalFunction(param_1, paramIdx, param_3);
            nuint strAdd = (nuint)LuaGetStringParam_Hook.OriginalFunction(param_1, paramIdx, param_3);
            logger.WriteLine(ASCIIEncoding.ASCII.GetString(memory.ReadRaw(strAdd, 12)));
            return (int)strAdd;
        }
        static void Scripts_SendMsg(int param_1)
        {
            //logger.WriteLine($"luaGetString {LuaGetStringParam(param_1, 3, "")}");
            Scripts_SendMsg_Hook.OriginalFunction(param_1);
        }

    }
}
