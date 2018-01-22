#region Header

/*
Copyright 2015 Enkhbold Nyamsuren (http://www.bcogs.net , http://www.bcogs.info/)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Namespace: HAT
Filename: HATAssetSettings.cs
Description:
*/

#endregion Header

namespace HAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using AssetManagerPackage;

    using AssetPackage;

    /// <summary>
    /// A hat asset settings.
    /// </summary>
    public class HATAssetSettings : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the asset_proof_of_concept_demo_CSharp.AssetSettings class.
        /// </summary>
        public HATAssetSettings()
            : base()
        {
            this.Scenarios = new List<HATScenario>();
            this.Players = new List<HATPlayer>();
            this.Mode = HATAsset.HATMode.perGame;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        ///
        /// <value>
        /// The mode.
        /// </value>
        [Category("Operational")]
        [Description("The mode can be set to adaption per turn or per game.")]
        [DefaultValue(HATAsset.HATMode.perGame)]
        public HATAsset.HATMode Mode
        {
            get;
            set;
        }

        //[DefaultValue(new HATScenario[] { })]
        /// <summary>
        /// Gets or sets the string[].
        /// </summary>
        ///
        /// <value>
        /// The scenarios.
        /// </value>
        [XmlArray]
        [XmlArrayItem("Player")]
        public List<HATPlayer> Players
        {
            get;
            set;
        }

        //[DefaultValue(new HATScenario[] { })]
        /// <summary>
        /// Gets or sets the Scenarios[].
        /// </summary>
        ///
        /// <value>
        /// The scenarios.
        /// </value>
        [XmlArray]
        [XmlArrayItem("Scenario")]
        public List<HATScenario> Scenarios
        {
            get;
            set;
        }

        #endregion Properties
    }

    /// <summary>
    /// A hat player.
    /// </summary>
    public class HATPlayer
    {
        /// <summary>
        /// Initializes a new instance of the HAT.HATPlayer class.
        /// </summary>
        public HATPlayer()
        {
            BaseSettings.UpdateDefaultValues(this);
        }

        #region Properties

        /// <summary>
        /// Gets the identifier of the adaptation.
        /// </summary>
        ///
        /// <value>
        /// The identifier of the adaptation.
        /// </value>
        [XmlAttribute("AdaptationID")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string AdaptationID
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets the identifier of the game.
        ///// </summary>
        /////
        ///// <value>
        ///// The identifier of the game.
        ///// </value>
        [XmlAttribute("GameID")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string GameID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the factor.
        /// </summary>
        ///
        /// <value>
        /// The k factor.
        /// </value>
        [DefaultValue(0.0075)]
        public double KFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Date/Time of the last played.
        /// </summary>
        ///
        /// <value>
        /// The last played.
        /// </value>
        [Description("The last time the user played a scenario.")]
        [DefaultValue(typeof(DateTime), "2015-01-01T01:01:01")]
        public DateTime LastPlayed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of plays.
        /// </summary>
        ///
        /// <value>
        /// The number of plays.
        /// </value>
        [DefaultValue(0)]
        public int PlayCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier of the player.
        /// </summary>
        ///
        /// <value>
        /// The identifier of the player.
        /// </value>
        [XmlAttribute("PlayerID")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string PlayerID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the rating.
        /// </summary>
        ///
        /// <value>
        /// The rating.
        /// </value>
        [DefaultValue(0.75)]
        public double Rating
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the uncertainty.
        /// </summary>
        ///
        /// <value>
        /// The uncertainty.
        /// </value>
        [DefaultValue(1.0)]
        public double Uncertainty
        {
            get;
            set;
        }

        #endregion Properties
    }

    /// <summary>
    /// A hat scenario.
    /// </summary>
    public class HATScenario
    {
        /// <summary>
        /// Initializes a new instance of the HAT.HATScenario class.
        /// </summary>
        public HATScenario()
        {
            BaseSettings.UpdateDefaultValues(this);
        }

        #region Properties

        /// <summary>
        /// Gets the identifier of the adaptation.
        /// </summary>
        ///
        /// <value>
        /// The identifier of the adaptation.
        /// </value>
        [XmlAttribute("AdaptationID")]
        [Description("The ID of the Adaptation.")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string AdaptationID
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets the identifier of the game.
        ///// </summary>
        /////
        ///// <value>
        ///// The identifier of the game.
        ///// </value>
        [XmlAttribute("GameID")]
        [Description("The ID of the Game to be Adapted.")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string GameID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the factor.
        /// </summary>
        ///
        /// <value>
        /// The k factor.
        /// </value>
        [DefaultValue(0.0075)]
        public double KFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Date/Time of the last played.
        /// </summary>
        ///
        /// <value>
        /// The last played.
        /// </value>
        [Description("The last time the user played the scenario.")]
        [DefaultValue(typeof(DateTime), "2015-01-01T01:01:01")]
        public DateTime LastPlayed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of plays.
        /// </summary>
        ///
        /// <value>
        /// The number of plays.
        /// </value>
        [Description("The number of times the user played the scenario.")]
        [DefaultValue(0)]
        public int PlayCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the rating.
        /// </summary>
        ///
        /// <value>
        /// The rating.
        /// </value>
        public double Rating
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the identifier of the scenario.
        /// </summary>
        ///
        /// <value>
        /// The identifier of the scenario.
        /// </value>
        [XmlAttribute("ScenarioID")]
        [Description("The ID of the Adaptation Scenario.")]
        [DefaultValue("")]
        [Category("Identifiers")]
        public string ScenarioID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time limit.
        /// </summary>
        ///
        /// <value>
        /// The time limit.
        /// </value>
        [Description("The time limit for the scenario.")]
        [DefaultValue(900000)]
        public int TimeLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the uncertainty.
        /// </summary>
        ///
        /// <value>
        /// The uncertainty.
        /// </value>
        [DefaultValue(1.0)]
        public double Uncertainty
        {
            get;
            set;
        }

        #endregion Properties

        #region Other

        ///// <summary>
        ///// Initializes a new instance of the HAT.HATScenario struct.
        ///// </summary>
        /////
        ///// <param name="scenarioID">  Identifier for the scenario. </param>
        ///// <param name="rating">      The rating. </param>
        ///// <param name="playCount">   Number of plays. </param>
        ///// <param name="kFactor">     The factor. </param>
        ///// <param name="uncertainty"> The uncertainty. </param>
        ///// <param name="lastPlayed">  The last played Date/Time. </param>
        ///// <param name="timeLimit">   The time limit. </param>
        //public HATScenario(
        //    string apdaptationID,
        //    string gameID,
        //    string scenarioID,
        //    double rating,
        //    int playCount,
        //    double kFactor,
        //    double uncertainty,
        //    DateTime lastPlayed,
        //    int timeLimit)
        //    : this()
        //{
        //    AdaptationID = apdaptationID;
        //    GameID = gameID;
        //    ScenarioID = scenarioID;
        //    Rating = rating;
        //    PlayCount = playCount;
        //    KFactor = kFactor;
        //    Uncertainty = uncertainty;
        //    LastPlayed = lastPlayed;
        //    TimeLimit = timeLimit;
        //}

        #endregion Other
    }
}