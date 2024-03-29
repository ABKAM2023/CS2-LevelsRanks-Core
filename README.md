![GitHub Repo stars](https://img.shields.io/github/stars/ABKAM2023/CS2-LevelsRanks-Core?style=for-the-badge)
![GitHub all releases](https://img.shields.io/github/downloads/ABKAM2023/CS2-LevelsRanks-Core/total?style=for-the-badge)

I'm sorry for my poor English.
# EN
**Levels Ranks** system is based on a simple principle: players perform various actions in the game, as a result of which they either gain or lose experience points. Achieving or losing a certain amount of these points leads to receiving a corresponding rank. The number of available ranks can be configured and edited at discretion.

# Installation
1. Install [Metamod:Source](https://www.sourcemm.net/downloads.php?branch=master) and [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (minimal version 188)
2. Download [C#] [Levels Ranks] Core
3. Unpack the archive and upload it to the game server
4. Start the server to create the necessary configuration files.
5. Connect the plugin to the database by entering the required data into the dbconfig.json file. Ensure the accuracy of the entered data.

# Commands
- `!lvl` opens the main statistics menu
- `!rank` shows basic information about your progress

# Variables
- `css_lvl_reset` clears player's statistics. Usage: css_lvl_reset `<steamid64>` `<data-type>` (data-type: `exp` clears values, rank; `stats` clears kills, deaths, shoots, hits, headshots, assists, round_win, round_lose; `time` clears playtime)
- `css_lvl_reload` reloads configuration files settings.yml, settings_ranks.yml, settings_stats.yml, lr_core.phrases.yml
- `css_lvl_give_points` awards experience points to a player. Usage: css_lvl_give_points `<steamid64>` `<amount>`
- `css_lvl_take_points` takes away experience points from a player. Usage: css_lvl_take_point `<steamid64>` `<amount>`

# Main Configuration File (settings.yml)
```
# Levels Ranks Plugin Configuration

# Minimum number of players required for experience points to be awarded
MinPlayersForExperience: 4

# Enable or disable points awarded for killing bots
GivePointsForBotKills: false

# Enable or disable the !rank command
IsRankCommandEnabled: true

# Enable or disable extra experience for special nicknames
EnableSpecialNicknameBonus: true

# Experience multiplier for special nicknames
BonusMultiplierForSpecialNickname: 2

# String to look for in a nickname to apply the multiplier
SpecialNicknameContains: "example.com"

# How many hours before statistics can be reset again
ResetStatsCooldownHours: 3

# Enable or disable kill streaks
# If true, kill streaks will be tracked and award extra points
EnableKillStreaks: true
```

# Configuration File with Rank Settings (settings_ranks.yml)
```
- id: 0
  name: Silver - I
  minExperience: 0
- id: 1
  name: Silver - II
  minExperience: 10
- id: 2
  name: Silver - III
  minExperience: 25
- id: 3
  name: Silver - IV
  minExperience: 50
- id: 4
  name: Silver Elite
  minExperience: 75
- id: 5
  name: Silver - Master Guardian
  minExperience: 100
- id: 6
  name: Gold Nova - I
  minExperience: 150
- id: 7
  name: Gold Nova - II
  minExperience: 200
- id: 8
  name: Gold Nova - III
  minExperience: 300
- id: 9
  name: Gold Nova - Master
  minExperience: 500
- id: 10
  name: Master Guardian - I
  minExperience: 750
- id: 11
  name: Master Guardian - II
  minExperience: 1000
- id: 12
  name: Master Guardian - Elite
  minExperience: 1500
- id: 13
  name: Distinguished Master Guardian
  minExperience: 2000
- id: 14
  name: Legendary Eagle
  minExperience: 3000
- id: 15
  name: Legendary Eagle Master
  minExperience: 5000
- id: 16
  name: Supreme Master First Class
  minExperience: 7500
- id: 17
  name: Global Elite
  minExperience: 10000
```

# Configuration File with Experience Amount Settings (settings_stats.yml)
```
# Experience Awarding Configuration

# Statistics Settings
# Points for Kill - The amount of points added to a player for killing an opponent.
PointsForKill: 5
# Points for Death - The amount of points subtracted from a player for dying.
PointsForDeath: -5
# Points for Headshot - Additional points for killing with a headshot.
PointsForHeadshot: 1
# Points for Assist - The amount of points added to a player for assisting in a kill.
PointsForAssist: 1
# Points for Suicide - The amount of points subtracted from a player for committing suicide.
PointsForSuicide: -6
# Points for Round Win - The amount of points added to a player for their team winning a round.
PointsPerRoundWin: 2
# Points for Round Loss - The amount of points subtracted from a player for their team losing a round.
PointsPerRoundLoss: -2
# Points for MVP - The amount of points added to a player for earning the MVP title of the round.
PointsPerMVP: 3
# Points for Bomb Planting - The amount of points added to a player for successfully planting the bomb.
PointsForBombPlanting: 2
# Points for Bomb Dropping - The amount of points subtracted from a player for dropping the bomb.
PointsForBombDropping: -2
# Points for Bomb Pickup - The amount of points added to a player for picking up the bomb.
PointsForBombPickup: 1
# Points for Bomb Defusal - The amount of points added to a player for defusing the bomb.
PointsForBombDefusal: 2
# Points for Bomb Explosion - The amount of points added to a player for the bomb exploding.
PointsForBombExploded: 2
# Points for Hostage Follows - The amount of points added to a player for a hostage following them.
PointsForHostageFollows: 2
# Points for Hostage Stops Following - The amount of points subtracted from a player for a hostage stops following them.
PointsForHostageStopsFollowing: -2
# Points for Hostage Rescued - The amount of points added to a player for rescuing a hostage.
PointsForHostageRescued: 4
# Points for DoubleKill.
PointsForDoubleKill: 2
# Points for TripleKill.
PointsForTripleKill: 3
# Points for Domination.
PointsForDomination: 4
# Points for Rampage.
PointsForRampage: 5
# Points for MegaKill.
PointsForMegaKill: 6
# Points for Ownage.
PointsForOwnage: 7
# Points for UltraKill.
PointsForUltraKill: 8
# Points for KillingSpree.
PointsForKillingSpree: 9
# Points for MonsterKill.
PointsForMonsterKill: 10
# Points for Unstoppable.
PointsForUnstoppable: 11
# Points for GodLike.
PointsForGodLike: 12
```

# Plugin Messages Configuration File (lr_core.phrases.yml)
```
# Plugin Message Settings

# Prefix before the message
Prefix: " {DarkRed}[LR]{White} "
# This message is sent at the beginning of each round
IntroMessage: "Enter: {Purple}!lvl {White}- main menu"
# Message about the number of active players
GetActivePlayerCountMsg: "Only {DarkRed}{CURRENT_PLAYERS} {White}players present out of {DarkRed}{MIN_PLAYERS}"
# Messages upon gaining experience
PointsChangeMessage: "Your experience:{COLOR} {POINTS} [{SIGN}{CHANGE_POINTS} for {REASON}]"
# Events
SuicideMessage: "suicide"
SuicideMessageColor: "{DarkRed}"
DeathMessage: "death"
DeathMessageColor: "{DarkRed}"
KillMessage: "kill"
KillMessageColor: "{Green}"
HeadshotMessage: "headshot"
HeadshotMessageColor: "{Yellow}"
AssistMessage: "assist"
AssistMessageColor: "{Blue}"
RoundWinMessage: "round win"
RoundWinMessageColor: "{Green}"
RoundLossMessage: "round loss"
RoundLossMessageColor: "{DarkRed}"
MVPMessage: "MVP"
MVPMessageColor: "{Gold}"
BombDefusalMessage: "bomb defusal"
BombDefusalMessageColor: "{Green}"
BombExplodedMessage: "bomb exploded"
BombExplodedMessageColor: "{Green}"
BombPlantingMessage: "bomb planting"
BombPlantingMessageColor: "{Green}"
BombDroppingMessage: "bomb dropping"
BombDroppingMessageColor: "{DarkRed}"
BombPickupMessage: "bomb pickup"
BombPickupMessageColor: "{Green}"
HostageFollowsMessage: "hostage follows"
HostageFollowsMessageColor: "{Green}"
HostageStopsFollowingMessage: "hostage stops following"
HostageStopsFollowingMessageColor: "{DarkRed}"
HostageRescuedMessage: "hostage rescued"
HostageRescuedMessageColor: "{Blue}"
# Kill Streaks
DoubleKillMessage: "Doublekill"
DoubleKillMessageColor: "{Purple}"
TripleKillMessage: "Triplekill"
TripleKillMessageColor: "{Purple}"
DominationMessage: "Domination"
DominationMessageColor: "{Purple}"
RampageMessage: "Rampage"
RampageMessageColor: "{Purple}"
MegaKillMessage: "Megakill"
MegaKillMessageColor: "{Purple}"
OwnageMessage: "Ownage"
OwnageMessageColor: "{Purple}"
UltraKillMessage: "Ultrakill"
UltraKillMessageColor: "{Purple}"
KillingSpreeMessage: "Killingspree"
KillingSpreeMessageColor: "{Purple}"
MonsterKillMessage: "Monsterkill"
MonsterKillMessageColor: "{Purple}"
UnstoppableMessage: "Unstoppable"
UnstoppableMessageColor: "{Purple}"
GodLikeMessage: "Godlike"
GodLikeMessageColor: "{Purple}"

# !rank
RankCommandMessage : "Rank: {Green}{RANK_NAME} {White}| Position: {Blue}{PLACE}/{TOTAL_PLAYERS} {White}| Experience: {Gold}{POINTS} {White}| Kills: {Green}{KILLS} {White}| Deaths: {DarkRed}{DEATHS} {White}| KDR: {Yellow}{KDR} {White}| Time on server: {Gold}{PLAY_TIME}"
TimeFormat: "{0}d {1}h {2}min"

# Main Menu
# Title of the main menu.
MainMenuTitle: "Main Menu"
# Menu option for statistics.
StatsMenuOption: "-> Statistics"
# Menu option for top players.
TopPlayersMenuOption: "-> Top Players"
# Menu option for the list of ranks.
RanksListMenuOption: "-> List of All Ranks"

# 'Top Players' Menu
# Title of the 'Top Players' menu.
TopPlayersMenuTitle: "Top Players"
# Menu option for top 10 by experience points.
TopPointsOption: "-> Top 10 by Experience Points"
# Menu option for top 10 by kills.
TopKillsOption: "-> Top 10 by Kills"
# Menu option for top 10 by deaths.
TopDeathsOption: "-> Top 10 by Deaths"
# Menu option for top 10 by KDR.
TopKDROption: "-> Top 10 by KDR"
# Menu option for top 10 by time.
TopTimeOption: "-> Top 10 by Time"
# Option to return to the main menu.
BackToMainMenuOption: "-> {Red}Back to Main Menu"

# 'Statistics' Menu
StatsMenuTitle: "Statistics"
MyStatsOption: "-> My Statistics"
ResetStatsOption: "-> Reset Statistics"
BackToMainMenuFromStatsOption: "-> {Red}Back to Main Menu"

# Confirmation Menu for Resetting Statistics
ResetStatsConfirmationTitle: "Are you sure you want to reset your statistics?"
ResetStatsConfirmOption: "-> {Green}Yes"
ResetStatsDenyOption: "-> {Red}No"

# Ranks List Menu
RanksMenuTitle: "List of Ranks"
RankMessageFormat: "{0} - {1} experience"
```

# Database Connection Configuration File (dbconfig.json)
```
{
  "DbHost": "YourHost",
  "DbUser": "YourUser",
  "DbPassword": "YourPassword",
  "DbName": "YourDatabase"
  "Name": "lvl_base"
}
```

# RU
**Levels Ranks** система базируется на простом принципе: игроки совершают разнообразные действия в игре, в результате которых они либо приобретают, либо теряют очки опыта. Достижение или потеря определенного объема этих очков ведет к получению соответствующего ранга. Количество доступных рангов может быть настроено и отредактировано по усмотрению.

# Установка
1. Установите [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master) и [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (минимальная версия 188)
2. Скачайте [C#] [Levels Ranks] Core
3. Распакуйте архив и загрузите его на игровой сервер
4. Запустите сервер, чтобы создать необходимые конфигурационные файлы.
5. Подключите плагин к базе данных, введя необходимые данные в файл dbconfig.json. Убедитесь в корректности введенных данных.

# Команды
- `!lvl` открывает главное меню статистики
- `!rank` показывает основную информацию о вашем прогрессе

# Переменные
- `css_lvl_reset` очищает статистику игрока. Использование: css_lvl_reset `<steamid64>` `<data-type>` (data-type: `exp` очистка values, rank; `stats` очистка kills, deaths, shoots, hits, headshots, assists, round_win, round_lose; `time` очистка playtime)
- `css_lvl_reload` перезагрузка конфигурационных файлов settings.yml, settings_ranks.yml, settings_stats.yml, lr_core.phrases.yml
- `css_lvl_give_points` выдаёт очки опыта игроку. Использование: css_lvl_give_points `<steamid64>` `<amount>`
- `css_lvl_take_points` забирает очки опыта у игрока. Использование: css_lvl_take_point `<steamid64>` `<amount>`

# Основной конфигурационный файл (settings.yml)
```
# Настройка плагина Levels Ranks

# Минимальное количество игроков для начисления опыта
MinPlayersForExperience: 4

# Включение или выключение начисления очков за убийство ботов
GivePointsForBotKills: false

# Включение или выключение команды !rank
IsRankCommandEnabled: true

# Включение или выключение дополнительного опыта для специальных никнеймов
EnableSpecialNicknameBonus: true

# Множитель опыта для специальных никнеймов
BonusMultiplierForSpecialNickname: 2

# Строка, которую нужно искать в никнейме для применения множителя
SpecialNicknameContains: "example.com"

# Через сколько часов можно будет снова сбросить статистику
ResetStatsCooldownHours: 3

# Включение или выключение серий убийств
# Если true, серии убийств будут учитываться и давать дополнительные очки
EnableKillStreaks: true
```

# Конфигурационный файл с настройкой рангов (settings_ranks.yml)
```
- id: 0
  name: Серебро - I
  minExperience: 0
- id: 1
  name: Серебро - II
  minExperience: 10
- id: 2
  name: Серебро - III
  minExperience: 25
- id: 3
  name: Серебро - IV
  minExperience: 50
- id: 4
  name: Серебро Элита
  minExperience: 75
- id: 5
  name: Серебро - Великий Магистр
  minExperience: 100
- id: 6
  name: Золотая Звезда - I
  minExperience: 150
- id: 7
  name: Золотая Звезда - II
  minExperience: 200
- id: 8
  name: Золотая Звезда - III
  minExperience: 300
- id: 9
  name: Золотая Звезда - Магистр
  minExperience: 500
- id: 10
  name: Магистр-хранитель - I
  minExperience: 750
- id: 11
  name: Магистр-хранитель - II
  minExperience: 1000
- id: 12
  name: Магистр-хранитель - Элита
  minExperience: 1500
- id: 13
  name: Заслуженный Магистр-хранитель
  minExperience: 2000
- id: 14
  name: Легендарный Беркут
  minExperience: 3000
- id: 15
  name: Легендарный Беркут-магистр
  minExperience: 5000
- id: 16
  name: Великий Магистр - Высшего Ранга
  minExperience: 7500
- id: 17
  name: Всемирная Элита
  minExperience: 10000
```

# Конфигурационный файл с настройками количества выдаваемого опыта (settings_stats.yml)
```
# Настройка выдачи опыта

# Настройки статистики
# Очки за убийство - количество очков, добавляемое игроку за убийство противника.
PointsForKill: 5
# Очки за смерть - количество очков, вычитаемое у игрока за смерть.
PointsForDeath: -5
# Очки за выстрел в голову - дополнительные очки за убийство с выстрелом в голову.
PointsForHeadshot: 1
# Очки за помощь - количество очков, добавляемое игроку за помощь в убийстве.
PointsForAssist: 1
# Очки за самоубийство - количество очков, вычитаемое у игрока за самоубийство.
PointsForSuicide: -6
# Очки за победу в раунде - количество очков, добавляемое игроку за победу его команды в раунде.
PointsPerRoundWin: 2
# Очки за проигрыш в раунде - количество очков, вычитаемое у игрока за проигрыш его команды в раунде.
PointsPerRoundLoss: -2
# Очки за MVP - количество очков, добавляемое игроку за получение звания MVP раунда.
PointsPerMVP: 3
# Очки за установку бомбы - количество очков, добавляемое игроку за успешную установку бомбы.
PointsForBombPlanting: 2
# Очки за выброс бомбы - количество очков, вычитаемое у игрока за выброс бомбы.
PointsForBombDropping: -2
# Очки за поднятие бомбы - количество очков, добавляемое игроку за поднятие бомбы.
PointsForBombPickup: 1
# Очки за обезвреживание бомбы
PointsForBombDefusal: 2
# Очки за взрыв бомбы
PointsForBombExploded: 2
# Очки за поднятие заложника
PointsForHostageFollows: 2
# Очки за потерю заложника
PointsForHostageStopsFollowing: -2
# Очки за спасение заложника
PointsForHostageRescued: 4
# Очки за DoubleKill.
PointsForDoubleKill: 2
# Очки за TripleKill.
PointsForTripleKill: 3
# Очки за Domination.
PointsForDomination: 4
# Очки за Rampage.
PointsForRampage: 5
# Очки за MegaKill.
PointsForMegaKill: 6
# Очки за Ownage.
PointsForOwnage: 7
# Очки за UltraKill.
PointsForUltraKill: 8
# Очки за KillingSpree.
PointsForKillingSpree: 9
# Очки за MonsterKill.
PointsForMonsterKill: 10
# Очки за Unstoppable.
PointsForUnstoppable: 11
# Очки за GodLike.
PointsForGodLike: 12
```

# Конфигурационный файл со сообщениями плагина (lr_core.phrases.yml)
```
# Настройка сообщений плагина

# Префикс перед сообщением
Prefix: " {DarkRed}[LR]{White} "
# Данное сообщение отправляется в начале каждого раунда
IntroMessage: "Введи: {Purple}!lvl {White}- главное меню"
# Сообщение о количестве активных игроков
GetActivePlayerCountMsg: "Присутствует только {DarkRed}{CURRENT_PLAYERS} {White}игроков из {DarkRed}{MIN_PLAYERS}"
# Сообщения при получении опыта
PointsChangeMessage: "Ваш опыт:{COLOR} {POINTS} [{SIGN}{CHANGE_POINTS} за {REASON}]"
# События
SuicideMessage: "самоубийство"
SuicideMessageColor: "{DarkRed}"
DeathMessage: "смерть"
DeathMessageColor: "{DarkRed}"
KillMessage: "убийство"
KillMessageColor: "{Green}"
HeadshotMessage: "выстрел в голову"
HeadshotMessageColor: "{Yellow}"
AssistMessage: "ассист"
AssistMessageColor: "{Blue}"
RoundWinMessage: "победа в раунде"
RoundWinMessageColor: "{Green}"
RoundLossMessage: "проигрыш в раунде"
RoundLossMessageColor: "{DarkRed}"
MVPMessage: "MVP"
MVPMessageColor: "{Gold}"
BombDefusalMessage: "обезвреживание бомбы"
BombDefusalMessageColor: "{Green}"
BombExplodedMessage: "взрыв бомбы"
BombExplodedMessageColor: "{Green}"
BombPlantingMessage: "установку бомбы"
BombPlantingMessageColor: "{Green}"
BombDroppingMessage: "выброс бомбы"
BombDroppingMessageColor: "{DarkRed}"
BombPickupMessage: "поднятие бомбы"
BombPickupMessageColor: "{Green}"
HostageFollowsMessage: "заложник следует"
HostageFollowsMessageColor: "{Green}"
HostageStopsFollowingMessage: "заложник перестал следовать"
HostageStopsFollowingMessageColor: "{DarkRed}"
HostageRescuedMessage: "заложник спасен"
HostageRescuedMessageColor: "{Blue}"
# Серии убийств
DoubleKillMessage: "Doublekill"
DoubleKillMessageColor: "{Purple}"
TripleKillMessage: "Triplekill"
TripleKillMessageColor: "{Purple}"
DominationMessage: "Domination"
DominationMessageColor: "{Purple}"
RampageMessage: "Rampage"
RampageMessageColor: "{Purple}"
MegaKillMessage: "Megakill"
MegaKillMessageColor: "{Purple}"
OwnageMessage: "Ownage"
OwnageMessageColor: "{Purple}"
UltraKillMessage: "Ultrakill"
UltraKillMessageColor: "{Purple}"
KillingSpreeMessage: "Killingspree"
KillingSpreeMessageColor: "{Purple}"
MonsterKillMessage: "Monsterkill"
MonsterKillMessageColor: "{Purple}"
UnstoppableMessage: "Unstoppable"
UnstoppableMessageColor: "{Purple}"
GodLikeMessage: "Godlike"
GodLikeMessageColor: "{Purple}"
# Настройки сообщений о точках
PointsGivenReason: "администратор выдал"
PointsGivenColor: "{Green}"
PointsTakenReason: "администратор забрал"
PointsTakenColor: "{DarkRed}"

# !rank
RankCommandMessage : "Звание: {Green}{RANK_NAME} {White}| Место: {Blue}{PLACE}/{TOTAL_PLAYERS} {White}| Опыт: {Gold}{POINTS} {White}| Убийства: {Green}{KILLS} {White}| Смерти: {DarkRed}{DEATHS} {White}| KDR: {Yellow}{KDR} {White}| Время на сервере: {Gold}{PLAY_TIME}"
TimeFormat: "{0}д {1}ч {2}мин"

# !top
TopCommandIntroMessage : "[ {Blue}Топ игроков{White} ]"
TopCommandPlayerMessage: "{INDEX}. {Grey}{NAME} - {White}{RANK} {Grey}- {Blue}{POINTS} очков"
TopCommandNoDataMessage: "[ {DarkRed}Ошибка{White} ] Нет данных о топ игроках."
TopCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."

# !topkills
TopKillsCommandIntroMessage: "[ {Green}Топ игроков по убийствам{White} ]"
TopKillsCommandPlayerMessage: "{INDEX}. {Grey}{NAME} - {Green}{KILLS} убийств{White}"
TopKillsCommandNoDataMessage: "[ {DarkRed}Ошибка{White} ] Нет данных о топ игроках по убийствам."
TopKillsCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."

# !topdeaths
TopDeathsCommandIntroMessage: "[ {DarkRed}Топ игроков по смертям{White} ]"
TopDeathsCommandPlayerMessage: "{INDEX}. {Grey}{NAME}{White} - {DarkRed}{DEATHS} смертей{White}"
TopDeathsCommandNoDataMessage: "[ {DarkRed}Ошибка{White} ] Нет данных о топ игроках по смертям."
TopDeathsCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."

# !topkdr
TopKDRCommandIntroMessage: "[ {Yellow}Топ игроков по KDR{White} ]"
TopKDRCommandPlayerMessage: "{INDEX}. {Grey}{NAME}{White} - {Yellow}KDR: {KDR}"
TopKDRCommandNoDataMessage: "[ {DarkRed}Ошибка{White} ] Нет данных о топ игроках по KDR."
TopKDRCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."

# !toptime
TopTimeCommandIntroMessage: "[ {Gold}Топ игроков по времени на сервере{White} ]"
TopTimeCommandPlayerMessage: "{INDEX}. {Grey}{NAME} - {Gold}{TIME}{White}"
TopTimeCommandNoDataMessage : "[ {DarkRed}Ошибка{White} ] Нет данных о топ игроках по времени на сервере."
TopTimeCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."
TopTimeFormat: "{0}д {1}ч {2}мин"

# !resetstats
ResetStatsCooldownMessage: " {DarkRed}[LR]{White}  Сбросить статистику можно только раз в 3 часа."
ResetStatsSuccessMessage: " {DarkRed}[LR]{White}  Ваша статистика сброшена."

# !ranks
RanksCommandIntroMessage: "[ {Gold}Список званий{White} ]"
RanksCommandRankMessage: "{NAME} - {Green}{EXPERIENCE} опыта{White}"
RanksCommandNoDataMessage: "[ {DarkRed}Ошибка{White} ] Нет данных о званиях."
RanksCommandErrorMessage: "[ {DarkRed}Ошибка{White} ] Произошла ошибка при выполнении команды."

# Главное меню
# Заголовок главного меню.
MainMenuTitle: "Главное меню"
# Опция меню для статистики.
StatsMenuOption: "-> Статистика"
# Опция меню для топ игроков.
TopPlayersMenuOption: "-> Топ игроков"
# Опция меню для списка званий.
RanksListMenuOption: "-> Список всех званий"

# Меню 'Топ игроков'
# Заголовок меню 'Топ игроков'.
TopPlayersMenuTitle: "Топ игроков"
# Опция меню для топ 10 по очкам опыта.
TopPointsOption: "-> Топ 10 по очкам опыта"
# Опция меню для топ 10 по убийствам.
TopKillsOption: "-> Топ 10 по убийствам"
# Опция меню для топ 10 по смертям.
TopDeathsOption: "-> Топ 10 по смертям"
# Опция меню для топ 10 по KDR.
TopKDROption: "-> Топ 10 по KDR"
# Опция меню для топ 10 по времени.
TopTimeOption: "-> Топ 10 по времени"
# Опция для возвращения в главное меню.
BackToMainMenuOption: "-> {Red}Назад в главное меню"

# Меню 'Статистика'
StatsMenuTitle: "Статистика"
MyStatsOption: "-> Моя статистика"
ResetStatsOption: "-> Сброс статистики"
BackToMainMenuFromStatsOption: "-> {Red}Назад в главное меню"

# Меню подтверждения сброса статистики
ResetStatsConfirmationTitle: "Вы уверены, что хотите сбросить статистику?"
ResetStatsConfirmOption: "-> {Green}Да"
ResetStatsDenyOption: "-> {Red}Нет"

# Меню списка званий
RanksMenuTitle: "Список званий"
RankMessageFormat: "{0} - {1} опыта"

# Настройки сообщений о точках
PointsGivenReason: "администратор выдал"
PointsGivenColor: "{Green}"
PointsTakenReason: "администратор забрал"
PointsTakenColor: "{DarkRed}"
```

# Конфигурационный файл для подключения базы данных (dbconfig.json)
```
{
  "DbHost": "YourHost",
  "DbUser": "YourUser",
  "DbPassword": "YourPassword",
  "DbName": "YourDatabase"
  "Name": "lvl_base"
}
```
