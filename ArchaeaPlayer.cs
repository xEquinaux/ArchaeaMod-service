using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using ReLogic.Graphics;

using ArchaeaMod.GenLegacy;
using ArchaeaMod.Mode;
using ArchaeaMod.Interface.UI;
using ArchaeaMod.Progression;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Timers;
using tUserInterface.Extension;
using System.Security.Policy;
using ArchaeaMod.NPCs;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ArchaeaMod.Structure;
using ArchaeaMod.Items;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using ArchaeaMod.Jobs.Projectiles;
using ArchaeaMod.Jobs;
using System.Diagnostics;
using Container = tUserInterface.ModUI.Container;
using tModPorter;

namespace ArchaeaMod
{
    public sealed class ClassID
    {
        public const int
            None = 0,
            All = 5,
            Melee = 1,

            Magic = 3,
            Ranged = 2,
            Throwing = -10,
            Summoner = 4;
    }
    public sealed class PylonID
    {
        public const int 
            Desert = 0,
            Hallow = 1,
            Jungle = 2,
            Mushroom = 3,
            Ocean = 4,
            Purity = 5,
            Snow = 6,
            Underground = 7,
            Victory = 8;
    }
    public struct PlayerClass
    {
        public static PlayerClass NewPlayer(int classChoice, int playerUID, int whoAmI)
        {
            var c = new PlayerClass();
            c.classChoice = classChoice;
            c.playerUID = playerUID;
            c.whoAmI = whoAmI;
            return c;
        }
        public static bool operator !=(PlayerClass a, PlayerClass b)
        {
            return a.playerUID != b.playerUID;
        }
        public static bool operator ==(PlayerClass a, PlayerClass b)
        {
            return a.playerUID == b.playerUID;
        }
        public int whoAmI;
        public int playerUID;
        public int classChoice;

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public sealed class JobID
    {
        public static class Sets
        {
            public static bool[] IsMelee
            {
                get
                {
                    bool[] array = new bool[15];
                    for (int i = 0; i < array.Length; i++)
                    {
                        switch (i)
                        {
                            case MELEE_Smith:
                            case MELEE_Warrior:
                            case MELEE_WhiteKnight:
                                array[i] = true;
                                break;
                        }
                    }
                    return array;
                }
            }
            public static bool[] IsRanged
            {
                get
                {
                    bool[] array = new bool[15];
                    for (int i = 0; i < array.Length; i++)
                    {
                        switch (i)
                        {
                            case RANGED_Bowsman:
                            case RANGED_CorperateUsurper:
                            case RANGED_Outlaw:
                                array[i] = true;
                                break;
                        }
                    }
                    return array;
                }
            }
            public static bool[] IsMage
            {
                get
                {
                    bool[] array = new bool[15];
                    for (int i = 0; i < array.Length; i++)
                    {
                        switch (i)
                        {
                            case MAGE_Botanist:
                            case MAGE_Witch:
                            case MAGE_Wizard:
                                array[i] = true;
                                break;
                        }
                    }
                    return array;
                }
            }
            public static bool[] IsSummoner
            {
                get
                {
                    bool[] array = new bool[15];
                    for (int i = 0; i < array.Length; i++)
                    {
                        switch (i)
                        {
                            case SUMMONER_Alchemist:
                            case SUMMONER_Scientist:
                            case SUMMONER_Surveyor:
                                array[i] = true;
                                break;
                        }
                    }
                    return array;
                }
            }
            public static bool[] IsAll
            {
                get
                {
                    bool[] array = new bool[15];
                    for (int i = 0; i < array.Length; i++)
                    {
                        switch (i)
                        {
                            case ALL_BusinessMan:
                            case ALL_Entrepreneur:
                            case ALL_Merchant:
                                array[i] = true;
                                break;
                        }
                    }
                    return array;
                }
            }
        }
        public readonly static string[] Name = new string[]
        {
            NAME_BusinessMan,
            NAME_Entrepreneur,
            NAME_Merchant,
            NAME_Botanist,
            NAME_Witch,
            NAME_Wizard,
            NAME_Smith,
            NAME_Warrior,
            NAME_WhiteKnight,
            NAME_Bowsman,
            NAME_CorporateUsurper,
            NAME_Outlaw,
            NAME_Alchemist,
            NAME_Scientist,
            NAME_Surveyor
        };
        public const string
            NAME_None = "None",
            NAME_Alchemist = "Alchemist",
            NAME_Botanist = "Botanist",
            NAME_Bowsman = "Bowsman",
            NAME_BusinessMan = "Business Man",
            NAME_CorporateUsurper = "Corporate Usurper",
            NAME_Entrepreneur = "Entrepreneur",
            NAME_Merchant = "Merchant",
            NAME_Outlaw = "Outlaw",
            NAME_Scientist = "Scientist",
            NAME_Smith = "Smith",
            NAME_Surveyor = "Surveyor",
            NAME_Warrior = "Warrior",
            NAME_WhiteKnight = "White Knight",
            NAME_Witch = "Witch",
            NAME_Wizard = "Wizard";
        public const int
            None = -1,
            ALL_BusinessMan = 0,
            ALL_Entrepreneur = 1,
            ALL_Merchant = 2,
            MAGE_Botanist = 3,
            MAGE_Witch = 4,
            MAGE_Wizard = 5,
            MELEE_Smith = 6,
            MELEE_Warrior = 7,
            MELEE_WhiteKnight = 8,
            RANGED_Bowsman = 9,
            RANGED_CorperateUsurper = 10,
            RANGED_Outlaw = 11,
            SUMMONER_Alchemist = 12,
            SUMMONER_Scientist = 13,
            SUMMONER_Surveyor = 14;
    }
    public class ArchaeaPlayer : ModPlayer
    {
        #region biome
        public bool MagnoBiome  => Player.InModBiome<MagnoBiome>();
        public bool SkyFort     => Player.InModBiome<SkyFortBiome>();
        public bool SkyPortal   => Player.InModBiome<SkyFortPortalBiome>();
        public bool Factory     => Player.InModBiome<FactoryBiome>();
        #endregion
        //  Class type
        public int classChoice
        {
            get { return classData.classChoice; }
            set { classData.classChoice = value; }
        }
        public bool elevatorConnected = false;
        public int playerUID = 0;
        public int _classChoice = 0;
        private bool classChosen;
        private PlayerClass classData;
        private int _jobChoice = -1;
        public int jobChoice 
        {
            get { return _jobChoice; } 
            set { _jobChoice = value; } 
        }
        private bool hasUsedLifeCrystal = false;

        //  Stat and trait
        public int remainingStat;
        public int overallMaxStat = 0;
        private int breathTimer;
        private const int breathTimerMax = 180;
        public int[] spentStat = new int[10];
        public bool[] trait = new bool[35];
        public bool[] objectiveStat = new bool[9];
        public int placedTiles = 0;
        public bool[] placedPylon = new bool[9];

        //  Misc
        private bool start;
        public bool debugMenu;
        public bool spawnMenu;
        private bool setInitMode = true;
        private bool setModeStats = false;
        private const int maxTime = 360;
        private int effectTime = maxTime;
        private int boundsCheck;
        private float darkAlpha = 0f;
        private Action<float, float> method;
        public bool classChecked;
        Effects.Polygon polygon = new Effects.Polygon();
        public bool loadComplete;
        public bool onEnterWorldStart = false;
        public int enterWorldTicks = 0;
        public short extraLife = 0;
        public int dungeonLocatorTicks = 0;
        public int locatorDirection = -1;
        private int hintTicks = 0;
        public int tileTime = 0;

        //  Jobs
        public bool showedDialog = false;
        private int[] _jobProgress = new int[15];
        public int[] jobProgress = new int[3];
        private int ticks3 = 0;
        public const int JOB_Progress_All = 21;
        public const int JOB_Progress_Mage = 30;
        public const int JOB_Progress_Melee = 24;
        public const int JOB_Progress_Ranged = 21;
        public const int JOB_Progress_Summoner = 15;
        private bool[] obtainedCatalogue = new bool[2];

        //  Stat variables
        //  Increase
        public float arrowSpeed = 1f;
        public float jumpHeight = 1f;
        public float attackSpeed = 1f;
        public float moveSpeed = 1f;
        public int breathTime = 1;
        public int toughness = 1;
        public float damageBuff = 1f;
        //  Decrease
        public float merchantDiscount = 1f;  
        public double oldPriceDiscount;
        public float percentDamageTaken = 1f;
        public float ammoReduction = 1f;

        //  More trait variables
        public int TRAIT_TIME_MaxDirtStack = 0;
        public int TRAIT_TIME_MAX_MaxDirtStack = 60 * 60 * 20 * 1; // Frame rate, Seconds, Minutes, Hours
        public int TRAIT_PlantedMushroom = 0;
        public int TRAIT_PlacedRails = 0;
        public int TRAIT_PlacedBricks = 0;
        public int TRAIT_PlacedPylon = 0;
        public int fallDistance = 0;
        public int fallDistance2 = 0;
        public int comparison = 0;
        public bool itemUsed = false;

        //  Biome bounds
        private bool outOfBounds;
        public bool[] zones = new bool[index];
        private const int index = 12;
        private Vector2 oldPosition;

        //  Spawn menu debugging
        private bool initMenu;
        private string[] label;
        private Rectangle box;
        private TextBox[] input;
        private Button[] button;

        //  Blueprints
        public Rectangle blueprint = Rectangle.Empty;
        public Button cancel = new Button("Cancel", new Rectangle(0, 0, 10 * 6, 32));
        public Button accept = new Button("Accept", new Rectangle(0, 0, 10 * 6, 32));
        
        private Rectangle SetXY(Rectangle bound, int x, int y)
        {
            return new Rectangle(x, y, bound.Width, bound.Height);
        }

