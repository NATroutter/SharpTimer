using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace SharpTimer
{
    public partial class SharpTimer
    {
        public void PrintAllEnabledCommands(CCSPlayerController player)
        {
            SharpTimerDebug($"Printing Commands for {player.PlayerName}");
            player.PrintToChat($"{msgPrefix}Available Commands:");

            if (respawnEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!r (css_r) - Respawns you");
            if (respawnEnabled && bonusRespawnPoses.Count != 0) player.PrintToChat($"{msgPrefix}{Theme.high}!rb <#> / !b <#> (css_rb / css_b) {Theme.text}- Respawns you to a bonus");
            if (topEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!top (css_top) {Theme.text}- Lists top 10 records on this map");
            if (topEnabled && bonusRespawnPoses.Count != 0) player.PrintToChat($"{msgPrefix}{Theme.high}!topbonus <#> (css_topbonus) {Theme.text}- Lists top 10 records of a bonus");
            if (rankEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!rank (css_rank) {Theme.text}- Shows your current rank and pb");
            if (globalRanksEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!points (css_points) {Theme.text}- Prints top 10 points");
            if (goToEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!goto <name> (css_goto){Theme.text} - Teleports you to a player");
            if (stageTriggerPoses.Count != 0) player.PrintToChat($"{msgPrefix}{Theme.high}!stage <#> (css_stage) {Theme.text}- Teleports you to a stage");

            if (cpEnabled)
            {
                player.PrintToChat($"{msgPrefix}{(currentMapName!.Contains("surf_") ? $"{Theme.high}!saveloc (css_saveloc) {Theme.text}- Saves a Loc" : $"{Theme.high}!cp (css_cp) {Theme.text}- Sets a Checkpoint")}");
                player.PrintToChat($"{msgPrefix}{(currentMapName!.Contains("surf_") ? $"{Theme.high}!loadloc (css_loadloc) {Theme.text}- Teleports you to the last Loc" : $"{Theme.high}!tp (css_tp) {Theme.text}- Teleports you to the last Checkpoint")}");
                player.PrintToChat($"{msgPrefix}{(currentMapName!.Contains("surf_") ? $"{Theme.high}!prevloc (css_prevloc) {Theme.text}- Teleports you one Loc back" : $"{Theme.high}!prevcp (css_prevcp) {Theme.text}- Teleports you one Checkpoint back")}");
                player.PrintToChat($"{msgPrefix}{(currentMapName!.Contains("surf_") ? $"{Theme.high}!nextloc (css_nextloc) {Theme.text}- Teleports you one Loc forward" : $"{Theme.high}!nextcp (css_nextcp) {Theme.text}- Teleports you one Checkpoint forward")}");
            }

            if (jumpStatsEnabled) player.PrintToChat($"{msgPrefix}{Theme.high}!jumpstats (css_jumpstats) {Theme.text}- Toggles JumpStats");
        }

        public void ForcePlayerSpeed(CCSPlayerController player, string activeWeapon)
        {

            try
            {
                activeWeapon ??= "no_knife";
                if (!weaponSpeedLookup.TryGetValue(activeWeapon, out WeaponSpeedStats weaponStats) || !player.IsValid) return;

                player.PlayerPawn.Value!.VelocityModifier = (float)(forcedPlayerSpeed / weaponStats.GetSpeed(player.PlayerPawn.Value.IsWalking));
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in ForcePlayerSpeed: {ex.Message}");
            }
        }

        private void AdjustPlayerVelocity(CCSPlayerController? player, float velocity, bool forceNoDebug = false)
        {
            if (!IsAllowedPlayer(player)) return;

            try
            {
                var currentX = player!.PlayerPawn.Value!.AbsVelocity.X;
                var currentY = player!.PlayerPawn.Value!.AbsVelocity.Y;
                var currentZ = player!.PlayerPawn.Value!.AbsVelocity.Z;

                var currentSpeedSquared = currentX * currentX + currentY * currentY + currentZ * currentZ;

                // Check if current speed is not zero to avoid division by zero
                if (currentSpeedSquared > 0)
                {
                    var currentSpeed = Math.Sqrt(currentSpeedSquared);

                    var normalizedX = currentX / currentSpeed;
                    var normalizedY = currentY / currentSpeed;
                    var normalizedZ = currentZ / currentSpeed;

                    var adjustedX = normalizedX * velocity; // Adjusted speed limit
                    var adjustedY = normalizedY * velocity; // Adjusted speed limit
                    var adjustedZ = normalizedZ * velocity; // Adjusted speed limit

                    player!.PlayerPawn.Value!.AbsVelocity.X = (float)adjustedX;
                    player!.PlayerPawn.Value!.AbsVelocity.Y = (float)adjustedY;
                    player!.PlayerPawn.Value!.AbsVelocity.Z = (float)adjustedZ;

                    if (!forceNoDebug) SharpTimerDebug($"Adjusted Velo for {player.PlayerName} to {player.PlayerPawn.Value.AbsVelocity}");
                }
                else
                {
                    if (!forceNoDebug) SharpTimerDebug($"Cannot adjust velocity for {player.PlayerName} because current speed is zero.");
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in AdjustPlayerVelocity: {ex.Message}");
            }
        }

        private void AdjustPlayerVelocity2D(CCSPlayerController? player, float velocity, bool forceNoDebug = false)
        {
            if (!IsAllowedPlayer(player)) return;

            try
            {
                var currentX = player!.PlayerPawn.Value!.AbsVelocity.X;
                var currentY = player!.PlayerPawn.Value!.AbsVelocity.Y;

                var currentSpeedSquared = currentX * currentX + currentY * currentY;

                // Check if current speed is not zero to avoid division by zero
                if (currentSpeedSquared > 0)
                {
                    var currentSpeed2D = Math.Sqrt(currentSpeedSquared);

                    var normalizedX = currentX / currentSpeed2D;
                    var normalizedY = currentY / currentSpeed2D;

                    var adjustedX = normalizedX * velocity; // Adjusted speed limit
                    var adjustedY = normalizedY * velocity; // Adjusted speed limit

                    player.PlayerPawn.Value.AbsVelocity.X = (float)adjustedX;
                    player.PlayerPawn.Value.AbsVelocity.Y = (float)adjustedY;

                    if (!forceNoDebug) SharpTimerDebug($"Adjusted Velo for {player.PlayerName} to {player.PlayerPawn.Value.AbsVelocity}");
                }
                else
                {
                    if (!forceNoDebug) SharpTimerDebug($"Cannot adjust velocity for {player.PlayerName} because current speed is zero.");
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in AdjustPlayerVelocity2D: {ex.Message}");
            }
        }

        private void RemovePlayerCollision(CCSPlayerController? player)
        {
            try
            {
                Server.NextFrame(() =>
                {
                    if (removeCollisionEnabled == false || !IsAllowedPlayer(player)) return;

                    player!.Pawn.Value!.Collision.CollisionAttribute.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;
                    player!.Pawn.Value!.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_DISSOLVING;

                    Utilities.SetStateChanged(player, "CCollisionProperty", "m_CollisionGroup");
                    Utilities.SetStateChanged(player, "CCollisionProperty", "m_collisionAttribute");

                    SharpTimerDebug($"Removed Collison for {player.PlayerName}");
                });
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in RemovePlayerCollision: {ex.Message}");
            }
        }

        public async Task<(int, string)> GetStageTime(string steamId, int stageIndex)
        {
            string fileName = $"{currentMapName!.ToLower()}_stage_times.json";
            string playerStageRecordsPath = Path.Join(gameDir, "csgo", "cfg", "SharpTimer", "PlayerStageData", fileName);

            try
            {
                using (JsonDocument? jsonDocument = await LoadJson(playerStageRecordsPath)!)
                {
                    if (jsonDocument != null)
                    {
                        string jsonContent = jsonDocument.RootElement.GetRawText();

                        Dictionary<string, PlayerStageData> playerData;
                        if (!string.IsNullOrEmpty(jsonContent))
                        {
                            playerData = JsonSerializer.Deserialize<Dictionary<string, PlayerStageData>>(jsonContent)!;

                            if (playerData!.TryGetValue(steamId, out var playerStageData))
                            {
                                if (playerStageData.StageTimes != null && playerStageData.StageTimes.TryGetValue(stageIndex, out var time) &&
                                    playerStageData.StageVelos != null && playerStageData.StageVelos.TryGetValue(stageIndex, out var speed))
                                {
                                    return (time, speed);
                                }
                            }
                        }
                    }
                    else
                    {
                        SharpTimerDebug($"Error in GetStageTime jsonDoc was null");
                    }
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in GetStageTime: {ex.Message}");
            }

            return (0, string.Empty);
        }

        private int GetPreviousPlayerRecord(CCSPlayerController? player, string steamId, int bonusX = 0)
        {
            if (!IsAllowedPlayer(player))
            {
                SharpTimerDebug("Player not allowed.");
                return 0;
            }

            string currentMapNamee = bonusX == 0 ? currentMapName! : $"{currentMapName}_bonus{bonusX}";

            string mapRecordsFileName = $"{currentMapNamee}.json";
            string mapRecordsPath = Path.Combine(playerRecordsPath!, mapRecordsFileName);

            if (!File.Exists(mapRecordsPath))
            {
                SharpTimerDebug($"Map records file does not exist: {mapRecordsPath}");
                return 0;
            }

            try
            {
                string json = File.ReadAllText(mapRecordsPath);
                Dictionary<string, PlayerRecord> records = JsonSerializer.Deserialize<Dictionary<string, PlayerRecord>>(json) ?? [];

                if (records.TryGetValue(steamId, out PlayerRecord? playerRecord))
                {
                    return playerRecord.TimerTicks;
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in GetPreviousPlayerRecord: {ex.Message}");
            }

            return 0;
        }

        public string GetPlayerPlacement(CCSPlayerController? player)
        {
            if (!IsAllowedPlayer(player) || !playerTimers[player!.Slot].IsTimerRunning) return "";


            int currentPlayerTime = playerTimers[player.Slot].TimerTicks;

            int placement = 1;

            foreach (var kvp in SortedCachedRecords!.Take(100))
            {
                int recordTimerTicks = kvp.Value.TimerTicks;

                if (currentPlayerTime > recordTimerTicks)
                {
                    placement++;
                }
                else
                {
                    break;
                }
            }
            if (placement > 100)
            {
                return "#100" + "+";
            }
            else
            {
                return "#" + placement;
            }
        }

        public async Task<string> GetPlayerMapPlacementWithTotal(CCSPlayerController? player, string steamId, string playerName, bool getRankImg = false, bool getPlacementOnly = false, int bonusX = 0)
        {
            try
            {
                if (!IsAllowedPlayer(player) && !IsAllowedSpectator(player))
                    return "";

                string currentMapNamee = bonusX == 0 ? currentMapName! : $"{currentMapName}_bonus{bonusX}";

                int savedPlayerTime = useMySQL ? await GetPreviousPlayerRecordFromDatabase(player, steamId, currentMapNamee!, playerName) : GetPreviousPlayerRecord(player, steamId, bonusX);

                if (savedPlayerTime == 0)
                    return getRankImg ? unrankedIcon : "Unranked";

                Dictionary<string, PlayerRecord> sortedRecords = useMySQL ? await GetSortedRecordsFromDatabase() : await GetSortedRecords();

                int placement = sortedRecords.Count(kv => kv.Value.TimerTicks < savedPlayerTime) + 1;
                int totalPlayers = sortedRecords.Count;
                double percentage = (double)placement / totalPlayers * 100;

                return CalculateRankStuff(totalPlayers, placement, percentage, getRankImg, getPlacementOnly);
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in GetPlayerMapPlacementWithTotal: {ex}");
                return "Unranked";
            }
        }

        public async Task<string> GetPlayerServerPlacement(CCSPlayerController? player, string steamId, string playerName, bool getRankImg = false, bool getPlacementOnly = false, bool getPointsOnly = false)
        {
            try
            {
                if (!IsAllowedPlayer(player) && !IsAllowedSpectator(player))
                    return "";

                int savedPlayerPoints = useMySQL ? await GetPlayerPointsFromDatabase(player, steamId, playerName) : 0;

                if (getPointsOnly)
                    return savedPlayerPoints.ToString();

                if (savedPlayerPoints == 0 || savedPlayerPoints <= minGlobalPointsForRank)
                    return getRankImg ? unrankedIcon : "Unranked";

                Dictionary<string, PlayerPoints> sortedPoints = useMySQL ? await GetSortedPointsFromDatabase() : [];

                int placement = sortedPoints.Count(kv => kv.Value.GlobalPoints > savedPlayerPoints) + 1;
                int totalPlayers = sortedPoints.Count;
                double percentage = (double)placement / totalPlayers * 100;

                return CalculateRankStuff(totalPlayers, placement, percentage, getRankImg, getPlacementOnly);
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in GetPlayerServerPlacement: {ex}");
                return "Unranked";
            }
        }

        public string CalculateRankStuff(int totalPlayers, int placement, double percentage, bool getRankImg = false, bool getPlacementOnly = false)
        {
            try
            {
                if (totalPlayers < 100)
                {
                    if (placement <= 1)
                        return getRankImg ? god3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God III");
                    else if (placement <= 2)
                        return getRankImg ? god2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God II");
                    else if (placement <= 3)
                        return getRankImg ? god1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God I");
                    else if (placement <= 10)
                        return getRankImg ? royalty3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty III");
                    else if (placement <= 15)
                        return getRankImg ? royalty2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty II");
                    else if (placement <= 20)
                        return getRankImg ? royalty1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty I");
                    else if (placement <= 25)
                        return getRankImg ? legend3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend III");
                    else if (placement <= 30)
                        return getRankImg ? legend2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend II");
                    else if (placement <= 35)
                        return getRankImg ? legend1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend I");
                    else if (placement <= 40)
                        return getRankImg ? master3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master III");
                    else if (placement <= 45)
                        return getRankImg ? master2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master II");
                    else if (placement <= 50)
                        return getRankImg ? master1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master I");
                    else if (placement <= 55)
                        return getRankImg ? diamond3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond III");
                    else if (placement <= 60)
                        return getRankImg ? diamond2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond II");
                    else if (placement <= 65)
                        return getRankImg ? diamond1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond I");
                    else if (placement <= 70)
                        return getRankImg ? platinum3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum III");
                    else if (placement <= 75)
                        return getRankImg ? platinum2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum II");
                    else if (placement <= 80)
                        return getRankImg ? platinum1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum I");
                    else if (placement <= 85)
                        return getRankImg ? gold3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold III");
                    else if (placement <= 90)
                        return getRankImg ? gold2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold II");
                    else if (placement <= 95)
                        return getRankImg ? gold1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold I");
                    else
                        return getRankImg ? silver1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Silver I");
                }
                else
                {
                    if (placement <= 1)
                        return getRankImg ? god3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God III");
                    else if (placement <= 2)
                        return getRankImg ? god2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God II");
                    else if (placement <= 3)
                        return getRankImg ? god1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"God I");
                    else if (percentage <= 1)
                        return getRankImg ? royalty3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty III");
                    else if (percentage <= 5.0)
                        return getRankImg ? royalty2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty II");
                    else if (percentage <= 10.0)
                        return getRankImg ? royalty1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Royalty I");
                    else if (percentage <= 15.0)
                        return getRankImg ? legend3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend III");
                    else if (percentage <= 20.0)
                        return getRankImg ? legend2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend II");
                    else if (percentage <= 25.0)
                        return getRankImg ? legend1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Legend I");
                    else if (percentage <= 30.0)
                        return getRankImg ? master3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master III");
                    else if (percentage <= 35.0)
                        return getRankImg ? master2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master II");
                    else if (percentage <= 40.0)
                        return getRankImg ? master1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Master I");
                    else if (percentage <= 45.0)
                        return getRankImg ? diamond3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond III");
                    else if (percentage <= 50.0)
                        return getRankImg ? diamond2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond II");
                    else if (percentage <= 55.0)
                        return getRankImg ? diamond1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Diamond I");
                    else if (percentage <= 60.0)
                        return getRankImg ? platinum3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum III");
                    else if (percentage <= 65.0)
                        return getRankImg ? platinum2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum II");
                    else if (percentage <= 70.0)
                        return getRankImg ? platinum1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Platinum I");
                    else if (percentage <= 75.0)
                        return getRankImg ? gold3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold III");
                    else if (percentage <= 80.0)
                        return getRankImg ? gold2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold II");
                    else if (percentage <= 85.0)
                        return getRankImg ? gold1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Gold I");
                    else if (percentage <= 90.0)
                        return getRankImg ? silver3Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Silver III");
                    else if (percentage <= 95.0)
                        return getRankImg ? silver2Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Silver II");
                    else
                        return getRankImg ? silver1Icon : (getPlacementOnly ? $"{placement}/{totalPlayers}" : $"Silver I");
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in CalculateRankStuff: {ex}");
                return "Unranked";
            }
        }

        public void InvalidateTimer(CCSPlayerController player, nint callerHandle = 0)
        {
            if (player.IsValid && playerTimers!.TryGetValue(player.Slot, out var playerTimer))
            {
                if (!playerTimers[player.Slot].IsTimerBlocked)
                {
                    playerCheckpoints.Remove(player.Slot);
                }
                playerTimers[player.Slot].TimerTicks = 0;
                playerTimers[player.Slot].BonusTimerTicks = 0;
                playerTimers[player.Slot].IsTimerRunning = false;
                playerTimers[player.Slot].IsBonusTimerRunning = false;
                if (stageTriggerCount != 0 && useStageTriggers == true)
                {
                    playerTimers[player.Slot].StageTimes!.Clear();
                    playerTimers[player.Slot].StageVelos!.Clear();
                    playerTimers[player.Slot].CurrentMapStage = stageTriggers.GetValueOrDefault(callerHandle, 0);
                }
                else if (cpTriggerCount != 0 && useStageTriggers == false)
                {
                    playerTimers[player.Slot].StageTimes!.Clear();
                    playerTimers[player.Slot].StageVelos!.Clear();
                    playerTimers[player.Slot].CurrentMapCheckpoint = 0;
                }
            }
        }

        private HookResult OnCommandJoinTeam(CCSPlayerController? player, CounterStrikeSharp.API.Modules.Commands.CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid) return HookResult.Handled;
            InvalidateTimer(player);
            return HookResult.Continue;
        }

        public async Task PrintMapTimeToChat(CCSPlayerController player, string steamID, string playerName, int oldticks, int newticks, int bonusX = 0, int timesFinished = 0)
        {
            if (!IsAllowedPlayer(player))
            {
                SharpTimerError($"Error in PrintMapTimeToChat: Player {playerName} not allowed or not on server anymore");
                return;
            }

            string ranking = await GetPlayerMapPlacementWithTotal(player, steamID, playerName, false, true);


            bool newSR = GetNumberBeforeSlash(ranking) == 1 && (oldticks > newticks || oldticks == 0);
            bool beatPB = oldticks > newticks;
            string newTime = FormatTime(newticks);
            string timeDifferenceNoCol = "";
            string timeDifference = "";
            if (oldticks != 0)
            {
                if (discordWebhookEnabled) timeDifferenceNoCol = FormatTimeDifference(newticks, oldticks, true);
                timeDifference = $"[{FormatTimeDifference(newticks, oldticks)}{Theme.text}] ";
            }

            Server.NextFrame(() =>
            {
                if (IsAllowedPlayer(player) && timesFinished > maxGlobalFreePoints && globalRanksFreePointsEnabled == true && oldticks < newticks)
                    player.PrintToChat(msgPrefix + $"You reached your maximum free points rewards of {Theme.high}{maxGlobalFreePoints}{Theme.text}!");

                if (newSR)
                {
                    Server.PrintToChatAll(msgPrefix + $"{Theme.high}{playerName} {Theme.text}set a new {(bonusX != 0 ? $"Bonus {bonusX} SR!" : "SR!")}");
                    if (discordWebhookPrintSR && discordWebhookEnabled && useMySQL == true) _ = Task.Run(async () => await DiscordRecordMessage(playerName, newTime, steamID, ranking, timesFinished, true, timeDifferenceNoCol, bonusX));
                }
                else if (beatPB)
                {
                    Server.PrintToChatAll(msgPrefix + $"{Theme.high}{playerName} {Theme.text}set a new {(bonusX != 0 ? $"Bonus {bonusX} PB!" : "Map PB!")}");
                    if (discordWebhookPrintPB && discordWebhookEnabled && useMySQL == true) _ = Task.Run(async () => await DiscordRecordMessage(playerName, newTime, steamID, ranking, timesFinished, false, timeDifferenceNoCol, bonusX));
                }
                else
                {
                    Server.PrintToChatAll(msgPrefix + $"{Theme.high}{playerName} {Theme.text}finished the {(bonusX != 0 ? $"Bonus {bonusX}!" : "Map!")}");
                    if (discordWebhookPrintPB && discordWebhookEnabled && timesFinished == 1 && useMySQL == true) _ = Task.Run(async () => await DiscordRecordMessage(playerName, newTime, steamID, ranking, timesFinished, false, timeDifferenceNoCol, bonusX));
                }

                if (useMySQL != false || bonusX != 0)
                    Server.PrintToChatAll(msgPrefix + $"{(bonusX != 0 ? $"" : $"Rank: [{Theme.high}{ranking}{Theme.text}] ")}{(timesFinished != 0 && useMySQL == true ? $"Times Finished: [{Theme.high}{timesFinished}{Theme.text}]" : "")}");

                Server.PrintToChatAll(msgPrefix + $"Time: [{Theme.high}{newTime}{Theme.text}] {timeDifference}");

                if (IsAllowedPlayer(player) && playerTimers[player.Slot].SoundsEnabled != false) player.ExecuteClientCommand($"play {beepSound}");

                if (enableReplays == true && enableSRreplayBot == true && newSR && (oldticks > newticks || oldticks == 0))
                {
                    _ = Task.Run(async () => await SpawnReplayBot());
                }
            });
        }

        public void AddScoreboardTagToPlayer(CCSPlayerController player, string tag)
        {
            try
            {

                if (string.IsNullOrEmpty(tag))
                    return;

                if (player == null || !player.IsValid)
                    return;

                string originalPlayerName = player.PlayerName;

                string stripedClanTag = RemovePlayerTags(player.Clan ?? "");

                player.Clan = $"{stripedClanTag}{(playerTimers[player.Slot].IsVip ? $"[{customVIPTag}]" : "")}[{tag}]";

                player.PlayerName = originalPlayerName + " ";

                AddTimer(0.1f, () =>
                {
                    if (player.IsValid)
                    {
                        Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                        Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
                    }
                });

                AddTimer(0.2f, () =>
                {
                    if (player.IsValid) player.PlayerName = originalPlayerName;
                });

                AddTimer(0.3f, () =>
                {
                    if (player.IsValid) Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
                });

                SharpTimerDebug($"Set Scoreboard Tag for {player.Clan} {player.PlayerName}");
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in AddScoreboardTagToPlayer: {ex.Message}");
            }
        }

        public void ChangePlayerName(CCSPlayerController player, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (player == null || !player.IsValid)
                return;

            player.PlayerName = name + " ";

            AddTimer(0.1f, () =>
            {
                if (player.IsValid)
                {
                    Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
                    Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
                }
            });

            AddTimer(0.2f, () =>
            {
                if (player.IsValid) player.PlayerName = name;
            });

            AddTimer(0.3f, () =>
            {
                if (player.IsValid) Utilities.SetStateChanged(player, "CBasePlayerController", "m_iszPlayerName");
            });

            SharpTimerDebug($"Changed PlayerName to {player.PlayerName}");
        }

        static void SetMoveType(CCSPlayerController player, MoveType_t nMoveType)
        {
            if (!player.IsValid) return;

            player.PlayerPawn.Value!.MoveType = nMoveType; // necessary to maintain client prediction
            player.PlayerPawn.Value!.ActualMoveType = nMoveType;
        }

        public char GetRankColorForChat(CCSPlayerController player)
        {
            try
            {
                if (playerTimers.TryGetValue(player.Slot, out PlayerTimerInfo? playerTimer))
                {
                    if (string.IsNullOrEmpty(playerTimer.CachedRank)) return ChatColors.Default;

                    char color = ChatColors.Default;

                    if (playerTimer.CachedRank.Contains("Unranked"))
                        color = ChatColors.Default;
                    else if (playerTimer.CachedRank.Contains("Silver"))
                        color = ChatColors.Silver;
                    else if (playerTimer.CachedRank.Contains("Gold"))
                        color = ChatColors.LightYellow;
                    else if (playerTimer.CachedRank.Contains("Platinum"))
                        color = ChatColors.Green;
                    else if (playerTimer.CachedRank.Contains("Diamond"))
                        color = ChatColors.LightBlue;
                    else if (playerTimer.CachedRank.Contains("Master"))
                        color = ChatColors.Purple;
                    else if (playerTimer.CachedRank.Contains("Legend"))
                        color = ChatColors.Lime;
                    else if (playerTimer.CachedRank.Contains("Royalty"))
                        color = ChatColors.Orange;
                    else if (playerTimer.CachedRank.Contains("God"))
                        color = ChatColors.LightRed;

                    return color;
                }
                else
                {
                    return ChatColors.Default;
                }
            }
            catch (Exception ex)
            {
                SharpTimerError($"Error in GetRankColorForChat: {ex.Message}");
                return ChatColors.Default;
            }
        }

        public static void SendCommandToEveryone(string command)
        {
            Utilities.GetPlayers().ForEach(player =>
            {
                if (player is { PawnIsAlive: true, IsValid: true })
                {
                    player.ExecuteClientCommand(command);
                }
            });
        }

        public static (int, int) GetPlayerTeamCount()
        {
            int ct_count = 0;
            int t_count = 0;

            Utilities.GetPlayers().ForEach(player =>
            {
                if (player is { PawnIsAlive: true, IsValid: true })
                {
                    if (player.Team == CsTeam.CounterTerrorist)
                        ct_count++;
                    else if (player.Team == CsTeam.Terrorist)
                        t_count++;
                }
            });

            return (ct_count, t_count);
        }
    }
}