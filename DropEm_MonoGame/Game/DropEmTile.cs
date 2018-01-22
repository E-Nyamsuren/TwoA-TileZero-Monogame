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
Filename: DropEmTile.cs
Description:
    Defines a class that represents a DropEm tile.
*/

#endregion Header

namespace DropEm.Game
{
    using System;

    public class DropEmTile
    {
        #region Constructors

        /// <summary>
        /// Values that represent tile colors.
        /// </summary>
        public enum TileColors
        {
            Blue = 0,
            Red = 1,
            Yellow = 2,
            Green = 3,
            Purple = 4,
            Orange = 5
        }

        /// <summary>
        /// Values that represent tile shapes.
        /// </summary>
        public enum TileShapes
        {
            Circle = 0,
            Square = 1,
            Diamond = 2,
            Star = 3,
            Plus = 4,
            Cross = 5
        }

        /// <summary>
        /// Initializes a new instance of the DropEm.Game.DropEmTile
        /// class.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        /// <param name="tileID">     Identifier for the tile. </param>
        public DropEmTile(int colorIndex, int shapeIndex, int tileID)
        {
            this.ColorIndex = colorIndex;
            this.ShapeIndex = shapeIndex;
            this.TileID = tileID;

            resetTile();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the can drop.
        /// </summary>
        ///
        /// <value>
        /// .
        /// </value>
        public bool CanDrop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the zero-based index of the color.
        /// </summary>
        ///
        /// <value>
        /// The color index.
        /// </value>
        public int ColorIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the playable.
        /// </summary>
        ///
        /// <value>
        /// .
        /// </value>
        public bool Playable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the zero-based index of the shape.
        /// </summary>
        ///
        /// <value>
        /// The shape index.
        /// </value>
        public int ShapeIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the identifier of the tile.
        /// </summary>
        ///
        /// <value>
        /// The identifier of the tile.
        /// </value>
        public int TileID
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets the tile.
        /// </summary>
        public void resetTile()
        {
            Playable = true;
            CanDrop = true;
        }

        /// <summary>
        /// Same tile.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        /// <param name="tileID">     Identifier for the tile. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool sameTile(int colorIndex, int shapeIndex, int tileID)
        {
            return (
                this.ColorIndex == colorIndex &&
                this.ShapeIndex == shapeIndex &&
                this.TileID == tileID);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        ///
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return String.Format("[{0}] {1} {2}", TileID, (TileColors)ColorIndex, (TileShapes)ShapeIndex);
        }

        #endregion Methods
    }
}