        public override void CopyClientState(ModPlayer clientClone)
        {
            ArchaeaPlayer modPlayer = (ArchaeaPlayer)clientClone;
            //  World
            //modPlayer.enterWorldTicks = enterWorldTicks;
            //  Trait variables
            modPlayer.trait = trait;
            modPlayer.placedPylon = placedPylon;
            modPlayer.placedTiles = placedTiles;
            modPlayer.TRAIT_PlacedBricks = TRAIT_PlacedBricks;
            modPlayer.TRAIT_PlacedRails = TRAIT_PlacedRails;
            modPlayer.TRAIT_PlantedMushroom = TRAIT_PlantedMushroom;
            modPlayer.TRAIT_TIME_MaxDirtStack = TRAIT_TIME_MaxDirtStack;
            modPlayer.fallDistance = fallDistance;
            modPlayer.fallDistance2 = fallDistance2;
            modPlayer.comparison = comparison;
            //  Stat variables
            modPlayer.objectiveStat = objectiveStat;
            modPlayer.overallMaxStat = overallMaxStat;
            modPlayer.remainingStat = remainingStat;
            //  Increase
            modPlayer.arrowSpeed = arrowSpeed;
            modPlayer.jumpHeight = jumpHeight;
            modPlayer.attackSpeed = attackSpeed;
            modPlayer.moveSpeed = moveSpeed;
            modPlayer.breathTime = breathTime;
            modPlayer.toughness = toughness;
            modPlayer.damageBuff = damageBuff;
            //  Decrease
            modPlayer.merchantDiscount = merchantDiscount;
            modPlayer.percentDamageTaken = percentDamageTaken;
            modPlayer.ammoReduction = ammoReduction;
            //  Items
            modPlayer.extraLife = extraLife;
            modPlayer.dungeonLocatorTicks = dungeonLocatorTicks;
            modPlayer.locatorDirection = locatorDirection;
            //modPlayer.fireStorm = fireStorm;
            //  End
            clientClone = modPlayer;
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            ArchaeaPlayer modPlayer = (ArchaeaPlayer)clientPlayer;
            //  World
            //enterWorldTicks = modPlayer.enterWorldTicks;
            //  Traits variables
            trait = modPlayer.trait;
            placedPylon = modPlayer.placedPylon;
            placedTiles = modPlayer.placedTiles;
            TRAIT_PlacedBricks = modPlayer.TRAIT_PlacedBricks;
            TRAIT_PlacedRails = modPlayer.TRAIT_PlacedRails;
            TRAIT_PlantedMushroom = modPlayer.TRAIT_PlantedMushroom;
            TRAIT_TIME_MaxDirtStack = modPlayer.TRAIT_TIME_MaxDirtStack;
            fallDistance = modPlayer.fallDistance;
            fallDistance2 = modPlayer.fallDistance2;
            comparison = modPlayer.comparison;
            //  Stat variables
            objectiveStat = modPlayer.objectiveStat;
            overallMaxStat = modPlayer.overallMaxStat;
            remainingStat = modPlayer.remainingStat;
            //  Increase
            arrowSpeed = modPlayer.arrowSpeed;
            jumpHeight = modPlayer.jumpHeight;
            attackSpeed = modPlayer.attackSpeed;
            moveSpeed = modPlayer.moveSpeed;
            breathTime = modPlayer.breathTime;
            toughness = modPlayer.toughness;
            damageBuff = modPlayer.damageBuff;
            //  Decrease
            merchantDiscount = modPlayer.merchantDiscount;
            percentDamageTaken = modPlayer.percentDamageTaken;
            ammoReduction = modPlayer.ammoReduction;
            //  Items
            extraLife = modPlayer.extraLife;
            dungeonLocatorTicks = modPlayer.dungeonLocatorTicks;
            locatorDirection = modPlayer.locatorDirection;
            //fireStorm = modPlayer.fireStorm;
            //  End
        }

        //  More class methods
        public static int HardChangeClass(Player player)
        {
            if (Main.netMode < 2)
            {
                ArchaeaPlayer modPlayer = player.GetModPlayer<ArchaeaPlayer>();
                //  Class select override
                if (Locks.Instance.OverrideClassSelect)
                {
                    modPlayer.classChoice = (int)Locks.Instance.ForceClass + 1;
                    modPlayer.classData.classChoice = (int)Locks.Instance.ForceClass + 1;
                    return (int)Locks.Instance.ForceClass + 1;
                }
                return modPlayer.classChoice;
            }
            return ClassID.None;
        }

        //  More job methods
        public int CompletedJobsCount()
        {
            JobProgress(classChoice, out int num);
            return num;
        }
        private static void SetJobTraitSuccess(Player player)
        {
            ArchaeaPlayer modPlayer = player.GetModPlayer<ArchaeaPlayer>();
            switch (modPlayer.classChoice)
            {
                case ClassID.All:
                    modPlayer.SetClassTrait(TraitID.ALL_Job, modPlayer.classChoice, true);
                    goto default;
                case ClassID.Magic:
                    modPlayer.SetClassTrait(TraitID.MAGE_Job, modPlayer.classChoice, true);
                    goto default;
                case ClassID.Melee:
                    modPlayer.SetClassTrait(TraitID.MELEE_Job, modPlayer.classChoice, true);
                    goto default;
                case ClassID.Ranged:
                    modPlayer.SetClassTrait(TraitID.RANGED_Job, modPlayer.classChoice, true);
                    goto default;
                case ClassID.Summoner:
                    modPlayer.SetClassTrait(TraitID.SUMMONER_Job, modPlayer.classChoice, true);
                    goto default;
                default:
                    SoundEngine.PlaySound(SoundID.Item4, player.Center);
                    break;
            }
        }
        public static bool JobProgressSuccess(Player player)
        {
            ArchaeaPlayer modPlayer = player.GetModPlayer<ArchaeaPlayer>();
            if (modPlayer.jobChoice == -1)
                return false;
            int index = 0;
            int success = 0;
            switch (modPlayer.classChoice)
            {
                case ClassID.All:
                    index = modPlayer.jobChoice;
                    success = JOB_Progress_All / 3;
                    break;
                case ClassID.Magic:
                    index = modPlayer.jobChoice - 3;
                    success = JOB_Progress_Mage / 3;
                    break;
                case ClassID.Melee:
                    index = modPlayer.jobChoice - 6;
                    success = JOB_Progress_Melee / 3;
                    break;
                case ClassID.Ranged:
                    index = modPlayer.jobChoice - 9;
                    success = JOB_Progress_Ranged / 3;
                    break;
                case ClassID.Summoner:
                    index = modPlayer.jobChoice - 12;
                    success = JOB_Progress_Summoner / 3;
                    break;
            }
            int num = 0;
            for (int i = 0; i < modPlayer.jobProgress.Length; i++)
            {
                num += modPlayer.jobProgress[i];
            }
            return modPlayer.jobProgress[index] >= success;
        }
        public int[] SetJobProgress(int jobID, ref Item item)
        {
            switch (jobID)
            {
                case JobID.ALL_BusinessMan:
                case JobID.ALL_Entrepreneur:
                case JobID.ALL_Merchant:
                    if (item.type == ItemID.GoldChest)
                    {
                        _jobProgress[jobID]++;
                        item.stack--;
                        goto default;
                    }
                    break;
                case JobID.MAGE_Botanist:
                case JobID.MAGE_Witch:
                case JobID.MAGE_Wizard:
                    if (item.type == ModContent.ItemType<Merged.Items.Materials.magno_core>())
                    {
                        _jobProgress[jobID]++;
                        item.stack--;
                        goto default;
                    }
                    break;
                case JobID.MELEE_Smith:
                case JobID.MELEE_Warrior:
                case JobID.MELEE_WhiteKnight:
                    if (item.type == ModContent.ItemType<Items.Materials.r_plate>())
                    {
                        _jobProgress[jobID]++;
                        item.stack--;
                        goto default;
                    }
                    break;
                case JobID.RANGED_Bowsman:
                case JobID.RANGED_CorperateUsurper:
                case JobID.RANGED_Outlaw:
                    if (item.type == ItemID.BlackLens)
                    {
                        _jobProgress[jobID]++;
                        item.stack--;
                        goto default;
                    }
                    break;
                case JobID.SUMMONER_Alchemist:
                case JobID.SUMMONER_Scientist:
                case JobID.SUMMONER_Surveyor:
                    if (item.type == ItemID.BirdStatue)
                    {
                        _jobProgress[jobID]++;
                        item.stack--;
                        goto default;
                    }
                    break;
                default:
                    SoundEngine.PlaySound(SoundID.Tink);
                    break;
            }
            return jobProgress = JobProgress(classChoice, out _);
        }
        private int[] JobProgress(int classID, out int numComplete)
        {
            int index = 0;
            int num = 0;
            switch (classID)
            {
                case ClassID.All:
                    index = 0;
                    num = JOB_Progress_All / 3;
                    break;
                case ClassID.Magic:
                    index = 3;
                    num = JOB_Progress_Mage / 3;
                    break;
                case ClassID.Melee:
                    index = 4;
                    num = JOB_Progress_Melee / 3;
                    break;
                case ClassID.Ranged:
                    index = 9;
                    num = JOB_Progress_Ranged / 3;
                    break;
                case ClassID.Summoner:
                    index = 12;
                    num = JOB_Progress_Summoner / 3;
                    break;
            }
            int[] array = _jobProgress.ToList().GetRange(index, 3).ToArray();
            numComplete = array.Count(t => t >= num);
            if (numComplete >= 3)
            {
                SetJobTraitSuccess(Player);
            }
            return array;
        }

