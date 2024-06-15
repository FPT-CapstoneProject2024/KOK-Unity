//using Fusion;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Video;

//namespace YoutubePlayer.Samples.PlayVideo
//{
//    [RequireComponent(typeof(Button))]
//    public class PlayVideoButton : MonoBehaviour
//    {
//        //public VideoPlayer videoPlayer;

//        Button m_Button;

//        void Start()
//        {
//            m_Button = GetComponent<Button>();
//            RPCVideoPlayerDemo.videoPlayer = FindAnyObjectByType<VideoPlayer>();
//            //m_Button.interactable = RPCVideoPlayerDemo.videoPlayer.isPrepared;
//            m_Button.interactable = true;
//            RPCVideoPlayerDemo.videoPlayer.prepareCompleted += VideoPlayerOnPrepareCompleted;
//        }

//        void VideoPlayerOnPrepareCompleted(VideoPlayer source)
//        {
//            m_Button.interactable = RPCVideoPlayerDemo.videoPlayer.isPrepared;
//        }

//        public void Play()
//        {
//            RPCVideoPlayerDemo.Rpc_Play(FindAnyObjectByType<NetworkRunner>(), 1);
//        }

//        void OnDestroy()
//        {
//            RPCVideoPlayerDemo.videoPlayer.prepareCompleted -= VideoPlayerOnPrepareCompleted;
//        }
//    }
//}
