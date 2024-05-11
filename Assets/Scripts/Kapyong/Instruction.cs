using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kapyong
{
    [Serializable]
    public class Instruction
    {
        [Serializable]
        public class Feature
        {
            public RectTransform dot;
            public string definition;
        }

        [SerializeField] private Feature[] features = default;
        [SerializeField] private TextMeshProUGUI instructionText = default;
        [SerializeField] private Button continueButton = default;
        [SerializeField] private Button skipButton = default;
        [SerializeField] private RectTransform mainDot = default;

        [SerializeField] private Animation instructionTweener = default;
        [SerializeField] private Animation dotTweener = default;

        private Action LoadNextScene;

        private int Index;

        private const string instructionTweenerClip = "InstructionSideIn";
        private const string dotTweenerClip = "UIImageFadeIn";

        public void Initialize(Action LoadNextScene)
        {
            this.LoadNextScene = LoadNextScene;

            AssingInputEvents();
            PrepareFeaturPanel();
        }

        private void AssingInputEvents()
        {
            continueButton.onClick.AddListener(OnContinue);
            skipButton.onClick.AddListener(OnLoadNextScene);
        }

        private void OnContinue()
        {
            if(Index >= features.Length - 1) OnLoadNextScene();
            else
            {
                Index++;

                dotTweener.Play(dotTweenerClip);
                instructionTweener.Play(instructionTweenerClip);

                mainDot.position = features[Index].dot.position;
                instructionText.text = features[Index].definition;
            }
        }

        private void PrepareFeaturPanel()
        {
            const int startIndex = 0;

            if (features.Length <= startIndex) return;

            mainDot.position = features[startIndex].dot.position;
            instructionText.text = features[startIndex].definition;
        }

        private void OnLoadNextScene() => LoadNextScene?.Invoke();
    }
}