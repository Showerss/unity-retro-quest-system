using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Retro.QuestSystem
{
    [DisallowMultipleComponent]
    public sealed class QuestHUDRow : MonoBehaviour
    {
        [Tooltip("Checkbox text, separate from the objective description.")]
        [SerializeField] private TextMeshProUGUI _checkbox;
        [Tooltip("Objective description and optional progress count.")]
        [SerializeField] private TextMeshProUGUI _description;
        [Tooltip("Ink Image parented to the description, outside the row layout group.")]
        [SerializeField] private Image _ink;

        private Color _activeColor;
        private Color _completedColor;
        private Color _checkboxColor;
        private string _completionText;
        private float _elapsed;
        private float _drawDuration;
        private float _holdDuration;
        private bool _settled;

        public QuestObjective Objective { get; private set; }
        public bool IsCelebrating { get; private set; }
        public bool IsConfigured => _checkbox != null && _description != null && _ink != null;

        public void Bind(QuestObjective objective, Color activeColor, Color completedColor, Color checkboxColor)
        {
            if (!ReferenceEquals(Objective, objective)) CancelCompletion();
            Objective = objective;
            _activeColor = activeColor;
            _completedColor = completedColor;
            _checkboxColor = checkboxColor;
        }

        public void ShowCurrentState()
        {
            CancelCompletion();
            SetText(FormatObjective(), Objective.isCompleted);
        }

        public void CaptureCompletion(float drawDuration, float holdDuration)
        {
            _completionText = FormatObjective();
            _drawDuration = Mathf.Max(0f, drawDuration);
            _holdDuration = Mathf.Max(0f, holdDuration);
            _elapsed = 0f;
            _settled = false;
            IsCelebrating = true;
            SetText(_completionText, false);
            _ink.color = _completedColor;
            _ink.raycastTarget = false;
            _ink.gameObject.SetActive(true);
            _ink.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            AdvanceCompletion(0f);
        }

        public bool AdvanceCompletion(float deltaTime)
        {
            if (!IsCelebrating) return false;
            _elapsed += Mathf.Max(0f, deltaTime);
            if (_elapsed < _drawDuration)
            {
                UpdateInk(_elapsed / _drawDuration);
                return false;
            }

            if (!_settled)
            {
                _settled = true;
                _ink.gameObject.SetActive(false);
                SetText(_completionText, true);
            }
            if (_elapsed < _drawDuration + _holdDuration) return false;

            IsCelebrating = false;
            return true;
        }

        public void CancelCompletion()
        {
            IsCelebrating = false;
            if (_ink != null) _ink.gameObject.SetActive(false);
        }

        private string FormatObjective()
        {
            string suffix = Objective.targetAmount > 1 ? $" ({Objective.currentAmount}/{Objective.targetAmount})" : "";
            return Objective.description + suffix;
        }

        private void SetText(string text, bool completed)
        {
            _checkbox.text = completed ? "[X]" : "[ ]";
            _checkbox.color = completed ? _completedColor : _checkboxColor;
            _description.text = completed ? $"<s>{text}</s>" : text;
            _description.color = completed ? _completedColor : _activeColor;
        }

        private void UpdateInk(float progress)
        {
            // Draw a short stroke on the first rendered line; TMP supplies the full, wrapping strike at rest.
            _description.ForceMeshUpdate();
            var info = _description.textInfo;
            if (info.characterCount == 0 || info.lineCount == 0) return;
            var line = info.lineInfo[0];
            var first = info.characterInfo[line.firstVisibleCharacterIndex];
            var last = info.characterInfo[line.lastVisibleCharacterIndex];
            var rect = _ink.rectTransform;
            rect.anchorMin = rect.anchorMax = _description.rectTransform.pivot;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(first.origin, first.baseLine + first.pointSize * 0.25f);
            rect.sizeDelta = new Vector2(Mathf.Max(0f, last.xAdvance - first.origin), 1.5f);
            rect.localScale = new Vector3(progress, 1f, 1f);
        }
    }
}
