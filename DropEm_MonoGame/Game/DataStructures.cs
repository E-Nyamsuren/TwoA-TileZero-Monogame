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
Filename: DataStructure.cs
Description:
    Defines a set of abstract data structures used by the DropEm game.
*/
#endregion Header

namespace DropEm.Game
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An abstract position.
    /// </summary>
    public class AbstractPos
    {
        #region Fields

        private int colIndex;
        private int rowIndex;
        private int score;
        private int tileIndex;

        #endregion Fields

        #region Constructors

        public AbstractPos(int tileIndex, int rowIndex, int colIndex, int score)
        {
            this.tileIndex = tileIndex;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
            this.score = score;
        }

        #endregion Constructors

        #region Methods

        public int getColIndex()
        {
            return colIndex;
        }

        public int getRowIndex()
        {
            return rowIndex;
        }

        public int getScore()
        {
            return score;
        }

        public int getTileIndex()
        {
            return tileIndex;
        }

        #endregion Methods
    }

    /// <summary>
    /// An abstract tile.
    /// </summary>
    public class AbstractTile
    {
        #region Fields

        public int colorIndex;
        public bool playableFlag;
        public int shapeIndex;
        public int tileID;

        #endregion Fields

        #region Constructors

        public AbstractTile(int colorIndex, int shapeIndex, int tileID, bool playableFlag)
        {
            this.colorIndex = colorIndex;
            this.shapeIndex = shapeIndex;
            this.tileID = tileID;
            this.playableFlag = playableFlag;
        }

        #endregion Constructors
    }

    /// <summary>
    /// [SC] the same sequence of tiles can be placed on board with different
    /// possible combinations of tile positions [SC] object of this class
    /// represents one possible combination of tile positions.
    /// </summary>
    public class CandidateTilePos
    {
        #region Fields

        private CandidateTileSeq candTileSequence; // [SC] a reference to parent CandidateTileSeq object that contains this objects
        private List<AbstractPos> posList; // [SC] a list of lists where child list contains: (arrayIndex of the tile, row position on board, col position on board, score if tile is placed)

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// DropEm.Game.DataStructure.CandidateTilePos class.
        /// </summary>
        ///
        /// <param name="candTileSequence"> The candidate tile sequence. </param>
        /// <param name="posList">          List of positions. </param>
        /// <param name="totalScore">       The total number of score. </param>
        public CandidateTilePos(CandidateTileSeq candTileSequence, List<AbstractPos> posList, int totalScore)
        {
            this.candTileSequence = candTileSequence;
            this.posList = posList;
            this.TotalScore = totalScore;
            candTileSequence.addCandTilePos(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the total number of score.
        /// </summary>
        ///
        /// <value>
        /// The total number of score.
        /// </value>
        public int TotalScore
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets abstr position at.
        /// </summary>
        ///
        /// <param name="index"> Zero-based index of the. </param>
        ///
        /// <returns>
        /// The abstr position at.
        /// </returns>
        public AbstractPos getAbstrPosAt(int index)
        {
            //if (posList != null && posList.Count > index)
            return posList[index];
        }

        /// <summary>
        /// Gets candidate tile sequence.
        /// </summary>
        ///
        /// <returns>
        /// The candidate tile sequence.
        /// </returns>
        public CandidateTileSeq getCandidateTileSeq()
        {
            return candTileSequence;
        }

        /// <summary>
        /// Gets combo length.
        /// </summary>
        ///
        /// <returns>
        /// The combo length.
        /// </returns>
        public int getComboLength()
        {
            return posList.Count;
        }

        #endregion Methods
    }

    /// <summary>
    /// The same list of tiles can be placed on a board in different possible
    /// orders (tile sequences).
    /// 
    /// Object of this class represents one possible sequence of tiles.
    /// </summary>
    public class CandidateTileSeq
    {
        #region Fields

        private List<CandidateTilePos> candTilePosList; // [SC] a list of CandidateTilePos objects
        private List<DropEmTile> tileSequence; // [SC] ordered sequence of tile objects

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// DropEm.Game.DataStructure.CandidateTileSeq class.
        /// </summary>
        ///
        /// <param name="tileSequence"> The tile sequence. </param>
        public CandidateTileSeq(List<DropEmTile> tileSequence)
        {
            this.tileSequence = tileSequence;

            candTilePosList = new List<CandidateTilePos>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets tile count.
        /// </summary>
        ///
        /// <returns>
        /// The tile count.
        /// </returns>
        public int TileCount
        {
            get
            {
                return tileSequence.Count; // [SC][TODO] for now let it crash if tileSequence == null; but it should not happen
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        ///
        /// <param name="tileIndex"> Zero-based index of the tile. </param>
        ///
        /// <returns>
        /// The indexed item.
        /// </returns>
        public DropEmTile this[int tileIndex]
        {
            //[TODO] if (tileSequence != null && tileSequence.Count > tileIndex)
            get
            {
                return tileSequence[tileIndex];
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds a cand tile position.
        /// </summary>
        ///
        /// <param name="candTilePos"> The cand tile position. </param>
        public void addCandTilePos(CandidateTilePos candTilePos)
        {
            candTilePosList.Add(candTilePos);
        }

        /// <summary>
        /// Gets position combo list.
        /// </summary>
        ///
        /// <returns>
        /// The position combo list.
        /// </returns>
        public List<CandidateTilePos> getPosComboList()
        {
            return candTilePosList;
        }

        #endregion Methods
    }

    /// <summary>
    /// A tree node.
    /// </summary>
    public class TreeNode
    {
        #region Fields

        private List<TreeNode> childNodes;
        private Object value;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the TreeNode class.
        /// </summary>
        ///
        /// <param name="value"> The value. </param>
        public TreeNode(Object value)
        {
            childNodes = new List<TreeNode>();
            this.value = value;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds a childe node.
        /// </summary>
        ///
        /// <param name="childNode"> The child node. </param>
        public void addChildeNode(TreeNode childNode)
        {
            childNodes.Add(childNode);
        }

        /// <summary>
        /// Adds a child node value.
        /// </summary>
        ///
        /// <param name="value"> The value. </param>
        ///
        /// <returns>
        /// A TreeNode.
        /// </returns>
        public TreeNode addChildNodeValue(Object value)
        {
            TreeNode childNode = new TreeNode(value);

            addChildeNode(childNode);

            return childNode;
        }

        /// <summary>
        /// Gets a child nodes.
        /// </summary>
        ///
        /// <returns>
        /// The child nodes.
        /// </returns>
        public List<TreeNode> getChildNodes()
        {
            return childNodes;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        ///
        /// <returns>
        /// The value.
        /// </returns>
        public Object getValue()
        {
            return value;
        }

        /// <summary>
        /// Query if this object has child nodes.
        /// </summary>
        ///
        /// <returns>
        /// true if child nodes, false if not.
        /// </returns>
        public bool hasChildNodes()
        {
            return (childNodes != null && childNodes.Count > 0);
        }

        #endregion Methods
    }
}