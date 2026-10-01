using Unity.Netcode;
using UnityEngine;

public enum PlayerTeam
{
    None,
    TeamA,
    TeamB
}

public class PlayerTeamState : NetworkBehaviour
{
    public NetworkVariable<PlayerTeam> CurrentTeam =
        new NetworkVariable<PlayerTeam>(
            PlayerTeam.None,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log(
            $"Player {OwnerClientId} spawned. Team = {CurrentTeam.Value}"
        );
    }
}