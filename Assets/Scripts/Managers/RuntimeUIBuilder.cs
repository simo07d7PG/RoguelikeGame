using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FinalRogue
{
    public static class RuntimeUIBuilder
    {
        const string CanvasName = "GameFlowCanvas";

        const float RefWidth = 1920f;
        const float RefHeight = 1080f;
        const int HudFontSize = 38;
        const int TitleFontSize = 56;
        const int BodyFontSize = 40;
        const int ButtonFontSize = 34;
        const int InputFontSize = 34;
        const int LeaderboardFontSize = 30;
        const float HudMargin = 48f;
        const float HudLineStep = 52f;
        static readonly Vector2 ButtonSize = new(420f, 76f);
        static readonly Vector2 InputSize = new(480f, 64f);

        public static bool BuildIfMissing()
        {
            if (GameObject.Find(CanvasName) != null)
                return false;

            EnsureEventSystem();

            var canvasObject = new GameObject(CanvasName);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(RefWidth, RefHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.referencePixelsPerUnit = 100f;

            canvasObject.AddComponent<GraphicRaycaster>();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            var hud = CreatePanel(canvasObject.transform, "HUD", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0f, 0f, 0f, 0f));
            CreateHudText(hud.transform, "HealthText", new Vector2(0f, 1f), new Vector2(HudMargin, -HudMargin), font, "HP -/-");
            CreateHudText(hud.transform, "CoinText", new Vector2(0f, 1f), new Vector2(HudMargin, -HudMargin - HudLineStep), font, "Coin 0");
            CreateHudText(hud.transform, "StageText", new Vector2(0f, 1f), new Vector2(HudMargin, -HudMargin - HudLineStep * 2f), font, "Stage 1");
            CreateHudText(hud.transform, "PlayTimeText", new Vector2(0.5f, 1f), new Vector2(0f, -HudMargin), font, "00:00", TextAnchor.UpperCenter);
            CreateHudText(hud.transform, "WeaponText", new Vector2(1f, 1f), new Vector2(-HudMargin, -HudMargin), font, "Weapon -", TextAnchor.UpperRight);
            CreateHudText(hud.transform, "AmmoText", new Vector2(1f, 1f), new Vector2(-HudMargin, -HudMargin - HudLineStep), font, "- / -", TextAnchor.UpperRight);

            var stageClear = CreateOverlayPanel(canvasObject.transform, "StageClear", "Stage Clear", "StageClearText", "StageClearContinueButton", "Continue", font);
            var shop = CreateShopPanel(canvasObject.transform, font);
            var gameOver = CreateGameOverPanel(canvasObject.transform, font);

            stageClear.SetActive(false);
            shop.SetActive(false);
            gameOver.SetActive(false);

            return true;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        static GameObject CreateShopPanel(Transform parent, Font font)
        {
            var panel = CreatePanel(parent, "Shop", new Vector2(0.15f, 0.1f), new Vector2(0.85f, 0.9f), Vector2.zero, Vector2.zero, new Color(0.1f, 0.1f, 0.15f, 0.95f));
            CreateCenteredText(panel.transform, "ShopTitle", font, "Shop", new Vector2(0f, 220f), TitleFontSize);
            CreateCenteredText(panel.transform, "ShopCoinText", font, "Coin 0", new Vector2(0f, 140f), BodyFontSize);
            CreateCenteredButton(panel.transform, "HealButton", "HealButtonText", font, "Heal", new Vector2(0f, 40f));
            CreateCenteredButton(panel.transform, "WeaponButton0", "WeaponButtonText0", font, "Weapon 1", new Vector2(0f, -50f));
            CreateCenteredButton(panel.transform, "WeaponButton1", "WeaponButtonText1", font, "Weapon 2", new Vector2(0f, -140f));
            CreateCenteredButton(panel.transform, "WeaponButton2", "WeaponButtonText2", font, "Weapon 3", new Vector2(0f, -230f));
            CreateCenteredButton(panel.transform, "ShopContinueButton", null, font, "Continue", new Vector2(0f, -330f));
            return panel;
        }

        static GameObject CreateGameOverPanel(Transform parent, Font font)
        {
            var panel = CreatePanel(parent, "GameOver", new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.92f), Vector2.zero, Vector2.zero, new Color(0.15f, 0.05f, 0.05f, 0.95f));
            CreateCenteredText(panel.transform, "GameOverText", font, "Game Over", new Vector2(0f, 260f), TitleFontSize);
            CreateInputField(panel.transform, "NicknameInput", font, new Vector2(0f, 140f));
            CreateCenteredButton(panel.transform, "RestartButton", null, font, "Restart", new Vector2(0f, -70f));

            Text leaderboard = CreateCenteredText(panel.transform, "LeaderboardText", font, "Leaderboard", new Vector2(0f, -280f), LeaderboardFontSize);
            leaderboard.alignment = TextAnchor.UpperCenter;
            leaderboard.rectTransform.sizeDelta = new Vector2(760f, 260f);
            return panel;
        }

        static GameObject CreateOverlayPanel(Transform parent, string panelName, string title, string bodyName, string buttonName, string buttonLabel, Font font)
        {
            var panel = CreatePanel(parent, panelName, new Vector2(0.2f, 0.22f), new Vector2(0.8f, 0.78f), Vector2.zero, Vector2.zero, new Color(0.08f, 0.12f, 0.2f, 0.95f));
            CreateCenteredText(panel.transform, $"{panelName}Title", font, title, new Vector2(0f, 150f), TitleFontSize);
            CreateCenteredText(panel.transform, bodyName, font, title, new Vector2(0f, 20f), BodyFontSize);
            CreateCenteredButton(panel.transform, buttonName, null, font, buttonLabel, new Vector2(0f, -140f));
            return panel;
        }

        static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var panelObject = new GameObject(name, typeof(RectTransform));
            panelObject.transform.SetParent(parent, false);
            panelObject.AddComponent<Image>().color = color;

            var rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return panelObject;
        }

        static Text CreateHudText(Transform parent, string name, Vector2 anchor, Vector2 position, Font font, string content, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = HudFontSize;
            text.color = Color.white;
            text.alignment = alignment;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(520f, 56f);
            return text;
        }

        static Text CreateCenteredText(Transform parent, string name, Font font, string content, Vector2 anchoredPosition, int fontSize)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(760f, 64f);
            return text;
        }

        static Button CreateCenteredButton(Transform parent, string name, string labelName, Font font, string label, Vector2 anchoredPosition)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.AddComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 1f);
            var button = buttonObject.AddComponent<Button>();

            var rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = ButtonSize;

            string textName = string.IsNullOrEmpty(labelName) ? $"{name}Label" : labelName;
            Text labelText = CreateCenteredText(buttonObject.transform, textName, font, label, Vector2.zero, ButtonFontSize);
            var textRect = labelText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        static InputField CreateInputField(Transform parent, string name, Font font, Vector2 anchoredPosition)
        {
            var inputObject = new GameObject(name, typeof(RectTransform));
            inputObject.transform.SetParent(parent, false);
            inputObject.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);
            var input = inputObject.AddComponent<InputField>();

            var rect = inputObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = InputSize;

            var text = CreateInputText(inputObject.transform, "Text", font, Color.white);
            var placeholder = CreateInputText(inputObject.transform, "Placeholder", font, new Color(1f, 1f, 1f, 0.35f));
            placeholder.text = "Nickname";

            input.textComponent = text;
            input.placeholder = placeholder;
            return input;
        }

        static Text CreateInputText(Transform parent, string name, Font font, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = InputFontSize;
            text.color = color;
            text.supportRichText = false;

            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(16f, 8f);
            textRect.offsetMax = new Vector2(-16f, -8f);
            return text;
        }
    }
}