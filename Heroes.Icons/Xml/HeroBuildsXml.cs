﻿using Heroes.Icons.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Heroes.Icons.Xml
{
    internal class HeroBuildsXml : XmlBase, IHeroBuilds
    {
        private int SelectedBuild;

        /// <summary>
        /// key is the build number
        /// </summary>
        private Dictionary<int, Tuple<string, string>> BuildPatchNotesByBuildNumber = new Dictionary<int, Tuple<string, string>>();

        /// <summary>
        /// key is attributeid, value is hero name
        /// </summary>
        private Dictionary<string, string> RealHeroNameByAttributeId = new Dictionary<string, string>();

        /// <summary>
        /// key is alt hero name, value real name
        /// example: Anubarak, Anub'arak
        /// </summary>
        private Dictionary<string, string> RealHeroNameByShortName = new Dictionary<string, string>();

        /// <summary>
        /// key is hero unit name, value real name
        /// example: HeroAbathur, Abathur
        /// </summary>
        private Dictionary<string, string> RealHeroNameByHeroUnitName = new Dictionary<string, string>();

        /// <summary>
        /// key is real hero name
        /// </summary>
        private Dictionary<string, Hero> HeroByHeroName = new Dictionary<string, Hero>();

        private HeroBuildsXml(string parentFile, string xmlBaseFolder, bool logger, int? build = null)
            : base(build ?? 0, logger)
        {
            XmlParentFile = parentFile;
            XmlBaseFolder = xmlBaseFolder;

            if (build == null)
                SetDefaultBuildDirectory();
            else
                SelectedBuild = build.Value;

            XmlFolder = Path.Combine(xmlBaseFolder, SelectedBuild.ToString());
        }

        public int CurrentLoadedHeroesBuild { get { return SelectedBuild; } }
        public int EarliestHeroesBuild { get; private set; } // cleared once initialized
        public int LatestHeroesBuild { get; private set; } // cleared once initialized
        public List<int> Builds { get; private set; } = new List<int>();

        public static HeroBuildsXml Initialize(string parentFile, string xmlBaseFolder, bool logger, int? build = null)
        {
            HeroBuildsXml xml = new HeroBuildsXml(parentFile, xmlBaseFolder, logger, build);
            xml.Parse();
            return xml;
        }

        /// <summary>
        /// Get the patch notes link from the given build number. Returns null if not found
        /// </summary>
        /// <param name="build">The build number</param>
        /// <returns></returns>
        public Tuple<string, string> GetPatchNotes(int build)
        {
            if (BuildPatchNotesByBuildNumber.TryGetValue(build, out Tuple<string, string> notes))
                return notes;
            else
                return null;
        }

        /// <summary>
        /// Returns the real hero name from the hero's attribute id
        /// </summary>
        /// <param name="attributeId">Four character hero id</param>
        /// <returns>Full hero name</returns>
        public string GetRealHeroNameFromAttributeId(string attributeId)
        {
            // no pick
            if (string.IsNullOrEmpty(attributeId))
                return string.Empty;

            if (RealHeroNameByAttributeId.TryGetValue(attributeId, out string heroName))
            {
                return heroName;
            }
            else
            {
                LogReferenceNameNotFound($"No hero name for attribute: {attributeId}");
                return null;
            }
        }

        /// <summary>
        /// Returns the real hero name from the short name
        /// </summary>
        /// <param name="shortName">short name of hero</param>
        /// <returns></returns>
        public string GetRealHeroNameFromShortName(string shortName)
        {
            // no pick
            if (string.IsNullOrEmpty(shortName))
                return string.Empty;

            if (RealHeroNameByShortName.TryGetValue(shortName, out string realName))
                return realName;
            else
                return null;
        }

        /// <summary>
        /// Returns the real hero name from the hero unit name
        /// </summary>
        /// <param name="heroUnitName">hero unit name</param>
        /// <returns></returns>
        public string GetRealHeroNameFromHeroUnitName(string heroUnitName)
        {
            // no pick
            if (string.IsNullOrEmpty(heroUnitName))
                return string.Empty;

            if (RealHeroNameByHeroUnitName.TryGetValue(heroUnitName, out string realName))
                return realName;
            else
                return null;
        }

        /// <summary>
        /// Checks to see if the hero name exists
        /// </summary>
        /// <param name="heroName">Real name of hero or short name</param>
        /// <returns>True if found</returns>
        public bool HeroExists(string heroName)
        {
            string realName = GetRealHeroNameFromShortName(heroName);

            if (string.IsNullOrEmpty(realName))
                realName = heroName;

            return HeroByHeroName.ContainsKey(realName);
        }

        /// <summary>
        /// Returns a list of (real) hero names for the given build
        /// </summary>
        /// <param name="build">The build number</param>
        /// <returns></returns>
        public List<string> GetListOfHeroes(int build)
        {
            List<string> heroes = new List<string>();
            foreach (var hero in HeroByHeroName)
            {
                if (hero.Value.BuildAvailable <= build)
                    heroes.Add(hero.Value.Name);
            }

            heroes.Sort();
            return heroes;
        }

        /// <summary>
        /// Returns the total amount of heroes (latest build)
        /// </summary>
        /// <returns></returns>
        public int TotalAmountOfHeroes()
        {
            return HeroByHeroName.Count;
        }

        /// <summary>
        /// Returns a Hero object
        /// </summary>
        /// <param name="heroName">Can be the real hero name or short name</param>
        /// <returns></returns>
        public Hero GetHeroInfo(string heroName)
        {
            string realName = GetRealHeroNameFromShortName(heroName);

            if (string.IsNullOrEmpty(realName))
                realName = heroName;

            if (heroName == null)
            {
                return new Hero
                {
                    Name = heroName,
                    Franchise = HeroFranchise.Unknown,
                    HeroPortrait = SetImageStreamString(HeroPortraitsFolderName, NoPortraitPick),
                    LoadingPortrait = SetImageStreamString(HeroLoadingScreenPortraitsFolderName, NoLoadingScreenPick),
                    LeaderboardPortrait = SetImageStreamString(HeroLeaderboardPortraitsFolderName, NoLeaderboardPick),
                };
            }

            if (HeroByHeroName.TryGetValue(realName, out Hero hero))
            {
                return hero;
            }
            else
            {
                return new Hero
                {
                    Name = heroName,
                    Franchise = HeroFranchise.Unknown,
                    HeroPortrait = SetImageStreamString(HeroPortraitsFolderName, NoPortraitFound),
                    LoadingPortrait = SetImageStreamString(HeroLoadingScreenPortraitsFolderName, NoLoadingScreenFound),
                    LeaderboardPortrait = SetImageStreamString(HeroLeaderboardPortraitsFolderName, NoLeaderboardFound),
                };
            }
        }

        protected override void Parse()
        {
            DuplicateBuildCheck();
            ParseParentFile();
            ParseChildFiles();
        }

        protected override void ParseChildFiles()
        {
            // create local variables for tooltips
            Dictionary<string, string> talentShortTooltip = new Dictionary<string, string>();
            Dictionary<string, string> talentLongTooltip = new Dictionary<string, string>();

            // key is the real hero name, value is the description of the hero
            Dictionary<string, string> heroDescriptionByHeroName = new Dictionary<string, string>();

            // load up all the talents
            LoadTalentTooltipStrings(talentShortTooltip, talentLongTooltip);

            // load up hero descriptions
            LoadHeroDescriptions(heroDescriptionByHeroName);

            foreach (var heroName in XmlChildFiles)
            {
                using (XmlReader reader = XmlReader.Create(Path.Combine(XmlMainFolderName, XmlBaseFolder, SelectedBuild.ToString(), $"{heroName}{DefaultFileExtension}"), GetXmlReaderSettings()))
                {
                    reader.MoveToContent();

                    string heroShortName = reader.Name;
                    if (heroShortName != heroName)
                        continue;

                    Hero hero = new Hero()
                    {
                        ShortName = reader.Name,
                    };

                    heroDescriptionByHeroName.TryGetValue(heroName, out string heroDescription);

                    // order is important
                    ParseHeroInformation(reader, hero, heroDescription);
                    ParseHeroRoles(reader, hero);
                    ParseHeroAbilities(reader, hero, talentShortTooltip, talentLongTooltip);
                    ParseHeroTalents(reader, hero, talentShortTooltip, talentLongTooltip);
                }
            }
        }

        // this should only run once on startup
        private void SetDefaultBuildDirectory()
        {
            // load up the builds from Builds.xml
            using (XmlReader reader = XmlReader.Create(Path.Combine(XmlMainFolderName, XmlBaseFolder, "Builds.xml")))
            {
                reader.MoveToContent();
                if (reader.Name != "Builds")
                    return;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string type = reader["type"];
                        string link = reader["link"];
                        string pre = reader["pre"];

                        try
                        {
                            if (reader.Read())
                            {
                                if (string.IsNullOrEmpty(pre))
                                {
                                    BuildPatchNotesByBuildNumber.Add(Convert.ToInt32(reader.Value), new Tuple<string, string>(type, link));
                                }
                                else
                                {
                                    var previousBuild = BuildPatchNotesByBuildNumber[Convert.ToInt32(pre)];
                                    BuildPatchNotesByBuildNumber.Add(Convert.ToInt32(reader.Value), new Tuple<string, string>(previousBuild.Item1, previousBuild.Item2));
                                }
                            }
                        }
                        catch (FormatException ex)
                        {
                            throw new ParseXmlException($"Could not convert to Int32: {pre} | {reader.Value}", ex);
                        }
                        catch (Exception ex)
                        {
                            throw new ParseXmlException($"Error on reading HeroBuilds.xml: {pre} | {reader.Value}", ex);
                        }
                    }
                }
            }

            foreach (var build in BuildPatchNotesByBuildNumber)
            {
                int buildNumber = build.Key;

                if (buildNumber >= MinimumBuild && !Directory.Exists(Path.Combine(XmlMainFolderName, XmlBaseFolder, buildNumber.ToString())))
                    throw new ParseXmlException($"Could not find required Build Folder: {Path.Combine(XmlMainFolderName, XmlBaseFolder, buildNumber.ToString())}");
                else
                    Builds.Add(buildNumber);
            }

            Builds = Builds.OrderByDescending(x => x).ToList();

            EarliestHeroesBuild = Builds[Builds.Count - 1] < MinimumBuild ? MinimumBuild : Builds[Builds.Count - 1];
            LatestHeroesBuild = SelectedBuild = Builds[0];
        }

        private string SetHeroTalentString(string hero, string fileName, bool isGenericTalent)
        {
            if (!(Path.GetExtension(fileName) != ".dds" || Path.GetExtension(fileName) != ".png"))
                throw new HeroesIconException($"Image file does not have .dds or .png extension [{fileName}]");

            if (!isGenericTalent)
                return SetImageStreamString($"{TalentFolderName}.{hero}", fileName.ToLowerInvariant());
            else
                return SetImageStreamString($"{TalentFolderName}.{TalentGenericFolderName}", fileName.ToLowerInvariant());
        }

        private void LoadTalentTooltipStrings(Dictionary<string, string> talentShortTooltip, Dictionary<string, string> talentFullTooltip)
        {
            try
            {
                using (StreamReader reader = new StreamReader(File.Open(Path.Combine(XmlMainFolderName, XmlBaseFolder, SelectedBuild.ToString(), ShortTalentTooltipFileName), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!line.StartsWith("--"))
                        {
                            string[] talent = line.Split(new char[] { '=' }, 2);

                            if (talentShortTooltip.ContainsKey(talent[0]))
                                throw new ArgumentException($"An item with the same key has already been added in Short Tooltips: {talent[0]}");

                            talentShortTooltip.Add(talent[0], talent[1]);
                        }
                    }
                }

                using (StreamReader reader = new StreamReader(File.Open(Path.Combine(XmlMainFolderName, XmlBaseFolder, SelectedBuild.ToString(), FullTalentTooltipFileName), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!line.StartsWith("--"))
                        {
                            string[] talent = line.Split(new char[] { '=' }, 2);

                            if (talentFullTooltip.ContainsKey(talent[0]))
                                throw new ArgumentException($"An item with the same key has already been added in Full Tooltips: {talent[0]}");

                            talentFullTooltip.Add(talent[0], talent[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseXmlException("Error on loading talent tooltips", ex);
            }
        }

        private void DuplicateBuildCheck()
        {
            while (CurrentBuild != 47479)
            {
                using (XmlReader reader = XmlReader.Create(Path.Combine(XmlMainFolderName, XmlFolder, XmlParentFile), GetXmlReaderSettings()))
                {
                    reader.MoveToContent();
                    string previousBuild = reader["pre"]; // check to see if we should load up a previous build

                    if (string.IsNullOrEmpty(previousBuild))
                        return;

                    if (int.TryParse(previousBuild, out int build))
                    {
                        XmlFolder = $@"{XmlBaseFolder}/{build}";
                        SelectedBuild = build;
                    }
                }
            }
        }

        private void LoadHeroDescriptions(Dictionary<string, string> heroDescriptions)
        {
            if (SelectedBuild < DescriptionsAddedBuild)
                return;

            try
            {
                using (StreamReader reader = new StreamReader(File.Open(Path.Combine(XmlMainFolderName, XmlBaseFolder, SelectedBuild.ToString(), HeroDescriptionsFileName), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!line.StartsWith("--"))
                        {
                            string[] description = line.Split(new char[] { '=' }, 2);

                            if (heroDescriptions.ContainsKey(description[0]))
                                throw new ArgumentException($"An item with the same key has already been added in hero descriptions: {description[0]}");

                            heroDescriptions.Add(description[0], description[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseXmlException("Error on loading hero desciptions", ex);
            }
        }

        private void ParseHeroInformation(XmlReader reader, Hero hero, string heroDescription)
        {
            // get real name
            // example: Anubarak-> (real) Anub'arak
            hero.Name = reader["name"];
            if (string.IsNullOrEmpty(hero.Name))
                hero.Name = hero.ShortName; // default to hero name

            // set the build that the hero is added
            hero.BuildAvailable = int.TryParse(reader["available"], out int buildAvailable) ? buildAvailable : 0;

            // set attributeid from hero name
            // example: Anub
            hero.AttributeId = reader["attributeid"];

            // set the unit name
            hero.UnitName = reader["unit"];

            // set the franchise: classic, diablo, overwatch, starcraft, warcraft
            hero.Franchise = Enum.TryParse(reader["franchise"], out HeroFranchise heroFranchise) ? heroFranchise : HeroFranchise.Unknown;

            // set the hero type - melee or ranged
            hero.Type = Enum.TryParse(reader["type"], out HeroType heroType) ? heroType : HeroType.Unknown;

            // set the difficulty of the hero - easy/medium/hard/etc...
            hero.Difficulty = Enum.TryParse(reader["difficulty"], out HeroDifficulty heroDifficulty) ? heroDifficulty : HeroDifficulty.Unknown;

            // set hero mana type
            hero.ManaType = Enum.TryParse(reader["mana"], out HeroMana heroMana) ? heroMana : HeroMana.Mana;

            // set portraits
            hero.HeroPortrait = SetImageStreamString(HeroPortraitsFolderName, reader["portrait"]);
            hero.LeaderboardPortrait = SetImageStreamString(HeroLeaderboardPortraitsFolderName, reader["leader"]);
            hero.LoadingPortrait = SetImageStreamString(HeroLoadingScreenPortraitsFolderName, reader["loading"]);

            // set hero description
            hero.Description = heroDescription;

            RealHeroNameByAttributeId.Add(hero.AttributeId, hero.Name);
            RealHeroNameByShortName.Add(hero.ShortName, hero.Name);

            // hero unit names, build 57797 is the first build where unit names were added in HMT
            if (SelectedBuild >= HeroUnitsAddedBuild)
                RealHeroNameByHeroUnitName.Add(hero.UnitName, hero.Name);
        }

        private void ParseHeroRoles(XmlReader reader, Hero hero)
        {
            reader.Read();
            if (reader.Name == "Roles")
            {
                reader.Read();
                string[] roles = reader.Value.Split(',');

                List<HeroRole> rolesList = new List<HeroRole>();

                foreach (var role in roles)
                {
                    rolesList.Add(Enum.TryParse(role, out HeroRole heroRole) ? heroRole : HeroRole.Unknown);
                }

                hero.Roles = rolesList;
            }

            reader.Read();
        }

        private void ParseHeroAbilities(XmlReader reader, Hero hero, Dictionary<string, string> talentShortTooltip, Dictionary<string, string> talentLongTooltip)
        {
            var ability = new Dictionary<string, Ability>();

            reader.Read();
            if (reader.Name == "Abilities")
            {
                // read each ability
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (Enum.TryParse(reader.Name, out AbilityTier tier))
                        {
                            while (reader.Read() && reader.Name != tier.ToString())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    string refAbilityName = reader.Name; // reference name of ability
                                    string realName = reader["name"] ?? string.Empty;  // real ingame name of ability
                                    string generic = reader["generic"] ?? "false";  // is the icon being used generic
                                    string desc = reader["desc"] ?? string.Empty; // reference name for ability tooltips
                                    int? mana = ConvertToNullableInt(reader["mana"]); // mana/brew/fury/etc... of the ability
                                    string perManaCost = reader["per-mana"] ?? "false"; // the time cost for mana
                                    int? cooldown = ConvertToNullableInt(reader["cooldown"]); // cooldown of the ability
                                    string charge = reader["ch-cooldown"] ?? "false"; // is the cooldown a charge cooldown

                                    if (!bool.TryParse(perManaCost, out bool isPerManaCost))
                                        isPerManaCost = false;

                                    if (!bool.TryParse(charge, out bool isCharge))
                                        isCharge = false;

                                    if (!bool.TryParse(generic, out bool isGeneric))
                                        isGeneric = false;

                                    if (reader.Read())
                                    {
                                        bool isGenericAbility = false;

                                        // create the tooltip
                                        if (!talentShortTooltip.TryGetValue(desc, out string shortDesc))
                                            shortDesc = string.Empty;

                                        if (!talentLongTooltip.TryGetValue(desc, out string longDesc))
                                            longDesc = string.Empty;

                                        if (ability.ContainsKey(refAbilityName))
                                            throw new ParseXmlException($"[{SelectedBuild}] [{hero.Name}] {refAbilityName} already exists");

                                        // create the talent
                                        ability.Add(refAbilityName, new Ability
                                        {
                                            Name = realName,
                                            ReferenceName = refAbilityName,
                                            IsIconGeneric = isGeneric,
                                            IsGeneric = isGenericAbility,
                                            TooltipDescriptionName = desc,
                                            Icon = SetHeroTalentString(hero.ShortName, reader.Value, isGeneric),
                                            Tier = tier,
                                            Tooltip = new TalentTooltip
                                            {
                                                Short = shortDesc,
                                                Full = longDesc,
                                                ManaType = hero.ManaType,
                                                Mana = mana,
                                                IsPerManaCost = isPerManaCost,
                                                Cooldown = cooldown,
                                                IsChargeCooldown = isCharge,
                                            },
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            reader.Read();
        }

        private void ParseHeroTalents(XmlReader reader, Hero hero, Dictionary<string, string> talentShortTooltip, Dictionary<string, string> talentLongTooltip)
        {
            var talents = new Dictionary<string, Talent>();

            reader.Read();
            if (reader.Name == "Talents")
            {
                // add talents, read each tier
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        // is tier Level1, Level4, etc...
                        if (Enum.TryParse(reader.Name, out TalentTier tier))
                        {
                            // read each talent in tier
                            while (reader.Read() && reader.Name != tier.ToString())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    string refTalentName = reader.Name; // reference name of talent
                                    string realName = reader["name"] ?? string.Empty;  // real ingame name of talent
                                    string generic = reader["generic"] ?? "false";  // is the icon being used generic
                                    string desc = reader["desc"] ?? string.Empty; // reference name for talent tooltips
                                    int? mana = ConvertToNullableInt(reader["mana"]); // mana/brew/fury/etc... of the talent
                                    string perManaCost = reader["per-mana"] ?? "false"; // the time cost for mana
                                    int? cooldown = ConvertToNullableInt(reader["cooldown"]); // cooldown of the talent
                                    string charge = reader["ch-cooldown"] ?? "false"; // is the cooldown a charge cooldown

                                    if (!bool.TryParse(perManaCost, out bool isPerManaCost))
                                        isPerManaCost = false;

                                    if (!bool.TryParse(charge, out bool isCharge))
                                        isCharge = false;

                                    if (!bool.TryParse(generic, out bool isGeneric))
                                        isGeneric = false;

                                    if (reader.Read())
                                    {
                                        bool isGenericTalent = false;

                                        // check if the talent is generic
                                        if (refTalentName.StartsWith("Generic") || refTalentName.StartsWith("HeroGeneric") || refTalentName.StartsWith("BattleMomentum"))
                                        {
                                            isGeneric = true;
                                            isGenericTalent = true;
                                        }

                                        // create the tooltip
                                        if (!talentShortTooltip.TryGetValue(desc, out string shortDesc))
                                            shortDesc = string.Empty;

                                        if (!talentLongTooltip.TryGetValue(desc, out string longDesc))
                                            longDesc = string.Empty;

                                        if (talents.ContainsKey(refTalentName))
                                            throw new ParseXmlException($"[{SelectedBuild}] [{hero.Name}] {refTalentName} already exists");

                                        // create the talent
                                        talents.Add(refTalentName, new Talent
                                        {
                                            Name = realName,
                                            ReferenceName = refTalentName,
                                            IsIconGeneric = isGeneric,
                                            IsGeneric = isGenericTalent,
                                            TooltipDescriptionName = desc,
                                            Icon = SetHeroTalentString(hero.ShortName, reader.Value, isGeneric),
                                            Tier = tier,
                                            Tooltip = new TalentTooltip
                                            {
                                                Short = shortDesc,
                                                Full = longDesc,
                                                ManaType = hero.ManaType,
                                                Mana = mana,
                                                IsPerManaCost = isPerManaCost,
                                                Cooldown = cooldown,
                                                IsChargeCooldown = isCharge,
                                            },
                                        });
                                    }
                                }
                            }
                        }
                    }
                } // end while
            }

            if (HeroExists(hero.Name))
                throw new ArgumentException($"Hero already added: {hero.Name}");

            talents.Add(string.Empty, new Talent
            {
                Name = "No pick",
                IsIconGeneric = true,
                IsGeneric = true,
                Icon = SetHeroTalentString(string.Empty, NoTalentIconPick, true),
            });

            talents.Add("NotFound", new Talent
            {
                Name = "Not found",
                Icon = SetHeroTalentString(string.Empty, NoTalentIconFound, true),
            });

            hero.Talents = talents;
            HeroByHeroName.Add(hero.Name, hero);
        }
    }
}