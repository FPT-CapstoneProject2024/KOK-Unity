using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;

namespace KOK
{
    public class FusionController : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static FusionController Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            FusionManager.Instance.OnConnectedToServer(runner);
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            FusionManager.Instance?.OnConnectFailed(runner, remoteAddress, reason);
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            FusionManager.Instance?.OnConnectRequest(runner, request, token);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            FusionManager.Instance?.OnCustomAuthenticationResponse(runner, data);
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            FusionManager.Instance?.OnDisconnectedFromServer(runner, reason);
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            FusionManager.Instance?.OnHostMigration(runner, hostMigrationToken);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            FusionManager.Instance?.OnInput(runner, input);

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            FusionManager.Instance?.OnInputMissing(runner, player, input);
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            FusionManager.Instance?.OnObjectEnterAOI(runner, obj, player);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            FusionManager.Instance?.OnObjectExitAOI(runner, obj, player);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            FusionManager.Instance?.OnPlayerJoined(runner, player);

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            FusionManager.Instance?.OnPlayerLeft(runner, player);
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            FusionManager.Instance?.OnReliableDataProgress(runner, player, key, progress);
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            FusionManager.Instance?.OnReliableDataReceived(runner, player, key, data);
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            FusionManager.Instance?.OnSceneLoadDone(runner);
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            FusionManager.Instance?.OnSceneLoadStart(runner);
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            FusionManager.Instance?.OnSessionListUpdated(runner, sessionList);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            FusionManager.Instance?.OnShutdown(runner, shutdownReason);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            FusionManager.Instance?.OnUserSimulationMessage(runner, message);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }



    }
}
