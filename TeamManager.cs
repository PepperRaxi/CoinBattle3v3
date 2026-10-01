using Unity.Netcode;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    private int teamACount = 0;
    private int teamBCount = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
            return;

        Debug.Log("TeamManager started on Server.");

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        base.OnNetworkDespawn();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer)
            return;

        Debug.Log($"Client connected: {clientId}");

        AssignClientTeam(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer)
            return;

        Debug.Log($"Client disconnected: {clientId}");
    }

    private void AssignClientTeam(ulong clientId)
    {
        if (!NetworkManager.ConnectedClients.TryGetValue(
                clientId,
                out NetworkClient client))
        {
            Debug.LogWarning(
                $"Could not find NetworkClient for {clientId}"
            );

            return;
        }

        if (client.PlayerObject == null)
        {
            Debug.LogWarning(
                $"PlayerObject for client {clientId} is not ready yet."
            );

            return;
        }

        AssignPlayerTeam(client.PlayerObject);
    }

    public PlayerTeam AssignTeam()
    {
        if (!IsServer)
            return PlayerTeam.None;

        if (teamACount <= teamBCount)
        {
            teamACount++;
            return PlayerTeam.TeamA;
        }
        else
        {
            teamBCount++;
            return PlayerTeam.TeamB;
        }
    }

    public void AssignPlayerTeam(NetworkObject playerObject)
    {
        if (!IsServer)
            return;

        PlayerTeamState teamState =
            playerObject.GetComponent<PlayerTeamState>();

        if (teamState == null)
        {
            Debug.LogError(
                "PlayerTeamState not found on Player!"
            );

            return;
        }

        PlayerTeam assignedTeam = AssignTeam();

        teamState.CurrentTeam.Value = assignedTeam;

        Debug.Log(
            $"Player {playerObject.OwnerClientId} assigned to {assignedTeam}"
        );
    }

    public void RemoveTeam(PlayerTeam team)
    {
        if (!IsServer)
            return;

        if (team == PlayerTeam.TeamA)
        {
            teamACount = Mathf.Max(0, teamACount - 1);
        }
        else if (team == PlayerTeam.TeamB)
        {
            teamBCount = Mathf.Max(0, teamBCount - 1);
        }
    }

    public int GetTeamACount()
    {
        return teamACount;
    }

    public int GetTeamBCount()
    {
        return teamBCount;
    }
}