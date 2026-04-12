using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using Reloaded.Mod.Interfaces.Structs;
using TFUSandboxMod.Configuration;
using TFUSandboxMod.Template;

namespace TFUSandboxMod
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public unsafe class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;
        Memory memory;




        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;
            memory = Memory.Instance;


            Hooks.Init(_hooks, _logger);
            LuaHooks.Init(_hooks, _logger);

            //bodge patch for prologue check, fix later
            if (_configuration.PrologueUnlock) { Memory.Instance.SafeWrite(0xfb318e, new byte[] { 0 }); }

            _logger.WriteLine("[Sandbox] Mod Started");

            _modLoader.ModLoading += ModLoaded;
        }


        public void ModLoaded(IModV1 mod, IModConfigV1 config)
        {
            _logger.WriteLine($"ModLoading {config.ModName}");
            string modDir = _modLoader.GetDirectoryForModId(config.ModId) + "/Sandbox/";
            if (Directory.Exists(modDir))
            {
                _logger.WriteLine("Valid mod " + config.ModName);
                foreach (string f in Directory.GetFiles(modDir, "*.*", SearchOption.AllDirectories))
                {
                    string file = f.Replace('\\', '/');
                    string fileInternal = file.Substring(modDir.Length);
                    _logger.WriteLine(fileInternal);
                    if(!FileDictionary.ContainsKey(fileInternal))
                    {
                        FileDictionary.Add(fileInternal, file);
                    }
                    else
                    {
                        FileDictionary[fileInternal] = file;
                    }
                }
            }
        }


        public static Dictionary<string, string> FileDictionary = new Dictionary<string, string>();


        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}