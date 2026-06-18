using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FinalRogue
{
    public static class LobbyUIBuilder
    {
        const string CanvasName = "LobbyCanvas";

        const float RefWidth = 1920f;
        const float RefHeight = 1080f;
        const int TitleFontSize = 56;
        const int ButtonFontSize = 34;
        const int InputFontSize = 34;
        const int LeaderboardFontSize = 30;
        const int LabelFontSize = 30;
        static readonly Vector2 ButtonSize = new(420f, 76f);
        static readonly Vector2 InputSize = new(480f, 64f);
        static readonly Vector2 DropdownSize = new(480f, 64f);
        static readonly Vector2 ToggleSize = new(480f, 56f);

        public static bool BuildIfMissing()
        {
            if (GameObject.Find(CanvasName) != null)
                return false;

            EnsureEventSystem();

            var canvasObject = new GameObject(CanvasName);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(RefWidth, RefHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var panel = CreatePanel(canvasObject.transform, "LobbyPanel", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.06f, 0.1f, 0.18f, 1f));

            CreateCenteredText(panel.transform, "LobbyTitleText", font, "Jelly Shoot", new Vector2(0f, 260f), TitleFontSize);
            CreateInputField(panel.transform, "LobbyNicknameInput", font, new Vector2(0f, 110f));
            CreateCenteredText(panel.transform, "ResolutionLabel", font, "Resolution", new Vector2(-250f, 20f), LabelFontSize);
            CreateResolutionDropdown(panel.transform, "ResolutionDropdown", font, new Vector2(60f, 20f));
            CreateFullscreenToggle(panel.transform, "FullscreenToggle", font, new Vector2(0f, -70f));
            CreateCenteredButton(panel.transform, "LobbyStartButton", null, font, "Start", new Vector2(0f, -170f));

            Text leaderboard = CreateCenteredText(panel.transform, "LobbyLeaderboardText", font, "Leaderboard", new Vector2(0f, -360f), LeaderboardFontSize);
            leaderboard.alignment = TextAnchor.UpperCenter;
            leaderboard.rectTransform.sizeDelta = new Vector2(760f, 280f);

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

        static Dropdown CreateResolutionDropdown(Transform parent, string name, Font font, Vector2 anchoredPosition)
        {
            var dropdownObject = new GameObject(name, typeof(RectTransform));
            dropdownObject.transform.SetParent(parent, false);
            dropdownObject.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);
            var dropdown = dropdownObject.AddComponent<Dropdown>();

            var rect = dropdownObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = DropdownSize;

            Text label = CreateInputText(dropdownObject.transform, "Label", font, Color.white);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-36f, -8f);
            label.alignment = TextAnchor.MiddleLeft;
            dropdown.captionText = label;

            var arrowObject = new GameObject("Arrow", typeof(RectTransform));
            arrowObject.transform.SetParent(dropdownObject.transform, false);
            var arrow = arrowObject.AddComponent<Text>();
            arrow.font = font;
            arrow.text = "v";
            arrow.alignment = TextAnchor.MiddleCenter;
            arrow.color = Color.white;
            arrow.fontSize = 24;
            var arrowRect = arrowObject.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1f, 0.5f);
            arrowRect.anchorMax = new Vector2(1f, 0.5f);
            arrowRect.pivot = new Vector2(1f, 0.5f);
            arrowRect.anchoredPosition = new Vector2(-12f, 0f);
            arrowRect.sizeDelta = new Vector2(24f, 24f);

            var template = CreateDropdownTemplate(dropdownObject.transform, font);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.itemText = template.GetComponentInChildren<Text>(true);

            return dropdown;
        }

        static GameObject CreateDropdownTemplate(Transform parent, Font font)
        {
            var templateObject = new GameObject("Template", typeof(RectTransform));
            templateObject.transform.SetParent(parent, false);
            templateObject.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f, 1f);
            templateObject.SetActive(false);

            var templateRect = templateObject.GetComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0f, 0f);
            templateRect.anchorMax = new Vector2(1f, 0f);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.anchoredPosition = new Vector2(0f, 2f);
            templateRect.sizeDelta = new Vector2(0f, 180f);

            var viewportObject = new GameObject("Viewport", typeof(RectTransform));
            viewportObject.transform.SetParent(templateObject.transform, false);
            viewportObject.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
            viewportObject.AddComponent<Mask>().showMaskGraphic = false;

            var viewportRect = viewportObject.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            var contentObject = new GameObject("Content", typeof(RectTransform));
            contentObject.transform.SetParent(viewportObject.transform, false);
            var contentRect = contentObject.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, 48f);

            var itemObject = new GameObject("Item", typeof(RectTransform));
            itemObject.transform.SetParent(contentObject.transform, false);
            itemObject.AddComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 0.35f);
            var toggle = itemObject.AddComponent<Toggle>();

            var itemRect = itemObject.GetComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0f, 0.5f);
            itemRect.anchorMax = new Vector2(1f, 0.5f);
            itemRect.sizeDelta = new Vector2(0f, 48f);

            var itemLabelObject = new GameObject("Item Label", typeof(RectTransform));
            itemLabelObject.transform.SetParent(itemObject.transform, false);
            var itemLabel = itemLabelObject.AddComponent<Text>();
            itemLabel.font = font;
            itemLabel.fontSize = 28;
            itemLabel.color = Color.white;
            itemLabel.alignment = TextAnchor.MiddleLeft;

            var itemLabelRect = itemLabelObject.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(16f, 0f);
            itemLabelRect.offsetMax = new Vector2(-16f, 0f);

            toggle.targetGraphic = itemObject.GetComponent<Image>();
            toggle.graphic = null;
            toggle.isOn = true;

            var scrollRect = templateObject.AddComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            return templateObject;
        }

        static Toggle CreateFullscreenToggle(Transform parent, string name, Font font, Vector2 anchoredPosition)
        {
            var rowObject = new GameObject($"{name}Row", typeof(RectTransform));
            rowObject.transform.SetParent(parent, false);

            var rowRect = rowObject.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = anchoredPosition;
            rowRect.sizeDelta = ToggleSize;

            var toggleObject = new GameObject(name, typeof(RectTransform));
            toggleObject.transform.SetParent(rowObject.transform, false);
            toggleObject.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);
            var toggle = toggleObject.AddComponent<Toggle>();

            var toggleRect = toggleObject.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0f, 0.5f);
            toggleRect.anchorMax = new Vector2(0f, 0.5f);
            toggleRect.pivot = new Vector2(0f, 0.5f);
            toggleRect.anchoredPosition = new Vector2(-220f, 0f);
            toggleRect.sizeDelta = new Vector2(40f, 40f);

            var checkObject = new GameObject("Checkmark", typeof(RectTransform));
            checkObject.transform.SetParent(toggleObject.transform, false);
            var checkImage = checkObject.AddComponent<Image>();
            checkImage.color = new Color(0.2f, 0.85f, 0.45f, 1f);

            var checkRect = checkObject.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = new Vector2(8f, 8f);
            checkRect.offsetMax = new Vector2(-8f, -8f);

            toggle.targetGraphic = toggleObject.GetComponent<Image>();
            toggle.graphic = checkImage;

            Text label = CreateCenteredText(rowObject.transform, "FullscreenLabel", font, "Fullscreen", new Vector2(30f, 0f), LabelFontSize);
            label.alignment = TextAnchor.MiddleLeft;
            label.rectTransform.sizeDelta = new Vector2(300f, 48f);

            return toggle;
        }
    }
}