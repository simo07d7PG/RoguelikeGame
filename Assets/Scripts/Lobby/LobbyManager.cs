using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FinalRogue
{
    [DefaultExecutionOrder(-300)]
    [AddComponentMenu("FinalRogue/Lobby Manager")]
    [DisallowMultipleComponent]
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] string gameSceneName = EntitySetupUtility.DefaultGameSceneName;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnsureInLobbyScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != EntitySetupUtility.DefaultLobbySceneName)
                return;

            if (EntitySetupUtility.FindFirst<LobbyManager>() != null)
                return;

            var managerObject = new GameObject("LobbyManager");
            managerObject.AddComponent<LobbyManager>();
        }

        InputField nicknameInput;
        Dropdown resolutionDropdown;
        Toggle fullscreenToggle;
        Button startButton;
        Text leaderboardText;

        void Awake()
        {
            DisplaySettings.Apply();
            SaveManager.EnsureExists();
            LobbyUIBuilder.BuildIfMissing();
            WireUi();
            RefreshUi();
        }

        void WireUi()
        {
            Canvas canvas = EntitySetupUtility.FindFirst<Canvas>();
            if (canvas == null)
                return;

            Transform root = canvas.transform;
            nicknameInput = UIHierarchyUtility.FindComponent<InputField>(root, "LobbyNicknameInput");
            resolutionDropdown = UIHierarchyUtility.FindComponent<Dropdown>(root, "ResolutionDropdown");
            fullscreenToggle = UIHierarchyUtility.FindComponent<Toggle>(root, "FullscreenToggle");
            startButton = UIHierarchyUtility.FindComponent<Button>(root, "LobbyStartButton");
            leaderboardText = UIHierarchyUtility.FindComponent<Text>(root, "LobbyLeaderboardText");

            if (resolutionDropdown != null)
            {
                resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            }

            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartClicked);
                startButton.onClick.AddListener(OnStartClicked);
            }
        }

        void RefreshUi()
        {
            if (nicknameInput != null)
                nicknameInput.text = LobbySessionData.LoadNickname();

            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                var options = new System.Collections.Generic.List<string>();
                foreach (var option in DisplaySettings.GetResolutionOptions())
                    options.Add(option.Label);

                resolutionDropdown.AddOptions(options);
                resolutionDropdown.SetValueWithoutNotify(DisplaySettings.GetSelectedResolutionIndex());
                resolutionDropdown.RefreshShownValue();
            }

            if (fullscreenToggle != null)
                fullscreenToggle.SetIsOnWithoutNotify(DisplaySettings.Fullscreen);

            if (leaderboardText != null && SaveManager.Instance != null)
                leaderboardText.text = GameFormatUtility.BuildLeaderboardText(SaveManager.Instance);
        }

        void OnResolutionChanged(int index) => DisplaySettings.SetResolutionByIndex(index);

        void OnFullscreenChanged(bool enabled) => DisplaySettings.SetFullscreen(enabled);

        void OnStartClicked()
        {
            LobbySessionData.SaveNickname(nicknameInput != null ? nicknameInput.text : string.Empty);
            SceneManager.LoadScene(gameSceneName);
        }
    }
}