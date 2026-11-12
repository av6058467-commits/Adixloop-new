using UnityEngine;
using UnityEngine.UI;

namespace AdixBattleRoyale.Emotes
{
    public sealed class EmoteWheelPresenter : MonoBehaviour
    {
        [SerializeField] private EmoteController target;
        [SerializeField] private Button[] slots = new Button[8];
        [SerializeField] private EmoteDefinition[] equippedEmotes = new EmoteDefinition[8];
        [SerializeField] private GameObject wheelRoot;

        private void Awake()
        {
            if (wheelRoot == null) wheelRoot = gameObject;
            if (target == null) target = FindObjectOfType<EmoteController>();

            for (int i = 0; i < slots.Length; i++)
            {
                int slotIndex = i;
                if (slots[i] != null)
                    slots[i].onClick.AddListener(() => SelectSlot(slotIndex));
            }
            Hide();
        }

        public void Show()
        {
            if (wheelRoot != null) wheelRoot.SetActive(true);
        }

        public void Hide()
        {
            if (wheelRoot != null) wheelRoot.SetActive(false);
        }

        public void Toggle()
        {
            if (wheelRoot != null) wheelRoot.SetActive(!wheelRoot.activeSelf);
        }

        public void SelectSlot(int index)
        {
            if (target == null || index < 0 || index >= equippedEmotes.Length) return;
            EmoteDefinition selected = equippedEmotes[index];
            if (selected != null && target.Play(selected)) Hide();
        }

        public void Equip(int slot, EmoteDefinition emote)
        {
            if (slot < 0 || slot >= equippedEmotes.Length) return;
            equippedEmotes[slot] = emote;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] != null) slots[i].onClick.RemoveAllListeners();
        }
    }
}
