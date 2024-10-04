![GitHub Repo stars](https://img.shields.io/github/stars/ABKAM2023/CS2-LevelsRanks-Core?style=for-the-badge)
![GitHub all releases](https://img.shields.io/github/downloads/ABKAM2023/CS2-LevelsRanks-Core/total?style=for-the-badge)

# EN
## Description
[C#] [Levels Ranks] Core system is based on a simple principle: players perform various actions in the game, which result in either gaining or losing experience points. Achieving or losing a certain amount of these points leads to obtaining the corresponding rank. The number of available ranks can be customized and edited at discretion.

The plugin has 3 types of statistics:
1. **Cumulative**: you start from the lowest rank and have to accumulate experience points starting from 0. The more you play, the higher your rank.
2. **Rating - Extended**: similar to HlstatsX. You start with an average rank and 1000 experience points. Your rank depends on your gaming skills.
3. **Rating - Simple**: similar to RankMe. Similar to the extended rating type, but without additional bonuses and with a different experience points calculation formula.

# Modules:
- [ExStats - GeoIP](https://github.com/ABKAM2023/CS2-LR-ExStats-GeoIP)
- [ExStats - Hits](https://github.com/ABKAM2023/CS2-LR-ExStats-Hits)
- [ExStats - Weapons](https://github.com/ABKAM2023/CS2-LR-ExStats-Weapons)
- [Module - NameReward](https://github.com/ABKAM2023/CS2-LR-NameReward)
- [Module - TimeReward](https://github.com/ABKAM2023/CS2-LR-TimeReward)
- [Module - Tag](https://github.com/ABKAM2023/CS2-LR-Module-Tag)
- [Module - FakeRank](https://github.com/ABKAM2023/CS2-LR-FakeRank)
- [VIP - ExperienceMultiplier](https://github.com/ABKAM2023/CS2-VIP-LR-ExperienceMultiplier/tree/v1%2C0)
- [VIP - CustomFakeRank](https://github.com/ABKAM2023/CS2_VIP_LR_CustomFakeRank/releases)

## Installation
1. Install [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master), [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp), [MenuManager](https://github.com/NickFox007/MenuManagerCS2/releases/), [AnyBaseLib](https://github.com/NickFox007/AnyBaseLibCS2/releases), [PlayerSettings](https://github.com/NickFox007/PlayerSettingsCS2/releases) and for sounds [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager/releases/).
2. Download [C#] [Levels Ranks] Core.
3. Unpack the archive and upload it to your game server.
4. Start the server to create the necessary configuration files.
5. Connect the plugin to the database by entering the required data in the `database.json` file. Ensure the entered data is correct.

## Commands
- `!lvl` - opens the main statistics menu.
- `!rank` - shows basic information about your progress.
- `!rank <nickname>` - shows basic information about the specified player.
- `!top` - opens the top players menu.

## Variables
- `css_lvl_reload` - reloads all plugin configuration files.
- `css_lvl_reset <type>` - resets statistics for all players.
  - `all` - resets all data.
  - `exp` - resets experience points data (`value`, `rank`).
  - `stats` - resets statistics data (`kills`, `deaths`, `shoots`, `hits`, `headshots`, `assists`, `round_win`, `round_lose`, `playtime`).
- `css_lvl_del <target>` - resets statistics for a specific player.
- `css_lvl_giveexp <target> <amount>` - gives or takes experience from a player (SteamID and SteamID64 can be specified).


# RU
## Описание
[C#] [Levels Ranks] Core система базируется на простом принципе: игроки совершают разнообразные действия в игре, в результате которых они либо приобретают, либо теряют очки опыта. Достижение или потеря определенного объема этих очков ведет к получению соответствующего ранга. Количество доступных рангов может быть настроено и отредактировано по усмотрению.

Плагин имеет 3 типа статистики:
1. **Накопительный**: вы начинаете с самого низшего звания и должны накапливать очки опыта, начиная с 0. Чем больше вы играете, тем выше ваше звание.
2. **Рейтинговый - расширенный**: аналог HlstatsX. Вы начинаете со среднего звания и 1000 очков опыта. Ваше звание зависит от ваших игровых навыков.
3. **Рейтинговый - простой**: аналог RankMe. Похож на расширенный рейтинговый тип, но без дополнительных бонусов и с другой формулой подсчета очков опыта.

# Модули:
- [ExStats - GeoIP](https://github.com/ABKAM2023/CS2-LR-ExStats-GeoIP)
- [ExStats - Hits](https://github.com/ABKAM2023/CS2-LR-ExStats-Hits)
- [Module - NameReward](https://github.com/ABKAM2023/CS2-LR-NameReward)
- [Module - TimeReward](https://github.com/ABKAM2023/CS2-LR-TimeReward)
- [Module - Tag](https://github.com/ABKAM2023/CS2-LR-Module-Tag)
- [Module - FakeRank](https://github.com/ABKAM2023/CS2-LR-FakeRank)
- [VIP - ExperienceMultiplier](https://github.com/ABKAM2023/CS2-VIP-LR-ExperienceMultiplier/tree/v1%2C0)
- [VIP - CustomFakeRank](https://github.com/ABKAM2023/CS2_VIP_LR_CustomFakeRank/releases)

## Установка
1. Установите [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master), [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp), [MenuManager](https://github.com/NickFox007/MenuManagerCS2/releases/), [AnyBaseLib](https://github.com/NickFox007/AnyBaseLibCS2/releases), [PlayerSettings](https://github.com/NickFox007/PlayerSettingsCS2/releases) и для работы звуков [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager/releases/).
2. Скачайте [C#] [Levels Ranks] Core.
3. Распакуйте архив и загрузите его на игровой сервер.
4. Запустите сервер, чтобы создать необходимые конфигурационные файлы.
5. Подключите плагин к базе данных, введя необходимые данные в файл `database.json`. Убедитесь в корректности введенных данных.

## Команды
- `!lvl` - открывает главное меню статистики.
- `!rank` - показывает основную информацию о вашем прогрессе.
- `!rank <ник>` - показывает основную информацию о указанном игроке.
- `!top` - открывает меню с топами.

## Переменные
- `css_lvl_reload` - перезагружает все конфигурационные файлы плагина.
- `css_lvl_reset <тип>` - сбрасывает статистику у всех игроков.
  - `all` - сбросит все данные.
  - `exp` - сбросит данные о очках опыта (`value`, `rank`).
  - `stats` - сбросит данные о статистике (`kills`, `deaths`, `shoots`, `hits`, `headshots`, `assists`, `round_win`, `round_lose`, `playtime`).
- `css_lvl_del <цель>` - сбрасывает статистику у конкретного игрока.
- `css_lvl_giveexp <цель> <количество>` - выдает или забирает опыт у игрока (можно указывать SteamID и SteamID64).

# EN
# Main Configuration File (settings.json)
```
{
//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  (***) Name of the table in the database (only Latin letters and no more than 32 characters).
//      If you have multiple servers in the project but want each server to have its own statistics, change the table name to anything else.
//      Necessary for cases where you store different statistics in one database.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_table": "lvl_base",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  (***) Choose the type of statistics
//
//      0 - Cumulative system.
//      The player's rank will gradually increase, starting from the first rank.
//
//      1 - Advanced rating system.
//      This system calculates players' experience points based on their skill level (similar to HlStats).
//
//      2 - Simple rating system.
//      This system of counting experience points is analogous to the RankMe counting system.
//
//      If you want to change the type of statistics, completely reset the statistics data in the database.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_type_statistics": "0",

  // What flags should an administrator have to access the Admin Panel?
  "lr_flag_adminmenu": "@lr/admin",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      What should be the title of the plugin menu, intended for more advanced projects
//      that like to customize their servers.
//
//      NOTE: you can also change all chat prefixes (Example: [LR]) in the translation file.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_plugin_title": "Levels Ranks v1.1.0",

  // Should rank up/down sounds be played? [0 - no, 1 - yes]
  "lr_sound": "1",

  // Path to the sound file to be played when ranking up (specify without sound/).
  "lr_sound_lvlup": "sounds/levels_ranks/levelup.vsnd_c",

  // Path to the sound file to be played when ranking down (specify without sound/).
  "lr_sound_lvldown": "sounds/levels_ranks/leveldown.vsnd_c",

  // Enable players to reset their statistics in the "Statistics" menu? [0 - no, 1 - yes]
  "lr_show_resetmystats": "1",

  // How many seconds should pass before a player can reset their statistics again?
  "lr_resetmystats_cooldown": "86400",

  // Minimum number of players required to award experience points.
  // The number of players is checked at the start of the round.
  "lr_minplayers_count": "4",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      How to display messages to the player about receiving experience points?
//
//      0 - do not show
//      1 - show for each player action
//      2 - show cumulative change at the end of the round
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_show_usualmessage": "1",

  // Show plugin messages at each spawn? [0 - no, 1 - yes]
  "lr_show_spawnmessage": "1",

  // Show messages to everyone when someone ranks up? [0 - no, 1 - yes]
  "lr_show_levelup_message": "1",

  // Show messages to everyone when someone ranks down? [0 - no, 1 - yes]
  "lr_show_leveldown_message": "1",

  // Show everyone the player's rank after the player writes the rank command? [0 - no, 1 - yes]
  "lr_show_rankmessage": "1",

  // Show the "All Ranks" item in the statistics menu? [0 - no, 1 - yes]
  "lr_show_ranklist": "1",

  // Allow players to gain/lose experience points when the round is over? [0 - no, 1 - yes]
  "lr_giveexp_roundend": "1",

  // Award experience points to players during warm-up? [0 - yes, 1 - no]
  "lr_block_warmup": "1",

  // Count team kills as TeamKill? [0 - yes, 1 - no (needed for "All Against All" mode servers)]
  "lr_allagainst_all": "1",

  // Award experience points to players when interacting with bots [0 - no, 1 - yes]
  "lr_experience_from_bots": "0"
}
```

#
Configuration File for Ranks (settings_ranks.json)
```
{
//  All rank names are recorded in the file (addons/counterstrikesharp/plugins/[LR] Core/lang) - you can change the rank names there.
//  The title of the rank block corresponds to the translation phrase used from (lang).
//  If you want to add your own ranks, create a block here, and then create a corresponding block in (lang) (the block title must match the block title in the translation file).
//
//  If you plan to introduce your own experience point values for ranks from lr_type_statistics 1 and 2, keep in mind that these modes have a starting value of 1000.
//  Considering their starting value is 1000, it is highly recommended to set this value for the rank (block) that is in the middle of the others.
//  Among the standard 18 ranks, the middle is considered to be rank 9, and it is in the rank_9 block that the value 1000 is set.
//
//  The parameters you change here can be updated on the server with the console command (css_lvl_reload) or from the Admin Panel in the plugin menu without restarting the server.

  // Do not change rank 1
  "1": {  
    "Value0": 0, // How many experience points are needed to access this rank (if lr_type_statistics 0 is used)
    "Value1": 0, // How many experience points are needed to access this rank (if lr_type_statistics 1 is used)
    "Value2": 0  // How many experience points are needed to access this rank (if lr_type_statistics 2 is used)
  },
  "2": {
    "Value0": 10,
    "Value1": 700,
    "Value2": 850
  },
  "3": {
    "Value0": 25,
    "Value1": 800,
    "Value2": 900
  },
  "4": {
    "Value0": 50,
    "Value1": 850,
    "Value2": 935
  },
  "5": {
    "Value0": 75,
    "Value1": 900,
    "Value2": 950
  },
  "6": {
    "Value0": 100,
    "Value1": 925,
    "Value2": 965
  },
  "7": {
    "Value0": 150,
    "Value1": 950,
    "Value2": 980
  },
  "8": {
    "Value0": 200,
    "Value1": 975,
    "Value2": 990
  },
  "9": {
    "Value0": 300,
    "Value1": 1000,
    "Value2": 1000
  },
  "10": {
    "Value0": 500,
    "Value1": 1100,
    "Value2": 1050
  },
  "11": {
    "Value0": 750,
    "Value1": 1250,
    "Value2": 1100
  },
  "12": {
    "Value0": 1000,
    "Value1": 1400,
    "Value2": 1200
  },
  "13": {
    "Value0": 1500,
    "Value1": 1600,
    "Value2": 1300
  },
  "14": {
    "Value0": 2000,
    "Value1": 1800,
    "Value2": 1400
  },
  "15": {
    "Value0": 3000,
    "Value1": 2100,
    "Value2": 1550
  },
  "16": {
    "Value0": 5000,
    "Value1": 2400,
    "Value2": 1750
  },
  "17": {
    "Value0": 7500,
    "Value1": 3000,
    "Value2": 2000
  },
  "18": {
    "Value0": 10000,
    "Value1": 4000,
    "Value2": 2500
  }
}
```

# Configuration File for Experience Points Settings (settings_stats.json)
```
{
//  The parameters you change here can be updated on the server with the console command (css_lvl_reload) or from the Admin Panel in the plugin menu without restarting the server.

  // Cumulative system.
  "Funded_System": {
    // To disable the awarding of experience points for a specific action, set the desired parameter to 0.
    // How many experience points a player:
    "lr_kill": 5, // gets for a kill
    "lr_death": -5, // loses for their death
    "lr_headshot": 1, // gets for a headshot
    "lr_assist": 1, // gets for an assist
    "lr_suicide": -6, // loses for suicide
    "lr_teamkill": -6, // loses for team kill
    "lr_winround": 2, // gets for winning a round
    "lr_loseround": -2, // loses for losing a round
    "lr_mvpround": 3, // gets for being the MVP of the round
    "lr_bombplanted": 2, // gets for planting the bomb
    "lr_bombdefused": 2, // gets for defusing the bomb
    "lr_bombdropped": -1, // loses for dropping the bomb
    "lr_bombpickup": 1, // gets for picking up the bomb
    "lr_hostagekilled": -4, // loses for killing a hostage
    "lr_hostagerescued": 3 // gets for rescuing a hostage
  },
  // Advanced rating system.
  "Rating_Extended": {
    // Experience points deduction coefficient for player's death (use at your own risk as it strongly influences rank distribution)
    //
    // Maximum: 1.20 - the player loses 20 percent more than the killer receives (almost impossible to reach higher ranks)
    // Standard: 1.00 - the player loses the same amount of experience points as the killer receives
    // Minimum: 0.80 - the player loses only 80 percent of the experience points from the actual value (reaching higher ranks becomes much easier)
    // 
    "lr_killcoeff": 1, // Recommended value: 1.04 (theoretically).

    // To disable the awarding of experience points for a specific action, set the desired parameter to 0.
    // How many experience points a player:

    "lr_headshot": 1, // gets for a headshot
    "lr_assist": 1, // gets for an assist
    "lr_suicide": -10, // loses for suicide
    "lr_teamkill": -5, // loses for team kill
    "lr_winround": 2, // gets for winning a round
    "lr_loseround": -2, // loses for losing a round
    "lr_mvpround": 1, // gets for being the MVP of the round
    "lr_bombplanted": 3, // gets for planting the bomb
    "lr_bombdefused": 3, // gets for defusing the bomb
    "lr_bombdropped": -2, // loses for dropping the bomb
    "lr_bombpickup": 2, // gets for picking up the bomb
    "lr_hostagekilled": -20, // loses for killing a hostage
    "lr_hostagerescued": 5 // gets for rescuing a hostage
  },
  // Simple rating system.
  "Rating_Simple": {
    // To disable the awarding of experience points for a specific action, set the desired parameter to 0.
    // How many experience points a player:

    "lr_headshot": 1, // gets for a headshot
    "lr_assist": 1, // gets for an assist
    "lr_suicide": 0, // loses for suicide
    "lr_teamkill": 0, // loses for team kill
    "lr_winround": 2, // gets for winning a round
    "lr_loseround": -2, // loses for losing a round
    "lr_mvpround": 1, // gets for being the MVP of the round
    "lr_bombplanted": 2, // gets for planting the bomb
    "lr_bombdefused": 2, // gets for defusing the bomb
    "lr_bombdropped": -1, // loses for dropping the bomb
    "lr_bombpickup": 1, // gets for picking up the bomb
    "lr_hostagekilled": 0, // loses for killing a hostage
    "lr_hostagerescued": 2 // gets for rescuing a hostage
  },
  // Kill streaks.
  "Special_Bonuses": {
    // Only for [Funded_System] and [Rating_Extended]
    // To disable the awarding of experience points for a specific action, set the value in quotes to 0.

    "lr_bonus_1": 2, // DoubleKill
    "lr_bonus_2": 3, // TripleKill
    "lr_bonus_3": 4, // Domination
    "lr_bonus_4": 5, // Rampage
    "lr_bonus_5": 6, // MegaKill
    "lr_bonus_6": 7, // Ownage
    "lr_bonus_7": 8, // UltraKill
    "lr_bonus_8": 9, // KillingSpree
    "lr_bonus_9": 10, // MonsterKill
    "lr_bonus_10": 11, // Unstoppable
    "lr_bonus_11": 12 // GodLike
  }
}
```

# Plugin Translation Configuration File (en.json)
```
{
  "rank_up_message": "{DarkRed}[LR] {White}You have been promoted to {Green}{0}",
  "rank_down_message": "{DarkRed}[LR] {White}You have been demoted to {DarkRed}{0}",
  "rank_up_broadcast": "{DarkRed}[LR] {White}Player {Orange}{0} {White}has been promoted to {Green}{1}",
  "rank_down_broadcast": "{DarkRed}[LR] {White}Player {Orange}{0} {White}has been demoted to {DarkRed}{1}",
  "suicide_color": "{DarkRed}",
  "suicide": "for suicide",
  "team_kill_color": "{DarkRed}",
  "team_kill": "for team kill",
  "death_color": "{DarkRed}",
  "death": "for your death",
  "kill_color": "{Green}",
  "kill": "for kill",
  "headshot_color": "{Yellow}",
  "headshot": "for headshot",
  "assist_color": "{Blue}",
  "assist": "for assist",
  "win_round_color": "{Green}",
  "win_round": "your team won",
  "lose_round_color": "{DarkRed}",
  "lose_round": "your team lost",
  "mvp_round_color": "{Blue}",
  "mvp_round": "MVP",
  "bomb_pickup_color": "{Green}",
  "bomb_pickup": "picked up the bomb",
  "bomb_dropped_color": "{DarkRed}",
  "bomb_dropped": "dropped the bomb",
  "bomb_defused_color": "{Green}",
  "bomb_defused": "bomb defused",
  "bomb_planted_color": "{Green}",
  "bomb_planted": "bomb planted",
  "hostage_rescued_color": "{LightPurple}",
  "hostage_rescued": "you rescued a hostage",
  "hostage_killed_color": "{DarkRed}",
  "hostage_killed": "you killed a hostage",
  "killstreak_bonus_color": "{Purple}",
  "killstreak_bonus": "for {0}",
  "killstreak_2_color": "{Purple}",
  "killstreak_2": "DoubleKill",
  "killstreak_3_color": "{Purple}",
  "killstreak_3": "TripleKill",
  "killstreak_4_color": "{Purple}",
  "killstreak_4": "Domination",
  "killstreak_5_color": "{Purple}",
  "killstreak_5": "Rampage",
  "killstreak_6_color": "{Purple}",
  "killstreak_6": "MegaKill",
  "killstreak_7_color": "{Purple}",
  "killstreak_7": "Ownage",
  "killstreak_8_color": "{Purple}",
  "killstreak_8": "UltraKill",
  "killstreak_9_color": "{Purple}",
  "killstreak_9": "KillingSpree",
  "killstreak_10_color": "{Purple}",
  "killstreak_10": "MonsterKill",
  "killstreak_11_color": "{Purple}",
  "killstreak_11": "Unstoppable",
  "killstreak_12_color": "{Purple}",
  "killstreak_12": "GodLike",
  "round_summary_positive": "{DarkRed}[LR] {White}You earned {Green}{0} {White}experience points this round",
  "round_summary_negative": "{DarkRed}[LR] {White}You lost {Red}{0} {White}experience points this round",
  "round_summary_neutral": "{DarkRed}[LR] {White}You did not earn any experience points this round!",
  "round_start_few_players": "{DarkRed}[LR] {White}There are only {DarkRed}{0} {White}players out of {DarkRed}{1}",
  "round_start_message": "{DarkRed}[LR] {White}Enter: {Purple}!lvl {White}- main menu",
  "command_player_only": "{DarkRed}[LR] {White}This command can only be called by a player.",
  "player_not_found": "{DarkRed}[LR] {White}Player {Green}{0} {White}not found.",
  "player_stats": "{DarkRed}[LR] {White}Player {Green}{0} {White}is ranked {Green}{1} / {2} {White}with {Orange}{3} {White}points, {Orange}{4} {White}kills, {Orange}{5} {White}deaths, and KDR {Orange}{6:F2}",
  "command_error": "{Red}[LR] {White}An error occurred while executing the command.",
  "command_console_only": "{DarkRed}[LR] {White}This command is only available from the server console.",
  "menu_api_not_found": "{DarkRed}[LR] {White}Menu API not found!",
  "user_data_not_found": "{DarkRed}[LR] {White}User data not found!",
  "admin_panel": "Admin Panel",
  "my_stats": "My Stats",
  "top_players": "TOP Players",
  "all_ranks": "All Ranks",
  "rank_item": "[ {0} ] {1}",
  "top_10_experience": "TOP-10 by experience points",
  "top_player_item": "{0}. [ {1} ] {2}",
  "top_10_activity": "TOP-10 by activity",
  "top_player_activity_item": "{0}. [ {1} ] {2}",
  "show_stats_in_chat": "Show stats in chat",
  "reset_stats": "Reset stats",
  "user_stats_title": "{DarkRed}[LR] {White}Your stats:",
  "user_stat_experience": "- Experience: {Green}{0}",
  "user_stat_kills": "- Kills: {Green}{0}",
  "user_stat_deaths": "- Deaths: {Green}{0}",
  "user_stat_headshots": "- Headshots: {Green}{0}",
  "user_stat_assists": "- Assists: {Green}{0}",
  "user_stat_shoots": "- Shots: {Green}{0}",
  "user_stat_hits": "- Hits: {Green}{0}",
  "user_stat_round_win": "- Rounds won: {Green}{0}",
  "user_stat_round_lose": "- Rounds lost: {Green}{0}",
  "user_stat_playtime": "- Playtime: {Green}{0}",
  "reset_stats_cooldown": "{DarkRed}[LR] {White}Stats reset will be available in {0}d {1}h {2}m {3}s.",
  "stats_reset_success": "{DarkRed}[LR] {White}Your stats have been reset.",
  "grant_revoke_points": "Grant/Revoke experience points",
  "reload_plugin_settings": "Reload plugin settings",
  "plugin_settings_reloaded": "{DarkRed}[LR] {White}Plugin settings have been successfully reloaded.",
  "grant_points": "Grant",
  "revoke_points": "Revoke",
  "select_player": "Select player",
  "points": "points",
  "admin_grant_points": "admin granted experience points",
  "admin_revoke_points": "admin revoked experience points",
  "points_granted": "{DarkRed}[LR] {White}Granted {Green}{0} {White}points to player {Green}{1}{White}.",
  "points_revoked": "{DarkRed}[LR] {White}Revoked {DarkRed}{0} {White}points from player {Green}{1}{White}.",
  "user_data_not_found_for_player": "User data not found for {0}.",
  "experience_message": "{DarkRed}[LR] {White}Your experience: {0}{1} [{2} {3}]",
  "rank_1": "Silver - I",
  "rank_2": "Silver - II",
  "rank_3": "Silver - III",
  "rank_4": "Silver - IV",
  "rank_5": "Silver Elite",
  "rank_6": "Silver - Great Master",
  "rank_7": "Gold Star - I",
  "rank_8": "Gold Star - II",
  "rank_9": "Gold Star - III",
  "rank_10": "Gold Star - Master",
  "rank_11": "Master Guardian - I",
  "rank_12": "Master Guardian - II",
  "rank_13": "Master Guardian - Elite",
  "rank_14": "Distinguished Master Guardian",
  "rank_15": "Legendary Eagle",
  "rank_16": "Legendary Eagle Master",
  "rank_17": "Supreme Master - First Class",
  "rank_18": "Global Elite"
}
```

# Database Connection Configuration File (database.json)
```
{
  "Database": "levelsranks",
  "User": "root",
  "Password": "",
  "Host": "localhost",
  "Port": 3306
}
```

# RU
# Основной конфигурационный файл (settings.json)
```
{
//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	(***) Наименование таблицы в базе данных (только латиница и не больше 32 символов).
//		Если вы имеете несколько серверов в проекте, но хотите, чтобы у каждого сервера была собственная статистика, то меняйте название таблицы на любое другое.
//		Необходим для тех случаев, когда вы храните разные статистики на одной базе данных.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_table": "lvl_base",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	(***) Выберите тип статистики
//
//		0 - Накопительная система.
//		Ранг игрока будет постепенно подниматься, начиная с первого ранга.
//
//		1 - Рейтинговая система (расширенная).
//		Данная система расчета очков опыта у игроков исходит от уровня игры игроков (аналог HlStats).
//
//		2 - Рейтинговая система (простая).
//		Данная система подсчета очков опыта является аналогией системы подсчета из RankMe.
//
//		Если вы хотите смените тип статистики, обнулите полностью данные статситики в Базе Данных.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_type_statistics": "0",

  // Какие флаги должен иметь администратор, чтобы иметь доступ к Панели Администратора?
  "lr_flag_adminmenu": "@lr/admin",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//		Какой Заголовок у меню плагина должен быть, предназначено для более продвинутых проектов
//		которые любят кастомизировать свои сервера.
//
//		ВНИМАНИЕ: все префиксы в чате (Пример: [LR]), вы также можете поменять в файле перевода.
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_plugin_title": "Levels Ranks v1.1.0",

  // Проигрывать ли звуки повышения/понижения ранга? [ 0 - нет, 1 - да ]
  "lr_sound": "1",

  // Путь до файла звука, который будет проигрываться при повышении ранга (указывать без sound/).
  "lr_sound_lvlup": "sounds/levels_ranks/levelup.vsnd_c",

  // Путь до файла звука, который будет проигрываться при понижении ранга (указывать без sound/).
  "lr_sound_lvldown": "sounds/levels_ranks/leveldown.vsnd_c",

  // Включить возможность игрокам сбросить свою статистику в меню "Статистика"? [ 0 - нет, 1 - да ].
  "lr_show_resetmystats": "1",

  // Сколько секунд должно пройти, чтобы можно было повторно сбросить свою статистику?
  "lr_resetmystats_cooldown": "86400",

  // Минимальное количество игроков, необходимое для выдачи очков опыта.
  // Количество игроков проверяется при старте раунда.
  "lr_minplayers_count": "4",

//////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//		Как показывать сообщения игроку о получении очков опыта?
//
//		0 - не показывать
//		1 - показывать за каждое действие игрока
//		2 - показывать в конце раунда суммарное изменение
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
  "lr_show_usualmessage": "1",

  // Показывать ли сообщения от плагина, при каждом возрождении? [ 0 - нет, 1 - да ]
  "lr_show_spawnmessage": "1",

  // Показывать ли сообщения всем о том, что кто-то поднял свое звание? [ 0 - нет, 1 - да ]
  "lr_show_levelup_message": "1",

  // Показывать ли сообщения всем о том, что кто-то потерял свое звание? [ 0 - нет, 1 - да ]
  "lr_show_leveldown_message": "1",

  // Показывать ли всем сообщение о том, какое место занимает игрок, после того, как он написал команду rank? [ 0 - нет, 1 - да ]
  "lr_show_rankmessage": "1",

  // Показывать ли в меню статистики пункт "Все звания"? [ 0 - нет, 1 - да ]
  "lr_show_ranklist": "1",

  // Разрешить ли игрокам получать/терять очки опыта, когда раунд завершился? [ 0 - нет, 1 - да ]
  "lr_giveexp_roundend": "1",

  // Выдавать ли игрокам очки опыта во время разминки? [ 0 - да, 1 - нет ]
  "lr_block_warmup": "1",

  // Считать ли убийство товарищей по команде за TeamKill? [ 0 - да, 1 - нет (нужно для серверов с режимом "Все против Всех") ]
  "lr_allagainst_all": "1",

  // Выдавать ли игрокам очки опыта при взаимодействии с ботами [ 0 - нет, 1 - да ]
  "lr_experience_from_bots": "0"
}
```

# Конфигурационный файл с настройкой рангов (settings_ranks.json)
```
{
//	Все наименования рангов записаны в файле ( addons/counterstrikesharp/plugins/[LR] Core/lang)  - там вы сможете менять названия рангов
//	Заголовок блока ранга отвечает за фразу перевода, которая будет использоваться из ( lang )
//	Если вы хотите внести свои ранги, то создав блок здесь, создайте блок в ( lang ) (заголовок блока должен соответствовать заголовку блока в файле перевода)
//
//	Если вы собираетесь вводить свои значения очков опыта для рангов из lr_type_statistics 1 и 2, учтите. У этих режимов стартовое значение 1000.
//	Учитывая, что у них стартовое значение 1000, вам крайне рекомендуется задать такое значение рангу (блоку), который находится посередине между остальными.
//	Среди стандартных 18 рангов, серединой считается 9, и именно в блоке rank_9 задано значение 1000.
//
//  Параметры, которые вы изменили здесь, можно обновить на сервере консольной командой ( css_lvl_reload ) или из Панели Администратора в меню плагина, не перезагружая сервер.

  // 1 ранг не трограть 
  "1": {  
    "Value0": 0, // Сколько очков опыта нужно иметь, чтобы был доступен этот ранг (если используется lr_type_statistics 0)
    "Value1": 0, // Сколько очков опыта нужно иметь, чтобы был доступен этот ранг (если используется lr_type_statistics 1)
    "Value2": 0 // Сколько очков опыта нужно иметь, чтобы был доступен этот ранг (если используется lr_type_statistics 2)
  },
  "2": {
    "Value0": 10,
    "Value1": 700,
    "Value2": 850
  },
  "3": {
    "Value0": 25,
    "Value1": 800,
    "Value2": 900
  },
  "4": {
    "Value0": 50,
    "Value1": 850,
    "Value2": 935
  },
  "5": {
    "Value0": 75,
    "Value1": 900,
    "Value2": 950
  },
  "6": {
    "Value0": 100,
    "Value1": 925,
    "Value2": 965
  },
  "7": {
    "Value0": 150,
    "Value1": 950,
    "Value2": 980
  },
  "8": {
    "Value0": 200,
    "Value1": 975,
    "Value2": 990
  },
  "9": {
    "Value0": 300,
    "Value1": 1000,
    "Value2": 1000
  },
  "10": {
    "Value0": 500,
    "Value1": 1100,
    "Value2": 1050
  },
  "11": {
    "Value0": 750,
    "Value1": 1250,
    "Value2": 1100
  },
  "12": {
    "Value0": 1000,
    "Value1": 1400,
    "Value2": 1200
  },
  "13": {
    "Value0": 1500,
    "Value1": 1600,
    "Value2": 1300
  },
  "14": {
    "Value0": 2000,
    "Value1": 1800,
    "Value2": 1400
  },
  "15": {
    "Value0": 3000,
    "Value1": 2100,
    "Value2": 1550
  },
  "16": {
    "Value0": 5000,
    "Value1": 2400,
    "Value2": 1750
  },
  "17": {
    "Value0": 7500,
    "Value1": 3000,
    "Value2": 2000
  },
  "18": {
    "Value0": 10000,
    "Value1": 4000,
    "Value2": 2500
  }
}
```

# Конфигурационный файл с настройками количества выдаваемого опыта (settings_stats.json)
```
{
//  Параметры, которые вы изменили здесь, можно обновить на сервере консольной командой ( css_lvl_reload ) или из Панели Администратора в меню плагина, не перезагружая сервер.

  // Накопительная система.
  "Funded_System": {
		// Для отключения выдачи очков опыта за конкретное действие, напишите 0 в нужном вам параметре.
		// Сколько очков опыта игрок:
    "lr_kill": 5, // получит за убийство
    "lr_death": -5, // потеряет за свою смерть
    "lr_headshot": 1, // получит за убийств в голову
    "lr_assist": 1, // получит за помощь в убийстве
    "lr_suicide": -6, // потеряет за суицид
    "lr_teamkill": -6, // потеряет за убийство товарища по команде
    "lr_winround": 2, // получит за победу в раунде
    "lr_loseround": -2, // потеряет за проигрыш в раунде
    "lr_mvpround": 3, // получит за лучшую результативность в раунде (MVP)
    "lr_bombplanted": 2, // получит за установку бомбы
    "lr_bombdefused": 2, // получит за разминирование бомбы
    "lr_bombdropped": -1, // потеряет за потерю бомбы
    "lr_bombpickup": 1, // получит за поднятие бомбы
    "lr_hostagekilled": -4, // потеряет за убийство заложника
    "lr_hostagerescued": 3 // получит за спасение заложника
  },
  // Рейтинговая система (расширенная).
  "Rating_Extended": {
		// Коэффициент вычитания очков опыта при смерти игрока (использовать на свой страх и риск, т.к. имеет сильное влияние на распределение игроков по рангам)
		//
		// Максимум: 1.20 - игрок теряет на 20 процентов больше, чем получает за него убийца (практически невозможно подняться до высших званий)
		// Стандарт: 1.00 - игрок теряет столько же очков опыта, сколько получает убийца
		// Минимум: 0.80 - игрок теряет только 80 процентов очков опыта от реального значения (подняться до высших званий станет намного проще)
		// 
    "lr_killcoeff": 1, // Рекомендуемое значение: 1.04 (теоретически).

		// Для отключения выдачи очков опыта за конкретное действие, напишите 0 в нужном вам параметре
		// Сколько очков опыта игрок:

    "lr_headshot": 1, // получит за убийств в голову
    "lr_assist": 1, // получит за помощь в убийстве
    "lr_suicide": -10, // потеряет за суицид
    "lr_teamkill": -5, // потеряет за убийство товарища по команде
    "lr_winround": 2, // получит за победу в раунде
    "lr_loseround": -2, // потеряет за проигрыш в раунде
    "lr_mvpround": 1, // получит за лучшую результативность в раунде (MVP)
    "lr_bombplanted": 3, // получит за установку бомбы
    "lr_bombdefused": 3, // получит за разминирование бомбы
    "lr_bombdropped": -2, // потеряет за потерю бомбы
    "lr_bombpickup": 2, // получит за поднятие бомбы
    "lr_hostagekilled": -20, // потеряет за убийство заложника
    "lr_hostagerescued": 5 // получит за спасение заложника
  },
	// Рейтинговая система (упрощенная).
  "Rating_Simple": {
		// Для отключения выдачи очков опыта за конкретное действие, напишите 0 в нужном вам параметре
		// Сколько очков опыта игрок:

    "lr_headshot": 1, // получит за убийств в голову
    "lr_assist": 1, // получит за помощь в убийстве
    "lr_suicide": 0, // потеряет за суицид
    "lr_teamkill": 0, // потеряет за убийство товарища по команде
    "lr_winround": 2, // получит за победу в раунде
    "lr_loseround": -2, // потеряет за проигрыш в раунде
    "lr_mvpround": 1, // получит за лучшую результативность в раунде (MVP)
    "lr_bombplanted": 2, // получит за установку бомбы
    "lr_bombdefused": 2, // получит за разминирование бомбы
    "lr_bombdropped": -1, // потеряет за потерю бомбы
    "lr_bombpickup": 1, // получит за поднятие бомбы
    "lr_hostagekilled": 0, // потеряет за убийство заложника
    "lr_hostagerescued": 2 // получит за спасение заложника
  },
	// Серии убийств.
  "Special_Bonuses": {
		// Только для [ Funded_System ] и [ Rating_Extended ]
		// Для отключения выдачи очков опыта за конкретное действие, напишите в кавычках 0

    "lr_bonus_1": 2, // DoubleKill
    "lr_bonus_2": 3, // TripleKill
    "lr_bonus_3": 4, // Domination
    "lr_bonus_4": 5, // Rampage
    "lr_bonus_5": 6, // MegaKill
    "lr_bonus_6": 7, // Ownage
    "lr_bonus_7": 8, // UltraKill
    "lr_bonus_8": 9, // KillingSpree
    "lr_bonus_9": 10, // MonsterKill
    "lr_bonus_10": 11, // Unstoppable
    "lr_bonus_11": 12 // GodLike
  }
}
```

# Конфигурационный файл с переводами плагина (ru.json)
```
{
  "rank_up_message": "{DarkRed}[LR] {White}Вы повышены в звании до {Green}{0}",
  "rank_down_message": "{DarkRed}[LR] {White}Вы понижены в звании до {DarkRed}{0}",
  "rank_up_broadcast": "{DarkRed}[LR] {White}Игрок {Orange}{0} {White}повысил свое звание до {Green}{1}",
  "rank_down_broadcast": "{DarkRed}[LR] {White}Игрок {Orange}{0} {White}понизил свое звание до {DarkRed}{1}",
  "suicide_color": "{DarkRed}",
  "suicide": "за самоубийство",
  "team_kill_color": "{DarkRed}",
  "team_kill": "за убийство товарища",
  "death_color": "{DarkRed}",
  "death": "за вашу смерть",
  "kill_color": "{Green}",
  "kill": "за убийство",
  "headshot_color": "{Yellow}",
  "headshot": "за убийство в голову",
  "assist_color": "{Blue}",
  "assist": "за помощь в убийстве",
  "win_round_color": "{Green}",
  "win_round": "ваша команда выиграла",
  "lose_round_color": "{DarkRed}",
  "lose_round": "ваша команда проиграла",
  "mvp_round_color": "{Blue}",
  "mvp_round": "MVP",
  "bomb_pickup_color": "{Green}",
  "bomb_pickup": "подобрали бомбу",
  "bomb_dropped_color": "{DarkRed}",
  "bomb_dropped": "потеряли бомбу",
  "bomb_defused_color": "{Green}",
  "bomb_defused": "разминирование бомбы",
  "bomb_planted_color": "{Green}",
  "bomb_planted": "установка бомбы",
  "hostage_rescued_color": "{LightPurple}",
  "hostage_rescued": "вы спасли заложника",
  "hostage_killed_color": "{DarkRed}",
  "hostage_killed": "вы убили заложника",
  "killstreak_bonus_color": "{Purple}",
  "killstreak_bonus": "за {0}",
  "killstreak_2_color": "{Purple}",
  "killstreak_2": "DoubleKill",
  "killstreak_3_color": "{Purple}",
  "killstreak_3": "TripleKill",
  "killstreak_4_color": "{Purple}",
  "killstreak_4": "Domination",
  "killstreak_5_color": "{Purple}",
  "killstreak_5": "Rampage",
  "killstreak_6_color": "{Purple}",
  "killstreak_6": "MegaKill",
  "killstreak_7_color": "{Purple}",
  "killstreak_7": "Ownage",
  "killstreak_8_color": "{Purple}",
  "killstreak_8": "UltraKill",
  "killstreak_9_color": "{Purple}",
  "killstreak_9": "KillingSpree",
  "killstreak_10_color": "{Purple}",
  "killstreak_10": "MonsterKill",
  "killstreak_11_color": "{Purple}",
  "killstreak_11": "Unstoppable",
  "killstreak_12_color": "{Purple}",
  "killstreak_12": "GodLike",
  "round_summary_positive": "{DarkRed}[LR] {White}За раунд вы заработали {Green}{0} {White}очков опыта",
  "round_summary_negative": "{DarkRed}[LR] {White}За раунд вы потеряли {Red}{0} {White}очков опыта",
  "round_summary_neutral": "{DarkRed}[LR] {White}За раунд вы не заработали очков опыта!",
  "round_start_few_players": "{DarkRed}[LR] {White}Присутствует только {DarkRed}{0} {White}игрока из {DarkRed}{1}",
  "round_start_message": "{DarkRed}[LR] {White}Введи: {Purple}!lvl {White}- главное меню",
  "command_player_only": "{DarkRed}[LR] {White}Команда может быть вызвана только игроком.",
  "player_not_found": "{DarkRed}[LR] {White}Игрок {Green}{0} {White}не найден.",
  "player_stats": "{DarkRed}[LR] {White}Игрок {Green}{0} {White}на {Green}{1} / {2} {White}месте, с {Orange}{3} {White}очками, {Orange}{4} {White}убийствами, {Orange}{5} {White}смертями и KDR {Orange}{6:F2}",
  "command_error": "{Red}[LR] {White}Произошла ошибка при выполнении команды.",
  "command_console_only": "{DarkRed}[LR] {White}Эта команда доступна только из консоли сервера.",
  "menu_api_not_found": "{DarkRed}[LR] {White}Меню API не найдено!",
  "user_data_not_found": "{DarkRed}[LR] {White}Данные пользователя не найдены!",
  "admin_panel": "Панель администратора",
  "my_stats": "Моя статистика",
  "top_players": "TOP игроков",
  "all_ranks": "Все ранги",
  "rank_item": "[ {0} ] {1}",
  "top_10_experience": "TOP-10 по очкам опыта",
   "top_player_item": "{0}. [ {1} ] {2}",
  "top_10_activity": "TOP-10 по активности",
  "top_player_activity_item": "{0}. [ {1} ] {2}",
  "show_stats_in_chat": "Вывести статистику в чат",
  "reset_stats": "Сбросить статистику",
  "user_stats_title": "{DarkRed}[LR] {White}Ваша статистика:",
  "user_stat_experience": "- Опыт: {Green}{0}",
  "user_stat_kills": "- Убийства: {Green}{0}",
  "user_stat_deaths": "- Смерти: {Green}{0}",
  "user_stat_headshots": "- Хедшоты: {Green}{0}",
  "user_stat_assists": "- Ассисты: {Green}{0}",
  "user_stat_shoots": "- Выстрелы: {Green}{0}",
  "user_stat_hits": "- Попадания: {Green}{0}",
  "user_stat_round_win": "- Раунды выиграны: {Green}{0}",
  "user_stat_round_lose": "- Раунды проиграны: {Green}{0}",
  "user_stat_playtime": "- Время в игре: {Green}{0}",
  "reset_stats_cooldown": "{DarkRed}[LR] {White}Сброс статистики будет доступен через {0}д {1}ч {2}м {3}с.",
  "stats_reset_success": "{DarkRed}[LR] {White}Ваша статистика была сброшена.",
  "grant_revoke_points": "Выдать/Забрать очки опыта",
  "reload_plugin_settings": "Перезагрузить настройки плагина",
  "plugin_settings_reloaded": "{DarkRed}[LR] {White}Настройки плагина были успешно перезагружены.",
  "grant_points": "Выдать",
  "revoke_points": "Забрать",
  "select_player": "Выберите игрока",
  "points": "очков",
  "admin_grant_points": "администратор выдал очки опыта",
  "admin_revoke_points": "администратор забрал очки опыта",
  "points_granted": "{DarkRed}[LR] {White}Выдано {Green}{0} {White}очков у игрока {Green}{1}{White}.",
  "points_revoked": "{DarkRed}[LR] {White}Забрано {DarkRed}{0} {White}очков у игрока {Green}{1}{White}.",
  "user_data_not_found_for_player": "Не удалось найти данные пользователя для {0}.",
  "experience_message": "{DarkRed}[LR] {White}Ваш опыт: {0}{1} [{2} {3}]",
  "rank_1": "Серебро - I",
  "rank_2": "Серебро - II",
  "rank_3": "Серебро - III",
  "rank_4": "Серебро - IV",
  "rank_5": "Серебро Элита",
  "rank_6": "Серебро - Великий Магистр",
  "rank_7": "Золотая Звезда - I",
  "rank_8": "Золотая Звезда - II",
  "rank_9": "Золотая Звезда - III",
  "rank_10": "Золотая Звезда - Магистр",
  "rank_11": "Магистр-хранитель - I",
  "rank_12": "Магистр-хранитель - II",
  "rank_13": "Магистр-хранитель - Элита",
  "rank_14": "Заслуженный Магистр-хранитель",
  "rank_15": "Легендарный Беркут",
  "rank_16": "Легендарный Беркут-магистр",
  "rank_17": "Великий Магистр - Высшего Ранга",
  "rank_18": "Всемирная Элита"
}
```

# Конфигурационный файл для подключения базы данных (database.json)
```
{
  "Database": "levelsranks",
  "User": "root",
  "Password": "",
  "Host": "localhost",
  "Port": 3306
}
```
