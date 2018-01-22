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
Filename: VirtualDropEmBoard.cs
Description:
    It is a logical implementation of a DropEm board. Belongs to the Model component.
*/

#endregion Header

namespace DropEm.Game
{
    /// <summary>
    /// A virtual drop em board.
    /// </summary>
    public class VirtualDropEmBoard
    {
        #region Fields

        /// <summary>
        /// Array of tiles.
        /// </summary>
        private DropEmTile[,] tileArray;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DropEm.Game.VirtualDropEmBoard class.
        /// </summary>
        public VirtualDropEmBoard()
        {
            resetBoard();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the number of cols.
        /// </summary>
        ///
        /// <value>
        /// The number of cols.
        /// </value>
        public int colCount
        {
            get
            {
                return Cfg.BOARD_COL_COUNT;
            }
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        ///
        /// <value>
        /// The number of rows.
        /// </value>
        public int rowCount
        {
            get
            {
                return Cfg.BOARD_ROW_COUNT;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        ///
        /// <param name="r"> The int to process. </param>
        /// <param name="c"> The int to process. </param>
        ///
        /// <returns>
        /// The indexed item.
        /// </returns>
        public DropEmTile this[int r, int c]
        {
            get
            {
                return tileArray[r, c];
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tile">       The tile. </param>
        /// <param name="validCheck"> true to valid check. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// An int.
        /// </returns>
        public int addTile(int rowIndex, int colIndex, DropEmTile tile, bool validCheck, DropEmTile[,] tileArrayP)
        {
            if (tileArrayP == null)
            {
                tileArrayP = tileArray;
            }

            int result = isValidMove(rowIndex, colIndex, tile, validCheck, tileArrayP, true);

            if (result != Cfg.NONE)
            {
                tileArrayP[rowIndex, colIndex] = tile;
            }

            return result;
        }

        /// <summary>
        /// Gets board copy.
        /// </summary>
        ///
        /// <returns>
        /// The board copy.
        /// </returns>
        public DropEmTile[,] getBoardCopy()
        {
            return Cfg.createBoardCopy(tileArray);
        }

        /// <summary>
        /// Is valid move.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tile">       The tile. </param>
        /// <param name="validCheck"> true to valid check. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        /// <param name="showMsg">      true to show, false to hide the message. </param>
        ///
        /// <returns>
        /// An int.
        /// </returns>
        public int isValidMove(int rowIndex, int colIndex, DropEmTile tile, bool validCheck, DropEmTile[,] tileArrayP, bool showMsg)
        {
            if (tileArrayP == null)
            {
                tileArrayP = tileArray;
            }

            int horizScore = 0;
            int vertScore = 0;

            if (rowIndex < 0 || rowIndex >= rowCount)
            {
                if (showMsg) Cfg.showMsg("Invalid row index: " + rowIndex + ".");
                return Cfg.NONE;
            }

            if (colIndex < 0 || colIndex >= colCount)
            {
                if (showMsg) Cfg.showMsg("Invalid column index: " + colIndex + ".");
                return Cfg.NONE;
            }

            if (hasTile(rowIndex, colIndex, tileArrayP))
            {
                if (showMsg) Cfg.showMsg("The cell already has a tile.");
                return Cfg.NONE;
            }

            if (validCheck)
            {

                // [SC] check if there is any tile adjacent to the destinatio position
                if (!hasLeftTile(rowIndex, colIndex, tileArrayP) && !hasRightTile(rowIndex, colIndex, tileArrayP)
                    && !hasBottomTile(rowIndex, colIndex, tileArrayP) && !hasTopTile(rowIndex, colIndex, tileArrayP)
                    )
                {
                    if (showMsg) Cfg.showMsg("A new tile should be placed next to the existing one.");
                    return Cfg.NONE;
                }

                // [SC] temporarily put the tile
                tileArrayP[rowIndex, colIndex] = tile;

                // [SC] check validity of the horizontal sequence of tiles
                if (hasLeftTile(rowIndex, colIndex, tileArrayP) || hasRightTile(rowIndex, colIndex, tileArrayP))
                {
                    horizScore = isValidSequence(rowIndex, colIndex, Cfg.Direction.HORIZONTAL, tileArrayP, showMsg);
                    if (horizScore == Cfg.NONE)
                    {
                        tileArrayP[rowIndex, colIndex] = null;
                        return Cfg.NONE;
                    }
                    else if (horizScore == Cfg.MAX_SEQ_SCORE)
                    {
                        // [SC] reward for completing a DropEm Game
                        horizScore = Cfg.GAME_REWARD;
                    }
                }

                // [SC] check validity of the vertical sequence of tiles
                if (hasTopTile(rowIndex, colIndex, tileArrayP) || hasBottomTile(rowIndex, colIndex, tileArrayP))
                {
                    vertScore = isValidSequence(rowIndex, colIndex, Cfg.Direction.VERTICAL, tileArrayP, showMsg);
                    if (vertScore == Cfg.NONE)
                    {
                        tileArrayP[rowIndex, colIndex] = null;
                        return Cfg.NONE;
                    }
                    else if (vertScore == Cfg.MAX_SEQ_SCORE)
                    {
                        // [SC] reward for completing a DropEm game
                        vertScore = Cfg.GAME_REWARD;
                    }
                }

                // [SC] remove the temporary tile
                tileArrayP[rowIndex, colIndex] = null;
            }

            return horizScore + vertScore;
        }

        /// <summary>
        /// Resets the board.
        /// </summary>
        public void resetBoard()
        {
            tileArray = new DropEmTile[rowCount, colCount];
        }

        /// <summary>
        /// Query if 'rowIndex' has bottom tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// returns true if the bottom cell adjacent to the indicated cell has a tile
        /// </returns>
        private bool hasBottomTile(int rowIndex, int colIndex, DropEmTile[,] tileArrayP)
        {
            return (rowIndex < (rowCount - 1) && tileArrayP[rowIndex + 1, colIndex] != null);
        }

        /// <summary>
        /// Query if 'rowIndex' has left tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// returns true if the left cell adjacent to the indicated cell has a tile
        /// </returns>
        private bool hasLeftTile(int rowIndex, int colIndex, DropEmTile[,] tileArrayP)
        {
            return (colIndex > 0 && tileArrayP[rowIndex, colIndex - 1] != null);
        }

        /// <summary>
        /// Query if 'rowIndex' has right tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// returns true if the right cell adjacent to the indicated cell has a tile
        /// </returns>
        private bool hasRightTile(int rowIndex, int colIndex, DropEmTile[,] tileArrayP)
        {
            return (colIndex < (colCount - 1) && tileArrayP[rowIndex, colIndex + 1] != null);
        }

        /// <summary>
        /// Query if 'rowIndex' has tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// returns true if the indicated cell has a tile
        /// </returns>
        private bool hasTile(int rowIndex, int colIndex, DropEmTile[,] tileArrayP)
        {
            return (tileArrayP[rowIndex, colIndex] != null);
        }

        /// <summary>
        /// Query if 'rowIndex' has top tile.
        /// </summary>
        ///
        /// <param name="rowIndex">   Zero-based index of the row. </param>
        /// <param name="colIndex">   Zero-based index of the col. </param>
        /// <param name="tileArrayP"> The tile array p. </param>
        ///
        /// <returns>
        /// returns true if the top cell adjacent to the indicated cell has a tile
        /// </returns>
        private bool hasTopTile(int rowIndex, int colIndex, DropEmTile[,] tileArrayP)
        {
            return (rowIndex > 0 && tileArrayP[rowIndex - 1, colIndex] != null);
        }

        /// <summary>
        /// Is valid sequence.
        /// </summary>
        ///
        /// <param name="rowIndex">    Zero-based index of the row. </param>
        /// <param name="colIndex">    Zero-based index of the col. </param>
        /// <param name="orientation"> The orientation. </param>
        /// <param name="tileArrayP">  The tile array p. </param>
        /// <param name="showMsg">      true to show, false to hide the message. </param>
        ///
        /// <returns>
        /// An int.
        /// </returns>
        private int isValidSequence(int rowIndex, int colIndex, Cfg.Direction orientation, DropEmTile[,] tileArrayP, bool showMsg)
        {
            int[] uniqueColors = new int[Cfg.MAX_VAL_INDEX];
            int uniqueColorCount = 0;

            int[] uniqueShapes = new int[Cfg.MAX_VAL_INDEX];
            int uniqueShapeCount = 0;

            int sequenceLength = 0;

            int currRow = rowIndex;
            int currCol = colIndex;

            for (int currIndex = 0; currIndex < Cfg.MAX_VAL_INDEX; currIndex++)
            {
                uniqueColors[currIndex] = Cfg.NONE;
                uniqueShapes[currIndex] = Cfg.NONE;
            }

            // [SC] start with the left-most or top-most tile in the sequence
            switch (orientation)
            {
                case Cfg.Direction.HORIZONTAL:
                    while (currCol > 0 && tileArrayP[currRow, currCol - 1] != null)
                    {
                        currCol--;
                    }
                    break;

                case Cfg.Direction.VERTICAL:
                    while (currRow > 0 && tileArrayP[currRow - 1, currCol] != null)
                    {
                        currRow--;
                    }
                    break;
            }

            // [SC] checking the validity of colors and shapes, and color-shape combination of the sequence
            while (currRow < rowCount && currCol < colCount)
            {
                DropEmTile currTile = tileArrayP[currRow, currCol];

                if (currTile == null)
                    break;

                // [SC] checking the validity of colors
                int currColorIndex = currTile.ColorIndex;
                if (uniqueColors[currColorIndex] == Cfg.NONE)
                {
                    uniqueColors[currColorIndex] = currColorIndex;
                    uniqueColorCount++;
                }
                else if (uniqueColorCount == 1)
                {
                }
                else
                {
                    if (showMsg) Cfg.showMsg("Invalid color sequence.");
                    return Cfg.NONE;
                }

                // [SC] checking the validity of shapes
                int currShapeIndex = currTile.ShapeIndex;
                if (uniqueShapes[currShapeIndex] == Cfg.NONE)
                {
                    uniqueShapes[currShapeIndex] = currShapeIndex;
                    uniqueShapeCount++;
                }
                else if (uniqueShapeCount == 1)
                {
                }
                else
                {
                    if (showMsg) Cfg.showMsg("Invalid shape sequence.");
                    return Cfg.NONE;
                }

                sequenceLength++;

                if (sequenceLength > 1)
                {
                    if ((uniqueColorCount == 1 && uniqueShapeCount == 1) || // [SC] both shape and color are same
                        (uniqueColorCount > 1 && uniqueShapeCount > 1) // both shape and color are different
                        )
                    {
                        if (showMsg) Cfg.showMsg("Invalid combination of color and shape.");
                        return Cfg.NONE;
                    }
                }

                // [TODO] update row
                switch (orientation)
                {
                    case Cfg.Direction.HORIZONTAL:
                        currCol++;
                        break;
                    case Cfg.Direction.VERTICAL:
                        currRow++;
                        break;
                }
            }

            return sequenceLength;
        }

        #endregion Methods
    }
}