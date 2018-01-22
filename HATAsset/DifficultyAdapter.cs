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
Filename: DifficultyAdapter.cs
Description:
    The asset implements ELO-based difficulty to skill adaptation algorithm described in "Klinkenberg, S., Straatemeier, M., & Van der Maas, H. L. J. (2011).
    Computer adaptive practice of maths ability using a new item response model for on the fly ability and difficulty estimation.
    Computers & Education, 57 (2), 1813-1824.".
*/

#endregion Header

namespace HAT
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Swiss;

    internal class DifficultyAdapter
    {
        #region Fields

        private const string DEF_DATE = "2015-01-01T01:01:01";
        private const double DEF_K = 0.0075; // [SC] The default value for the K constant
        private const double DEF_K_DOWN = 0.5; // [SC] The default value for the downward uncertainty weight
        private const double DEF_K_UP = 4.0; // [SC] the default value for the upward uncertainty weight
        private const double DEF_MAX_DELAY = 30; // [SC] The default value for the max number of days after which player's or item's undertainty reaches the maximum

        //// [TODO][SC perhaps all getter/setter functions should just assign default values instead of throwing exceptions
        private const double DEF_MAX_PLAY = 40; // [SC] The default value for the max number of administrations that should result in minimum uncertaint in item's or player's ratings
        private const double DEF_THETA = 0;
        private const double DEF_U = 1.0; // [SC] The default value for the provisional uncertainty to be assigned to an item or player
        private const double TARGET_DISTR_MEAN = 0.75;
        private const double TARGET_DISTR_SD = 0.1;
        private const double TARGET_LOWER_LIMIT = 0.5;
        private const double TARGET_UPPER_LIMIT = 1;
        private const string TIMESTAMP_FORMAT = "s"; // Sortable DateTime as used in XML serializing : 's' -> 'yyyy-mm-ddThh:mm:ss'

        private HATAsset asset;
        private double kConst;
        private double kDown;
        private double kUp;
        private double maxDelay;

        //// [TODO][SC] need to setup getter and setters as well as proper value validity checks
        private double maxPlay;
        private string provDate;
        private double provU;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the HAT.DifficultyAdapter class.
        /// 
        /// Assign default values if max play frequency and max non-play delay values
        /// are not provided.
        /// 
        /// Add a reference to the HATAsset so we can use it.
        /// </summary>
        ///
        /// <remarks>
        /// Alternative for the asset parameter would be to ask the AssetManager for
        /// a reference.
        /// </remarks>
        ///
        /// <param name="asset"> The asset. </param>
        internal DifficultyAdapter(HATAsset asset)
        {
            maxPlay = DEF_MAX_PLAY;
            maxDelay = DEF_MAX_DELAY;

            kConst = DEF_K;
            kUp = DEF_K_UP;
            kDown = DEF_K_DOWN;

            provU = DEF_U;

            provDate = DEF_DATE;

            this.asset = asset;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Getter/setter for the K constant.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The k constant.
        /// </value>
        private double KConst
        {
            get { return kConst; }
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("K constant cannot be a negative number.");
                }
                else
                {
                    kConst = value;
                }
            }
        }

        /// <summary>
        /// Getter/setter for the downward uncertainty weight.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The downward uncertainty weight.
        /// </value>
        private double KDown
        {
            get
            {
                return kDown;
            }
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("The downward uncertainty weight cannot be a negative number.");
                }
                else
                {
                    kDown = value;
                }
            }
        }

        /// <summary>
        /// Getter/setter for the upward uncertainty weight.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The upward uncertainty weight.
        /// </value>
        private double KUp
        {
            get
            {
                return kUp;
            }
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentException("The upward uncertianty weight cannot be a negative number.");
                }
                else
                {
                    kUp = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum delay.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The maximum delay.
        /// </value>
        private double MaxDelay
        {
            get
            {
                return maxDelay;
            }
            set
            {
                if (value <= 0)
                {
                    throw new System.ArgumentException("The maximum number of delay days should be higher than 0.");
                }
                else
                {
                    maxDelay = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum play.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The maximum play.
        /// </value>
        private double MaxPlay
        {
            get
            {
                return maxPlay;
            }
            set
            {
                if (value <= 0)
                {
                    throw new System.ArgumentException("The maximum administration parameter should be higher than 0.");
                }
                else
                {
                    maxPlay = value;
                }
            }
        }

        /// <summary>
        /// Gets the provisional play date.
        /// </summary>
        ///
        /// <value>
        /// The provisional play date.
        /// </value>
        private string ProvDate
        {
            get
            {
                return provDate;
            }
        }

        /// <summary>
        /// Gets the prov theta.
        /// </summary>
        ///
        /// <value>
        /// The prov theta.
        /// </value>
        private double ProvTheta
        {
            get
            {
                return DEF_THETA;
            }
        }

        /// <summary>
        /// Gets target distributed mean.
        /// </summary>
        ///
        /// <value>
        /// The target distributed mean.
        /// </value>
        internal double TargetDistributedMean
        {
            get
            {
                return TARGET_DISTR_MEAN;
            }
        }

        /// <summary>
        /// Gets or sets the provisional uncertainty.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <value>
        /// The provisional uncertainty.
        /// </value>
        private double ProvU
        {
            get
            {
                return provU;
            }
            set
            {
                if (0 < value || value > 1)
                {
                    throw new System.ArgumentException("Provisional uncertainty value should be between 0 and 1");
                }
                else
                {
                    provU = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Target scenario identifier.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <param name="adaptID">  Identifier for the adapt. </param>
        /// <param name="gameID">   Identifier for the game. </param>
        /// <param name="playerID"> Identifier for the player. </param>
        ///
        /// <returns>
        /// A string.
        /// </returns>
        internal string CalculateTargetScenarioID(string adaptID, string gameID, string playerID)
        {
            // [SC] get player rating.
            double playerRating = asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.Rating));

            // [SC] get IDs of available scenarios
            List<string> scenarioIDList = asset.AllScenariosIDs(adaptID, gameID);
            if (scenarioIDList.Count == 0)
            {
                throw new System.ArgumentException("No scenarios found for adaptation " + adaptID + " in game " + gameID);
            }

            double targetScenarioRating = TargetBeta(playerRating);
            double minDistance = 0;
            string minDistanceScenarioID = null;
            double minPlayCount = 0;

            foreach (string scenarioID in scenarioIDList)
            {
                if (String.IsNullOrEmpty(scenarioID))
                {
                    throw new System.ArgumentException("Null scenario ID found for adaptation " + adaptID + " in game " + gameID);
                }

                int scenarioPlayCount = asset.ScenarioSetting<int>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.PlayCount));
                double scenarioRating = asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.Rating));

                double distance = Math.Abs(scenarioRating - targetScenarioRating);
                if (minDistanceScenarioID == null || distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceScenarioID = scenarioID;
                    minPlayCount = scenarioPlayCount;
                }
                else if (distance == minDistance && scenarioPlayCount < minPlayCount)
                {
                    minDistance = distance;
                    minDistanceScenarioID = scenarioID;
                    minPlayCount = scenarioPlayCount;
                }
            }

            return minDistanceScenarioID;
        }

        /// <summary>
        /// Updates the ratings.
        /// </summary>
        ///
        /// <param name="adaptID">       Identifier for the adapt. </param>
        /// <param name="gameID">        Identifier for the game. </param>
        /// <param name="playerID">      Identifier for the player. </param>
        /// <param name="scenarioID">    Identifier for the scenario. </param>
        /// <param name="rt">            The right. </param>
        /// <param name="correctAnswer"> The correct answer. </param>
        internal void UpdateRatings(string adaptID, string gameID, string playerID, string scenarioID, double rt, double correctAnswer)
        {
            // [SC] getting player data
            double playerRating;
            int playerPlayCount;
            double playerUncertainty;
            DateTime playerLastPlayed;

            try
            {
                playerRating = asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.Rating));
                playerPlayCount = asset.PlayerSetting<int>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.PlayCount));
                playerUncertainty = asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.Uncertainty));
                playerLastPlayed = asset.PlayerSetting<DateTime>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.LastPlayed));
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("Cannot calculate new ratings. Player data is missing.");
                return;
            }

            // [SC] getting scenario data
            double scenarioRating;
            int scenarioPlayCount;
            double scenarioUncertainty;
            DateTime scenarioLastPlayed;
            int scenarioTimeLimit;

            try
            {
                scenarioRating = asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.Rating));
                scenarioPlayCount = asset.ScenarioSetting<int>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.PlayCount));
                scenarioUncertainty = asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.Uncertainty));
                scenarioLastPlayed = asset.ScenarioSetting<DateTime>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.LastPlayed));
                scenarioTimeLimit = asset.ScenarioSetting<int>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.TimeLimit));
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("Cannot calculate new ratings. Scenario data is missing.");
                return;
            }

            DateTime currDateTime = DateTime.UtcNow;

            // [SC] parsing player data
            double playerLastPlayedDays = (currDateTime - playerLastPlayed).Days;
            if (playerLastPlayedDays > DEF_MAX_DELAY)
            {
                playerLastPlayedDays = maxDelay;
            }

            // [SC] parsing scenario data
            double scenarioLastPlayedDays = (currDateTime - scenarioLastPlayed).Days;
            if (scenarioLastPlayedDays > DEF_MAX_DELAY)
            {
                scenarioLastPlayedDays = maxDelay;
            }

            // [SC] calculating actual and expected scores
            double actualScore = calcActualScore(correctAnswer, rt, scenarioTimeLimit);
            double expectScore = calcExpectedScore(playerRating, scenarioRating, scenarioTimeLimit);

            // [SC] calculating player and scenario uncertainties
            double playerNewUncertainty = calcThetaUncertainty(playerUncertainty, playerLastPlayedDays);
            double scenarioNewUncertainty = calcBetaUncertainty(scenarioUncertainty, scenarioLastPlayedDays);

            // [SC] calculating player and sceario K factors
            double playerNewKFct = calcThetaKFctr(playerNewUncertainty, scenarioNewUncertainty);
            double scenarioNewKFct = calcBetaKFctr(playerNewUncertainty, scenarioNewUncertainty);

            // [SC] calculating player and scenario ratings
            double playerNewRating = calcTheta(playerRating, playerNewKFct, actualScore, expectScore);
            double scenarioNewRating = calcBeta(scenarioRating, scenarioNewKFct, actualScore, expectScore);

            // [SC] updating player and scenario play counts
            int playerNewPlayCount = playerPlayCount + 1;
            int scenarioNewPlayCount = scenarioPlayCount + 1;

            // [SC] storing updated player data
            asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.Rating), playerNewRating);
            asset.PlayerSetting<int>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.PlayCount), playerNewPlayCount);
            asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.KFactor), playerNewKFct);
            asset.PlayerSetting<double>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.Uncertainty), playerNewUncertainty);
            asset.PlayerSetting<DateTime>(adaptID, gameID, playerID, ObjectUtils.GetMemberName<HATPlayer>(p => p.LastPlayed), currDateTime);

            // [SC] storing updated scenario data
            asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.Rating), scenarioNewRating);
            asset.ScenarioSetting<int>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.PlayCount), scenarioNewPlayCount);
            asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.KFactor), scenarioNewKFct);
            asset.ScenarioSetting<double>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.Uncertainty), scenarioNewUncertainty);
            asset.ScenarioSetting<DateTime>(adaptID, gameID, scenarioID, ObjectUtils.GetMemberName<HATScenario>(p => p.LastPlayed), currDateTime);

            asset.SaveSettings("HATAssetAppSettings.xml");

            // [SC] creating game log
            asset.CreateNewRecord(adaptID, gameID, playerID, scenarioID, rt, correctAnswer, playerNewRating, scenarioNewRating, currDateTime);
        }

        /// <summary>
        /// Calculates actual score given success/failure outcome and response time.
        /// </summary>
        ///
        /// <param name="correctAnswer">   The correct answer. </param>
        /// <param name="responseTime">    a response time in milliseconds. </param>
        /// <param name="itemMaxDuration">  maximum duration of time given to a
        ///                                 player to provide an answer. </param>
        ///
        /// <returns>
        /// actual score as a double.
        /// </returns>
        ///
        /// ### <param name="correctAnswerFlag">    should be either 0, for failure,
        ///                                         or 1 for success. </param>
        private double calcActualScore(double correctAnswer, double responseTime, double itemMaxDuration)
        {
            validateResponseTime(responseTime);
            validateItemMaxDuration(itemMaxDuration);

            double discrParam = getDiscriminationParam(itemMaxDuration);
            return (double)(((2 * correctAnswer) - 1) * ((discrParam * itemMaxDuration) - (discrParam * responseTime)));
        }

        /// <summary>
        /// Calculates a new beta rating.
        /// </summary>
        ///
        /// <param name="currBeta">    current beta rating. </param>
        /// <param name="betaKFctr">   K factor for the beta rating. </param>
        /// <param name="actualScore"> actual performance score. </param>
        /// <param name="expectScore"> expected performance score. </param>
        ///
        /// <returns>
        /// a double value for new beta rating.
        /// </returns>
        private double calcBeta(double currBeta, double betaKFctr, double actualScore, double expectScore)
        {
            return currBeta + (betaKFctr * (expectScore - actualScore));
        }

        /// <summary>
        /// Calculates a new K factor for the beta rating
        /// </summary>
        /// <param name="currThetaU">current uncertainty fot the theta rating</param>
        /// <param name="currBetaU">current uncertainty for the beta rating</param>
        /// <returns>a double value of a new K factor for the beta rating</returns>
        private double calcBetaKFctr(double currThetaU, double currBetaU)
        {
            return kConst * (1 + (kUp * currBetaU) - (kDown * currThetaU));
        }

        /// <summary>
        /// Calculates a new uncertainty for the beta rating.
        /// </summary>
        ///
        /// <param name="currBetaU">        current uncertainty value for the beta
        ///                                 rating. </param>
        /// <param name="currDelayCount">   the current number of consecutive days
        ///                                 the item has not beein played. </param>
        ///
        /// <returns>
        /// a new uncertainty value for the beta rating.
        /// </returns>
        private double calcBetaUncertainty(double currBetaU, double currDelayCount)
        {
            double newBetaU = currBetaU - (1.0 / maxPlay) + (currDelayCount / maxDelay);
            if (newBetaU < 0) newBetaU = 0.0;
            else if (newBetaU > 1) newBetaU = 1.0;
            return newBetaU;
        }

        /// <summary>
        /// Calculates expected score given player's skill rating and item's
        /// difficulty rating.
        /// </summary>
        ///
        /// <param name="playerTheta">     player's skill rating. </param>
        /// <param name="itemBeta">        item's difficulty rating. </param>
        /// <param name="itemMaxDuration">  maximum duration of time given to a
        ///                                 player to provide an answer. </param>
        ///
        /// <returns>
        /// expected score as a double.
        /// </returns>
        private double calcExpectedScore(double playerTheta, double itemBeta, double itemMaxDuration)
        {
            validateItemMaxDuration(itemMaxDuration);

            double weight = getDiscriminationParam(itemMaxDuration) * itemMaxDuration;
            double expFctr = (double)Math.Exp(2.0 * weight * (playerTheta - itemBeta));

            return (weight * ((expFctr + 1.0) / (expFctr - 1.0))) - (1.0 / (playerTheta - itemBeta));
        }

        /// <summary>
        /// Calculates a new theta rating.
        /// </summary>
        ///
        /// <param name="currTheta">   current theta rating. </param>
        /// <param name="thetaKFctr">  K factor for the theta rating. </param>
        /// <param name="actualScore"> actual performance score. </param>
        /// <param name="expectScore"> expected performance score. </param>
        ///
        /// <returns>
        /// a double value for the new theta rating.
        /// </returns>
        private double calcTheta(double currTheta, double thetaKFctr, double actualScore, double expectScore)
        {
            return currTheta + (thetaKFctr * (actualScore - expectScore));
        }

        /// <summary>
        /// Calculates a new K factor for theta rating
        /// </summary>
        /// <param name="currThetaU">current uncertainty for the theta rating</param>
        /// <param name="currBetaU">current uncertainty for the beta rating</param>
        /// <returns>a double value of a new K factor for the theta rating</returns>
        private double calcThetaKFctr(double currThetaU, double currBetaU)
        {
            return kConst * (1 + (kUp * currThetaU) - (kDown * currBetaU));
        }

        /// <summary>
        /// Calculates a new uncertainty for the theta rating.
        /// </summary>
        ///
        /// <param name="currThetaU">       current uncertainty value for theta
        ///                                 rating. </param>
        /// <param name="currDelayCount">   the current number of consecutive days
        ///                                 the player has not played. </param>
        ///
        /// <returns>
        /// a new uncertainty value for theta rating.
        /// </returns>
        private double calcThetaUncertainty(double currThetaU, double currDelayCount)
        {
            double newThetaU = currThetaU - (1.0 / maxPlay) + (currDelayCount / maxDelay);
            if (newThetaU < 0) newThetaU = 0.0;
            else if (newThetaU > 1) newThetaU = 1.0;
            return newThetaU;
        }

        /// <summary>
        /// Calculates discrimination parameter a_i necessary to calculate expected
        /// and actual scores.
        /// </summary>
        ///
        /// <param name="itemMaxDuration">  maximum duration of time given to a
        ///                                 player to provide an answer; should be
        ///                                 player. </param>
        ///
        /// <returns>
        /// discrimination parameter a_i as double number.
        /// </returns>
        private double getDiscriminationParam(double itemMaxDuration)
        {
            return (double)(1.0 / itemMaxDuration);
        }

        /// <summary>
        /// Target beta.
        /// </summary>
        ///
        /// <param name="theta"> The theta. </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>
        private double TargetBeta(double theta)
        {
            double randomNum;
            do
            {
                randomNum = SimpleRNG.GetNormal(TARGET_DISTR_MEAN, TARGET_DISTR_SD);
            } while (randomNum <= TARGET_LOWER_LIMIT || randomNum >= TARGET_UPPER_LIMIT);
            return theta + Math.Log(randomNum / (1 - randomNum));
        }

        /// <summary>
        /// Tests the validity of the value representing the max amount of time to
        /// respond.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <param name="itemMaxDuration"> . </param>
        private void validateItemMaxDuration(double itemMaxDuration)
        {
            if (itemMaxDuration == 0) throw new System.ArgumentException("Parameter cannot be 0.", "itemMaxDuration");
            if (itemMaxDuration < 0) throw new System.ArgumentException("Parameter cannot be negative.", "itemMaxDuration");
        }

        //// [TODO][SC] what to do with expceptions: let the program crash?, or catch them?, or let to propagate to the main method? create a log file?
        /// <summary>
        /// Tests the validity of the value representing the response time.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments
        ///                                         have unsupported or illegal values. </exception>
        ///
        /// <param name="responseTime"> . </param>
        private void validateResponseTime(double responseTime)
        {
            if (responseTime == 0) throw new System.ArgumentException("Parameter cannot be 0.", "responseTime");
            if (responseTime < 0) throw new System.ArgumentException("Parameter cannot be negative.", "responseTime");
        }

        #endregion Methods
    }
}