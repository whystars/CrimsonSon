using AudioManagerAPI.Defaults;
using AudioManagerAPI.Features.Enums;
using CustomPlayerEffects;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Stores;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MapGeneration;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Random = UnityEngine.Random;

namespace CrimsonSon;

public class CrimsonSon : Plugin<CSConfig>
{
    public override string Name { get; } = "CrimsonSon";
    public override string Description { get; } = "一个实现了深红之子阵营的插件，内容丰富";
    public override string Author { get; } = "Crystal";
    public override Version Version { get; } = new(1, 1, 0);
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
    public override LoadPriority Priority { get; } = LoadPriority.Lowest;
    public override bool IsTransparent { get; } = false;

    public CSEventsHandler EventsHandler { get; } = new();
    public static CrimsonSon Instance { get; set; } = null!;

    public enum RoleID
    {
        SCP999B = 0,
        HolyFather = 1,
        Heathen = 2,
        Hallow = 3,
        Follower = 4,
        EvilFollower = 5,
        Fanatic = 6
    }

    private static readonly Dictionary<RoleID, List<ItemType>> DefaultInventory = new()
    {
        { RoleID.SCP999B, new List<ItemType> { ItemType.KeycardO5, ItemType.ArmorHeavy, ItemType.SCP500, ItemType.SCP500, ItemType.GrenadeHE } },
        { RoleID.HolyFather, new List<ItemType> { ItemType.KeycardO5, ItemType.GunFRMG0, ItemType.ArmorHeavy, ItemType.Medkit, ItemType.Medkit, ItemType.Adrenaline, ItemType.SCP1576, ItemType.GrenadeHE } },
        { RoleID.Heathen, new List<ItemType> { ItemType.KeycardO5, ItemType.GunRevolver, ItemType.ArmorHeavy, ItemType.Jailbird, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeHE, ItemType.GrenadeFlash } },
        { RoleID.Hallow, new List<ItemType> { ItemType.KeycardChaosInsurgency, ItemType.GunE11SR, ItemType.ArmorCombat, ItemType.Medkit, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeHE } },
        { RoleID.Follower, new List<ItemType> { ItemType.KeycardChaosInsurgency, ItemType.GunAK, ItemType.ArmorCombat, ItemType.Medkit, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeFlash } },
        { RoleID.EvilFollower, new List<ItemType> { ItemType.KeycardMTFOperative, ItemType.GunLogicer, ItemType.ArmorCombat, ItemType.Medkit, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeFlash } },
        { RoleID.Fanatic, new List<ItemType> { ItemType.KeycardMTFOperative, ItemType.GunCom45, ItemType.ArmorCombat, ItemType.Medkit, ItemType.Medkit, ItemType.Adrenaline, ItemType.AntiSCP207, ItemType.GrenadeFlash } }
    };

