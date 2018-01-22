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
Filename: HATAsset.cs
Description:
*/

#endregion Header

namespace HAT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using AssetPackage;
    using Swiss;

    /// <summary>
    /// A hat asset.
    /// </summary>
    public class HATAsset : BaseAsset
    {
        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private HATAssetSettings settings = null;

        /// <summary>
        /// The adapter.
        /// </summary>
        private DifficultyAdapter adapter;

        /// <summary>
        /// Values that represent hat modes.
        /// </summary>
        public enum HATMode
        {
            perTurn,
            perGame,
        }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        ///
        /// <remarks>   Besides the toXml() and fromXml() methods, we never use this
        ///             property but use
        ///                it's correctly typed backing field 'settings' instead. </remarks>
        /// <remarks>   This property should go into each asset having Settings of
        ///             its own. </remarks>
        /// <remarks>   The actual class used should be derived from BaseAsset (and
        ///             not directly from ISetting). </remarks>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public override ISettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = (value as HATAssetSettings);
            }
        }

        /// <summary>
        /// Initializes a new instance of the HAT.HATAsset class.
        /// </summary>
        public HATAsset()
            : this(null)
        {
            //
        }

        /// <summary>
        /// Initializes a new instance of the HAT.HATAsset class.
        /// </summary>
        ///
        /// <param name="bridge"> The bridge. </param>
        public HATAsset(IBridge bridge)
            : base(bridge)
        {
            InitSettings();

            //! 6) Create the AI adapter.
            // 
            this.adapter = new DifficultyAdapter(this);
        }

        private void InitSettings()
        {
            //! 1) Create empty settings
            // 
            settings = new HATAssetSettings();

            //! 2) Try to load run-time settings.
            // 
            if (!LoadSettings("HATAssetAppSettings.xml"))
            {
                LogToConsole(ConsoleColor.Green, "HATAsset: Loaded Default Settings");

                //! 3) If no run-time settings try to load default settings instead.
                // 
                // Loads from <my documents>\visual studio 2013\Projects\MonoGame\MonoGame1\bin\WindowsGL\Debug\Resources\HATAssetAppSettings.
                // So project that uses the HATAsset instead of the HAT Assembly.
                // We need to add defaults in-this assembly in case the default is missing.
                LoadDefaultSettings();

                if (hasSettings && settings.Scenarios.Count == 0 || settings.Players.Count == 0)
                {
                    LogToConsole(ConsoleColor.Green, "HATAsset: Generating Default Settings");

                    settings.Mode = HATMode.perGame;
                }

                //! 4) Check if settings contain scenarios. If not add them and save them as default.
                // 
                if (hasSettings && settings.Scenarios.Count == 0)
                {
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Very Easy AI",
                                Rating = 0.7,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-01-01T01:01:01"),
                                TimeLimit = 900000
                            });
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Easy AI",
                                Rating = 1.0,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-01-01T01:01:01"),
                                TimeLimit = 900000
                            });
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Medium Color AI",
                                Rating = 1.3,
                                PlayCount = 0,
                                KFactor = 0.00075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-01-01T01:01:01"),
                                TimeLimit = 900000
                            });
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Medium Shape AI",
                                Rating = 1.3,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-01-01T01:01:01"),
                                TimeLimit = 900000
                            });
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Hard AI",
                                Rating = 1.6,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-01-01T01:01:01"),
                                TimeLimit = 900000
                            });
                    settings.Scenarios.Add(
                            new HATScenario()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                ScenarioID = "Very Hard AI",
                                Rating = 1.9,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-07-29T12:02:23"),
                                TimeLimit = 900000
                            });

                    SaveDefaultSettings(true);

                    LogToConsole(ConsoleColor.Green, String.Format("HATAsset: Added {0} Secnario(s)", settings.Scenarios.Count));
                }

                //! 5) Check if settings contain players. If not add them and save them as default.
                // 
                if (hasSettings && settings.Players.Count == 0)
                {
                    settings.Players.Add(
                            new HATPlayer()
                            {
                                AdaptationID = "Game difficulty - Player skill",
                                GameID = "DropEm",
                                PlayerID = "Noob",
                                Rating = 0.75,
                                PlayCount = 0,
                                KFactor = 0.0075,
                                Uncertainty = 1.0,
                                LastPlayed = DateTime.Parse("2015-07-29T12:02:23")
                            });

                    SaveDefaultSettings(true);

                    LogToConsole(ConsoleColor.Green, String.Format("HATAsset: Added {0} Player(s)", settings.Players.Count));
                }
            }
            else
            {
                LogToConsole(ConsoleColor.Green, "HATAsset: Loaded (Run-Time) Settings");
            }
        }

        /// <summary>
        /// Get a Property of the Player setting.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="adaptID">  Identifier for the adapt. </param>
        /// <param name="gameID">   Identifier for the game. </param>
        /// <param name="playerID"> Identifier for the player. </param>
        /// <param name="item">     The item. </param>
        ///
        /// <returns>
        /// A T.
        /// </returns>
        internal T PlayerSetting<T>(string adaptID, string gameID, string playerID, string item)
        {
            if (settings.Players.Exists(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.PlayerID.Equals(playerID)))
            {
                HATPlayer player = settings.Players.Find(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.PlayerID.Equals(playerID));

                PropertyInfo pi = typeof(HATPlayer).GetProperty(item);
                if (pi != null && pi.CanRead)
                {
                    return (T)pi.GetValue(player, new Object[] { });
                }
            }

            throw new ArgumentException(String.Format("Unable to get {0} for player {1} for adaptation {2} in game {3}.", item, playerID, adaptID, gameID));
        }

        /// <summary>
        /// Set a Property of the Player setting.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="adaptID">  Identifier for the adapt. </param>
        /// <param name="gameID">   Identifier for the game. </param>
        /// <param name="playerID"> Identifier for the player. </param>
        /// <param name="item">     The item. </param>
        /// <param name="value">    The value. </param>
        internal void PlayerSetting<T>(string adaptID, string gameID, string playerID, string item, T value)
        {
            if (settings.Players.Exists(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.PlayerID.Equals(playerID)))
            {
                HATPlayer player = settings.Players.Find(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.PlayerID.Equals(playerID));

                PropertyInfo pi = typeof(HATPlayer).GetProperty(item);
                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(player, value, new Object[] { });
                }
            }
        }

        /// <summary>
        /// Get a Property of the Scenario setting.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="adaptID">    Identifier for the adapt. </param>
        /// <param name="gameID">     Identifier for the game. </param>
        /// <param name="scenarioID"> Identifier for the scenario. </param>
        /// <param name="item">       The item. </param>
        ///
        /// <returns>
        /// A T.
        /// </returns>
        internal T ScenarioSetting<T>(string adaptID, string gameID, string scenarioID, string item)
        {
            if (settings.Scenarios.Exists(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.ScenarioID.Equals(scenarioID)))
            {
                HATScenario scenario = settings.Scenarios.Find(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.ScenarioID.Equals(scenarioID));

                PropertyInfo pi = typeof(HATScenario).GetProperty(item);
                if (pi != null && pi.CanRead)
                {
                    return (T)pi.GetValue(scenario, new Object[] { });
                }
            }

            throw new ArgumentException(String.Format("Unable to get {0} for scenario {1} for adaptation {2} in game {3}.", item, scenarioID, adaptID, gameID));
        }

        /// <summary>
        /// Set a Property of the Scenario setting.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="adaptID">    Identifier for the adapt. </param>
        /// <param name="gameID">     Identifier for the game. </param>
        /// <param name="scenarioID"> Identifier for the scenario. </param>
        /// <param name="item">       The item. </param>
        internal void ScenarioSetting<T>(string adaptID, string gameID, string scenarioID, string item, T value)
        {
            if (settings.Scenarios.Exists(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.ScenarioID.Equals(scenarioID)))
            {
                HATScenario scenario = settings.Scenarios.Find(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID) && p.ScenarioID.Equals(scenarioID));

                PropertyInfo pi = typeof(HATScenario).GetProperty(item);
                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(scenario, value, new Object[] { });
                }
            }
        }

        /// <summary>
        /// Gets a list containing all scenarios IDs.
        /// </summary>
        ///
        /// <param name="adaptID"> Identifier for the adapt. </param>
        /// <param name="gameID">  Identifier for the game. </param>
        ///
        /// <returns>
        /// all scenarios.
        /// </returns>
        internal List<string> AllScenariosIDs(string adaptID, string gameID)
        {
            return settings.Scenarios
                .FindAll(p => p.AdaptationID.Equals(adaptID) && p.GameID.Equals(gameID))
                .OrderBy(p => p.Rating)
                .Select(p => p.ScenarioID)
                .ToList<string>();
        }

        /// <summary>
        /// Get the Target scenario ID from the adapter.
        /// </summary>
        ///
        /// <param name="adaptID">  Identifier for the adapt. </param>
        /// <param name="gameID">   Identifier for the game. </param>
        /// <param name="playerID"> Identifier for the player. </param>
        ///
        /// <returns>
        /// A string.
        /// </returns>
        public string TargetScenarioID(string adaptID, string gameID, string playerID)
        {
            String scenarioID = adapter.CalculateTargetScenarioID(adaptID, gameID, playerID);

            LogToConsole(ConsoleColor.Yellow, String.Format(
@"HATAsset.TargetScenarioID('{0}',
                          '{1}',
                          '{2}') -> {3}", adaptID, gameID, playerID, scenarioID));

            return scenarioID;
        }

        /// <summary>
        /// Updates the ratings of the adapter.
        /// </summary>
        ///
        /// <param name="adaptID">       Identifier for the adapt. </param>
        /// <param name="gameID">        Identifier for the game. </param>
        /// <param name="playerID">      Identifier for the player. </param>
        /// <param name="scenarioID">    Identifier for the scenario. </param>
        /// <param name="rt">            The right. </param>
        /// <param name="correctAnswer"> The correct answer. </param>
        public void UpdateRatings(string adaptID, string gameID, string playerID, string scenarioID, double rt, double correctAnswer)
        {
            LogToConsole(ConsoleColor.Yellow, String.Format(
@"HATAsset.UpdateRatings('{0}',
                       '{1}',
                       '{2}',
                       '{3}',
                       {4:0.0},
                       {5:0.0})", adaptID, gameID, playerID, scenarioID, rt, correctAnswer));

            adapter.UpdateRatings(adaptID, gameID, playerID, scenarioID, rt, correctAnswer);
        }

        /// <summary>
        /// Current accuracy.
        /// </summary>
        ///
        /// <param name="adaptID">  Identifier for the adapt. </param>
        /// <param name="gameID">   Identifier for the game. </param>
        /// <param name="playerID"> Identifier for the player. </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>
        public double CurrentAccuracy(string adaptID, string gameID, string playerID)
        {
            double accuracy = 0;

            IDataStorage ds = getInterface<IDataStorage>();

            if (ds != null)
            {
                string logfile = "gameplaylogs.xml";

                XDocument doc;

                if (!ds.Exists(logfile))
                {
                    doc = XDocument.Parse(@"<GameplaysData />");
                }
                else
                {
                    doc = XDocument.Parse(ds.Load(logfile));
                }

                String xGame = String.Format(@"//GameplaysData/Adaptation[@AdaptationID='{0}']/Game[@GameID='{1}']/Gameplay[@PlayerID='{2}']/Accuracy", adaptID, gameID, playerID);

                if (doc.XPathSelectElements(xGame).Count() != 0)
                {
                    //! The DifficultyAdapter tries to get the accuracy at 0.75, ie TARGET_DISTR_MEAN (so we call it 100%).
                    // 
                    accuracy = ((double)doc.XPathSelectElements(xGame).Average(p => Double.Parse(p.Value)) / adapter.TargetDistributedMean);
                }
            }

            LogToConsole(ConsoleColor.Yellow, String.Format(
@"HATAsset.CurrentAccuracy('{0}',
                         '{1}',
                         '{2}') -> {3:0.000}", adaptID, gameID, playerID, accuracy));

            return accuracy;
        }

        /// <summary>
        /// Creates new record to the game log.
        /// </summary>
        ///
        /// <param name="adaptID">        Identifier for the adapt. </param>
        /// <param name="gameID">         Identifier for the game. </param>
        /// <param name="playerID">       Identifier for the player. </param>
        /// <param name="scenarioID">     Identifier for the scenario. </param>
        /// <param name="rt">             The right. </param>
        /// <param name="accuracy">       The correct answer. </param>
        /// <param name="userRating">     The player new rating. </param>
        /// <param name="scenarioRating"> The scenario new rating. </param>
        /// <param name="timestamp">      The current date time. </param>
        internal void CreateNewRecord(string adaptID, string gameID, string playerID, string scenarioID,
            double rt, double accuracy, double userRating, double scenarioRating, DateTime timestamp)
        {
            IDataStorage ds = getInterface<IDataStorage>();

            if (ds != null)
            {
                string logfile = "gameplaylogs.xml";

                XDocument doc;

                if (!ds.Exists(logfile))
                {
                    doc = XDocument.Parse(@"<GameplaysData />");
                }
                else
                {
                    doc = XDocument.Parse(ds.Load(logfile));
                }

                String xAdaptation = String.Format(@"//GameplaysData/Adaptation[@AdaptationID='{0}']", adaptID);
                if (doc.XPathSelectElement(xAdaptation) == null)
                {
                    doc.Root.Add(new XElement("Adaptation",
                        new XAttribute("AdaptationID", adaptID)));
                }

                String xGame = String.Format(@"//GameplaysData/Adaptation[@AdaptationID='{0}']/Game[@GameID='{1}']", adaptID, gameID);
                if (doc.XPathSelectElement(xGame) == null)
                {
                    doc.XPathSelectElement(xAdaptation).Add(new XElement("Game",
                        new XAttribute("GameID", gameID)));
                }
                //<Gameplay PlayerID="Noob" ScenarioID="Easy AI" Timestamp="2015/07/29 09:01:20">
                //        <RT>115657</RT>
                //        <Accuracy>1</Accuracy>
                //        <PlayerRating>0.0399778033856024</PlayerRating>
                //        <ScenarioRating>0.960022196614398</ScenarioRating>
                //      </Gameplay>
                doc.XPathSelectElement(xGame).Add(
                    new XElement("Gameplay",
                        new XAttribute("PlayerID", playerID),
                        new XAttribute("ScenarioID", scenarioID),
                        new XAttribute("Timestamp", timestamp),
                        new XElement("RT", rt),
                        new XElement("Accuracy", accuracy),
                        new XElement("PlayerRating", userRating),
                        new XElement("ScenarioRating", scenarioRating)));

                ds.Save(logfile, doc.ToString());
            }
        }

        /// <summary>
        /// Logs to ConsoleDialog.
        /// </summary>
        ///
        /// <param name="color"> The color. </param>
        /// <param name="msg">   The message. </param>
        private static void LogToConsole(ConsoleColor color, String msg)
        {
            ConsoleDialog.WriteLine(String.Empty);
            ConsoleDialog.PushForeColor();
            ConsoleDialog.ForegroundColor = color;
            ConsoleDialog.WriteLine(msg);
            ConsoleDialog.PopForeColor();
            ConsoleDialog.WriteLine(String.Empty);
        }

        public string TurnScenarioID { get; set; }
    }
}
