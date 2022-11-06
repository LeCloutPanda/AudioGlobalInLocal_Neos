using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace AudioGlobalInLocal
{
    public class Patch : NeosMod
    {
        public override string Name => "Audio-Be-Local";
        public override string Author => "LeCloutPanda";
        public override string Version => "1.0.0";

        public static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> SPATIALIZE = new ModConfigurationKey<bool>("Spatialize?", "", () => false);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<AudioRolloffMode> ROLLOFF_MODE = new ModConfigurationKey<AudioRolloffMode>("Roll off mode", "", () => AudioRolloffMode.Linear);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<float> MAX_DISTANCE = new ModConfigurationKey<float>("Max distance of Audio Output", "", () => 2f);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<AudioDistanceSpace> DISTANCE_SPACE = new ModConfigurationKey<AudioDistanceSpace>("Distance Space", "", () => AudioDistanceSpace.Global);

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config.Save(true);

            Harmony harmony = new Harmony($"dev.{Author}.{Name}");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(UniversalImporter))]
        class PatchAudioImport
        {
            [HarmonyPostfix]
            [HarmonyPatch("DetectMultimedia")]
            public static void Fix(AudioPlayerOrb playerOrb)
            {
                if (!config.GetValue(ENABLED)) return;

                var output = playerOrb.Slot.GetComponent<AudioOutput>();

                output.Spatialize.Value = config.GetValue(SPATIALIZE);
                output.RolloffMode.Value = config.GetValue(ROLLOFF_MODE);
                output.MinDistance.Value = 0;
                output.MaxDistance.Value = config.GetValue(MAX_DISTANCE);
                output.DistanceSpace.Value = config.GetValue(DISTANCE_SPACE);
            }
        }
    }
}