        private void InitStatRemaining()
        {
            int num = 0;
            for (int i = 0; i < spentStat.Length; i++)
            {
                num += spentStat[i];
            }
            overallMaxStat = Math.Abs(num - 45);
        }
        public override void LoadData(TagCompound tag)
        {
            //  Class selction
            playerUID = tag.GetInt("PlayerID");
            if (playerUID == 0)
                playerUID = GetHashCode();
            classData.playerUID = playerUID;
            classData.classChoice = tag.GetByte("Class");
            //  Job selection
            jobChoice = tag.GetInt("_Job");
            showedDialog = tag.GetBool("JobDialog");
            for (int i = 0; i < jobProgress.Length; i++)
            {
                jobProgress[i] = tag.GetInt($"job_prog{i}");
            }
            for (int i = 0; i < _jobProgress.Length; i++)
            {
                _jobProgress[i] = tag.GetInt($"_job_prog{i}");
            }
            obtainedCatalogue[0] = tag.GetBool("_obtained0");
            obtainedCatalogue[1] = tag.GetBool("_obtained1");
            //  Progression stat poins
            remainingStat = tag.GetInt("remainingStat");
            overallMaxStat = tag.GetInt("overallMaxStat");
            spentStat[0] = tag.GetInt("ArrowSpeed");
            spentStat[1] = tag.GetInt("JumpHeight");
            spentStat[2] = tag.GetInt("AttackSpeed");
            spentStat[3] = tag.GetInt("MoveSpeed");
            spentStat[4] = tag.GetInt("BreathTime");
            spentStat[5] = tag.GetInt("Toughness");
            spentStat[6] = tag.GetInt("DamageBuff");
            spentStat[7] = tag.GetInt("MerchantDiscount");
            spentStat[8] = tag.GetInt("PercentDamageTaken");
            spentStat[9] = tag.GetInt("AmmoReduction");
            for (int i = 0; i < objectiveStat.Length; i++)
            {
                objectiveStat[i] = tag.GetBool($"stat{i}");
            }
            //  Class trait requirements
            placedTiles = tag.GetInt("placedTiles");
            TRAIT_PlacedBricks = tag.GetInt("placedBricks");
            TRAIT_PlacedRails = tag.GetInt("placedRails");
            TRAIT_PlantedMushroom = tag.GetInt("plantedMushroom");
            placedPylon[PylonID.Desert] = tag.GetBool("desertPylon");
            placedPylon[PylonID.Hallow] = tag.GetBool("hallowPylon");
            placedPylon[PylonID.Jungle] = tag.GetBool("junglePylon");
            placedPylon[PylonID.Mushroom] = tag.GetBool("mushroomPylon");
            placedPylon[PylonID.Ocean] = tag.GetBool("oceanPylon");
            placedPylon[PylonID.Purity] = tag.GetBool("purityPylon");
            placedPylon[PylonID.Snow] = tag.GetBool("snowPylon");
            placedPylon[PylonID.Underground] = tag.GetBool("undergroundPylon");
            placedPylon[PylonID.Victory] = tag.GetBool("victoryPylon");
            for (int i = 0; i < trait.Length; i++)
            {
                trait[i] = tag.GetBool($"trait{i}");
            }
            //  World
            enterWorldTicks = tag.GetInt("enterWorldStart");
            //  Items
            extraLife = tag.GetShort("extraLives");
            hasUsedLifeCrystal = tag.GetBool("hasUsedLifeCrystal");

        }
        public override void SaveData(TagCompound tag)
        {
            //  Class selction
            tag.Add("PlayerID", playerUID);
            tag.Add("Class", (byte)classData.classChoice);
            //  Job selection
            tag.Add("_Job", jobChoice);
            tag.Add("JobDialog", showedDialog);
            for (int i = 0; i < jobProgress.Length; i++)
            {
                tag.Add($"job_prog{i}", jobProgress[i]);
            }
            for (int i = 0; i < _jobProgress.Length; i++)
            {
                tag.Add($"_job_prog{i}", _jobProgress[i]);
            }
            tag.Add("_obtained0", obtainedCatalogue[0]);
            tag.Add("_obtained1", obtainedCatalogue[1]);
            //  Progression stat poins
            tag.Add("remainingStat", remainingStat);
            tag.Add("overallMaxStat", overallMaxStat);
            tag.Add("ArrowSpeed", spentStat[0]);
            tag.Add("JumpHeight", spentStat[1]);
            tag.Add("AttackSpeed", spentStat[2]);
            tag.Add("MoveSpeed", spentStat[3]);
            tag.Add("BreathTime", spentStat[4]);
            tag.Add("Toughness", spentStat[5]);
            tag.Add("DamageBuff", spentStat[6]);
            tag.Add("MerchantDiscount", spentStat[7]);
            tag.Add("PercentDamageTaken", spentStat[8]);
            tag.Add("AmmoReduction", spentStat[9]);
            for (int i = 0; i < objectiveStat.Length; i++)
            {
                tag.Add($"stat{i}", objectiveStat[i]);
            }
            //  Class trait requirements
            tag.Add("placedTiles", placedTiles);
            tag.Add("placedRails", TRAIT_PlacedRails);
            tag.Add("placedBricks", TRAIT_PlacedBricks);
            tag.Add("plantedMushroom", TRAIT_PlantedMushroom);
            tag.Add("desertPylon", placedPylon[PylonID.Desert]);
            tag.Add("hallowPylon", placedPylon[PylonID.Hallow]);
            tag.Add("junglePylon", placedPylon[PylonID.Jungle]);
            tag.Add("mushroomPylon", placedPylon[PylonID.Mushroom]);
            tag.Add("oceanPylon", placedPylon[PylonID.Ocean]);
            tag.Add("purityPylon", placedPylon[PylonID.Purity]);
            tag.Add("snowPylon", placedPylon[PylonID.Snow]);
            tag.Add("undergroundPylon", placedPylon[PylonID.Underground]);
            tag.Add("victoryPylon", placedPylon[PylonID.Victory]);
            for (int i = 0; i < trait.Length; i++)
            {
                tag.Add($"trait{i}", trait[i]);
            }
            //  World
            tag.Add("enterWorldStart", enterWorldTicks);
            //  Items
            tag.Add("extraLives", extraLife);
            tag.Add("hasUsedLifeCrystal", hasUsedLifeCrystal);
        }

        public bool HasClassTrait(int index, int classID)
        {
            return trait[index] && classID == classChoice;
        }

