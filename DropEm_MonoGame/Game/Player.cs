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
Filename: Player.cs
Description:
    Defines a class that implements a player that can be either a construct for human player or purely AI player.
*/
#endregion Header

namespace DropEm.Game
{
    using System.Collections.Generic;

    using System.Diagnostics;

    /// <summary>
    /// A player.
    /// </summary>
    public class Player
    {
        private DropEmGame game;

        /// <summary>
        /// Gets a value indicating whether the human.
        /// </summary>
        ///
        /// <value>
        /// true if human, false if not.
        /// </value>
        public bool Human
        {
            get;
            private set;
        }

        private List<DropEmTile> playerTiles;
        private DropEmTile selectedTile;

        private int colorReq;
        private int shapeReq;

        private int playerScore;

        /// <summary>
        /// Initializes a new instance of the DropEm.Game.Player class.
        /// </summary>
        ///
        /// <param name="playerIndex"> The player index. </param>
        /// <param name="playerType">  The type of the player. </param>
        /// <param name="hintFlag">    true if hint flag, false if not. </param>
        /// <param name="game">        The game. </param>
        public Player(int playerIndex, string playerType, bool hintFlag, DropEmGame game)
        {
            this.game = game;
            this.PlayerType = playerType;

            this.Human = (PlayerType.Equals(Cfg.HUMAN_PLAYER));

            this.PlayerIndex = playerIndex;
            this.HintFlag = hintFlag;

            this.PlayerName = createPlayerName();

            this.playerScore = 0;

            playerTiles = new List<DropEmTile>();

            resetGameVars();
        }

        /// <summary>
        /// Creates player name.
        /// </summary>
        ///
        /// <returns>
        /// The new player name.
        /// </returns>
        private string createPlayerName()
        {
            return string.Format("{0} {1}", PlayerType, PlayerIndex);
        }

        /// <summary>
        /// Resets the game variables.
        /// </summary>
        public void resetGameVars()
        {
            resetTurnVars();

            playerScore = 0;
        }

        /// <summary>
        /// Resets the turn variables.
        /// </summary>
        public void resetTurnVars()
        {
            resetSelected();

            resetColorReq();
            resetShapeReq();
            resetTiles();

            CanDrop = true;
            CanMove = true;
        }

