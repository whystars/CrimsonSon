using MapGeneration;
using System.Collections.Generic;
using System.ComponentModel;

namespace CrimsonSon;

public class CSConfig
{
    [Description("召唤深红之子所需的最少观察者人数。")]
    public int MinimumSpectatorsToSummon { get; set; } = 3;

    [Description("单次最多召唤多少名深红之子成员。")]
    public int MaximumSummonCount { get; set; } = 15;

    [Description("是否允许旁观状态的 dummy NPC 计入深红之子召唤候选。发布到正式服时可关闭。")]
    public bool CountDummySpectatorsForSummon { get; set; } = true;

    [Description("深红献祭仪式持续时间（秒）。")]
    public int RitualDurationSeconds { get; set; } = 120;

    [Description("召唤成功时入场音效音量。")]
    public float SummonAudioVolume { get; set; } = 0.8f;

    [Description("仪式开始时的全局音效音量。")]
    public float EventAudioVolume { get; set; } = 1.0f;

    [Description("CASSIE 配套音效音量。")]
    public float CassieAudioVolume { get; set; } = 0.8f;

    [Description("深红之子召唤时 CASSIE message 自动发送多少个用空格分隔的英文句号。实测约 1 个句号延长 1 秒显示时间，不会追加到中文字幕后面。")]
    public int CassieBroadcastHoldSeconds { get; set; } = 12;

    [Description("深红之子第二波出生房间。")]
    public RoomName SpawnRoom { get; set; } = RoomName.Outside;

    [Description("SCP-999-B 开局出生房间。")]
    public RoomName Spawn999BRoom { get; set; } = RoomName.LczGlassroom;

    [Description("角色介绍 HUD 的 X 坐标。")]
    public float RoleDescriptionX { get; set; } = 0f;

    [Description("角色介绍 HUD 的 Y 坐标。")]
    public float RoleDescriptionY { get; set; } = 1000f;

    [Description("角色介绍 HUD 的字体大小。")]
    public int RoleDescriptionFontSize { get; set; } = 20;

    [Description("倒计时 HUD 的 X 坐标。")]
    public float RitualCountdownX { get; set; } = -800f;

    [Description("倒计时 HUD 的 Y 坐标。")]
    public float RitualCountdownY { get; set; } = 200f;

    [Description("倒计时 HUD 的字体大小。")]
    public int RitualCountdownFontSize { get; set; } = 25;

    [Description("全局提示 HUD 的 X 坐标。")]
    public float NotificationX { get; set; } = -800f;

    [Description("全局提示 HUD 的 Y 坐标。")]
    public float NotificationY { get; set; } = 300f;

    [Description("全局提示 HUD 的字体大小。")]
    public int NotificationFontSize { get; set; } = 25;

    [Description("各深红角色的人数上限、权重和血量配置。键名使用 RoleID 枚举名。")]
    public Dictionary<string, CSRoleConfig> Roles { get; set; } = new()
    {
        ["SCP999B"] = new CSRoleConfig { MaxPlayers = 0, Weight = 0, Health = 2000f },
        ["HolyFather"] = new CSRoleConfig { MaxPlayers = 1, Weight = 100, Health = 200f },
        ["Heathen"] = new CSRoleConfig { MaxPlayers = 1, Weight = 25, Health = 150f },
        ["Hallow"] = new CSRoleConfig { MaxPlayers = 2, Weight = 20, Health = 120f },
        ["Follower"] = new CSRoleConfig { MaxPlayers = 5, Weight = 15, Health = 120f },
        ["EvilFollower"] = new CSRoleConfig { MaxPlayers = 3, Weight = 10, Health = 120f },
        ["Fanatic"] = new CSRoleConfig { MaxPlayers = 3, Weight = 10, Health = 120f },
    };
}

public class CSRoleConfig
{
    [Description("该角色最大人数。")]
    public int? MaxPlayers { get; set; }

    [Description("该角色权重。SCP999B 不参与第二波分配。")]
    public int? Weight { get; set; }

    [Description("该角色初始血量。")]
    public float? Health { get; set; }
}
