using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using PlayerRoles;
using System.Linq;
using UnityEngine;
using static CrimsonSon.CrimsonSon;
using Logger = LabApi.Features.Console.Logger;

namespace CrimsonSon;

public class CSEventsHandler : CustomEventsHandler
{
    public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        var player = ev.Player;
        if (player == null) return;

        Instance.EnsurePlayerHud(player);
        base.OnPlayerJoined(ev);
    }

    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        Player p = ev.Player;
        if (p == null) return;

        if (p.GetDataStore<MemberData>().IsMember && ev.NewRole.RoleTypeId == RoleTypeId.Tutorial)
        {
            Logger.Info($"{p.Nickname} 成为 {Instance.Translations[p.GetDataStore<MemberData>().characterID].Name}。");
            Vector3? targetPos = null;
            var firstRoom = Room.Get(Instance._config.SpawnRoom).FirstOrDefault();
            if (firstRoom != null)
            {
                targetPos = firstRoom.Position + new Vector3(0, 1, 0);
            }
            else
            {
                Logger.Warn($"未获取到房间 {Instance._config.SpawnRoom} 的位置。");
            }

            if (targetPos.HasValue)
            {
                Timing.CallDelayed(0.1f, () => p.Position = targetPos.Value);
            }
        }
        else if (p.GetDataStore<MemberData>().IsMember)
        {
            Instance.ResetPlayerData(p);
        }

        base.OnPlayerChangedRole(ev);
    }

    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        Player p = ev.Player;
        if (p == null) return;

        Instance.ResetPlayerData(p);
        Instance.RemovePlayerHud(p);
        base.OnPlayerLeft(ev);
    }

    public override void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        Player p = ev.Player;
        if (p == null) return;

        Instance.ResetPlayerData(p);
        base.OnPlayerDeath(ev);
    }

    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        Instance.CleanupForRoundEnd();
        foreach (var p in Player.List)
        {
            Instance.ResetPlayerData(p);
        }

        base.OnServerRoundEnded(ev);
    }

    public override void OnServerRoundStarted()
    {
        Instance.CleanupForRoundEnd();
        Instance.WaveHasSpawned = false;
        Timing.CallDelayed(0.1f, () => Instance.Choose999B());
        base.OnServerRoundStarted();
    }

    public override void OnPlayerUsingItem(PlayerUsingItemEventArgs ev)
    {
        if (ev.Player == null) return;

        var p = ev.Player;
        if (!Instance.IsHoldingEvent &&
            p.GetDataStore<MemberData>().IsMember &&
            p.GetDataStore<MemberData>().characterID == RoleID.HolyFather &&
            ev.UsableItem.Type == ItemType.SCP1576 &&
            p.Room != null &&
            (p.Room.Name == RoomName.Hcz049 || p.Room.Name == RoomName.Hcz106))
        {
            Instance.InSCP049Chamber = p.Room.Name == RoomName.Hcz049;
            Timing.RunCoroutine(Instance.EventCD(p));
            ev.IsAllowed = false;
            ev.Player.DropItem(ev.UsableItem);
        }

        base.OnPlayerUsingItem(ev);
    }

    public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
    {
        if (ev.Player == null) return;

        if (Instance.IsHoldingEvent && ev.Pickup.Type == ItemType.SCP1576)
        {
            ev.IsAllowed = false;
        }

        base.OnPlayerPickingUpItem(ev);
    }
}
