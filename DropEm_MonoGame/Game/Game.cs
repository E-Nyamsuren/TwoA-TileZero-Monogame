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

Namespace: DropEm.Game
Filename: Game.cs
Description:
    Implements the main logic of the DropEm game.
*/

#endregion Header

namespace DropEm.Game
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using HAT;

    /// <summary>
    /// A DropEm game.
    /// </summary>
    public class DropEmGame
    {
        #region Fields

        /// <summary>
        /// The correct answer.
        /// </summary>
        private double correctAnswer;

        private int playabelTileCount;
        private int playedTileCount; // [SC] increments whenever a starting tile is put on a board or player receives a tile without dropping;
        private List<Player> players;
        private int selectedColIndex;
        private int selectedRowIndex;
        private List<DropEmTile> tileBag;
        private VirtualDropEmBoard virtualBoard;

        /// <summary>
        /// The software.
        /// </summary>
        private Stopwatch perGameStopWatch = new Stopwatch();
        private Stopwatch perTurnStopWatch = new Stopwatch();

        private string adaptID;
        private string scenarioID;

        HATAsset hatAsset;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DropEm.Game.DropEmGame
        /// class.
        /// </summary>
        public DropEmGame(HATAsset hatAsset)
        {
            this.hatAsset = hatAsset;

            Cfg.showMsg("AdaptationMode={0}", (hatAsset.Settings as HATAssetSettings).Mode);

            this.virtualBoard = new VirtualDropEmBoard();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the active game flag.
        /// </summary>
        ///
        /// <value>
        /// true if active game flag, false if not.
        /// </value>
        public bool ActiveGameFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the active player index.
        /// </summary>
        ///
        /// <value>
        /// The active player index.
        /// </value>
        public int ActivePlayerIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the end game flag.
        /// </summary>
        ///
        /// <value>
        /// true if end game flag, false if not.
        /// </value>
        public bool EndGameFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the end turn flag.
        /// </summary>
        ///
        /// <value>
        /// true if end turn flag, false if not.
        /// </value>
        public bool EndTurnFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the human player.
        /// </summary>
        ///
        /// <value>
        /// The human player.
        /// </value>
        public Player humanPlayer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the new game initialise flag.
        /// </summary>
        ///
        /// <value>
        /// true if new game initialise flag, false if not.
        /// </value>
        public bool NewGameInitFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of players.
        /// </summary>
        ///
        /// <value>
        /// The number of players.
        /// </value>
        public int PlayerCount
        {
            get
            {
                return players.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the start turn flag.
        /// </summary>
        ///
        /// <value>
        /// true if start turn flag, false if not.
        /// </value>
        public bool StartTurnFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ai player turn score.
        /// </summary>
        ///
        /// <value>
        /// The ai player turn score.
        /// </value>
        public int aiPlayerTurnScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the human player turn score.
        /// </summary>
        ///
        /// <value>
        /// The human player turn score.
        /// </value>
        private int humanPlayerTurnScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ai player.
        /// </summary>
        ///
        /// <value>
        /// The ai player.
        /// </value>
        public Player aiPlayer
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Drop human player tile.
        /// </summary>
        public void dropHumanPlayerTile()
        {
            dropPlayerTile(humanPlayer.PlayerIndex);
        }

        /// <summary>
        /// Drop player tile.
        /// </summary>
        ///
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        public void dropPlayerTile(int playerIndex)
        {
            if (!ActiveGameFlag)
            {
                return;
            }

            if (playerIndex != ActivePlayerIndex)
            {
                Cfg.showMsg("It is not your turn!");
                return;
            }

            Player activePlayer = players[ActivePlayerIndex];

            // [SC] check if player drop tiles
            if (!activePlayer.CanDrop)
            {
                Cfg.showMsg("Cannot drop a tile after putting a tile on a board!"); // [TODO]
                return;
            }

            // [SC] check if bag has tiles
            if (tileBag.Count == 0)
            {
                Cfg.showMsg("Cannot drop a tile! The bag is empty."); // [TODO]
                return;
            }

            // [SC] check if player tile is selected
            if (!activePlayer.isTileSelected)
            {
                Cfg.showMsg("Select a tile at first!"); // [TODO]
                return;
            }

            DropEmTile tile = activePlayer.SelectedTile;

            // [SC] make sure that the tile being dropped is not a replacement tile of previously dropped tile
            if (!tile.CanDrop)
            {
                Cfg.showMsg("Cannot drop a replacement tile!");
                return;
            }

            foreach (DropEmTile newTile in tileBag)
            {
                // [SC] make sure that the new tile does not have the same features as the dropped tile
                if (newTile.ColorIndex == tile.ColorIndex &&
                    newTile.ShapeIndex == tile.ShapeIndex)
                {
                    continue;
                }
                // [SC] remove the dropped tile from player's stack
                activePlayer.removeTile(tile);
                // [SC] add the dropped tile into the bag
                tileBag.Add(tile);

                // [SC] remove the new tile from the bag
                tileBag.Remove(newTile);
                // [SC] add the new tile to player's stack
                activePlayer.addTile(newTile);
                // [SC] make sure that the new tile cannot be dropped in the same turn
                newTile.CanDrop = false;

                // [SC] shuffle the bag
                tileBag.Shuffle();

                // [SC] prevent the player from moving tiles into the board
                activePlayer.CanMove = false;

                break;
            }
        }

        /// <summary>
        /// Ends a game.
        /// </summary>
        public void endGame()
        {
            if (!ActiveGameFlag)
            {
                return;
            }

            Player activePlayer = players[ActivePlayerIndex];
            activePlayer.increaseScore(Cfg.LAST_PLAYER_REWARD);

            int maxScore = Cfg.NONE;
            List<Player> maxScorePlayers = new List<Player>();
            foreach (Player player in players)
            {
                int playerScore = player.getPlayerScore();
                if (maxScore == Cfg.NONE || maxScore == playerScore)
                {
                    maxScorePlayers.Add(player);
                    maxScore = playerScore;
                }
                else if (maxScore < playerScore)
                {
                    maxScorePlayers.Clear();
                    maxScorePlayers.Add(player);
                    maxScore = playerScore;
                }
            }

            if (maxScorePlayers.Count > 1)
            {
                Cfg.showMsg("It is a draw!");
            }
            else
            {
                Cfg.showMsg(maxScorePlayers[0].PlayerName + " won the game!");

                if (maxScorePlayers[0] == humanPlayer)
                {
                    correctAnswer = 1;
                }
            }

            ActiveGameFlag = false;

            perGameStopWatch.Stop();
            double rt = perGameStopWatch.ElapsedMilliseconds;
            perGameStopWatch.Reset();

            // [SC] notify the asset about results of the game

            // [TODO] disable all the controls
            hatAsset.UpdateRatings(adaptID, Cfg.GAME_ID, humanPlayer.PlayerName, scenarioID, rt, correctAnswer);
        }

        /// <summary>
        /// Query if a board cell is selected.
        /// </summary>
        ///
        /// <returns>
        /// true if selected, false if not.
        /// </returns>
        public bool isSelected()
        {
            return (selectedRowIndex != Cfg.NONE && selectedColIndex != Cfg.NONE);
        }

        /// <summary>
        /// Ends human player turn.
        /// </summary>
        public void endHumanPlayerTurn()
        {
            endTurn(humanPlayer.PlayerIndex);
        }

        /// <summary>
        /// Ends a turn.
        /// </summary>
        ///
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        public void endTurn(int playerIndex)
        {
            if (!ActiveGameFlag)
            {
                Cfg.showMsg("No active game.");
            }
            else if (playerIndex != ActivePlayerIndex)
            {
                Cfg.showMsg("Not your turn {0}!", getPlayerNameByIndex(playerIndex));
            }
            else
            {
                EndTurnFlag = true;
            }
        }

        /// <summary>
        /// Ends a turn.
        /// </summary>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool endTurn()
        {
            // [SC] reset board position
            resetSelected();

            Player activePlayer = players[ActivePlayerIndex];

            if ((hatAsset.Settings as HATAssetSettings).Mode == HATAsset.HATMode.perTurn)
            {
                perTurnStopWatch.Stop();

                double turnRT = perTurnStopWatch.ElapsedMilliseconds;

                perTurnStopWatch.Reset();

                if (aiPlayerTurnScore != Cfg.NONE)
                {
                    // [SC] get human player's accuracy for the turn
                    double turnAccuracy = 1;

                    // [SC] if the AI player has a higher or equal overall score and scored more points in the last turn than the human player 
                    // then assume that human player lost in last two turns
                    //if (aiPlayer.getPlayerScore() >= humanPlayer.getPlayerScore() && aiPlayerTurnScore >= humanPlayerTurnScore) turnAccuracy = 0;
                    if (aiPlayerTurnScore >= humanPlayerTurnScore)
                    {
                        turnAccuracy = 0;
                    }

                    // [SC] updating player and scenario ratings 
                    // veg - Seems only to update the GUI!
                    //qwirkleWindow.endTurn(turnRT, turnAccuracy);
                }

            }

            // [SC] reset player's variables that are persistent only for a turn
            activePlayer.resetTurnVars();

            // [SC] refill player's tile array with new tiles from bag
            fillPlayerTiles(activePlayer);

            // [SC] if player has no tiles then end the game
            if (activePlayer.PlayerTileCount == 0)
            {
                return true;
            }

            // [SC] make the next player in a queue as a current player
            if (++ActivePlayerIndex >= players.Count)
            {
                ActivePlayerIndex = 0;
            }

            // [SC] set a flag to start a next turn
            StartTurnFlag = true;

            return false;
        }

        /// <summary>
        /// Force end game.
        /// </summary>
        public void forceEndGame()
        {
            ActiveGameFlag = false;
        }

        /// <summary>
        /// Gets human player tile by index.
        /// </summary>
        ///
        /// <param name="tileIndex"> Zero-based index of the tile. </param>
        ///
        /// <returns>
        /// The human player tile by index.
        /// </returns>
        public AbstractTile getHumanPlayerTileByIndex(int tileIndex)
        {
            DropEmTile tile = humanPlayer.getTileAt(tileIndex);

            return tile != null ?
                new AbstractTile(tile.ColorIndex, tile.ShapeIndex, tile.TileID, tile.Playable) :
                null;
        }

        /// <summary>
        /// Gets player name by index.
        /// </summary>
        ///
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        ///
        /// <returns>
        /// The player name by index.
        /// </returns>
        public string getPlayerNameByIndex(int playerIndex)
        {
            return players[playerIndex].PlayerName;
        }

        /// <summary>
        /// Gets player score by index.
        /// </summary>
        ///
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        ///
        /// <returns>
        /// The player score by index.
        /// </returns>
        public int getPlayerScoreByIndex(int playerIndex)
        {
            return players[playerIndex].getPlayerScore();
        }

        /// <summary>
        /// Gets virtual board.
        /// </summary>
        ///
        /// <returns>
        /// The virtual board.
        /// </returns>
        public VirtualDropEmBoard getVirtualBoard()
        {
            return virtualBoard;
        }

        /// <summary>
        /// Initialises the new game.
        /// </summary>
        ///
        /// <param name="adaptID">           Identifier for the adapt. </param>
        /// <param name="playerName">        Name of the player. </param>
        /// <param name="aiID">              Identifier for the ai. </param>
        /// <param name="giveHint">          true to give hint. </param>
        /// <param name="playableTileCount"> Number of playable tiles. </param>
        public void initNewGame(string adaptID, string scenarioID, string playerName, string[] aiID, bool giveHint, int playableTileCount)
        {
            this.adaptID = adaptID;

            this.scenarioID = scenarioID;

            int aiPlayerCount = aiID.Length;

            createTileBag();

            if (verifyPlayableTileCount(playableTileCount, aiPlayerCount))
            {
                this.playabelTileCount = playableTileCount;
            }
            else
            {
                this.playabelTileCount = tileBag.Count;
            }

            playedTileCount = 0;

            virtualBoard.resetBoard();

            createPlayers(playerName, aiID, giveHint); // [SC] should be called after 3 tiles were put on a board

            ActivePlayerIndex = 0;

            correctAnswer = 0;

            NewGameInitFlag = true;

            if (perTurnStopWatch.IsRunning)
            {
                perTurnStopWatch.Stop();
            }
            perTurnStopWatch.Reset();

            aiPlayerTurnScore = Cfg.NONE;
            humanPlayerTurnScore = Cfg.NONE;
        }

        /// <summary>
        /// Place human player tile on board.
        /// </summary>
        public void placeHumanPlayerTileOnBoard()
        {
            placePlayerTileOnBoard(humanPlayer.PlayerIndex);
        }

        /// <summary>
        /// Place player tile on board.
        /// </summary>
        ///
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool placePlayerTileOnBoard(int playerIndex)
        {
            if (!ActiveGameFlag)
            {
                return false;
            }

            if (playerIndex != ActivePlayerIndex)
            {
                Cfg.showMsg("It is not your turn!");
                return false;
            }

            Player activePlayer = players[ActivePlayerIndex];

            // [SC] check if player can put tiles on a board
            if (!activePlayer.CanMove)
            {
                Cfg.showMsg("Cannot move a tile after dropping a tile!"); // [TODO]
                return false;
            }

            // [SC] check if board position is selected
            if (!isSelected())
            {
                Cfg.showMsg("Select a board position at first!"); // [TODO]
                return false;
            }

            // [SC] check if player tile is selected
            if (!activePlayer.isTileSelected)
            {
                Cfg.showMsg("Select a tile at first!"); // [TODO]
                return false;
            }

            DropEmTile tile = activePlayer.SelectedTile;

            int result = putTileOnBoard(selectedRowIndex, selectedColIndex, tile, true);

            if (result != Cfg.NONE)
            {
                // [SC] increase player's score
                activePlayer.increaseScore(result);

                if ((hatAsset.Settings as HATAssetSettings).Mode == HATAsset.HATMode.perTurn)
                {
                    if (activePlayer.isHuman)
                    {
                        humanPlayerTurnScore += result;
                    }
                    else
                    {
                        aiPlayerTurnScore += result;
                    }
                }

                // [SC] remove the tile from the player and reset player selection
                activePlayer.removeSelectedTile();

                // [SC] disable mismatching tiles
                activePlayer.disableMismatchedTiles(tile.ColorIndex, tile.ShapeIndex);

                // [SC] prevent the player from dropping tiles in the same turn
                activePlayer.CanDrop = false;

                // [SC] reset board selection
                resetSelected();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets the selected.
        /// </summary>
        public void resetSelected()
        {
            selectedRowIndex = Cfg.NONE;
            selectedColIndex = Cfg.NONE;
        }

        /// <summary>
        /// Sets selected cell.
        /// </summary>
        ///
        /// <param name="rowIndex">    Zero-based index of the row. </param>
        /// <param name="colIndex">    Zero-based index of the col. </param>
        /// <param name="playerIndex"> Zero-based index of the player. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool setSelectedCell(int rowIndex, int colIndex, int playerIndex)
        {
            if (playerIndex >= players.Count)
            {
                Cfg.showMsg("Unknown player with an index " + playerIndex + "."); //[TODO]
            }
            else if (ActivePlayerIndex != playerIndex)
            {
                Cfg.showMsg("It is not your turn, " + players[playerIndex].PlayerName + "!"); //[TODO]
            }
            else
            {
                selectedRowIndex = rowIndex;
                selectedColIndex = colIndex;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets selected player tile.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        /// <param name="tileID">     Identifier for the tile. </param>
        public void setSelectedPlayerTile(int colorIndex, int shapeIndex, int tileID)
        {
            // [SC] check if it is human player's turn
            if (ActivePlayerIndex != humanPlayer.PlayerIndex)
            {
                Cfg.showMsg("It is not your turn, " + humanPlayer.PlayerName + "!"); //[TODO]
                return;
            }

            // [TODO] what if parameters values are Cfg.NONE
            if (!humanPlayer.setSelectedTile(colorIndex, shapeIndex, tileID))
            {
                Cfg.showMsg(String.Format("The {0} {1} tile is not playable!", (DropEmTile.TileColors)colorIndex, (DropEmTile.TileShapes)shapeIndex));
            }
        }

        /// <summary>
        /// Starts new game.
        /// </summary>
        public void startNewGame()
        {
            if (!NewGameInitFlag)
            {
                Cfg.showMsg("Another game is already running.");
                return;
            }

            if (ActiveGameFlag)
            {
                Cfg.showMsg("Another game is already running.");
                return;
            }

            putStartingTiles();

            // [SC] check if stopwatch is running
            if (perGameStopWatch.IsRunning)
            {
                perGameStopWatch.Stop();
                perGameStopWatch.Reset();
            }

            NewGameInitFlag = false;

            StartTurnFlag = true;
            EndTurnFlag = false;

            ActiveGameFlag = true;

            perGameStopWatch.Start();
        }

        /// <summary>
        /// Starts a turn.
        /// </summary>
        public void startTurn()
        {
            if (!ActiveGameFlag)
            {
                return;
            }

            Player activePlayer = players[ActivePlayerIndex];

            // [TODO] play beep music
            Cfg.showMsg(activePlayer.PlayerName + "'s turn.");

            if (activePlayer.isHuman)
            {
                activePlayer.getAIHint();

                if ((hatAsset.Settings as HATAssetSettings).Mode == HATAsset.HATMode.perTurn)
                {
                    perTurnStopWatch.Start();

                    humanPlayerTurnScore = 0;
                }
            }
            else
            {
                if ((hatAsset.Settings as HATAssetSettings).Mode == HATAsset.HATMode.perTurn)
                {
                    // [SC] reset AI player's turn score
                    aiPlayerTurnScore = 0;

                    // [SC] decide on a new player ID
                    string aiPlayerID = hatAsset.TargetScenarioID(adaptID, Cfg.GAME_ID, humanPlayer.PlayerName);

                    // [SC] set new AI player ID
                    activePlayer.PlayerType = aiPlayerID;
                }
                activePlayer.invokeAI();
                endTurn(ActivePlayerIndex);
            }
        }

        /// <summary>
        /// Creates the players.
        /// </summary>
        ///
        /// <param name="playerName"> Name of the player. </param>
        /// <param name="aiID">       Identifier for the ai. </param>
        /// <param name="giveHint">   true to give hint. </param>
        private void createPlayers(string playerName, string[] aiID, bool giveHint)
        {
            players = new List<Player>();

            // [SC] creating a human player
            Player humanPlayer = new Player(0, Cfg.HUMAN_PLAYER, giveHint, this);
            humanPlayer.PlayerName = playerName;
            this.humanPlayer = humanPlayer;
            fillPlayerTiles(humanPlayer);

            players.Add(humanPlayer);

            // [SC] creating AI players
            for (int currPlayerIndex = 0; currPlayerIndex < aiID.Length; currPlayerIndex++)
            {
                Player currPlayer = new Player(currPlayerIndex + 1, aiID[currPlayerIndex], false, this);

                fillPlayerTiles(currPlayer);
                players.Add(currPlayer);

                aiPlayer = currPlayer;
            }
        }

        /// <summary>
        /// Creates tile bag.
        /// </summary>
        private void createTileBag()
        {
            tileBag = new List<DropEmTile>();

            for (int colorIndex = 0; colorIndex < Cfg.MAX_VAL_INDEX; colorIndex++)
            {
                for (int shapeIndex = 0; shapeIndex < Cfg.MAX_VAL_INDEX; shapeIndex++)
                {
                    for (int tileID = 0; tileID < Cfg.MAX_TILE_ID; tileID++)
                    {
                        tileBag.Add(new DropEmTile(colorIndex, shapeIndex, tileID));
                    }
                }
            }

            tileBag.Shuffle();
        }

        /// <summary>
        /// Fill player tiles.
        /// </summary>
        ///
        /// <param name="player"> The player. </param>
        private void fillPlayerTiles(Player player)
        {
            // [SC] make sure the tile bag is not empty and not all playable tiles are used
            while (tileBag.Count > 0 && playedTileCount < playabelTileCount)
            {
                DropEmTile tile = tileBag.ElementAt(0);
                if (player.addTile(tile))
                {
                    tileBag.Remove(tile);
                    ++playedTileCount;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Puts starting tiles.
        /// </summary>
        private void putStartingTiles()
        {
            int startCol = virtualBoard.colCount / 2 - Cfg.START_TILE_COUNT / 2;
            int startRow = virtualBoard.rowCount / 2;

            for (int counter = 0; counter < Cfg.START_TILE_COUNT; counter++)
            {
                int currCol = startCol + counter;
                DropEmTile tile = (DropEmTile)tileBag.ElementAt(0);

                int result = putTileOnBoard(startRow, currCol, tile, false);

                // [TODO] need to terminate the game
                if (result == Cfg.NONE)
                {
                    Cfg.showMsg("Error putting starting tiles");
                    break;
                }

                tileBag.Remove(tile);
                ++playedTileCount;
            }
        }

        /// <summary>
        /// put a given tile on a specified board position; validCheck is true then verify if the move conforms to game rules
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tile">       The tile. </param>
        /// <param name="validCheck"> true to valid check. </param>
        ///
        /// <returns>
        /// An int.
        /// </returns>
        private int putTileOnBoard(int rowIndex, int colIndex, DropEmTile tile, bool validCheck)
        {
            return virtualBoard.addTile(rowIndex, colIndex, tile, validCheck, null);
        }

        /// <summary>
        /// Verify playable tile count.
        /// </summary>
        ///
        /// <param name="tileCount">     Number of tiles. </param>
        /// <param name="aiPlayerCount"> Number of ai players. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        private bool verifyPlayableTileCount(int tileCount, int aiPlayerCount)
        {
            int minTileCount = Cfg.START_TILE_COUNT + (aiPlayerCount + 1) * Cfg.MAX_PLAYER_TILE_COUNT;
            if (tileCount < minTileCount)
            {
                Cfg.showMsg("The minimum umber of playable tiles should be " + minTileCount + ". Using default bag size.");
                return false;
            }
            return true;
        }

        #endregion Methods
    }
}