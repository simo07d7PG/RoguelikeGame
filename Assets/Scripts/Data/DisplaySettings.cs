using UnityEngine;

namespace FinalRogue
{
    public static class DisplaySettings
    {
        const string PrefFullscreen = "finalrogue_display_fullscreen";
        const string PrefWidth = "finalrogue_display_width";
        const string PrefHeight = "finalrogue_display_height";

        public readonly struct ResolutionOption
        {
            public int Width { get; }
            public int Height { get; }
            public string Label { get; }

            public ResolutionOption(int width, int height, string label = null)
            {
                Width = width;
                Height = height;
                Label = string.IsNullOrEmpty(label) ? $"{width} x {height}" : label;
            }
        }

        static readonly ResolutionOption[] Presets =
        {
            new(3840, 2160),
            new(2560, 1440),
            new(1920, 1080),
            new(1600, 900),
            new(1280, 720),
            new(1280, 800)
        };

        public static bool Fullscreen { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public static ResolutionOption[] GetResolutionOptions()
        {
            Resolution native = Screen.currentResolution;
            var options = new ResolutionOption[Presets.Length + 1];
            options[0] = new ResolutionOption(native.width, native.height, $"Native ({native.width} x {native.height})");

            for (int i = 0; i < Presets.Length; i++)
                options[i + 1] = Presets[i];

            return options;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ApplyOnStartup() => Apply();

        public static void Load()
        {
            Resolution fallback = Screen.currentResolution;
            Fullscreen = PlayerPrefs.GetInt(PrefFullscreen, Screen.fullScreen ? 1 : 0) == 1;
            Width = PlayerPrefs.GetInt(PrefWidth, fallback.width);
            Height = PlayerPrefs.GetInt(PrefHeight, fallback.height);
        }

        public static void Save()
        {
            PlayerPrefs.SetInt(PrefFullscreen, Fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(PrefWidth, Width);
            PlayerPrefs.SetInt(PrefHeight, Height);
            PlayerPrefs.Save();
        }

        public static void Apply()
        {
            Load();
            Screen.SetResolution(
                Width,
                Height,
                Fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }

        public static void SetFullscreen(bool enabled)
        {
            Fullscreen = enabled;
            Save();
            Apply();
        }

        public static void SetResolution(int width, int height)
        {
            Width = Mathf.Max(640, width);
            Height = Mathf.Max(480, height);
            Save();
            Apply();
        }

        public static int GetSelectedResolutionIndex()
        {
            ResolutionOption[] options = GetResolutionOptions();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Width == Width && options[i].Height == Height)
                    return i;
            }

            return 0;
        }

        public static void SetResolutionByIndex(int index)
        {
            ResolutionOption[] options = GetResolutionOptions();
            if (options.Length == 0)
                return;

            index = Mathf.Clamp(index, 0, options.Length - 1);
            SetResolution(options[index].Width, options[index].Height);
        }
    }
}