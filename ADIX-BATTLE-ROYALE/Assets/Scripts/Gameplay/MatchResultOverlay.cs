using UnityEngine;
using UnityEngine.UI;
using AdixBattleRoyale.Combat;
using AdixBattleRoyale.Match;

namespace AdixBattleRoyale.Gameplay
{
    public sealed class MatchResultOverlay : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private BattleRoyaleMatchDirector matchDirector;
        private GameObject panel;
        private Text resultText;
        private bool shown;

        private void Start()
        {
            if (playerHealth != null) playerHealth.Eliminated += ShowDefeat;
            BuildOverlay();
        }

        private void Update()
        {
            if (!shown && matchDirector != null && matchDirector.MatchStarted && matchDirector.AliveBots == 0)
                ShowVictory();
        }

        private void BuildOverlay()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            panel = new GameObject("Match Result Overlay", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, 0.32f);
            rect.anchorMax = new Vector2(0.8f, 0.68f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.015f, 0.035f, 0.06f, 0.94f);

            GameObject label = new GameObject("Result", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(panel.transform, false);
            RectTransform textRect = label.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12f, 12f);
            textRect.offsetMax = new Vector2(-12f, -12f);
            resultText = label.GetComponent<Text>();
            resultText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            resultText.fontSize = 42;
            resultText.alignment = TextAnchor.MiddleCenter;
            resultText.color = Color.white;
            panel.SetActive(false);
        }

        public void ShowVictory() => Show("VICTORY", "सभी दुश्मन समाप्त!");
        public void ShowDefeat() => Show("ELIMINATED", "मैच समाप्त");

        private void Show(string title, string subtitle)
        {
            if (shown) return;
            shown = true;
            if (panel == null) return;
            panel.SetActive(true);
            if (resultText != null) resultText.text = title + "\n" + subtitle + "\n\nमैच रीस्टार्ट करने के लिए ऐप दोबारा खोलें";
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            if (playerHealth != null) playerHealth.Eliminated -= ShowDefeat;
        }
    }
}