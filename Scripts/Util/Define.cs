
public class Define
{
    #region Animator Parameters

    public const string IsMove = "IsMove";
    public const string IsDebuff = "IsDebuff";
    public const string IsDeath = "IsDeath";

    public const string Attack = "Attack";
    public const string Damaged = "Damaged";
    public const string Death = "Death";
    public const string Other = "Other";

    #endregion

    #region Tag

    public const string Player_Tag = "Player";
    public const string Enemy_Tag = "Enemy";
    public const string Boss_Tag = "Boss";
    public const string Ground_Tag = "Ground";

    public const string MagicProjectile_Tag = "MagicProjectile";
    public const string ArrowProjectile_Tag = "ArrowProjectile";

    #endregion

    #region Layer

    public const string Player_Layer = "Player";
    public const string Enemy_Layer = "Enemy";

    #endregion

    #region Prefabs Path

    // 스킬 경로
    public const string Baptism_of_Flame_Prefab_Path = "Prefabs/Skill/Baptism of Flame";
    public const string Blessing_of_Regeneration_Prefab_Path = "Prefabs/Skill/Blessing of Regeneration";
    public const string Blessing_of_the_Goddess_Prefab_Path = "Prefabs/Skill/Blessing of the Goddess";
    public const string Curse_of_the_Reaper_Prefab_Path = "Prefabs/Skill/Curse of the Reaper";
    public const string Freeze_Circle_Prefab_Path = "Prefabs/Skill/Freeze Circle";
    public const string Frozen_Judgment_Prefab_Path = "Prefabs/Skill/Frozen Judgment";
    public const string Lightning_Armament_Prefab_Path = "Prefabs/Skill/Lightning Armament";
    public const string Purify_Prefab_Path = "Prefabs/Skill/Purify";

    // 스킬 창 아이콘 경로
    public const string Skill_Icon_Prefab_Path = "Prefabs/Skill Icon/Skill Icon";

    // 적 유닛 Projectile 경로
    public const string Devil_Arrow_Prefab_Path = "Prefabs/Projectile/Devil Arrow";
    public const string Elf_Arrow_Prefab_Path = "Prefabs/Projectile/Elf Arrow";
    public const string Human_Arrow_Prefab_Path = "Prefabs/Projectile/Human Arrow";
    public const string Skeleton_Arrow_Prefab_Path = "Prefabs/Projectile/Skeleton Arrow";
    public const string Red_Fire_Ball_Prefab_Path = "Prefabs/Projectile/Red Fire Ball";
    public const string Green_Fire_Ball_Prefab_Path = "Prefabs/Projectile/Green Fire Ball";
    public const string Blue_Fire_Ball_Prefabs_Path = "Prefabs/Projectile/Blue Fire Ball";

    // 재화 경로
    public const string GoldPrefab = "Prefabs/Goods/Gold";
    public const string DiamondPrefab = "Prefabs/Goods/Diamond";

    // 룬 경로
    public const string RunePrefab_Path = "Prefabs/Rune";

    // 메일 경로
    public const string MailPrefab_Path = "Prefabs/Mail/Mail List";
    public const string RewardPrefab_Path = "Prefabs/Mail/Reward";

    // 공지 사항 경로
    public const string NoticePrefab_Path = "Prefabs/Notice/Notice List";

    // 데미지 텍스트 경로
    public const string DamageText_Path = "Prefabs/DamageText/DamageText";

    #endregion

    #region Scriptable Path

    // 스킬 스크립트 테이블 경로
    public const string Skill_Scriptable_Path = "Scriptable Objects/Skill";
    // 맵 스크립트 테이블 경로
    public const string Map_Scriptable_Path = "Scriptable Objects/Map";