    public static readonly Dictionary<RoleID, CSRoleTrans> PreTranslations = new()
    {
        { RoleID.SCP999B, new CSRoleTrans { Name = "SCP-999-B", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你是SCP-999的复制体, 但你是知道你自己是深红之王的儿子, 相比于那个忘本仔, 你更坚信自己是对的。\n所以, 你保护好自己并前往广播室召唤深红之子!" } },
        { RoleID.HolyFather, new CSRoleTrans { Name = "深红之子 教皇", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>] 举行深红献祭仪式召唤深红之王!" } },
        { RoleID.Heathen, new CSRoleTrans { Name = "深红之子 异教徒", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。" } },
        { RoleID.Hallow, new CSRoleTrans { Name = "深红之子 圣徒", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。" } },
        { RoleID.Follower, new CSRoleTrans { Name = "深红之子 信徒", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。" } },
        { RoleID.EvilFollower, new CSRoleTrans { Name = "深红之子 恶信徒", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。" } },
        { RoleID.Fanatic, new CSRoleTrans { Name = "深红之子 狂信徒", Description = "你是\n [<b><color=red> {Name} </color></b>] \n你们是由蛇之手分裂出来的部队, 你们和他们不同, 你们信仰于你们的主! \n你的目标是保护教皇, 一起前往 [<color=#00FFFF> SCP-049 收容室 </color>] 或 [<color=yellow> SCP-106 的收容室 </color>], 确保仪式顺利举行。" } }
    };

    private static readonly Dictionary<RoleID, CSRoleConfig> DefaultRoleSettings = new()
    {
        { RoleID.SCP999B, new CSRoleConfig { MaxPlayers = 0, Weight = 0, Health = 2000f } },
        { RoleID.HolyFather, new CSRoleConfig { MaxPlayers = 1, Weight = 100, Health = 200f } },
        { RoleID.Heathen, new CSRoleConfig { MaxPlayers = 1, Weight = 25, Health = 150f } },
        { RoleID.Hallow, new CSRoleConfig { MaxPlayers = 2, Weight = 20, Health = 120f } },
        { RoleID.Follower, new CSRoleConfig { MaxPlayers = 5, Weight = 15, Health = 120f } },
        { RoleID.EvilFollower, new CSRoleConfig { MaxPlayers = 3, Weight = 10, Health = 120f } },
        { RoleID.Fanatic, new CSRoleConfig { MaxPlayers = 3, Weight = 10, Health = 120f } }
    };

    private const string SUMMON_KEY = "SUMMON";
    private const string EVENT_KEY = "EVENT";
    private const string CASSIE_KEY = "CASSIE";

    public CSConfig _config = new();
    public CSTranslation _trans = new();

    public bool WaveHasSpawned;
    public bool IsHoldingEvent;
    public bool InSCP049Chamber;
    public Dictionary<RoleID, CSRoleTrans> Translations = new();
    public string RoleList = string.Empty;
    public Dictionary<RoleID, CSRoleTemplate> roleTemplates = new();
    public readonly Dictionary<Player, PlayerHud> _playerHuds = new();

    private int _eventSessionId;
    private int _cassieSessionId;
    private int _summonSessionId;
    private int? _ritualOwnerPlayerId;

    public struct CSRoleTrans
    {
        public string Name;
        public string Description;
    }

    public class MemberData : CustomDataStore
    {
        public MemberData(Player owner) : base(owner) { }
        public bool IsMember { get; set; }
        public RoleID characterID { get; set; }
    }

    public struct CSRoleTemplate
    {
        public RoleID ID;
        public CSRoleTrans info;
        public List<ItemType> items;
        public float health;
        public int maxPlayers;
        public int weight;
    }

    public override void LoadConfigs()
    {
        base.LoadConfigs();
        Instance = this;

        if (!this.TryLoadConfig<CSConfig>("config.yml", out _config) || _config == null)
        {
            Logger.Warn($"{Name} 配置文件加载失败，将使用默认配置继续运行。");
            _config = new CSConfig();
        }

        if (!this.TryLoadConfig<CSTranslation>("translations.yml", out _trans) || _trans == null)
        {
            Logger.Warn($"{Name} 翻译文件加载失败，将使用默认文本继续运行。");
            _trans = new CSTranslation();
        }

        LoadTranslations();
        GenerateRoleList();
        OrganizeRoleTemplates();
    }

    public override void Enable()
    {
        Instance = this;
        RegisterAudio();
        CustomHandlersManager.RegisterEventsHandler(EventsHandler);
        Logger.Info($"{Name} 插件加载成功! v{Version} by {Author} - {Description}");
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(EventsHandler);
        CleanupRitualState(false);

        foreach (var hud in _playerHuds.Values.ToList())
        {
            hud.Dispose();
        }

        _playerHuds.Clear();
        DefaultAudioManager.Stop(_cassieSessionId);
        DefaultAudioManager.Stop(_summonSessionId);
    }

    public void EnsurePlayerHud(Player player)
    {
        if (player == null)
        {
            return;
        }

        if (_playerHuds.TryGetValue(player, out var existingHud))
        {
            existingHud.Initialize();
            return;
        }

        var playerHud = new PlayerHud(player, _config);
        playerHud.Initialize();
        _playerHuds[player] = playerHud;
    }

    public void RemovePlayerHud(Player player)
    {
        if (player == null)
        {
            return;
        }

        if (_playerHuds.TryGetValue(player, out var hud))
        {
            hud.Dispose();
            _playerHuds.Remove(player);
        }
    }

    public IEnumerator<float> CallOf999B(Player player)
    {
        while (Round.IsRoundInProgress)
        {
            if (!IsCurrent999B(player))
            {
                yield break;
            }

            if (player.Room != null && player.Room.Name == RoomName.EzIntercom)
            {
                SummonCSFaction();
                yield break;
            }

            yield return Timing.WaitForSeconds(1f);
        }
    }

    public IEnumerator<float> EventCD(Player player)
    {
        if (IsHoldingEvent || player == null || !IsValidHolyFather(player))
        {
            yield break;
        }

        IsHoldingEvent = true;
        _ritualOwnerPlayerId = player.PlayerId;
        _eventSessionId = DefaultAudioManager.Instance.PlayGlobalAudio(
            key: EVENT_KEY,
            loop: false,
            volume: Mathf.Clamp01(_config.EventAudioVolume),
            priority: AudioPriority.High,
            validPlayersFilter: target => target.GetDataStore<MemberData>().IsMember);

        string chamberName = InSCP049Chamber ? "SCP-049 的收容室" : "SCP-106 的收容室";
        ShowTemporaryMessageToAll(_trans.RitualStarted, 5f);

        int remainingSeconds = Math.Max(1, _config.RitualDurationSeconds);
        ShowCountdownToAll(chamberName, remainingSeconds);

        while (remainingSeconds > 0)
        {
            if (!IsValidHolyFather(player) || !Round.IsRoundInProgress)
            {
                InterruptRitual(_trans.RitualInterrupted);
                yield break;
            }

            yield return Timing.WaitForSeconds(1f);
            remainingSeconds--;

            if (remainingSeconds > 0)
            {
                UpdateCountdownToAll(chamberName, remainingSeconds);
            }
        }

        if (!IsValidHolyFather(player) || !Round.IsRoundInProgress)
        {
            InterruptRitual(_trans.RitualInterrupted);
            yield break;
        }

        CleanupRitualState(false);

        foreach (var ply in Player.List)
        {
            if (ply == null || !ply.IsAlive)
            {
                continue;
            }

            if (ply.GetDataStore<MemberData>().IsMember)
            {
                continue;
            }

            if (ply.Role == RoleTypeId.Tutorial)
            {
                continue;
            }

            ply.Kill(_trans.KingKillReason);
        }

        if (!Round.End(true))
        {
            Logger.Warn("深红献祭完成后尝试结束回合失败，可能由游戏原生结算接管。");
        }
    }

    public void SummonCSFaction()
    {
        if (!Round.IsRoundInProgress || WaveHasSpawned)
        {
            return;
        }

        WaveHasSpawned = true;
        var spectators = Player.List.Where(p => p != null && p.Role == RoleTypeId.Spectator).ToList();
        if (spectators.Count < Math.Max(1, _config.MinimumSpectatorsToSummon))
        {
            FailedShowToAll(_trans.NotEnoughSpectators);

            if (CanRetrySummon())
            {
                WaveHasSpawned = false;
                Logger.Info("深红之子召唤失败：观察者人数不足，但 SCP-999-B 仍可继续尝试。");
            }
            else
            {
                ShowTemporaryMessageToAll(_trans.SummonRetryUnavailable, 5f);
                Logger.Warn("深红之子召唤失败：观察者人数不足，且无法再次尝试。");
            }

            return;
        }

        SucceedShowToAll(_trans.SummonSuccess);
        BroadcastCSFaction();

        int toSpawn = Math.Min(spectators.Count, Math.Max(1, _config.MaximumSummonCount));
        var roleCounts = Enum.GetValues(typeof(RoleID))
            .Cast<RoleID>()
            .ToDictionary(roleId => roleId, _ => 0);
        var guaranteedRoles = new List<RoleID> { RoleID.HolyFather, RoleID.Heathen, RoleID.Hallow };
        var availablePlayers = spectators.OrderBy(_ => Guid.NewGuid()).ToList();

        foreach (var roleId in guaranteedRoles)
        {
            if (toSpawn <= 0)
            {
                break;
            }

            var nextPlayer = availablePlayers.FirstOrDefault();
            if (nextPlayer == null)
            {
                break;
            }

            SetCustomRoles(roleId, nextPlayer);
            roleCounts[roleId]++;
            availablePlayers.RemoveAt(0);
            toSpawn--;
        }

        int remainingSlots = Math.Min(availablePlayers.Count, toSpawn);
        for (int i = 0; i < remainingSlots; i++)
        {
            var chosenRole = SelectRoleByWeight(roleCounts);
            var target = availablePlayers.FirstOrDefault();
            if (target == null)
            {
                break;
            }

            SetCustomRoles(chosenRole, target);
            roleCounts[chosenRole]++;
            availablePlayers.RemoveAt(0);
        }

        Logger.Info("深红之子已被召唤!");
    }

    public void BroadcastCSFaction()
    {
        Announcer.Clear();
        Announcer.Message("", _trans.CassieBroadcast, false);

        DefaultAudioManager.Stop(_cassieSessionId);
        _cassieSessionId = DefaultAudioManager.Instance.PlayGlobalAudio(
            key: CASSIE_KEY,
            loop: false,
            volume: Mathf.Clamp01(_config.CassieAudioVolume),
            priority: AudioPriority.High,
            validPlayersFilter: _ => true);

        DefaultAudioManager.Stop(_summonSessionId);
        _summonSessionId = DefaultAudioManager.Instance.PlayAudio(
            key: SUMMON_KEY,
            position: new Vector3(10f, 0f, 10f),
            loop: false,
            volume: Mathf.Clamp01(_config.SummonAudioVolume),
            minDistance: 5f,
            maxDistance: 25f,
            isSpatial: true,
            priority: AudioPriority.High,
            validPlayersFilter: target => target.Role == RoleTypeId.Tutorial);
    }

    public void SetCustomRoles(RoleID id, Player player)
    {
        if (player == null)
        {
            Logger.Warn("尝试设置角色失败：玩家对象为 null。");
            return;
        }

        if (!Enum.IsDefined(typeof(RoleID), id))
        {
            Logger.Warn($"尝试设置角色失败：无效的角色 ID {id}。");
            return;
        }

        if (player.GetDataStore<MemberData>().IsMember)
        {
            ResetPlayerData(player);
        }

        EnsurePlayerHud(player);
        player.ClearItems();
        player.GetDataStore<MemberData>().IsMember = true;
        player.GetDataStore<MemberData>().characterID = id;

        if (id == RoleID.SCP999B)
        {
            Timing.RunCoroutine(CallOf999B(player));
        }

        player.SetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
        RoleInit(id, player);
    }

    public void RoleInit(RoleID id, Player player)
    {
        if (!roleTemplates.TryGetValue(id, out var template))
        {
            Logger.Warn($"未找到角色模板 {id}，初始化失败。");
            return;
        }

        player.MaxHealth = template.health;
        player.Health = template.health;

        foreach (var item in template.items)
        {
            player.AddItem(item);
        }

        player.EnableEffect<Scp207>(2, 0);
        player.EnableEffect<Scp1853>(1, 0);
        player.DisableEffect<Poisoned>();

        ShowHud(player);
        SetCustomInfo(player);
    }

    public void ShowHud(Player player)
    {
        if (player == null)
        {
            return;
        }

        player.InfoArea |= PlayerInfoArea.CustomInfo;
        player.InfoArea &= ~PlayerInfoArea.Role;

        if (TryGetPlayerHud(player, out var playerHud))
        {
            playerHud.ShowRoleDescription(Translations[player.GetDataStore<MemberData>().characterID].Description);
        }
    }

    public void HideHud(Player player)
    {
        if (player == null)
        {
            return;
        }

        player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        player.InfoArea |= PlayerInfoArea.Role;

        if (TryGetPlayerHud(player, out var playerHud))
        {
            playerHud.HideRoleDescription();
            playerHud.HideCountdown();
        }
    }

    public void ResetPlayerData(Player player)
    {
        if (player == null)
        {
            return;
        }

        var data = player.GetDataStore<MemberData>();
        bool wasMember = data.IsMember;
        RoleID previousRole = data.characterID;

        data.characterID = default;
        data.IsMember = false;
        HideHud(player);
        ClearCustomInfo(player);

        if (wasMember && previousRole == RoleID.HolyFather && IsHoldingEvent && _ritualOwnerPlayerId == player.PlayerId)
        {
            InterruptRitual(_trans.RitualInterrupted);
        }
    }

    public void SetCustomInfo(Player player)
    {
        if (player == null)
        {
            return;
        }

        if (player.GetDataStore<MemberData>().IsMember)
        {
            player.CustomInfo = Translations[player.GetDataStore<MemberData>().characterID].Name;
        }
    }

    public void ClearCustomInfo(Player player)
    {
        if (player != null)
        {
            player.CustomInfo = string.Empty;
        }
    }

    public void Choose999B()
    {
        if (!Round.IsRoundInProgress)
        {
            return;
        }

        var classDs = Player.List.Where(p => p != null && p.Role == RoleTypeId.ClassD).ToList();
        if (classDs.Count == 0)
        {
            Logger.Warn("没有找到可用于选取 SCP-999-B 的 D 级人员。");
            return;
        }

        var chosen = classDs[Random.Range(0, classDs.Count)];
        SetCustomRoles(RoleID.SCP999B, chosen);

        Vector3? targetPos = GetRoomSpawnPosition(_config.Spawn999BRoom);
        if (targetPos.HasValue)
        {
            Timing.CallDelayed(0.1f, () => chosen.Position = targetPos.Value);
        }
    }

    public void InterruptRitual(string message)
    {
        if (!IsHoldingEvent)
        {
            return;
        }

        CleanupRitualState();
        ShowTemporaryMessageToAll(message, 5f);
        Logger.Info("深红献祭仪式已被中断。");
    }

    public void CleanupForRoundEnd()
    {
        WaveHasSpawned = false;
        CleanupRitualState(false);
    }

    private void RegisterAudio()
    {
        DefaultAudioManager.RegisterAudio(SUMMON_KEY, () => Assembly.GetExecutingAssembly().GetManifestResourceStream("CrimsonSon.Audio.summon.wav"));
        DefaultAudioManager.RegisterAudio(EVENT_KEY, () => Assembly.GetExecutingAssembly().GetManifestResourceStream("CrimsonSon.Audio.event.wav"));
        DefaultAudioManager.RegisterAudio(CASSIE_KEY, () => Assembly.GetExecutingAssembly().GetManifestResourceStream("CrimsonSon.Audio.cassie.wav"));
    }

    private void LoadTranslations()
    {
        Translations = new Dictionary<RoleID, CSRoleTrans>();
        foreach (RoleID roleId in Enum.GetValues(typeof(RoleID)))
        {
            var defaultTranslation = PreTranslations[roleId];
            _trans.RoleTranslations.TryGetValue(roleId.ToString(), out var overrideTranslation);

            string resolvedName = string.IsNullOrWhiteSpace(overrideTranslation?.Name)
                ? defaultTranslation.Name
                : overrideTranslation.Name;
            string resolvedDescription = string.IsNullOrWhiteSpace(overrideTranslation?.Description)
                ? defaultTranslation.Description
                : overrideTranslation.Description;

            Translations[roleId] = new CSRoleTrans
            {
                Name = resolvedName,
                Description = resolvedDescription.Replace("{Name}", resolvedName)
            };
        }
    }

    private void GenerateRoleList()
    {
        RoleList = "\n<color=green>------------------------------</color>\n";
        foreach (RoleID roleId in Enum.GetValues(typeof(RoleID)))
        {
            RoleList += $"[<color=white>{(int)roleId}</color>]: <color=red>{Translations[roleId].Name}</color>\n";
        }

        RoleList += "<color=green>------------------------------</color>";
    }

    private void OrganizeRoleTemplates()
    {
        roleTemplates.Clear();
        foreach (RoleID roleId in Enum.GetValues(typeof(RoleID)))
        {
            var roleConfig = ResolveRoleConfig(roleId);
            roleTemplates[roleId] = new CSRoleTemplate
            {
                ID = roleId,
                info = Translations[roleId],
                items = new List<ItemType>(DefaultInventory[roleId]),
                health = roleConfig.Health ?? DefaultRoleSettings[roleId].Health ?? 100f,
                maxPlayers = roleConfig.MaxPlayers ?? DefaultRoleSettings[roleId].MaxPlayers ?? 0,
                weight = roleConfig.Weight ?? DefaultRoleSettings[roleId].Weight ?? 0
            };
        }
    }

    private CSRoleConfig ResolveRoleConfig(RoleID roleId)
    {
        var fallback = DefaultRoleSettings[roleId];
        if (_config.Roles == null || !_config.Roles.TryGetValue(roleId.ToString(), out var configuredRole) || configuredRole == null)
        {
            return fallback;
        }

        return new CSRoleConfig
        {
            MaxPlayers = configuredRole.MaxPlayers ?? fallback.MaxPlayers,
            Weight = configuredRole.Weight ?? fallback.Weight,
            Health = configuredRole.Health ?? fallback.Health
        };
    }

    private RoleID SelectRoleByWeight(Dictionary<RoleID, int> currentCounts)
    {
        int totalWeight = 0;
        foreach (var kvp in roleTemplates)
        {
            if (kvp.Key == RoleID.SCP999B)
            {
                continue;
            }

            if (currentCounts[kvp.Key] >= kvp.Value.maxPlayers)
            {
                continue;
            }

            totalWeight += kvp.Value.weight;
        }

        if (totalWeight <= 0)
        {
            return roleTemplates.First(kvp => kvp.Key != RoleID.SCP999B).Key;
        }

        int randomPoint = Random.Range(0, totalWeight);
        foreach (var kvp in roleTemplates)
        {
            if (kvp.Key == RoleID.SCP999B || currentCounts[kvp.Key] >= kvp.Value.maxPlayers)
            {
                continue;
            }

            randomPoint -= kvp.Value.weight;
            if (randomPoint < 0)
            {
                return kvp.Key;
            }
        }

        return RoleID.Follower;
    }

    private bool CanRetrySummon()
    {
        var current999B = FindCurrent999B();
        return current999B != null && current999B.Room != null && current999B.Room.Name == RoomName.EzIntercom && Round.IsRoundInProgress;
    }

    private Player FindCurrent999B()
    {
        return Player.List.FirstOrDefault(IsCurrent999B);
    }

    private bool IsCurrent999B(Player player)
    {
        if (player == null || !player.IsAlive)
        {
            return false;
        }

        var data = player.GetDataStore<MemberData>();
        return data.IsMember && data.characterID == RoleID.SCP999B && player.Role == RoleTypeId.Tutorial;
    }

    private bool IsValidHolyFather(Player player)
    {
        if (player == null || !player.IsAlive)
        {
            return false;
        }

        if (!Player.List.Any(p => p != null && p.PlayerId == player.PlayerId))
        {
            return false;
        }

        var data = player.GetDataStore<MemberData>();
        return data.IsMember && data.characterID == RoleID.HolyFather && player.Role == RoleTypeId.Tutorial;
    }

    private void CleanupRitualState(bool hideHud = true)
    {
        IsHoldingEvent = false;
        InSCP049Chamber = false;
        _ritualOwnerPlayerId = null;

        if (_eventSessionId != 0)
        {
            DefaultAudioManager.Stop(_eventSessionId);
            _eventSessionId = 0;
        }

        if (hideHud)
        {
            HideCountdownForAll();
            return;
        }

        HideCountdownForAll();
    }

    private Vector3? GetRoomSpawnPosition(RoomName roomName)
    {
        var firstRoom = Room.Get(roomName).FirstOrDefault();
        if (firstRoom == null)
        {
            Logger.Warn($"未获取到房间 {roomName} 的位置。");
            return null;
        }

        return firstRoom.Position + new Vector3(0f, 1f, 0f);
    }

    private void SucceedShowToAll(string message)
    {
        ShowTemporaryMessageToAll(message, 5f);
    }

    private void FailedShowToAll(string message)
    {
        ShowTemporaryMessageToAll(message, 5f);
    }

    private void ShowTemporaryMessageToAll(string message, float duration)
    {
        foreach (var player in Player.List)
        {
            if (player == null)
            {
                continue;
            }

            if (TryGetPlayerHud(player, out var playerHud))
            {
                playerHud.ShowNotification(message, duration);
            }
        }
    }

    private void ShowCountdownToAll(string roomName, int seconds)
    {
        foreach (var player in Player.List)
        {
            if (player == null)
            {
                continue;
            }

            if (TryGetPlayerHud(player, out var playerHud))
            {
                playerHud.ShowCountdown(FormatCountdown(roomName, seconds));
            }
        }
    }

    private void UpdateCountdownToAll(string roomName, int seconds)
    {
        foreach (var player in Player.List)
        {
            if (player == null)
            {
                continue;
            }

            if (TryGetPlayerHud(player, out var playerHud))
            {
                playerHud.UpdateCountdown(FormatCountdown(roomName, seconds));
            }
        }
    }

    private void HideCountdownForAll()
    {
        foreach (var player in Player.List)
        {
            if (player == null)
            {
                continue;
            }

            if (TryGetPlayerHud(player, out var playerHud))
            {
                playerHud.HideCountdown();
            }
        }
    }

    private string FormatCountdown(string roomName, int seconds)
    {
        return _trans.KingKillingCD
            .Replace("{Time}", seconds.ToString())
            .Replace("{Room}", roomName);
    }

    private bool TryGetPlayerHud(Player player, out PlayerHud playerHud)
    {
        playerHud = null;
        if (player == null)
        {
            return false;
        }

        if (!_playerHuds.TryGetValue(player, out playerHud) || playerHud == null)
        {
            EnsurePlayerHud(player);
            return _playerHuds.TryGetValue(player, out playerHud) && playerHud != null;
        }

        return true;
    }

    public class PlayerHud : IDisposable
    {
        private readonly Player _player;
        private readonly CSConfig _config;
        private Hint _roleDescriptionHint;
        private Hint _countdownHint;
        private Hint _notificationHint;
        private bool _initialized;

        public PlayerHud(Player player, CSConfig config)
        {
            _player = player;
            _config = config;
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            var display = PlayerDisplay.Get(_player);
            if (display == null)
            {
                Logger.Warn($"PlayerHud: 尝试为玩家 {_player.DisplayName} 初始化 HUD，但未找到 PlayerDisplay。");
                return;
            }

            _roleDescriptionHint = new Hint
            {
                Id = "CrimsonSonRoleDesc",
                Text = string.Empty,
                FontSize = _config.RoleDescriptionFontSize,
                XCoordinate = _config.RoleDescriptionX,
                YCoordinate = _config.RoleDescriptionY,
                Alignment = HintAlignment.Center,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Hide = true,
            };

            _countdownHint = new Hint
            {
                Id = "KingKillingCDHint",
                Text = string.Empty,
                FontSize = _config.RitualCountdownFontSize,
                XCoordinate = _config.RitualCountdownX,
                YCoordinate = _config.RitualCountdownY,
                Alignment = HintAlignment.Center,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Hide = true,
            };

            _notificationHint = new Hint
            {
                Id = "CrimsonSonNotificationHint",
                Text = string.Empty,
                FontSize = _config.NotificationFontSize,
                XCoordinate = _config.NotificationX,
                YCoordinate = _config.NotificationY,
                Alignment = HintAlignment.Center,
                YCoordinateAlign = HintVerticalAlign.Bottom,
                Hide = true,
            };

            display.AddHint(_roleDescriptionHint);
            display.AddHint(_countdownHint);
            display.AddHint(_notificationHint);
            _initialized = true;
        }

        public void ShowRoleDescription(string description)
        {
            if (_roleDescriptionHint == null)
            {
                return;
            }

            _roleDescriptionHint.Text = description;
            _roleDescriptionHint.Hide = false;
        }

        public void HideRoleDescription()
        {
            if (_roleDescriptionHint != null)
            {
                _roleDescriptionHint.Hide = true;
            }
        }

        public void ShowCountdown(string text)
        {
            if (_countdownHint == null)
            {
                return;
            }

            _countdownHint.Text = text;
            _countdownHint.Hide = false;
        }

        public void UpdateCountdown(string text)
        {
            if (_countdownHint == null)
            {
                return;
            }

            _countdownHint.Text = text;
        }

        public void HideCountdown()
        {
            if (_countdownHint != null)
            {
                _countdownHint.Hide = true;
            }
        }

        public void ShowNotification(string text, float duration)
        {
            if (_notificationHint == null)
            {
                return;
            }

            _notificationHint.Text = text;
            _notificationHint.Hide = false;
            _notificationHint.HideAfter(duration);
        }

        public void Dispose()
        {
            var display = PlayerDisplay.Get(_player);
            if (display != null)
            {
                display.RemoveHint(_roleDescriptionHint?.Id);
                display.RemoveHint(_countdownHint?.Id);
                display.RemoveHint(_notificationHint?.Id);
            }

            _roleDescriptionHint = null;
            _countdownHint = null;
            _notificationHint = null;
            _initialized = false;
        }
    }
}
