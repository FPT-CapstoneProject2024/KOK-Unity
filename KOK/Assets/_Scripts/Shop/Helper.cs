using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace KOK.Assets._Scripts
{
    public class Helper
    {
        public IEnumerator FadeTextToFullAlpha(float fadeDuration, TMP_Text notificationText)
        {
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                SetTextAlpha(alpha, notificationText);
                yield return null;
            }
        }

        public IEnumerator FadeTextToZeroAlpha(float fadeDuration, TMP_Text notificationText)
        {
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
                SetTextAlpha(alpha, notificationText);
                yield return null;
            }
        }

        public void SetTextAlpha(float alpha, TMP_Text notificationText)
        {
            Color color = notificationText.color;
            color.a = alpha;
            notificationText.color = color;
        }
    }
}