        public bool SetClassTrait(int index, int classID, bool flag)
        {
            if (trait[index])
                return false;
            if (flag && classID == classChoice)
            {
                overallMaxStat++;
                remainingStat++;
                trait[index] = flag;
                //  Dialogue that displays general Trait acquired
                ModContent.GetInstance<ModeUI>().ticks = 300;
                SoundEngine.PlaySound(SoundID.Item4, Player.Center);
                return true;
            }
            return false;
        }
        public static bool SetClassTrait(int index, int classID, bool flag, int whoAmI)
        {
            ArchaeaPlayer player = Main.player[whoAmI].GetModPlayer<ArchaeaPlayer>();
            if (player.trait[index])
                return false;
            //  player.placedTiles needs to be moved to an Update
            if (flag && classID == player.classChoice)
            {
                player.overallMaxStat++;
                player.remainingStat++;
                player.trait[index] = flag;
                //  Dialogue that displays general Trait acquired
                ModContent.GetInstance<ModeUI>().ticks = 300;
                SoundEngine.PlaySound(SoundID.Item4, Main.player[whoAmI].Center);
                return true;
            }
            return false;
        }
        public static bool CheckHasTrait(int index, int classID, int whoAmI)
        {
            if (whoAmI >= Main.player.Length || !Main.player[whoAmI].active || whoAmI < 0)
                return false;

            ArchaeaPlayer player = Main.LocalPlayer.GetModPlayer<ArchaeaPlayer>();
            if (player.classChoice == classID && player.trait[index])
                return true;
            return false;
        }
        public bool CheckHasTrait(int index, int classID)
        {
            if (classChoice == classID && trait[index])
                return true;
            return false;
        }
        public bool CheckValidJob(int classID, int jobChoice)
        {
            switch (classID)
            {
                case ClassID.All:
                    return JobID.Sets.IsAll[jobChoice];
                case ClassID.Magic:
                    return JobID.Sets.IsMage[jobChoice];
                case ClassID.Melee:
                    return JobID.Sets.IsMelee[jobChoice];
                case ClassID.Ranged:
                    return JobID.Sets.IsRanged[jobChoice];
                case ClassID.Summoner:
                    return JobID.Sets.IsSummoner[jobChoice];
                default:
                    return false;
            }
        }
        public bool CheckHasJob(int jobID)
        {
            return jobChoice == jobID;
        }
        public void SelectJob(int __classChoice, int _jobChoice)
        {
            if (CheckValidJob(__classChoice, _jobChoice))
            {
                jobChoice = _jobChoice;
            }
        }
        /* Unused
        public int ArrowSpeed;
        public int JumpHeight;
        public int AttackSpeed;
        public int MoveSpeed;
        public int BreathTime;
        public int Toughness;
        public int DamageBuff;
        public int MerchantDiscount;
        public int PercentDamageTaken;
        public int AmmoReduction;
        */
        public void FakeUseLifeCrystal(Item item)
        {
            if (ModContent.GetInstance<ModeToggle>().archaeaMode)
            {
                if (Player.statLifeMax < 9999)
                {
                    Player.statLifeMax += ArchaeaMode.LifeCrystal();
                    Player.statLifeMax = Math.Min(Player.statLifeMax, 9999);
                    item.stack--;
                }
                Player.ApplyItemAnimation(item);
                SoundEngine.PlaySound(SoundID.Item4, Player.Center);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerLifeMana);
            }
            if ((ModContent.GetInstance<ModeToggle>().archaeaMode && Player.statLifeMax2 >= 9499)
                || Player.statLifeMax2 >= 380)
            {
                SetClassTrait(TraitID.SUMMONER_MinionDmg, ClassID.Summoner, true);
            }
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            if (ModContent.GetInstance<ModeToggle>().archaeaMode)
            { 
                healValue = ArchaeaMode.HealPotion(item.healLife);
            }
        }
        public override bool CanUseItem(Item item)
        {
            //if (!ModContent.GetInstance<ModeToggle>().archaeaMode)
            //    return ClassItemCheck();
            if (elevatorConnected)
            {
                return false;
            }
            if (Player.HasBuff<Jobs.Buffs.Zombie>())
            {
                return false;
            }
            switch (item.type)
            {
                case ItemID.LifeCrystal:
                    FakeUseLifeCrystal(item);
                    return !ModContent.GetInstance<ModeToggle>().archaeaMode;
                case ItemID.LifeFruit:
                    if (ModContent.GetInstance<ModeToggle>().archaeaMode)
                    { 
                        if (Player.statLifeMax < 9999)
                        { 
                            Player.statLifeMax += ArchaeaMode.LifeCrystal(5);
                            Player.statLifeMax = Math.Min(Player.statLifeMax, 9999);
                            item.stack--;
                        }
                        Player.ApplyItemAnimation(item);
                        SoundEngine.PlaySound(SoundID.Item4, Player.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerLifeMana);
                        return false;
                    }
                    break;
                case ItemID.ManaCrystal:
                    break;
                    #region Mana capped at 400
                    if (Player.statManaMax < 999)
                    {
                        Player.statManaMax += ArchaeaMode.ManaCrystal();
                        Player.statManaMax = Math.Min(Player.statManaMax, 999);
                        item.stack--;
                    }
                    Player.ApplyItemAnimation(item);
                    SoundEngine.PlaySound(SoundID.Item29, Player.Center);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.PlayerLifeMana);
                    return false;
                    #endregion
                case ItemID.LesserHealingPotion:
                case ItemID.HealingPotion: 
                case ItemID.GreaterHealingPotion:
                case ItemID.SuperHealingPotion:         
                    break;
                    Player.statLife += ArchaeaMode.HealPotion(item.healLife);
                    Player.ApplyItemAnimation(item);
                    item.stack--;
                    SoundEngine.PlaySound(SoundID.Item3, Player.Center);
                    return false;
                case ItemID.LesserManaPotion:
                case ItemID.ManaPotion:
                case ItemID.GreaterManaPotion:
                case ItemID.SuperManaPotion:
                    break;
                    #region Mana capped at 400
                    Player.statMana += ArchaeaMode.ManaPotion(item.healMana);
                    Player.ApplyItemAnimation(item);
                    item.stack--;
                    SoundEngine.PlaySound(SoundID.Item3, Player.Center);
                    return false;
                    #endregion
            }
            return ClassItemCheck();
        }
        public static int ModeOffResetStats(int lifeMax2)
        {
            if (lifeMax2 != 100 && lifeMax2 > 500)
            {
                int offset = 0;
                if (lifeMax2 == 9999)
                {
                    offset = 5;
                }
                int extra = (lifeMax2 - 100) / 25;
                extra += offset;
                lifeMax2 = 100 + extra;
                return lifeMax2;
                //Player.statLife = Player.statLifeMax2;
            }
            return lifeMax2;
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //    NetMessage.SendData(MessageID.PlayerLifeMana);
        }
        public void PreSavePlayer(Player Player)
        {
            return;
            Player.statLifeMax2 = ModeOffResetStats(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
        }
        public void PostSavePlayer(Player Player)
        {
            return;
            Player.statLifeMax2 = LifeMaxMode(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
            //Player.statLife = Player.statLifeMax;
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //    NetMessage.SendData(MessageID.PlayerLifeMana);
        }
        public override void PreSavePlayer()
        {
            //  Turn off life scaling
            Player.statLifeMax2 = ModeOffResetStats(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
            return; //  This is not necessary and turning on or off Archaea Mode sets the values manually
            Player.statLifeMax2 = ModeOffResetStats(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
        }
        public override void PostSavePlayer()
        {
            return; //  This is not necessary and turning on or off Archaea Mode sets the values manually
            Player.statLifeMax2 = LifeMaxMode(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
            //Player.statLife = Player.statLifeMax;
            //if (Main.netMode == NetmodeID.MultiplayerClient)
            //    NetMessage.SendData(MessageID.PlayerLifeMana);
        }
        public static int LifeMaxMode(int lifeMax2)
        {
            if (lifeMax2 == 100) return 100;
            if (lifeMax2 > 500)  return lifeMax2;
            int extra = lifeMax2 - 100;
            lifeMax2 = Math.Min(9999, Math.Max(100, 100 + ArchaeaMode.LifeCrystal(extra)));
            return lifeMax2;
        }
        Timer debugTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
        public override void OnEnterWorld()
        {
            //Main.NewText(ModContent.NPCType<NPCs.Bosses.factory_computer>());
            //  DEBUG
            //debugTimer.Enabled = true;
            //debugTimer.AutoReset = true;
            //debugTimer.Elapsed += DebugTimer_Elapsed;
            //debugTimer.Start();
            SetModeStats(ModContent.GetInstance<ModeToggle>().archaeaMode);
            InitStat(Player);
            if (Effects.Barrier.barrier == null)
            {
                Effects.Barrier.Initialize();
            }
        }

        public static void NewText(string text)
        {
            Main.NewText(text, Color.LightBlue);
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if (CheckHasTrait(TraitID.SUMMONER_MinionDmg, ClassID.Summoner, Main.myPlayer))
            {
                if (item.DamageType == DamageClass.Summon)
                {
                    damage.Base = 1.2f;
                }
            }
            if (Player.HasBuff<Jobs.Buffs.Fortitude>())
            {
                if (item.DamageType == DamageClass.Melee)
                {
                    damage.Base *= 3;
                }
            }
            if (CheckHasTrait(TraitID.MAGE_Job, ClassID.Magic, Main.myPlayer))
            {
                if (item.DamageType == DamageClass.Magic)
                {
                    damage.Base *= 1.1f;
                }
            }
        }

        private void DebugTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //  DEBUG
            return;
            if (!Player.active) return;
            InitStatRemaining();
            if (overallMaxStat > 0)
            {
                remainingStat += 1;
                SoundEngine.PlaySound(SoundID.Item29, Player.Center);
            }
            else 
            {
                debugTimer.Stop();
                debugTimer.Dispose();
            }
        }

        public static DamageClass GetDamageClass(ArchaeaPlayer modPlayer)
        {     
            switch (modPlayer.classChoice)
            {
                default:
                case ClassID.None:
                    return DamageClass.Default;
                case ClassID.Melee:
                    return DamageClass.Melee;
                case ClassID.Ranged:
                    return DamageClass.Ranged;
                case ClassID.Magic:
                    return DamageClass.Magic;
                case ClassID.Summoner:
                    return DamageClass.Summon;
                case ClassID.All:
                    return DamageClass.Generic;
            }
        }

        public void SetModeStats(bool modeFlag, int whoAmI)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) 
            { 
                NetHandler.Send(Packet.SetModeLife, 256, whoAmI, b: modeFlag);
            }
            else
            {
                if (modeFlag)
                {
                    PostSavePlayer(Player);
                }
                else
                {
                    PreSavePlayer(Player);
                }
            }
        }
        public void SetModeStats(bool modeFlag)
        {
            if (modeFlag)
            {
                PostSavePlayer(Player);
            }
            else
            {
                PreSavePlayer(Player);
            }
            return;
            Player.statLifeMax2 = ModeOffResetStats(Player.statLifeMax2);
            Player.statLifeMax = Player.statLifeMax2;
            //if (modeFlag)
            //    PostSavePlayer();
            //else ModeOffResetStats(Player.statLifeMax2);
        }
        #region Progression
        int ticks4 = 0;
        public override void PreUpdateBuffs()
        {
            if (!Main.dayTime && ticks4++ % 300 == 0)
            { 
                if (CheckHasTrait(TraitID.RANGED_Job, ClassID.Ranged))
                {
                    Player.AddBuff(BuffID.Hunter, 600);
                }
            }
        }
        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            price = (int)(price / merchantDiscount);
        }
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity *= attackSpeed;
        }
        public override void PostUpdateBuffs()
        {
            //  ALERT; dumb idea
            HardChangeClass(Player);
        }
        public override void PostUpdateRunSpeeds()
        {
            if (CheckHasTrait(TraitID.ALL_IncJumpHeight, ClassID.All))
            {
                Player.jumpHeight = (int)(Player.jumpHeight * 1.25f);
            }
            if (CheckHasTrait(TraitID.MAGE_MoveSpeed, ClassID.Magic))
            { 
                Player.maxRunSpeed *= 1.10f;
            }
            if (CheckHasTrait(TraitID.SUMMONER_Job, ClassID.Summoner))
            {
                Player.maxRunSpeed *= 1.10f;
            }
            Player.maxRunSpeed *= moveSpeed;
            Player.jumpHeight = (int)(Player.jumpHeight * jumpHeight);
            Player.statDefense += toughness;
        }
        public override void PostUpdateMiscEffects()
        {
            if (ModContent.GetInstance<ModeToggle>().archaeaMode)
            {
                if (Player.statLifeMax2 > 100 && Player.statLifeMax2 <= 500)
                { 
                    Player.statLifeMax2 = LifeMaxMode(Player.statLifeMax2);
                    Player.statLifeMax = Player.statLifeMax2;
                }
            }
            else
            {
                if (Player.statLifeMax >= 600)
                {
                    Player.statLifeMax2 = ModeOffResetStats(Player.statLifeMax2);
                    Player.statLifeMax = Player.statLifeMax2;
                }
            }
            if (Player.wet && Player.breath < Player.breathMax && breathTimer-- < breathTime)
            {
                Player.breath += 10;
                breathTimer = breathTimerMax;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (modifiers.DamageSource.SourceItem != null)
            { 
                if (ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[modifiers.DamageSource.SourceItem.type] && ArchaeaItem.HasEquipped(Player, ModContent.ItemType<Jobs.Items.Armors.BomVest>()))
                {
                    modifiers.FinalDamage *= 0.8f;
                }
            }
            if (CheckHasTrait(TraitID.MAGE_DamageReduce, ClassID.Magic))
            {
                modifiers.FinalDamage *= 0.9f;
            }
            if (CheckHasTrait(TraitID.MAGE_NoKb, ClassID.Magic))
            {
                if (Main.rand.NextFloat() < 0.5f)
                {
                    Player.noKnockback = true;
                }
            }
            if (!ModContent.GetInstance<ModeToggle>().archaeaMode)
                return;
            modifiers.FinalDamage += ArchaeaMode.ModeScale(ArchaeaMode.StatWho.Player, ArchaeaMode.Stat.Damage, ModContent.GetInstance<ModeToggle>().damageScale, Player.statDefense, DamageClass.Default);
        }
        public override float UseSpeedMultiplier(Item item)
        {
            return attackSpeed;
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            return Main.rand.NextFloat() < Math.Abs(ammoReduction - 2f);
        }
        public bool SpendStatPoint(int index, int num = 1, bool force = false)
        {
            if (!force) {
                if (remainingStat <= 0)
                    return false;
                else if (remainingStat > 0)
                    remainingStat--;
                spentStat[index] += num;
            }
            switch (index)
            {
                case ProgressID.ArrowSpeed:
                    arrowSpeed += num / 33f;  // 3% per point       Double, Check magic quiver buff
                    break;
                case ProgressID.JumpHeight:
                    jumpHeight += num / 22.2f; // 4.5% per point    Double, Check shiny red balloon value
                    break;
                case ProgressID.AttackSpeed:
                    attackSpeed += num / 24f;  // 4.1% per point    Double, Check titan's mits value
                    break;
                case ProgressID.MoveSpeed:
                    moveSpeed += num / 15f;   // 6.67% per point    Triple, Check lightning boots value
                    break;
                case ProgressID.BreathTime:
                    breathTime += num * 2;        // new: +2            old: 0.2% [6.67%] per point    [Triple], Check breathing rod value
                    break;
                case ProgressID.Toughness:
                    toughness += num;         // Add to armor
                    break;
                case ProgressID.DamageBuff:
                    damageBuff += num / 100f;  // 1% per point
                    break;
                case ProgressID.MerchantDiscount:
                    merchantDiscount += num / 100f;   // 1% per point
                    break;
                case ProgressID.PercentDamageReduction:
                    percentDamageTaken -= num / 100f; // 1% per point
                    break;
                case ProgressID.AmmoReduction:
                    ammoReduction += num / 88.8f; // 1.1% per point   Double, Check ammo box
                    break;
            }
            return true;
        }
        public void InitStat(Player player)
        {
            //  Handled in player Load
            for (int i = 0; i < spentStat.Length; i++) {
                SpendStatPoint(i, spentStat[i], true);
            }
        }
        #endregion

        #region Melee Leap
        private void Leap(int type, float speed, float radius, ref int tick)
        {
            Func<bool> collision = new Func<bool>(() =>
            { 
                if (Collision.SolidCollision(Player.position, Player.width, Player.height) || ground == Vector2.Zero)
                {
                    return true;
                }
                return false;
            });
            if (startDirection == 1)
            {
                leapStart += Draw.radian * speed;
                if (leapStart > Draw.radian * (360f - 90f) || collision())
                {
                    if (type == _LeapAttack)
                    {
                        Helper.LeftMouse();
                    }
                    Player.velocity.X = (float)Math.Sqrt(radius);
                    Player.velocity.Y = 0;
                    ground = Vector2.Zero;
                    tick = 0;
                    return;
                }
            }
            else
            {
                leapStart -= Draw.radian * speed;
                if (Math.Abs(leapStart) > Draw.radian * (180f - 90f) || collision())
                {
                    if (type == _LeapAttack)
                    {
                        Helper.LeftMouse();
                    }
                    Player.velocity.X = -(float)Math.Sqrt(radius);
                    Player.velocity.Y = 0;
                    ground = Vector2.Zero;
                    tick = 0;
                    return;
                }
            }
            Player.position = ground + LeapAngle(leapStart, radius);
        }
        private Vector2 LeapAngle(float angle, float radius)
        {
            float cos = (float)(radius * Math.Cos(angle));
            float sine = (float)(radius * 1.5f * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        private Vector2 findGround(float radius, int direction = 1)
        {
            if (direction == 1)
            {
                return Player.position + new Vector2(radius, 0f);
            }
            else return Player.position - new Vector2(radius, 0f);
            #region
            int x = (int)Main.screenPosition.X + (direction == 1 ? Main.screenWidth / 2 : 0);
            int y = (int)Main.screenPosition.Y;
            Rectangle rect = new Rectangle(x, y, Main.screenWidth / 2, Main.screenHeight);
            for (int i = 0; i < x / 16; i++)
            {
                for (int j = 0; j < y / 16; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i + 1, j].HasTile)
                    {
                        var ground = new Vector2(i * 16, j * 16);
                        if (Collision.CanHitLine(Player.Center, Player.width, Player.height, ground, Player.width, Player.height))
                        {
                            return ground;
                        }
                    }
                }
            }
            return Vector2.Zero;
            #endregion
        }
        private float angleDirection(int direction)
        {
            if (direction == 1)
            {
                return 180f * Draw.radian;
            }
            else return 0f;
        }
        private bool IsOnGround()
        {
            int i = (Player.Hitbox.Left + 8) / 16;
            int j = Player.Hitbox.Bottom / 16;
            return Main.tile[i, j + 1].HasTile && Main.tile[i + 1, j + 1].HasTile;
        }
        private void InitLeap(ref int tick)
        {
            if (IsOnGround())
            {
                Player.direction = Main.MouseWorld.X < Player.position.X ? -1 : 1;
                startDirection = Player.direction;
                leapStart = angleDirection(Player.direction);
                ground = findGround(Player.position.Distance(Main.MouseWorld) / 2, Player.direction);
                tick = -1;
            }
        }
        public const int _Leap = 0, _LeapAttack = 1;
        Vector2 ground = Vector2.Zero;
        int ticks, ticks2;
        int leap, leap2;
        int dash;
        int startDirection;
        int timeSinceLastDash;
        float leapStart;
        #endregion
        
        public override void PreUpdate()
        {
            //  Force class choice to avert swapping
            if (classChosen && !Locks.Instance.OverrideClassSelect)
            {
                var c = ArchaeaWorld.playerClass.FirstOrDefault(t => t.playerUID == playerUID);
                if (c != default)
                { 
                    if (classChoice != c.classChoice)
                    {
                        if (c.classChoice != ClassID.None)
                        {
                            classChoice = c.classChoice;
                        }
                    }
                }
            }
            //  ALERT; dumb idea
            HardChangeClass(Player);
            //  Jobs turn-in box
            if (Main.netMode < 2)
            {
                Container box = ModContent.GetInstance<ModeUI>().turnInBox;
                box.active = true;
                box.UpdateInput(Player, true, true, true);
                if (box.content != null && box.content.type != ItemID.None && box.content.stack > 0)
                {
                    if (ticks3 > 600)
                        ticks3 = 0;
                    if (++ticks3 % 20 == 0)
                    {
                        if (!JobProgressSuccess(Player))
                        {
                            SetJobProgress(jobChoice, ref box.content);
                        }
                    }
                }
            }

            //  Fire storm scroll effect
            FireStorm();

            //  Leap and Leap attack
            if (ground == Vector2.Zero)
            {
                if (leap > 0 && leap2 > 0)
                {
                    if (ticks++ > 120)
                    {
                        leap = 0;
                        leap2 = 0;
                    }
                }
                if (CheckHasTrait(TraitID.MELEE_Leap, ClassID.Melee) && ArchaeaMain.leapBind.JustPressed)
                {
                    if (IsOnGround())
                    { 
                        leap += 2;
                    }
                }
                if (CheckHasTrait(TraitID.MELEE_LeapAttack, ClassID.Melee) && ArchaeaMain.leapAttackBind.JustPressed)
                {
                    if (IsOnGround())
                    { 
                        leap2 += 2;
                    }
                }
            }
            if (leap >= 2)
                InitLeap(ref leap);
            if (leap2 >= 2)
                InitLeap(ref leap2);
            if (ground != Vector2.Zero)
            {
                if (leap is -1)
                    Leap(_Leap, 3f, Player.position.Distance(Main.MouseWorld) / 2, ref leap);
                if (leap2 is -1)
                    Leap(_LeapAttack, 3f, Player.position.Distance(Main.MouseWorld) / 2, ref leap2);
            }
            //  Dash trait
            if (CheckHasTrait(TraitID.ALL_Dash, ClassID.All))
            {
                if (timeSinceLastDash < 600)
                    timeSinceLastDash++;
                if (ticks2++ > 90)
                {
                    ticks = 0;
                }
                if (timeSinceLastDash > 180)
                { 
                    if (KeyPress(Keys.A) || KeyPress(Keys.D))
                    {
                        dash++;
                    }
                    if (dash >= 2)
                    {
                        Player.velocity.X = (Player.direction * Player.moveSpeed * 8f);
                        timeSinceLastDash = 0;
                        dash = 0;
                    }
                }
            }
            if (Main.dedServ || Effects.Barrier.barrier == null)
                return;
            for (int i = 0; i < Effects.Barrier.barrier.Length; i++)
                Effects.Barrier.barrier[i]?.Update(Player);
            #region debug
            //return;
            NPC npc = Main.npc.FirstOrDefault(t => t.active && t.type == NPCID.Mechanic);
            if (npc != default)
            {
                int i = (int)npc.position.X / 16;
                int j = (int)npc.position.Y / 16;
                for (int n = -1; n < 4; n++)
                {
                    Wiring.TripWire(i + n, j, 1, 1);
                    NetMessage.SendTileSquare(Main.myPlayer, i + n, j);
                }
            }
            if (KeyPress(Keys.O))
            {
                int i = (int)Main.MouseWorld.X / 16;
                int j = (int)Main.MouseWorld.Y / 16;
                WorldGen.PlaceTile(i, j, 711);
            }
            if (setModeStats)
            {
                Player.QuickSpawnItem(Item.GetSource_None(), ModContent.ItemType<Items.MagnoGun_3>());
                Player.QuickSpawnItem(Item.GetSource_None(), ModContent.ItemType<Items.GhostlyChains>());
                Player.QuickSpawnItem(Item.GetSource_None(), ModContent.ItemType<Items.PossessedMusket>());
                Player.QuickSpawnItem(Item.GetSource_None(), ModContent.ItemType<Items.PossessedSpiculum>());
                setModeStats = false;
            }
            Color textColor = Color.Yellow;
            if (!init)
            {
                // DEBUG of sky boss testing -- immediate spawn upon world join
                /*
                NPC.NewNPC(NPC.GetBossSpawnSource(Player.whoAmI), (int)Player.position.X, (int)Player.position.Y, ModNPCID.SkyBoss);
                init = true;
                */
            }
            if (Player.chest >= 0 && Main.chest[Player.chest] != null)
            {
                Merged.Tiles.m_chest.ChestSummon(Main.chest[Player.chest].x / 16, Main.chest[Player.chest].y / 16);
            }
            //ModContent.GetInstance<ArchaeaWorld>().downedMagno = true;
            //  ITEM TEXT and SKY FORT DEBUG GEN
            //if (!start && !Main.dedServ && KeyPress(Keys.F1) && KeyHold(Keys.Up))
            //{
            //    if (Main.netMode == 0)
            //    {
            //        Main.NewText("To enter commands, input [Tab + (Hold) Left Control] (instead of Enter), [F2 + LeftControl] for item spawning using chat search via item name, [F3 + LeftControl] for NPC debug and balancing", Color.LightBlue);
            //        Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold [Left Control + Left Alt] and click to go to mouse", textColor);
            //    }
            //    if (Main.netMode == 2)
            //        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Input /info and use [Tab] to list commands"), textColor);
            //    start = true;
            //}
            
            if (!start && KeyHold(Keys.LeftAlt))
            {
                if (KeyPress(Keys.LeftControl))
                {
                    //SkyHall hall = new SkyHall();
                    //hall.SkyFortGen();
                    /*
                    Vector2 position;
                    do
                    {
                        position = new Vector2(WorldGen.genRand.Next(200, Main.maxTilesX - 200), 50);
                    } while (position.X < Main.spawnTileX + 150 && position.X > Main.spawnTileX - 150);
                    var s = new Structures(position, ArchaeaWorld.skyBrick, ArchaeaWorld.skyBrickWall);
                    s.InitializeFort();
                    */
                    if (Main.netMode == 0)
                    {
                        for (int i = 0; i < Main.rightWorld / 16; i++)
                            for (int j = 0; j < Main.bottomWorld / 16; j++)
                            {
                                Main.mapInit = true;
                                Main.loadMap = true;
                                Main.refreshMap = true;
                                Main.updateMap = true;
                                Main.Map.Update(i, j, 255);
                                Main.Map.ConsumeUpdate(i, j);
                            }
                        start = true;
                    }
                }
            }
            if (KeyHold(Keys.LeftControl) && KeyHold(Keys.LeftAlt) && LeftClick())
            {
                if (Main.netMode == 2)
                    NetHandler.Send(Packet.TeleportPlayer, -1, -1, Player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
                else Player.Teleport(Main.MouseWorld);
            }

            
            //string chat = (string)Main.chatText.Clone();
            //bool enteredCommand = KeyPress(Keys.Tab);
            //if (chat.StartsWith("/info") && KeyHold(Keys.LeftControl))
            //{
            //    if (enteredCommand)
            //    {
            //        if (Main.netMode != 2)
            //        {
            //            Main.NewText("Commands: /list 'npcs' 'items1' 'items2' 'items3', /npc [name], /npc 'strike', /item [name], /spawn, /day, /night, /rain 'off' 'on', hold Left Control and click to go to mouse", textColor);
            //            Main.NewText("Press [F2] and type an item name in chat, then hover over item icon", textColor);
            //            Main.NewText("[F3] for NPC debug and balancing", textColor);
            //        }
            //    }
            //}
            //if (chat.StartsWith("/") && KeyHold(Keys.LeftControl))
            //{
            //    if (chat.StartsWith("/list"))
            //    {
            //        string[] npcs = new string[]
            //        {
            //            "Fanatic",
            //            "Hatchling_head",
            //            "Mimic",
            //            "Sky_1",
            //            "Sky_2",
            //            "Sky_3",
            //            "Slime_Itchy",
            //            "Slime_Mercurial",
            //            "Magnoliac_head",
            //            "Sky_boss",
            //            "Sky_boss_legacy"
            //        };
            //        string[] items1 = new string[]
            //        {
            //            "cinnabar_bow",
            //            "cinnabar_dagger",
            //            "cinnabar_hamaxe",
            //            "cinnabar_pickaxe",
            //            "magno_Book",
            //            "magno_summonstaff",
            //            "magno_treasurebag",
            //            "magno_trophy",
            //            "magno_yoyo"
            //        };
            //        string[] items2 = new string[]
            //        {
            //            "c_Staff",
            //            "c_Sword",
            //            "n_Staff",
            //            "r_Catcher",
            //            "r_Flail",
            //            "r_Javelin",
            //            "r_Tomohawk",
            //            "ShockLegs",
            //            "ShockMask",
            //            "ShockPlate"
            //        };
            //        string[] items3 = new string[]
            //        {
            //            "Broadsword",
            //            "Calling",
            //            "Deflector",
            //            "Sabre",
            //            "Staff"
            //        };
            //        if (chat.Contains("npcs"))
            //        {
            //            if (enteredCommand)
            //                foreach (string s in npcs)
            //                    Main.NewText(s + " " + mod.NPCType(s), textColor);
            //        }
            //        if (chat.Contains("items1"))
            //        {
            //            if (enteredCommand)
            //                foreach (string s in items1)
            //                    Main.NewText(s, textColor);
            //        }
            //        if (chat.Contains("items2"))
            //        {
            //            if (enteredCommand)
            //                foreach (string s in items2)
            //                    Main.NewText(s, textColor);
            //        }
            //        if (chat.Contains("items3"))
            //        {
            //            if (enteredCommand)
            //                foreach (string s in items3)
            //                    Main.NewText(s, textColor);
            //        }
            //    }
            //    if (chat.StartsWith("/npc"))
            //    {
            //        string text = Main.chatText.Substring(Main.chatText.IndexOf(' ') + 1);
            //        if (!chat.Contains("strike"))
            //        {
            //            if (enteredCommand)
            //            {
            //                NPC n = mod.GetNPC(text).npc;
            //                if (Main.netMode != 0)
            //                    NetHandler.Send(Packet.SpawnNPC, 256, -1, n.type, Main.MouseWorld.X, Main.MouseWorld.Y, player.whoAmI, n.boss);
            //                else
            //                {
            //                    if (n.boss)
            //                        NPC.SpawnOnPlayer(player.whoAmI, n.type);
            //                    else NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, n.type);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (enteredCommand)
            //                foreach (NPC npc in Main.npc)
            //                    if (npc.active && !npc.friendly && npc.life > 0)
            //                    {
            //                        npc.StrikeNPC(npc.lifeMax, 0f, 1, true);
            //                        if (Main.netMode != 0)
            //                            NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI);
            //                    }
            //        }
            //    }
            //    if (chat.StartsWith("/item"))
            //    {
            //        string text = Main.chatText;
            //        if (enteredCommand)
            //        {
            //            string itemType = text.Substring("/item ".Length);
            //            string stackCount = "";
            //            if (itemType.Count(t => t == ' ') != 0)
            //                stackCount = itemType.Substring(text.LastIndexOf(' ') + 1);
            //            bool modded = false;
            //            int type;
            //            int stack = 0;
            //            int.TryParse(stackCount, out stack);
            //            if (modded = !int.TryParse(itemType, out type))
            //                type = mod.ItemType(itemType);
            //            if (modded)
            //            {
            //                int t = Item.NewItem(Main.MouseWorld, type, mod.GetItem(itemType).item.maxStack);
            //                if (Main.netMode != 0)
            //                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
            //            }
            //            else
            //            {
            //                int.TryParse(stackCount, out stack);
            //                int t2 = Item.NewItem(Main.MouseWorld, type, stack == 0 ? 1 : stack);
            //                if (Main.netMode != 0)
            //                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t2);
            //            }
            //        }
            //    }
            //    if (chat.StartsWith("/spawn"))
            //        if (enteredCommand)
            //        {
            //            if (Main.netMode != 0)
            //                NetHandler.Send(Packet.TeleportPlayer, 256, -1, Main.LocalPlayer.whoAmI, Main.spawnTileX * 16, Main.spawnTileY * 16);
            //            else
            //                player.Teleport(new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16));
            //        }
            //    if (chat.StartsWith("/day"))
            //    {
            //        if (enteredCommand)
            //        {
            //            float time = 10f * 60f * 60f / 2f;
            //            if (Main.netMode == 0)
            //            {
            //                Main.dayTime = true;
            //                Main.time = time;
            //            }
            //            else NetHandler.Send(Packet.WorldTime, 256, -1, 0, time, 0f, 0, true);
            //        }
            //    }
            //    if (chat.StartsWith("/night"))
            //    {
            //        if (enteredCommand)
            //        {
            //            float time = 8f * 60f * 60f / 2f;
            //            if (Main.netMode == 0)
            //            {
            //                Main.dayTime = false;
            //                Main.time = time;
            //            }
            //            else NetHandler.Send(Packet.WorldTime, 256, -1, 0, time, 0f, 0, false);
            //        }
            //    }
            //    if (chat.StartsWith("/rain"))
            //    {
            //        if (chat.Contains("off"))
            //            if (enteredCommand)
            //                Main.raining = false;
            //        if (chat.Contains("on"))
            //            if (enteredCommand)
            //                Main.raining = true;
            //    }
            //}
            //if (enteredCommand)
            //{
            //    Main.chatText = string.Empty;
            //    Main.drawingPlayerChat = false;
            //    Main.chatRelease = false;
            //}
            if (KeyPress(Keys.F2) && KeyHold(Keys.LeftControl))
            {
                if (Main.netMode == 1)
                    NetHandler.Send(Packet.Debug, 256, -1, Player.whoAmI);
                else debugMenu = !debugMenu;
            }
            if (KeyPress(Keys.F3) && KeyHold(Keys.LeftControl))
            {
                if (Main.netMode == 1)
                    NetHandler.Send(Packet.Debug, 256, -1, Player.whoAmI, 1f);
                else spawnMenu = !spawnMenu;
            }
            #endregion
        }
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool RightHold()
        {
            return Main.mouseRight;
        }
        public static bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public static bool KeyHold(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }

        public override void PreUpdateMovement()
        {
            if (Factory && Player.wet)
            {
                Player.velocity /= 1.05f;
                if (ArchaeaItem.Elapsed(90))
                {
                    Player.Hurt(PlayerDeathReason.LegacyDefault(), 10, Player.direction);
                    Player.AddBuff(BuffID.Burning, 2, false);
                }
            }
        }
        public override void PostUpdateEquips()
        {
            if (CheckHasTrait(TraitID.ALL_TenDefense, ClassID.All))
            {
                Player.statDefense += 10;
            }
            if (CheckHasTrait(TraitID.SUMMONER_PlusCount, ClassID.Summoner))
            {
                Player.maxMinions += 2;
            }
            if (!ModContent.GetInstance<ModeToggle>().archaeaMode)
                return;
            //float ratio = 100f / 500f;
            //float result = Player.statDefense / ratio;
            //  Generic 10% buff
            Player.statDefense /= 0.9f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType == DamageClass.Melee)
            {
                if (Player.HasBuff(ModContent.BuffType<Buffs.flask_mercury>()))
                {
                    target.AddBuff(ModContent.BuffType<Buffs.mercury>(), 600);
                }
            }
        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (CheckHasTrait(TraitID.MAGE_DecreaseCost, ClassID.Magic))
            {
                mult = 0.8f;
            }
        }
        private bool flag = true;
        public void NPCVendorScaling(float scale = 1f)
        {
            //  Merchant discount
            if (flag && Player.talkNPC > 0)
            {
                Player.currentShoppingSettings.PriceAdjustment /= scale;
                flag = false;
            }
            if (flag)
            {
                oldPriceDiscount = Player.currentShoppingSettings.PriceAdjustment;
            }
            if (Player.talkNPC <= 0)
            {
                Player.currentShoppingSettings.PriceAdjustment = oldPriceDiscount;
                flag = true;
            }
        }
        private int counter = 0;
        public override void PostUpdate()
        {
            foreach (Room r in ArchaeaMod.Structure.Factory.room)
            {
                r.Update(Player);
            }
            if (!obtainedCatalogue[0] && !obtainedCatalogue[1])
            {
                Player.QuickSpawnItem(Player.GetSource_DropAsItem(), ModContent.ItemType<Jobs.Items.job_bag>());
                obtainedCatalogue[0] = true;
            }
            float totalDiscount = merchantDiscount;
            if (CheckHasTrait(TraitID.ALL_Job, ClassID.All))
            {
                totalDiscount += 0.1f;
            }
            NPCVendorScaling(totalDiscount);
            //  Trait characteristics
            //  Wall jump
            if (CheckHasTrait(TraitID.ALL_WallJump, ClassID.All))
            {
                int iLeft   = (int)(Player.position.X - 8) / 16;
                int iRight  = (int)(Player.position.Y + Player.width + 8) / 16;
                int j       = (int)(Player.position.Y + Player.height - 8) / 16;
                Tile tLeft  = Main.tile[iLeft, j];
                Tile tRight = Main.tile[iRight, j];
                if (tLeft.HasTile && Main.tileSolid[tLeft.TileType])
                {
                    if (ArchaeaMain.wallJump.JustPressed && Player.controlRight && Player.velocity.Y != 0)
                    {
                        Player.velocity.Y = -Player.jumpSpeed * 2f;
                        Player.fallStart = (int)Player.position.Y;
                        Player.fallStart2 = (int)Player.position.Y;
                    }
                }
                if (tRight.HasTile && Main.tileSolid[tRight.TileType])
                {
                    if (ArchaeaMain.wallJump.JustPressed && Player.controlLeft && Player.velocity.Y != 0)
                    {
                        Player.velocity.Y = -Player.jumpSpeed * 2f;
                        Player.fallStart = (int)Player.position.Y;
                        Player.fallStart2 = (int)Player.position.Y;
                    }
                }
            }
            //  Extra double jump
            if (CheckHasTrait(TraitID.ALL_DoubleJump, ClassID.All))
            {
                if (Player.velocity.Y != 0f)
                {
                    if (ArchaeaMain.doubleJump.JustPressed && counter == 0)
                    {
                        counter++;
                        Player.DoubleJumpVisuals();
                        SoundEngine.PlaySound(SoundID.DoubleJump, Player.Center);
                        Player.velocity.Y = -Player.jumpSpeed * 2f;
                        Player.fallStart = (int)Player.position.Y;
                        Player.fallStart2 = (int)Player.position.Y;
                    }
                }
                else 
                { 
                    counter = 0;
                }
            }

            if (outOfBounds)
            {
                effectTime--;
                for (float i = 0; i < Math.PI * 2f; i += Draw.radians(effectTime / 64f))
                {
                    int offset = 4;
                    float x = (float)(Player.Center.X - offset + (effectTime / 4) * Math.Cos(i));
                    float y = (float)(Player.Center.Y + (effectTime / 4) * Math.Sin(i));
                    var dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, DustID.Torch, Main.rand.NextFloat(-0.5f, 0.5f), -2f, 0, default(Color), 2f);
                    dust.noGravity = true;
                }
                if ((int)Main.time % 60 == 0)
                    boundsCheck++;
                if (boundsCheck == 5)
                {
                    if (oldPosition != Vector2.Zero)
                    {
                        if (Main.netMode == 0)
                            Player.Teleport(oldPosition);
                        else NetHandler.Send(Packet.TeleportPlayer, 256, -1, Player.whoAmI, oldPosition.X, oldPosition.Y);
                    }
                    oldPosition = Vector2.Zero;
                    effectTime = maxTime;
                    boundsCheck = 0;
                    outOfBounds = false;
                }
            }
            else
            {
                oldPosition = Vector2.Zero;
                effectTime = maxTime;
                boundsCheck = 0;
            }
            if (classChoice != ClassID.None && !classChosen)
            {
                var c = ArchaeaWorld.playerClass.FirstOrDefault(t => t.playerUID == playerUID);
                if (c == default)
                {
                    ArchaeaWorld.playerClass.Add(PlayerClass.NewPlayer(classChoice, playerUID, Player.whoAmI));
                }
                classChosen = true;
            }
            //  Keeping class choice consistent in PreUpdate
        /*  if (classChoice != ClassID.None && classChosen)
            { 
                _classChoice = classChoice;
            }*/

            //  side quests
            if (Player.position.Y > Main.UnderworldLayer * 16f)
            {
                SetClassTrait(TraitID.RANGED_Fire, ClassID.Ranged, true);
            }
            int stack = 0, maxStack = 999;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].type == ItemID.DirtBlock)
                {
                    stack += Player.inventory[i].stack;
                }
            }
            if (HasClassTrait(TraitID.RANGED_Ice, ClassID.Ranged) || stack < maxStack)
            {
                TRAIT_TIME_MaxDirtStack = 0;
                TRAIT_TIME_MAX_MaxDirtStack = 60 * 60 * 20 * 1;
            }
            else
            {
                TRAIT_TIME_MaxDirtStack += Main.frameRate;
                if (TRAIT_TIME_MaxDirtStack / 60 / 60 >= TRAIT_TIME_MAX_MaxDirtStack)
                {
                    SetClassTrait(TraitID.RANGED_Ice, ClassID.Ranged, true);
                }
            }
            //  DEBUG this
            //  Unknown how fallStart interacts with an entire fall
            //  Does it compare start fall tile to end fall tile? or does it increment?
            if (Player.velocity.Y == Player.maxFallSpeed && Player.ropeCount <= 0)
            {
                if (fallDistance == 0)
                { 
                    //  Refer to "tile fall data.txt"
                    fallDistance = (int)Player.position.Y / 16;
                }
                fallDistance2 = (int)Player.position.Y / 16;
            }
            else 
            {
                fallDistance2 = 0;
                fallDistance = 0;
            }
            comparison = Math.Max(fallDistance2 - fallDistance, 0);
            if (comparison >= 1000)
            {
                SetClassTrait(TraitID.MELEE_Leap, ClassID.Melee, true);
            }
            if (TRAIT_PlantedMushroom >= 80)
            {
                SetClassTrait(TraitID.MAGE_NoManaCost, ClassID.Magic, true);
            }
            if (Player.ZoneBeach)
            {
                SetClassTrait(TraitID.MAGE_NoKb, ClassID.Magic, true);
            }
            if (Player.ZoneMeteor)
            {
                SetClassTrait(TraitID.MAGE_DecreaseCost, ClassID.Magic, true);
            }
            if (TRAIT_PlacedBricks >= 500)
            {
                SetClassTrait(TraitID.SUMMONER_NoManaCost, ClassID.Summoner, true);
            }
            if (TRAIT_PlacedRails >= 1000)
            {
                SetClassTrait(TraitID.MAGE_DamageReduce, ClassID.Magic, true);
            }
            if (Player.position.Y / 16 < 100)
            {
                SetClassTrait(TraitID.SUMMONER_ThrowStar, ClassID.Summoner, true);
            }
            if (placedTiles >= 250)
            { 
                SetClassTrait(TraitID.MELEE_DoubleKb, ClassID.Melee, true);
            }
            if (Player.statManaMax2 == 200)
            {
                SetClassTrait(TraitID.MELEE_Flask, ClassID.Melee, true);
            }
            SetClassTrait(TraitID.RANGED_Tracking, ClassID.Ranged, Player.downedDD2EventAnyDifficulty);
            SetClassTrait(TraitID.RANGED_Ichor, ClassID.Ranged, NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3);
            SetClassTrait(TraitID.MELEE_DoubleSwing, ClassID.Melee, NPC.downedGoblins && NPC.downedMartians && NPC.downedPirates);
        }
        public static void RadialDustDiffusion(Vector2 origin, double x, double y, float radius, int dustType, int damage, bool hostile, int owner = 255)
        {
            int index = Projectile.NewProjectile(Projectile.GetSource_None(), new Vector2((float)x, (float)y), Vector2.Zero, ModContent.ProjectileType<Projectiles.dust_diffusion>(), damage, 1f, owner, dustType, radius);
            Main.projectile[index].timeLeft = 200;
            Main.projectile[index].tileCollide = false;
            Main.projectile[index].localAI[0] = 10;
            Main.projectile[index].hostile = hostile;
        }
        public static void RadialDustDiffusion(Vector2 origin, double x, double y, float radius, int frequency, int dustType, int damage, bool hostile, int owner = 255)
        {
            if (Main.dedServ) return;
            if (frequency <= 0)
                frequency = 10;
            if ((int)Main.time % 10 == 0)
            {
                int index = Projectile.NewProjectile(Projectile.GetSource_None(), new Vector2((float)x, (float)y), Vector2.Zero, ModContent.ProjectileType<Projectiles.dust_diffusion>(), damage, 1f, owner, dustType, radius);
                Main.projectile[index].timeLeft = 200;
                Main.projectile[index].tileCollide = false;
                Main.projectile[index].localAI[0] = 10;
                Main.projectile[index].localAI[1] = radius;
                Main.projectile[index].hostile = hostile;
                Main.projectile[index].netUpdate = true;
            }
        }
        public bool ClassItemCheck()
        {
            Item item = Player.inventory[Player.selectedItem];
            bool nonTool = item.pick == 0 && item.axe == 0 && item.hammer == 0;
            switch (classChoice)
            {
                case -1:
                    if (nonTool && item.damage > 0)
                        return false;
                    break;
                case ClassID.Melee:
                    if (!item.CountsAsClass(DamageClass.Melee))
                        goto case -1;
                    break;
                case ClassID.Magic:
                    if (!item.CountsAsClass(DamageClass.Magic))
                        goto case -1;
                    break;
                case ClassID.Ranged:
                    if (!item.CountsAsClass(DamageClass.Ranged))
                        goto case -1;
                    break;
                case ClassID.Summoner:
                    if (!item.CountsAsClass(DamageClass.Summon))
                        goto case -1;
                    break;
                case ClassID.Throwing:
                    if (!item.CountsAsClass(DamageClass.Throwing))
                        goto case -1;
                    break;
                case ClassID.All:
                    break;
            }
            return true;
        }
        public void ClassHotbar()
        {
            for (int i = 0; i < 10; i++)
            {
                Item item = Player.inventory[i];
                bool nonTool = item.pick == 0 && item.axe == 0 && item.hammer == 0;
                switch (classChoice)
                {
                    case ClassID.Melee:
                        if (!item.CountsAsClass(DamageClass.Melee) && nonTool && item.damage > 0)
                        {
                            MoveItem(item);
                            item.type = ItemID.None;
                            return;
                        }
                        break;
                }
            }
        }
        private void MoveItem(Item item)
        {
            for (int i = Player.inventory.Length - 10; i >= 10; i--)
            {
                Item slot = Player.inventory[i];
                if (slot.Name == "" || slot.stack < 1 || slot == null || slot.type == ItemID.None)
                {
                    Player.inventory[i] = item.Clone();
                    return;
                }
            }
        }
        
        public static Color[] zoneColor = new Color[]
        {
            Color.LightYellow,
            Color.Plum,
            Color.DarkRed,
            Color.Yellow,
            Color.SlateGray,

        };
            
        public void BiomeBounds()
        {
            zones = new bool[]
            {
                Player.ZoneBeach,
                Player.ZoneCorrupt,
                Player.ZoneCrimson,
                Player.ZoneDesert,
                Player.ZoneDungeon,
                Player.ZoneHallow,
                Player.ZoneJungle,
                Player.ZoneMeteor,
                Player.ZoneOverworldHeight,
                Player.ZoneSnow,
                Player.ZoneUndergroundDesert,
                SkyFort,
                MagnoBiome
            };
            var modWorld = ModContent.GetInstance<ArchaeaWorld>();
            if (modWorld.cordonBounds)
            {
                for (int i = 0; i < zones.Length; i++)
                {
                    if (zones[i])
                    {
                        if (outOfBounds = !ObjectiveMet(i))
                        {
                            if (oldPosition == Vector2.Zero)
                                oldPosition = Player.position;
                            break;
                        }
                        else
                        {
                            outOfBounds = false;
                        }
                    }
                }
            }
        }
        private bool ObjectiveMet(int zone)
        {
            switch (zone)
            {
                case BiomeID.Beach:
                    return true;
                case BiomeID.Desert:
                    break;
                case BiomeID.Snow:
                    return true;
                case BiomeID.Fort:
                    break;
            }
            return false;
        }
        public void DarkenedVision()
        {
            if (!SkyFort || ModContent.GetInstance<ArchaeaWorld>().downedNecrosis)
            {
                if (darkAlpha > 0f)
                { 
                    darkAlpha -= 1f / 150f;
                }
            }
            else
            {
                float spaceLayer = (float)Math.Min(1f, Math.Max(0f, Main.worldSurface * 16 / 2.5f / Player.position.Y));
                if (darkAlpha < spaceLayer)
                {
                    darkAlpha += 1f / 150f;
                }
                else darkAlpha -= 1f / 150f;
            }
            Texture2D texture = TextureAssets.MagicPixel.Value;
            Color color = Color.Black * Math.Min(darkAlpha, 1f);
            int range = 200;
            int side = Main.screenWidth / 2 - range + (Main.screenWidth % 2);
            int top = Main.screenHeight / 2 - range + (Main.screenHeight % 2);

            var _left = new Rectangle(0, 0, side, Main.screenHeight);
            var _right = new Rectangle(Main.screenWidth - side, 0, side, Main.screenHeight);
            var _top = new Rectangle(side, 0, range * 2, top);
            var _bottom = new Rectangle(side, Main.screenHeight - top, range * 2, top);
            var _center = new Rectangle(side, top, range * 2, range * 2);
            if (_top.Intersects(_right))
                _top.Width -= 1;
            if (_bottom.Intersects(_right))
                _bottom.Width -= 1;
            if (_center.Intersects(_bottom))
                _center.Height -= 1;
            if (_center.Intersects(_right))
                _center.Width -= 1;

            sb.Draw(texture, _left, color);
            sb.Draw(texture, _right, color);
            sb.Draw(texture, _top, color);
            sb.Draw(texture, _bottom, color);
            sb.Draw(Mod.Assets.Request<Texture2D>("Gores/fort_vignette_ui").Value, _center, color);
        }
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        int t = 180;
        public static bool OptionsDisplayed = true;
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //  Need a little bit of engineering to resize
            /* if (blueprint != Rectangle.Empty)
               {
                   if (blueprint.Resize())
                   { 
                       blueprint.Width  = blueprint.Width  + (blueprint.Width % 16);
                       blueprint.Height = blueprint.Height + (blueprint.Height % 16);
                   }
                   sb.Draw(TextureAssets.MagicPixel.Value, blueprint, Color.CornflowerBlue * 0.2f);
                   sb.DrawString(FontAssets.MouseText.Value, (blueprint.Width * 2 / 16 + blueprint.Height * 2 / 16).ToString(), blueprint.TopLeft(), Color.White);
                   SetXY(accept.box, blueprint.Left, blueprint.Bottom - accept.box.Height * 2 - 4);
                   SetXY(cancel.box, blueprint.Left, blueprint.Bottom - cancel.box.Height);
                   accept.Draw();
                   cancel.Draw();
                   if (accept.LeftClick())
                   {
                       int i = (int)(blueprint.X + Main.screenPosition.X) / 16;
                       int j = (int)(blueprint.Y + Main.screenPosition.Y) / 16;
                       int w = blueprint.Width  / 16;
                       int h = blueprint.Height / 16;
                       for (int m = i; m < i + w; m++)
                       {
                           for (int n = j; n < j + h; n++)
                           {
                           }
                       }
                   }
                   if (cancel.LeftClick())
                   {
                       blueprint = Rectangle.Empty;
                   }
               }   */
            if (/*classChoice == ClassID.None &&*/ drawInfo.drawPlayer.active && drawInfo.drawPlayer.whoAmI == Main.LocalPlayer.whoAmI && !drawInfo.drawPlayer.dead)
            {
                var c = ArchaeaWorld.playerClass.FirstOrDefault(t => t.playerUID == playerUID);
                if (c != default)
                {
                    classChoice = c.classChoice;
                }
                if (OptionsUI.MainOptions(drawInfo.drawPlayer, setInitMode))
                    setInitMode = false;
            }
            if (drawInfo.drawPlayer.active && drawInfo.drawPlayer.whoAmI == Main.LocalPlayer.whoAmI)
            {
                if (!Main.dedServ && Effects.Barrier.barrier != null)
                {
                    for (int i = 0; i < Effects.Barrier.barrier.Length; i++)
                        Effects.Barrier.barrier[i]?.Draw(sb, Player);
                }
            }
            if (drawInfo.drawPlayer.active)
            {
                ModeUI.DrawTextUI(sb, Main.screenHeight - 200, "Set hotkeys in the Control settings.", ref enterWorldTicks, 1800);
            }
            if (dungeonLocatorTicks > 0 && dungeonLocatorTicks < 900)
            {
                string text = locatorDirection == -1 ? "Dungeon left." : "Dungeon right.";
                ModeUI.DrawTextUI(sb, Main.screenHeight - 200, text, ref locatorDirection, 900);
            }
            if (ArchaeaMain.extraLife.JustPressed)
            {
                t = 0;
            }
            if (t >= 0 && t <= 300)
            {
                ModeUI.DrawTextUI(sb, Main.screenHeight - 200, $"Extra lives: {extraLife}", ref t, 300);
            }
            if (Player.statLifeMax == 100)
            {
                hintTicks = 0;
            }
            if (Player.statLifeMax == 600)
            {
                if (!hasUsedLifeCrystal)
                {
                    ModeUI.DrawTextUI(sb, Main.screenHeight - 200, "Reminder: Archaea Mode is enabled.", ref hintTicks, 600);
                    if (hintTicks >= 600)
                    {
                        hasUsedLifeCrystal = true;
                    }
                }
            }
            if (debugMenu)
                DebugMenu();
            if (spawnMenu)
                SpawnMenu();
            #region innactive draw testing
            
            return;
            /*
            //var tex = ModContent.GetInstance<Items.Alternate.MagnoCannon>().tex;
            if (Items.Alternate.MagnoCannon.tex != null && Player.controlUseItem)
            {
                sb.Draw(Items.Alternate.MagnoCannon.tex, Player.Center - Main.screenPosition, null, Color.White, Items.Alternate.MagnoCannon.angle, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            //  START
            float x = drawInfo.drawPlayer.Center.X;
            float y = drawInfo.drawPlayer.Center.Y;
            int width = 32 * 3;
            int height = 32 * 3;
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Purple);
            pen.Width = 2;
            var mem = Effects.Fx.GenerateImage(polygon, width * 2, height * 2, pen, System.Drawing.Color.Green);
            Texture2D tex = Effects.Fx.FromStream(mem);

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            sb.Draw(tex, new Vector2(x - width, y - height) - Main.screenPosition, Color.White);
            //  END
            */
            #endregion
        }
        private bool init;
        private bool flag2;
        private List<string> name = new List<string>();
        private List<int> id = new List<int>();
        private List<ItemDebug> debugList = new List<ItemDebug>();
        private void DebugMenu()
        {
            if (!flag2)
            {
                debugList.Clear();
                for (int i = 0; i < TextureAssets.Item.Length; i++)
                {
                    int item = Item.NewItem(Item.GetSource_None(), Vector2.Zero, i, 1);
                    debugList.Add(new ItemDebug()
                    {
                        index = i,
                        texture = TextureAssets.Item[i].Value,
                        name = Main.item[item].Name
                    });
                    if (item < Main.item.Length)
                        Main.item[item].active = false;
                }
                flag2 = true;
            }
            Func<string, Texture2D[]> search = delegate(string Name)
            {
                List<Texture2D> t = new List<Texture2D>();
                if (Name.Length > 2 && debugList != null && debugList.Count > 0)
                {
                    for (int i = 0; i < debugList.Count; i++)
                    {
                        if (debugList[i].name.ToLower().Contains(Name.ToLower()))
                        {
                            t.Add(TextureAssets.Item[i].Value);
                        }
                    }
                }
                t.Add(TextureAssets.MagicPixel.Value);
                return t.ToArray();
            };
            if (Main.chatText != null && Main.chatText.Length > 2)
            {
                Texture2D[] array = search(Main.chatText);
                if (array != null && array.Length > 0 && array[0] != TextureAssets.MagicPixel.Value)
                {
                    //int.TryParse(Main.chatText, out int num);
                    ItemDebug _index = debugList.FirstOrDefault(t => t.name.ToLower() == Main.chatText.ToLower());// || t.name.ToLower().Contains(Main.chatText.ToLower()));//TextureAssets.Item..ToList().IndexOf(array[0]); // Need to translate Texture2D value to a texture asset
                    if (_index == default)
                        return;
                    int x = 20;
                    int y = 112;
                    sb.Draw(_index.texture, new Vector2(x, y), Color.White);
                    sb.DrawString(FontAssets.MouseText.Value, string.Format("{0} {1}", _index.name, _index.index), new Vector2(x + 50, y + 4), Color.White);
                    
                    Rectangle grab = new Rectangle(x, y, 48, 48);
                    if (grab.Contains(Main.MouseScreen.ToPoint()))
                    {
                        sb.DrawString(FontAssets.MouseText.Value, "Left/Right click", new Vector2(x, y + 50), Color.White);
                        if (LeftClick() || RightHold())
                        {
                            int t = Item.NewItem(Item.GetSource_None(), Player.Center, _index.index);
                            if (Main.netMode != 0)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
                        }
                    }
                }
            }
        }
        class ItemDebug
        {
            public int index;
            public string name;
            public Texture2D texture;
            public Item item;
        }
        private void SpawnMenu()
        {
            int x = 80;
            int y = 180;
            int width = 300;
            int height = 106;
            if (!initMenu)
            {
                label = new string[]
                {
                    "ID/Name",
                    "Life",
                    "Defense",
                    "Damage",
                    "KB Resist"
                };
                box = new Rectangle(x - 10, y, width + 164, height);
                input = new TextBox[]
                {
                    new TextBox(new Rectangle(x + 100, y + 4, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 24, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 44, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 64, width - 20, 18)),
                    new TextBox(new Rectangle(x + 100, y + 84, width - 20, 18))
                };
                button = new Button[]
                {
                    new Button("Spawn", new Rectangle(x + width + 90, y + 4, 60, 18)),
                    new Button("Clear", new Rectangle(x + width + 90, y + 34, 60, 18))
                };
                initMenu = true;
            }
            sb.Draw(TextureAssets.MagicPixel.Value, box, Color.Black * 0.25f);
            for (int n = 0; n < label.Length; n++)
                sb.DrawString(FontAssets.MouseText.Value, label[n], new Vector2(x - 6, y + 4 + n * 20), Color.White * 0.9f);
            foreach (TextBox t in input)
            {
                if (t.box.Contains(Main.MouseScreen.ToPoint()) && LeftClick())
                {
                    foreach (var i in input)
                        i.active = false;
                    t.active = true;
                }
                if (t.active)
                    t.UpdateInput();
                t.DrawText();
            }
            foreach (Button b in button)
            {
                if (b.LeftClick())
                {
                    if (b.text == "Clear")
                    {
                        foreach (var t in input)
                        {
                            t.text = "";
                            t.active = false;
                        }
                    }
                    else if (b.text == "Spawn")
                    {
                        float[] vars = new float[5];
                        for (int i = 0; i < input.Length; i++)
                        {
                            float.TryParse(input[i].text, out vars[i]);
                        }
                        float randX = Main.rand.NextFloat(Player.position.X - 300, Player.position.X + 300);
                        float Y = Player.position.Y - 100;
                        if (!int.TryParse(input[0].text, out _))
                        {
                            string s = input[0].text.Substring(0, 1).ToUpper();
                            string search = s + input[0].text.Substring(1);
                            if (!NPCID.Search.TryGetId(search, out int num))
                            {
                                search = "ArchaeaMod/" + search;
                            }
                            NPCID.Search.TryGetId(search, out num);
                            vars[0] = num;
                        }
                        if (Main.netMode != 0)
                            NetHandler.Send(Packet.SpawnNPC, -1, -1, (int)vars[0], vars[1], vars[2], (int)vars[3], false, vars[4], Main.MouseWorld.X, Main.MouseWorld.Y);
                        else
                        {
                            int n = NPC.NewNPC(NPC.GetSource_None(), (int)randX, (int)Y, (int)vars[0], 0);
                            Main.npc[n].lifeMax = (int)vars[1];
                            Main.npc[n].life = (int)vars[1];
                            Main.npc[n].defense = (int)vars[2];
                            Main.npc[n].damage = (int)vars[3];
                            Main.npc[n].knockBackResist = vars[4];
                        }
                    }
                }
                b.Draw();
            }
        }
        sealed class BiomeID
        {
            public const int
                Beach = 0,
                Corrupt = 1,
                Crimson = 2,
                Desert = 3,
                Dungeon = 4,
                Hallowed = 5,
                Jungle = 6,
                Meteor = 7,
                Overworld = 8,
                Snow = 9,
                UGDesert = 10,
                Fort = 11,
                Magno = 12;
        }
        public static bool IsEquipped(Player player, int head, int body, int legs)
        {
            if (player.armor[0].type == head && player.armor[1].type == body && player.armor[2].type == legs)
                return true;
            return false;
        }
        public static bool AccIsEquipped(Player player, int type)
        {
            return player.armor.Where(t => t.type == type).Count() > 0;
        }
        int numProjectiles = 0;
        public bool fireStorm = false;
        public void FireStorm()
        {
            if (Main.rand.NextBool(3))
            {
                if (fireStorm)
                {
                    int i = Main.rand.Next(1, 3);
                    int k = Main.rand.Next(0 + i, 10 + i);
                    int dmg = (int)(15 * (float)i - 0.5f);
                    float kb = (float)i - 0.5f;                                                                                                                                                                                                                             //  Add FireRain projectile
                    int FireRain = Projectile.NewProjectile(Projectile.GetSource_None(), new Vector2(Player.position.X + (float)Main.rand.Next(-100 * k, 100 * k), Player.position.Y - 800f + (float)Main.rand.Next(-50, 50)), new Vector2(Main.rand.Next(-2 * i, 2 * i), 12f), ModContent.ProjectileType<FireRain>(), dmg, kb, Player.whoAmI);
                    Main.projectile[FireRain].aiStyle = 0;
                    Main.projectile[FireRain].timeLeft = 300;
                    Main.projectile[FireRain].tileCollide = false;
                    Main.projectile[FireRain].scale *= i - 0.5f;
                    Main.projectile[FireRain].velocity.Y *= Main.rand.Next(1, 2) * 0.75f;
                    Main.projectile[FireRain].hostile = false;
                    Main.projectile[FireRain].friendly = true;
                    Main.projectile[FireRain].penetrate = -1;
                    Main.projectile[FireRain].netUpdate = true;
                    if (Main.rand.NextBool(3))
                    { 
                        SoundEngine.PlaySound(SoundID.Item8, Main.projectile[FireRain].Center);
                    }
                    numProjectiles--;
                }
            }
            if (numProjectiles <= 0)
            {
                fireStorm = false;
                numProjectiles = 150;
            }
        }
    }

    public class Draw
    {
        public const float radian = 0.017f;
        public static float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
}
