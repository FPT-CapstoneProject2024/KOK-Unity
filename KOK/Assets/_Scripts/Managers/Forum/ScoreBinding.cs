using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostRating;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace KOK
{
    public class ScoreBinding : MonoBehaviour
    {
        [SerializeField] Gradient gradient;
        [SerializeField] Image sliderFill;

        [SerializeField] Slider scoreGiveSlider;
        [SerializeField] TMP_Text scoreGiveLabel;
        [SerializeField] int defaultScore;
        private PostBinding postBinding;

        public void Init(int ownScore, PostBinding postBinding)
        {
            if (ownScore <= 0) {
                ownScore = defaultScore;
            }

            scoreGiveSlider.value = ownScore;

            this.postBinding = postBinding;
            OnSliderValueChange();
        }
        public void OnSliderValueChange()
        {
            scoreGiveLabel.text = scoreGiveSlider.value.ToString();
            sliderFill.color = gradient.Evaluate(scoreGiveSlider.value/scoreGiveSlider.maxValue);
            scoreGiveLabel.color = sliderFill.color;
        }


        public void GiveScoreSubmit()
        {
            if (scoreGiveLabel.text.IsNullOrEmpty())
            {
                return;
            }
            int scoreGive = Int32.Parse(scoreGiveLabel.text);
            Debug.Log(scoreGive);   
            ApiHelper.Instance.GetComponent<PostRatingController>()
                .CreatePostRatingCoroutine(    
                    new()
                    {
                        PostId = (Guid)postBinding.post.PostId,
                        MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                        Score = scoreGive                        
                    },
                    (postRating) => {
                        postBinding.forumNewFeedManager.RefreshCurrentPost();
                        gameObject.SetActive(false);
                    },
                    (ex) => { }
                );


        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