    // Enemy Stat 스크립트 테이블 경로
    public const string Boss_Stat_Scriptable_Path = "Scriptable Objects/Enemy Stat/Boss Stats";
    public const string Archer_Stat_Scriptable_Path = "Scriptable Objects/Enemy Stat/Archer Stats";
    public const string Warrior_Stat_Scriptable_Path = "Scriptable Objects/Enemy Stat/Warrior Stats";
    public const string Wizard_Stat_Scriptable_Path = "Scriptable Objects/Enemy Stat/Wizard Stats";

    // Enemy Drop 스크립트 테이블 경로
    public const string normalDropData = "Scriptable Objects/Item/NormalDropData";
    public const string BossDropData = "Scriptable Objects/Item/BossDropData";

    // 장비 등급 스크립트 테이블 경로
    public const string Common_Path = "Scriptable Objects/Equipment/Common";
    public const string UnCommon_Path = "Scriptable Objects/Equipment/Uncommon";
    public const string Rare_Path = "Scriptable Objects/Equipment/Rare";
    public const string Legendary_Path = "Scriptable Objects/Equipment/Legendary";

    // 룬 스크립트 테이블 경로
    public const string Shadows_Path = "Scriptable Objects/Rune/CriticalDataSo";
    public const string Earth_Path = "Scriptable Objects/Rune/DefenseDataSo";
    public const string Fortune_Path = "Scriptable Objects/Rune/GoldDataSo";
    public const string Rage_Path = "Scriptable Objects/Rune/PowerDataSO";
    public const string Life_Path = "Scriptable Objects/Rune/ReproductionDataSo";
    public const string Swiftness_Path = "Scriptable Objects/Rune/SpeedDataSo";

    #endregion

    #region Sprite Path

    // 스킬 아이콘 경로
    public const string Baptism_of_Flame_Icon_Path = "Sprites/Skill Icon/Baptism of Flame Icon";
    public const string Blessing_of_Regeneration_Icon_Path = "Sprites/Skill Icon/Blessing of Regeneration Icon 1";
    public const string Blessing_of_the_Goddess_Icon_Path = "Sprites/Skill Icon/Blessing of the Goddess Icon";
    public const string Curse_of_the_Reaper_Icon_Path = "Sprites/Skill Icon/Curse of the Reaper Icon";
    public const string Freeze_Circle_Icon_Path = "Sprites/Skill Icon/Freeze Circle Icon";
    public const string Frozen_Judgment_Icon_Path = "Sprites/Skill Icon/Frozen Judgment Icon";
    public const string Lightning_Armament_Icon_Path = "Sprites/Skill Icon/Lightning Armament Icon 1";
    public const string Purify_Icon_Path = "Sprites/Skill Icon/Purify Icon";

    #endregion

    #region Sound Path

    // BGM
    public const string Geralt_of_Rivia_BGM_Path = "Sound/BGM/The Witcher 3 Wild Hunt OST - Geralt of Rivia";
    public const string Night_Market_BGM_Path = "Sound/BGM/Stardew Valley OST - Night Market";

    // SFX
    public const string Meteor_Rain_SFX_Path = "Sound/SFX/Meteor Rain";
    public const string Charming_Twinkle_Sound_for_Fantasy_and_Magic_SFX_Path = "Sound/SFX/Charming Twinkle Sound for Fantasy and Magic";
    public const string Sweep_Sound_Effect_SFX_Path = "Sound/SFX/Sweep Sound Effect";
    public const string Whoosh_Dark_SFX_Path = "Sound/SFX/Whoosh Dark";
    public const string Snow_on_Umbrella_SFX_Path = "Sound/SFX/Snow on Umbrella";
    public const string Glass_Cinematic_Hit_SFX_Path = "Sound/SFX/Glass Cinematic Hit";
    public const string Electric_Sparks_SFX_Path = "Sound/SFX/Electric Sparks";
    public const string Angelic_Pad_Loop_SFX_Path = "Sound/SFX/Angelic Pad Loop";

    public const string Button_SFX_Path = "Sound/SFX/Button";

    #endregion

    #region Constants

    public static readonly int Expiration_Days = 3;
    public static readonly string User_Data_Table = "USER_DATA";
    #endregion
}