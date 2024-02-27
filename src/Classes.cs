using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace SharpTimer
{
    // Cache for map ents
    public class EntityCache
    {
        public List<CBaseTrigger> Triggers { get; private set; }
        public List<CInfoTeleportDestination> InfoTeleportDestinations { get; private set; }
        public List<CTriggerPush> TriggerPushEntities { get; private set; }
        public List<CPointEntity> InfoTargetEntities { get; private set; }

        public EntityCache()
        {
            Triggers = new List<CBaseTrigger>();
            InfoTeleportDestinations = new List<CInfoTeleportDestination>();
            TriggerPushEntities = new List<CTriggerPush>();
            InfoTargetEntities = new List<CPointEntity>();
            UpdateCache();
        }

        public void UpdateCache()
        {
            Triggers = Utilities.FindAllEntitiesByDesignerName<CBaseTrigger>("trigger_multiple").ToList();
            InfoTeleportDestinations = Utilities.FindAllEntitiesByDesignerName<CInfoTeleportDestination>("info_teleport_destination").ToList();
            TriggerPushEntities = Utilities.FindAllEntitiesByDesignerName<CTriggerPush>("trigger_push").ToList();
        }
    }

    // MapData JSON
    public class MapInfo
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapStartTrigger { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapStartC1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapStartC2 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapEndTrigger { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapEndC1 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapEndC2 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RespawnPos { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverrideDisableTelehop { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverrideMaxSpeedLimit { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverrideStageRequirement { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverrideTriggerPushFix { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GlobalPointsMultiplier { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapTier { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MapType { get; set; }
    }

    public class PlayerTimerInfo
    {
        //timer
        public bool IsTimerRunning { get; set; }
        public bool IsTimerBlocked { get; set; }
        public int TimerTicks { get; set; }
        public bool IsBonusTimerRunning { get; set; }
        public int BonusTimerTicks { get; set; }
        public int BonusStage { get; set; }

        //replay
        public bool IsReplaying { get; set; }
        public bool IsRecordingReplay { get; set; }

        //hud
        public string? ReplayHUDString { get; set; }
        public string? RankHUDIcon { get; set; }
        public string? CachedRank { get; set; }
        public bool IsRankPbCached { get; set; }
        public bool IsSpecTargetCached { get; set; }
        public string? PreSpeed { get; set; }
        public string? CachedPB { get; set; }
        public string? CachedMapPlacement { get; set; }

        //logic
        public int? TicksInAir { get; set; }
        public int TicksOnBhopBlock { get; set; }
        public int CheckpointIndex { get; set; }
        public Dictionary<int, int>? StageTimes { get; set; }
        public Dictionary<int, string>? StageVelos { get; set; }
        public int CurrentMapStage { get; set; }
        public int CurrentMapCheckpoint { get; set; }
        public CCSPlayer_MovementServices? MovementService { get; set; }

        //player settings/stats
        public bool Azerty { get; set; }
        public bool HideTimerHud { get; set; }
        public bool HideKeys { get; set; }
        public bool SoundsEnabled { get; set; }
        public bool HideJumpStats { get; set; }
        public int PlayerFov { get; set; }
        public int TimesConnected { get; set; }
        public int TicksSinceLastCmd { get; set; }
        public int TicksSinceLastRankUpdate { get; set; }

        //super special stuff for testers
        public bool IsTester { get; set; }
        public string? TesterSparkleGif { get; set; }
        public string? TesterPausedGif { get; set; }

        //vip stuff 
        public bool IsVip { get; set; }
        public string? VipReplayGif { get; set; }
        public string? VipPausedGif { get; set; }

        //admin stuff
        public bool IsNoclipEnabled { get; set; }
        public bool IsAddingStartZone { get; set; }
        public string? StartZoneC1 { get; set; }
        public string? StartZoneC2 { get; set; }
        public bool IsAddingEndZone { get; set; }
        public string? EndZoneC1 { get; set; }
        public string? EndZoneC2 { get; set; }
        public string? RespawnPos { get; set; }
        public Dictionary<int, CBeam>? ZoneToolWire { get; set; }

        //set respawn
        public string? SetRespawnPos { get; set; }
        public string? SetRespawnAng { get; set; }
    }

    public class PlayerJumpStats
    {
        public int FramesOnGround { get; set; }
        public int LastFramesOnGround { get; set; }
        public bool OnGround { get; set; }
        public bool LastOnGround { get; set; }
        public string? LastPos { get; set; }
        public string? LastSpeed { get; set; }
        public string? LastEyeAngle { get; set; }
        public string? JumpPos { get; set; }
        public string? OldJumpPos { get; set; }
        public string? JumpSpeed { get; set; }
        public bool Jumped { get; set; }
        public string? LastJumpType { get; set; }
        public bool LastDucked { get; set; }
        public bool LandedFromSound { get; set; }
        public bool LastLandedFromSound { get; set; }
        public int WTicks { get; set; }
        public List<JumpFrames> jumpFrames { get; set; } = new List<JumpFrames>();

        public class JumpFrames
        {
            public string PositionString { get; set; }
            public string RotationString { get; set; }
            public string SpeedString { get; set; }
            public double MaxSpeed { get; set; }
            public double MaxHeight { get; set; }
            public bool LastLeft { get; set; }
            public bool LastRight { get; set; }
            public bool LastLeftRight { get; set; }
        }

        public List<JumpInterp> jumpInterp { get; set; } = new List<JumpInterp>();
        public class JumpInterp
        {
            public string InterpString { get; set; }
        }
    }

    //Replay stuff
    public class PlayerReplays
    {
        public int CurrentPlaybackFrame { get; set; }
        public int BonusX { get; set; }
        public List<ReplayFrames> replayFrames { get; set; } = new List<ReplayFrames>();

        public class ReplayFrames
        {
            public string PositionString { get; set; }
            public string RotationString { get; set; }
            public string SpeedString { get; set; }
            public PlayerButtons? Buttons { get; set; }
            public uint Flags { get; set; }
            public MoveType_t MoveType { get; set; }
        }
    }

    public class IndexedReplayFrames
    {
        public int Index { get; set; }
        public PlayerReplays.ReplayFrames Frame { get; set; }
    }

    // PlayerRecords for JSON
    public class PlayerRecord
    {
        public string? PlayerName { get; set; }
        public int TimerTicks { get; set; }
    }

    // PlayerPoints for MySql
    public class PlayerPoints
    {
        public string? PlayerName { get; set; }
        public int GlobalPoints { get; set; }
    }

    // KZ checkpoints
    public class PlayerCheckpoint
    {
        public string? PositionString { get; set; }
        public string? RotationString { get; set; }
        public string? SpeedString { get; set; }
    }

    // Stage times and velos
    public class PlayerStageData
    {
        public Dictionary<int, int>? StageTimes { get; set; }
        public Dictionary<int, string>? StageVelos { get; set; }
    }

    // Trigger push
    public class TriggerPushData
    {
        public float PushSpeed { get; set; }
        public QAngle PushEntitySpace { get; set; }
        public Vector PushDirEntitySpace { get; set; }
        public Vector PushMins { get; set; }
        public Vector PushMaxs { get; set; }
        public TriggerPushData(float pushSpeed, QAngle pushEntitySpace, Vector pushDirEntitySpace, Vector pushMins, Vector pushMaxs)
        {
            PushSpeed = pushSpeed;
            PushEntitySpace = pushEntitySpace;
            PushDirEntitySpace = pushDirEntitySpace;
            PushMins = pushMins;
            PushMaxs = pushMaxs;
        }
    }
}