        /// <summary>
        /// Adds a tile.
        /// </summary>
        ///
        /// <param name="tile"> The tile. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool addTile(DropEmTile tile)
        {
            if (PlayerTileCount < Cfg.MAX_PLAYER_TILE_COUNT)
            {
                playerTiles.Add(tile);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the tile described by tile.
        /// </summary>
        ///
        /// <param name="tile"> The tile. </param>
        public void removeTile(DropEmTile tile)
        {
            playerTiles.Remove(tile);
        }

        /// <summary>
        /// Gets tile at.
        /// </summary>
        ///
        /// <param name="index"> Zero-based index of the. </param>
        ///
        /// <returns>
        /// The tile at.
        /// </returns>
        public DropEmTile getTileAt(int index)
        {
            if (index < playerTiles.Count) return playerTiles[index];
            else return null;
        }

        /// <summary>
        /// Gets the number of player tiles.
        /// </summary>
        ///
        /// <value>
        /// The number of player tiles.
        /// </value>
        public int PlayerTileCount
        {
            get
            {
                return (playerTiles != null) ? playerTiles.Count : 0;
            }
        }

        ////// END: generic functions for manipulating tiles
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets selected tile.
        /// </summary>
        ///
        /// <param name="tile"> The tile. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        private bool setSelectedTile(DropEmTile tile)
        {
            // [TODO] make sure the tile is one of the player's tiles
            if (!tile.Playable)
            {
                Cfg.showMsg("The tile " + tile.ColorIndex + "-" + tile.ShapeIndex + " is not playable.");
            }
            else
            {
                selectedTile = tile;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets selected tile.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        /// <param name="tileID">     Identifier for the tile. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool setSelectedTile(int colorIndex, int shapeIndex, int tileID)
        {
            resetSelected();
            foreach (DropEmTile tile in playerTiles)
            {
                if (tile.sameTile(colorIndex, shapeIndex, tileID) && tile.Playable)
                {
                    return setSelectedTile(tile);
                }
            }
            return false;
        }

        /// <summary>
        /// Resets the selected.
        /// </summary>
        public void resetSelected()
        {
            selectedTile = null;
        }

        /// <summary>
        /// Gets a value indicating whether this object is tile selected.
        /// </summary>
        ///
        /// <value>
        /// true if this object is tile selected, false if not.
        /// </value>
        public bool isTileSelected
        {
            get
            {
                return (selectedTile != null);
            }
        }

        /// <summary>
        /// Gets the selected tile.
        /// </summary>
        ///
        /// <value>
        /// The selected tile.
        /// </value>
        public DropEmTile SelectedTile
        {
            get
            {
                return selectedTile;
            }
        }

        /// <summary>
        /// Removes the selected tile.
        /// </summary>
        public void removeSelectedTile()
        {
            removeTile(selectedTile);
            resetSelected();
        }

        /// <summary>
        /// Disables the mismatched tiles.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        public void disableMismatchedTiles(int colorIndex, int shapeIndex)
        {
            if (!hasColorReq() && !hasShapeReq())
            {
                setColorReq(colorIndex);
                setShapeReq(shapeIndex);
            }
            else if (hasColorReq() && hasShapeReq())
            {
                if (sameColorReq(colorIndex) && !sameShapeReq(shapeIndex))
                    resetShapeReq();
                else if (!sameColorReq(colorIndex) && sameShapeReq(shapeIndex))
                    resetColorReq();
            }

            if (hasColorReq() && hasShapeReq())
            {
                for (int currTileIndex = 0; currTileIndex < playerTiles.Count; currTileIndex++)
                {
                    DropEmTile tile = playerTiles[currTileIndex];
                    if (!sameColorReq(tile.ColorIndex) && !sameShapeReq(tile.ShapeIndex))
                        tile.Playable = false;
                }
            }
            else if (hasColorReq() && !hasShapeReq())
            {
                for (int currTileIndex = 0; currTileIndex < playerTiles.Count; currTileIndex++)
                {
                    DropEmTile tile = playerTiles[currTileIndex];
                    if (!sameColorReq(tile.ColorIndex))
                        tile.Playable = false;
                }
            }
            else if (!hasColorReq() && hasShapeReq())
            {
                for (int currTileIndex = 0; currTileIndex < playerTiles.Count; currTileIndex++)
                {
                    DropEmTile tile = playerTiles[currTileIndex];
                    if (!sameShapeReq(tile.ShapeIndex))
                        tile.Playable = false;
                }
            }
        }

        /// <summary>
        /// Resets the tiles.
        /// </summary>
        public void resetTiles()
        {
            for (int currTileIndex = 0; currTileIndex < playerTiles.Count; currTileIndex++)
                playerTiles[currTileIndex].resetTile();
        }

        /// <summary>
        /// Gets a value indicating whether the hint flag.
        /// </summary>
        ///
        /// <value>
        /// true if hint flag, false if not.
        /// </value>
        public bool HintFlag
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        ///
        /// <value>
        /// The name of the player.
        /// </value>
        public string PlayerName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets player score.
        /// </summary>
        ///
        /// <returns>
        /// The player score.
        /// </returns>
        public int getPlayerScore()
        {
            return playerScore;
        }

        /// <summary>
        /// Increase score.
        /// </summary>
        ///
        /// <param name="score"> The score. </param>
        public void increaseScore(int score)
        {
            playerScore += score;
        }

        /// <summary>
        /// Gets the zero-based index of the player.
        /// </summary>
        ///
        /// <value>
        /// The player index.
        /// </value>
        public int PlayerIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the player.
        /// </summary>
        ///
        /// <value>
        /// The type of the player.
        /// </value>
        public string PlayerType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether this object is human.
        /// </summary>
        ///
        /// <value>
        /// true if this object is human, false if not.
        /// </value>
        public bool isHuman
        {
            get
            {
                return Human;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we can drop.
        /// </summary>
        ///
        /// <value>
        /// true if we can drop, false if not.
        /// </value>
        public bool CanDrop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether we can move.
        /// </summary>
        ///
        /// <value>
        /// true if we can move, false if not.
        /// </value>
        public bool CanMove
        {
            get;
            set;
        }

        ////// END: can move and can drop flags
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets color request.
        /// </summary>
        ///
        /// <returns>
        /// The color request.
        /// </returns>
        public int getColorReq()
        {
            return colorReq;
        }

        /// <summary>
        /// Sets color request.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        public void setColorReq(int colorIndex)
        {
            this.colorReq = colorIndex;
        }

        /// <summary>
        /// Query if this object has color request.
        /// </summary>
        ///
        /// <returns>
        /// true if color request, false if not.
        /// </returns>
        public bool hasColorReq()
        {
            if (colorReq != Cfg.NONE) return true;
            else return false;
        }

        /// <summary>
        /// Same color request.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool sameColorReq(int colorIndex)
        {
            if (this.colorReq == colorIndex) return true;
            else return false;
        }

        /// <summary>
        /// Resets the color request.
        /// </summary>
        public void resetColorReq()
        {
            colorReq = Cfg.NONE;
        }

        ////// END: functions for color requirements
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets shape request.
        /// </summary>
        ///
        /// <returns>
        /// The shape request.
        /// </returns>
        public int getShapeReq()
        {
            return shapeReq;
        }

        /// <summary>
        /// Sets shape request.
        /// </summary>
        ///
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        public void setShapeReq(int shapeIndex)
        {
            this.shapeReq = shapeIndex;
        }

        /// <summary>
        /// Query if this object has shape request.
        /// </summary>
        ///
        /// <returns>
        /// true if shape request, false if not.
        /// </returns>
        public bool hasShapeReq()
        {
            if (shapeReq != Cfg.NONE) return true;
            else return false;
        }

        /// <summary>
        /// Same shape request.
        /// </summary>
        ///
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool sameShapeReq(int shapeIndex)
        {
            if (this.shapeReq == shapeIndex) return true;
            else return false;
        }

        /// <summary>
        /// Resets the shape request.
        /// </summary>
        public void resetShapeReq()
        {
            shapeReq = Cfg.NONE;
        }

        ////// END: functions for shape requirements
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets an i hint.
        /// </summary>
        public void getAIHint()
        {
            // Debug.Print("Hint by AI!");
        }

        ////// END: hint functionality
        /////////////////////////////////////////////////////////////////

        public void invokeAI()
        {
            if (PlayerType.Equals(Cfg.VERY_EASY_AI))
            {
                invokeVeryEasyAI();
            }
            else if (PlayerType.Equals(Cfg.EASY_AI))
            {
                invokeEasyAI();
            }
            else if (PlayerType.Equals(Cfg.MEDIUM_COLOR_AI))
            {
                invokeColorOnlyMediumAI();
            }
            else if (PlayerType.Equals(Cfg.MEDIUM_SHAPE_AI))
            {
                invokeShapeOnlyMediumAI();
            }
            else if (PlayerType.Equals(Cfg.HARD_AI))
            {
                invokeHardAI();
            }
            else if (PlayerType.Equals(Cfg.VERY_HARD_AI))
            {
                invokeVeryHardAI();
            }
        }

        /////////////////////////////////////////////////////////////////
        ////// START: very easy ai functionality
        #region very easy AI

        /// <summary>
        /// Executes the very easy an i on a different thread, and waits for the
        /// result. Puts only a single tile on a board.
        /// </summary>
        public void invokeVeryEasyAI()
        {
            VirtualDropEmBoard virtualBoard = game.getVirtualBoard();
            int rowCount = virtualBoard.rowCount;
            int colCount = virtualBoard.colCount;

            // [SC] using copy since indices in playerTiles may change due to removed tiles
            List<DropEmTile> tempPlayerTiles = playerTiles.listShallowClone();

            Stopwatch sw = new Stopwatch();
            bool tilePlacedFlag = false;
            bool shouldDropFlag = true;

            foreach (DropEmTile tile in tempPlayerTiles)
            {
                // [SC] check if the tile is playable
                if (!tile.Playable)
                {
                    continue;
                }

                for (int currRowIndex = 0; currRowIndex < rowCount && !tilePlacedFlag; currRowIndex++)
                {
                    for (int currColIndex = 0; currColIndex < colCount; currColIndex++)
                    {

                        int resultScore = virtualBoard.isValidMove(currRowIndex, currColIndex, tile, true, null, false);

                        if (resultScore != Cfg.NONE)
                        {
                            //sw.Start();
                            //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                            //sw.Stop();
                            //sw.Reset();

                            setSelectedTile(tile);
                            game.setSelectedCell(currRowIndex, currColIndex, PlayerIndex);
                            game.placePlayerTileOnBoard(PlayerIndex);
                            tilePlacedFlag = true;
                            shouldDropFlag = false;
                            break;
                        }
                    }
                }

                if (tilePlacedFlag) break;
            }

            if (shouldDropFlag)
            {
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
        }
        #endregion
        ////// END: very easy ai functionality
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        ////// START: easy ai functionality
        #region easy AI

        /// <summary>
        /// Executes the easy an i on a different thread, and waits for the
        /// result.
        /// </summary>
        public void invokeEasyAI()
        {
            VirtualDropEmBoard virtualBoard = game.getVirtualBoard();
            int rowCount = virtualBoard.rowCount;
            int colCount = virtualBoard.colCount;

            // [SC] using copy since indices in playerTiles may change due to removed tiles
            List<DropEmTile> tempPlayerTiles = playerTiles.listShallowClone();

            Stopwatch sw = new Stopwatch();
            bool tilePlacedFlag = false;
            bool shouldDropFlag = true;

            foreach (DropEmTile tile in tempPlayerTiles)
            {
                // [SC] check if the tile is playable
                if (!tile.Playable)
                {
                    continue;
                }

                // [SC] add some daly before making next move
                if (tilePlacedFlag)
                {
                    //sw.Start();
                    //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                    //sw.Stop();
                    //sw.Reset();
                    tilePlacedFlag = false;
                }

                for (int currRowIndex = 0; currRowIndex < rowCount && !tilePlacedFlag; currRowIndex++)
                {
                    for (int currColIndex = 0; currColIndex < colCount; currColIndex++)
                    {
                        int resultScore = virtualBoard.isValidMove(currRowIndex, currColIndex, tile, true, null, false);

                        if (resultScore != Cfg.NONE)
                        {
                            setSelectedTile(tile);
                            game.setSelectedCell(currRowIndex, currColIndex, PlayerIndex);
                            game.placePlayerTileOnBoard(PlayerIndex);
                            tilePlacedFlag = true;
                            shouldDropFlag = false;
                            break;
                        }
                    }
                }
            }

            if (shouldDropFlag)
            {
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
        }
        #endregion
        ////// END: easy ai functionality
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        ////// START: medium ai functionality - color only
        #region medium AI with color dimension only

        /// <summary>
        /// Executes the color only medium an i on a different thread, and waits for
        /// the result.
        /// </summary>
        public void invokeColorOnlyMediumAI()
        {
            CandidateTilePos selectedPosCombo = calculateMoves(true, false, false);

            if (selectedPosCombo == null)
            { // [SC] no tiles to put on a board
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
            else if (CanMove)
            {
                CandidateTileSeq tileSeq = selectedPosCombo.getCandidateTileSeq();
                int totalMoveCount = selectedPosCombo.getComboLength();
                int currMoveCount = 0;

                Stopwatch sw = new Stopwatch();

                while (currMoveCount < totalMoveCount)
                {
                    //sw.Start();
                    //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                    //sw.Stop();
                    //sw.Reset();

                    AbstractPos abstrPos = selectedPosCombo.getAbstrPosAt(currMoveCount);
                    int rowIndex = abstrPos.getRowIndex();
                    int colIndex = abstrPos.getColIndex();
                    int tileIndex = abstrPos.getTileIndex();

                    DropEmTile tile = tileSeq[tileIndex];

                    setSelectedTile(tile);

                    game.setSelectedCell(rowIndex, colIndex, PlayerIndex);

                    game.placePlayerTileOnBoard(PlayerIndex);

                    currMoveCount++;
                }
            }
        }
        #endregion
        ////// END: medium ai functionality - color only
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        ////// START: medium ai functionality - shape only
        #region medium AI with shape dimension only

        /// <summary>
        /// Executes the shape only medium an i on a different thread, and waits for
        /// the result.
        /// </summary>
        public void invokeShapeOnlyMediumAI()
        {
            CandidateTilePos selectedPosCombo = calculateMoves(false, true, false);

            if (selectedPosCombo == null)
            { // [SC] no tiles to put on a board
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
            else if (CanMove)
            {
                CandidateTileSeq tileSeq = selectedPosCombo.getCandidateTileSeq();
                int totalMoveCount = selectedPosCombo.getComboLength();
                int currMoveCount = 0;

                Stopwatch sw = new Stopwatch();

                while (currMoveCount < totalMoveCount)
                {
                    //sw.Start();
                    //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                    //sw.Stop();
                    //sw.Reset();

                    AbstractPos abstrPos = selectedPosCombo.getAbstrPosAt(currMoveCount);
                    int rowIndex = abstrPos.getRowIndex();
                    int colIndex = abstrPos.getColIndex();
                    int tileIndex = abstrPos.getTileIndex();

                    DropEmTile tile = tileSeq[tileIndex];

                    setSelectedTile(tile);

                    game.setSelectedCell(rowIndex, colIndex, PlayerIndex);

                    game.placePlayerTileOnBoard(PlayerIndex);

                    currMoveCount++;
                }
            }
        }
        #endregion
        ////// END: medium ai functionality - shape only
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        ////// START: hard ai functionality
        #region hard AI

        /// <summary>
        /// Executes the hard an i on a different thread, and waits for the
        /// result.
        /// </summary>
        public void invokeHardAI()
        {
            CandidateTilePos selectedPosCombo = calculateMoves(true, true, true);

            if (selectedPosCombo == null)
            { // [SC] no tiles to put on a board
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
            else if (CanMove)
            {
                CandidateTileSeq tileSeq = selectedPosCombo.getCandidateTileSeq();
                int totalMoveCount = selectedPosCombo.getComboLength();
                int currMoveCount = 0;

                Stopwatch sw = new Stopwatch();

                while (currMoveCount < totalMoveCount)
                {
                    //sw.Start();
                    //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                    //sw.Stop();
                    //sw.Reset();

                    AbstractPos abstrPos = selectedPosCombo.getAbstrPosAt(currMoveCount);
                    int rowIndex = abstrPos.getRowIndex();
                    int colIndex = abstrPos.getColIndex();
                    int tileIndex = abstrPos.getTileIndex();

                    DropEmTile tile = tileSeq[tileIndex];

                    setSelectedTile(tile);

                    game.setSelectedCell(rowIndex, colIndex, PlayerIndex);

                    game.placePlayerTileOnBoard(PlayerIndex);

                    currMoveCount++;
                }
            }
        }

        /// <summary>
        /// Board position permission traverse tree paths.
        /// 
        /// Gets the combo with the second highest score
        /// </summary>
        ///
        /// <param name="rootNode">           The root node. </param>
        /// <param name="currPath">           Full pathname of the curr file. </param>
        /// <param name="candTileSeq">        The cand tile sequence. </param>
        /// <param name="currScore">          The curr score. </param>
        /// <param name="chosenPosComboList">   List of chosen position comboes. </param>
        /// <param name="maxScorePosCombo">     The maximum score position combo. </param>
        private void boardPosPermTraverseTreePaths(TreeNode rootNode, List<AbstractPos> currPath, CandidateTileSeq candTileSeq, int currScore
                                                    , List<CandidateTilePos> chosenPosComboList, List<CandidateTilePos> maxScorePosCombo)
        {

            if (rootNode.hasChildNodes())
            {
                List<TreeNode> childNodes = rootNode.getChildNodes();
                foreach (TreeNode childNode in childNodes)
                {
                    List<AbstractPos> newPath = currPath.listShallowClone();
                    AbstractPos pos = (AbstractPos)childNode.getValue();
                    newPath.Add(pos);
                    boardPosPermTraverseTreePaths(childNode, newPath, candTileSeq, currScore + pos.getScore(), chosenPosComboList, maxScorePosCombo);
                }
            }
            else
            { // [SC] reached the final leaf; no more moves in the combo
                if (currScore > 0)
                {
                    CandidateTilePos newCandTilePos = new CandidateTilePos(candTileSeq, currPath, currScore);

                    if (maxScorePosCombo.Count == 0)
                    {
                        maxScorePosCombo.Add(newCandTilePos);

                        chosenPosComboList.Clear();
                        chosenPosComboList.Add(newCandTilePos);
                    }
                    else if (maxScorePosCombo[0].TotalScore == currScore)
                    {
                        if (chosenPosComboList.Count != 0 && chosenPosComboList[0].TotalScore == currScore)
                        {
                            chosenPosComboList.Add(newCandTilePos);
                        }
                    }
                    else if (maxScorePosCombo[0].TotalScore < currScore)
                    {
                        chosenPosComboList.Clear();
                        chosenPosComboList.Add(maxScorePosCombo[0]);

                        maxScorePosCombo.Clear();
                        maxScorePosCombo.Add(newCandTilePos);
                    }
                    else if (maxScorePosCombo[0].TotalScore > currScore)
                    {
                        if (chosenPosComboList.Count == 0)
                        {
                            chosenPosComboList.Add(newCandTilePos);
                        }
                        else if (chosenPosComboList[0].TotalScore == maxScorePosCombo[0].TotalScore)
                        {
                            chosenPosComboList.Clear();
                            chosenPosComboList.Add(newCandTilePos);
                        }
                        else if (chosenPosComboList[0].TotalScore < currScore)
                        {
                            chosenPosComboList.Clear();
                            chosenPosComboList.Add(newCandTilePos);
                        }
                        else if (chosenPosComboList[0].TotalScore == currScore)
                        {
                            chosenPosComboList.Add(newCandTilePos);
                        }
                    }
                }
            }
        }
        #endregion hard AI
        ////// END: hard ai functionality
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        ////// START: very hard ai functionality
        #region very hard AI

        /// <summary>
        /// Executes the very hard an i on a different thread, and waits for the
        /// result.
        /// </summary>
        public void invokeVeryHardAI()
        {
            CandidateTilePos selectedPosCombo = calculateMoves(true, true, false);

            if (selectedPosCombo == null)
            { // [SC] no tiles to put on a board
                // [SC] dropping a random tile
                setSelectedTile(playerTiles.getRandomElement());
                game.dropPlayerTile(PlayerIndex);
                Cfg.showMsg(PlayerName + " dropped a tile.");
            }
            else if (CanMove)
            {
                CandidateTileSeq tileSeq = selectedPosCombo.getCandidateTileSeq();
                int totalMoveCount = selectedPosCombo.getComboLength();
                int currMoveCount = 0;

                Stopwatch sw = new Stopwatch();

                while (currMoveCount < totalMoveCount)
                {
                    //sw.Start();
                    //while (sw.ElapsedMilliseconds < Cfg.ACTION_DELAY) ;
                    //sw.Stop();
                    //sw.Reset();

                    AbstractPos abstrPos = selectedPosCombo.getAbstrPosAt(currMoveCount);
                    int rowIndex = abstrPos.getRowIndex();
                    int colIndex = abstrPos.getColIndex();
                    int tileIndex = abstrPos.getTileIndex();

                    DropEmTile tile = tileSeq[tileIndex];

                    setSelectedTile(tile);

                    game.setSelectedCell(rowIndex, colIndex, PlayerIndex);

                    game.placePlayerTileOnBoard(PlayerIndex);

                    currMoveCount++;
                }
            }
        }

        /// <summary>
        /// 1. Create lists of tiles where each list is a group of tiles with the
        /// same color or shape. 2. For each group of tiles, create lists of tile
        /// sequences where each list contains a sequence of tiles in a unique order.
        /// 3. For each tile sequence, create a combination of unique board positions.
        /// </summary>
        ///
        /// <param name="considerColor"> true to consider color. </param>
        /// <param name="considerShape"> true to consider shape. </param>
        /// <param name="suboptiomal">   true to suboptiomal. </param>
        ///
        /// <returns>
        /// The calculated moves.
        /// </returns>
        public CandidateTilePos calculateMoves(bool considerColor, bool considerShape, bool suboptiomal)
        {
            List<CandidateTileSeq> candTileSeqList = new List<CandidateTileSeq>(); // [TODO]

            List<DropEmTile>[] colorTileLists = new List<DropEmTile>[Cfg.MAX_VAL_INDEX]; // [SC] each list contains player's tiles of the same color
            List<DropEmTile>[] shapeTileLists = new List<DropEmTile>[Cfg.MAX_VAL_INDEX]; // [SC] each list contains player's tiles of the same shape

            // [SC] identifying unique color and shape values in player's tile array
            // [SC] and sorting those tiles into separate lists according to same color or shape value
            foreach (DropEmTile tile in playerTiles)
            {
                if (colorTileLists[tile.ColorIndex] == null) colorTileLists[tile.ColorIndex] = new List<DropEmTile>();
                if (shapeTileLists[tile.ShapeIndex] == null) shapeTileLists[tile.ShapeIndex] = new List<DropEmTile>();

                colorTileLists[tile.ColorIndex].Add(tile);
                shapeTileLists[tile.ShapeIndex].Add(tile);
            }

            if (considerColor)
            {
                for (int colorIndex = 0; colorIndex < colorTileLists.Length; colorIndex++)
                {
                    if (colorTileLists[colorIndex] != null && colorTileLists[colorIndex].Count > 0)
                    {
                        List<DropEmTile> tileList = colorTileLists[colorIndex]; // [SC] this list contains tiles of with one particular color

                        TreeNode rootNode = new TreeNode(null);
                        for (int tileIndex = 0; tileIndex < tileList.Count; tileIndex++)
                            tileListPermAddChildNodes(tileList, tileIndex, rootNode);

                        tileListPermTraverseTreePaths(rootNode, new List<DropEmTile>(), colorIndex, Cfg.COLOR_ATTR, candTileSeqList);
                    }
                }
            }

            if (considerShape)
            {
                for (int shapeIndex = 0; shapeIndex < shapeTileLists.Length; shapeIndex++)
                {
                    if (shapeTileLists[shapeIndex] != null && shapeTileLists[shapeIndex].Count > 0)
                    {
                        List<DropEmTile> tileList = shapeTileLists[shapeIndex]; // [SC] this list contains tiles of with one particular shape

                        TreeNode rootNode = new TreeNode(null);
                        for (int tileIndex = 0; tileIndex < tileList.Count; tileIndex++)
                            tileListPermAddChildNodes(tileList, tileIndex, rootNode);

                        tileListPermTraverseTreePaths(rootNode, new List<DropEmTile>(), shapeIndex, Cfg.SHAPE_ATTR, candTileSeqList);
                    }
                }
            }

            VirtualDropEmBoard virtualBoard = game.getVirtualBoard();
            DropEmTile[,] tileArray = virtualBoard.getBoardCopy();

            List<CandidateTilePos> chosenPosComboList = new List<CandidateTilePos>();
            List<CandidateTilePos> maxScorePosComboList = new List<CandidateTilePos>();

            foreach (CandidateTileSeq candTileSeq in candTileSeqList)
            {
                TreeNode rootNode = new TreeNode(null);

                boardPosPermAddChildNodes(candTileSeq, 0, rootNode, tileArray, virtualBoard);
                if (suboptiomal)
                    boardPosPermTraverseTreePaths(rootNode, new List<AbstractPos>(), candTileSeq, 0, chosenPosComboList, maxScorePosComboList);
                else
                    boardPosPermTraverseTreePaths(rootNode, new List<AbstractPos>(), candTileSeq, 0, chosenPosComboList);
            }

            return chosenPosComboList.getRandomElement();
        }

        /////////////////////////////////////////////////////////////////
        ////// START: A code for creating all possible permutations of board positions 
        ////// of tiles in given list, starting from left-most tile in the array

        /// <summary>
        /// Board position permission add child nodes.
        /// </summary>
        ///
        /// <param name="candTileSeq">   The cand tile sequence. </param>
        /// <param name="currTileIndex"> Zero-based index of the curr tile. </param>
        /// <param name="parentNode">    The parent node. </param>
        /// <param name="tileArray">     Array of tiles. </param>
        /// <param name="virtualBoard">  The virtual board. </param>
        private void boardPosPermAddChildNodes(CandidateTileSeq candTileSeq, int currTileIndex, TreeNode parentNode, DropEmTile[,] tileArray, VirtualDropEmBoard virtualBoard)
        {
            if (currTileIndex >= candTileSeq.TileCount)
            {
                return;
            }

            DropEmTile tile = candTileSeq[currTileIndex];

            for (int currRowIndex = 0; currRowIndex < tileArray.GetLength(0); currRowIndex++)
            {
                for (int currColIndex = 0; currColIndex < tileArray.GetLength(1); currColIndex++)
                {
                    int resultScore = virtualBoard.isValidMove(currRowIndex, currColIndex, tile, true, tileArray, false);

                    if (resultScore != Cfg.NONE)
                    {
                        DropEmTile[,] newTileArray = Cfg.createBoardCopy(tileArray);
                        virtualBoard.addTile(currRowIndex, currColIndex, tile, false, newTileArray);
                        TreeNode childNode = parentNode.addChildNodeValue(new AbstractPos(currTileIndex, currRowIndex, currColIndex, resultScore));
                        boardPosPermAddChildNodes(candTileSeq, currTileIndex + 1, childNode, newTileArray, virtualBoard);
                    }
                }
            }
        }

        /// <summary>
        /// Board position permission traverse tree paths.
        /// 
        ///  Gets the combo with the second highest score.
        /// </summary>
        ///
        /// <param name="rootNode">             The root node. </param>
        /// <param name="currPath">             Full pathname of the curr file. </param>
        /// <param name="candTileSeq">          The cand tile sequence. </param>
        /// <param name="currScore">            The curr score. </param>
        /// <param name="maxScorePosComboList"> List of maximum score position
        ///                                     comboes. </param>
        private void boardPosPermTraverseTreePaths(TreeNode rootNode, List<AbstractPos> currPath, CandidateTileSeq candTileSeq, int currScore, List<CandidateTilePos> maxScorePosComboList)
        {
            if (rootNode.hasChildNodes())
            {
                List<TreeNode> childNodes = rootNode.getChildNodes();
                foreach (TreeNode childNode in childNodes)
                {
                    List<AbstractPos> newPath = currPath.listShallowClone();
                    AbstractPos pos = (AbstractPos)childNode.getValue();
                    newPath.Add(pos);
                    boardPosPermTraverseTreePaths(childNode, newPath, candTileSeq, currScore + pos.getScore(), maxScorePosComboList);
                }
            }
            else
            { // [SC] reached the final leaf; no more moves in the combo
                if (currScore > 0)
                {
                    CandidateTilePos newCandTilePos = new CandidateTilePos(candTileSeq, currPath, currScore);

                    if (maxScorePosComboList.Count == 0)
                    {
                        maxScorePosComboList.Add(newCandTilePos);
                    }
                    else if (maxScorePosComboList[0].TotalScore == currScore)
                    {
                        maxScorePosComboList.Add(newCandTilePos);
                    }
                    else if (maxScorePosComboList[0].TotalScore < currScore)
                    {
                        maxScorePosComboList.Clear();
                        maxScorePosComboList.Add(newCandTilePos);
                    }
                }
            }
        }

        ////// END: A code for creating all possible permutations of board positions 
        ////// of tiles in given list, starting from left-most tile in the array
        /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Tile list permission add child nodes.
        /// </summary>
        ///
        /// <param name="tileList">      List of tiles. </param>
        /// <param name="childValIndex"> Zero-based index of the child value. </param>
        /// <param name="rootNode">      The root node. </param>
        private void tileListPermAddChildNodes(List<DropEmTile> tileList, int childValIndex, TreeNode rootNode)
        {
            List<DropEmTile> newList = tileList.listShallowClone();
            DropEmTile childNodeValue = newList.Pop(childValIndex);
            TreeNode childNode = rootNode.addChildNodeValue(childNodeValue);

            for (int tileIndex = 0; tileIndex < newList.Count; tileIndex++)
                tileListPermAddChildNodes(newList, tileIndex, childNode);
        }

        /// <summary>
        /// Tile list permission traverse tree paths.
        /// </summary>
        ///
        /// <param name="rootNode">        The root node. </param>
        /// <param name="currPath">        Full pathname of the curr file. </param>
        /// <param name="attrValueIndex">   Zero-based index of the attribute value. </param>
        /// <param name="attrIndex">       Zero-based index of the attribute. </param>
        /// <param name="candTileSeqList"> List of cand tile sequences. </param>
        private void tileListPermTraverseTreePaths(TreeNode rootNode, List<DropEmTile> currPath, int attrValueIndex, int attrIndex, List<CandidateTileSeq> candTileSeqList)
        {
            if (rootNode.hasChildNodes())
            {
                List<TreeNode> childNodes = rootNode.getChildNodes();
                foreach (TreeNode childNode in childNodes)
                {
                    List<DropEmTile> newPath = currPath.listShallowClone();
                    newPath.Add((DropEmTile)childNode.getValue());
                    tileListPermTraverseTreePaths(childNode, newPath, attrValueIndex, attrIndex, candTileSeqList);
                }
            }
            else
            {
                candTileSeqList.Add(new CandidateTileSeq(currPath));
            }
        }

        ////// END: A code for creating all possible permutations of tiles in list
        /////////////////////////////////////////////////////////////////

        #endregion very hard AI

        ////// END: very hard ai functionality
        /////////////////////////////////////////////////////////////////
    }
}
