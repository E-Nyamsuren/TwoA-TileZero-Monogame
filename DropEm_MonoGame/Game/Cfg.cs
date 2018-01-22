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
Filename: Cfg.cs
Description:
    Contains configuration settings as constants, static variables and static utility methods for the DropEm game.
*/

#endregion Header

//using System.Windows.Controls; // [SC] For using Image class.
//using System.Windows.Media.Imaging; // BitmapImage
namespace DropEm.Game
{
    using System;
    using System.Collections.Generic;
    using Swiss;

    /// <summary>
    /// A configuration.
    /// </summary>
    public static class Cfg
    {
        #region Fields

        //public const long ACTION_DELAY = 2000;

        public const int BOARD_COL_COUNT = 12;
        public const int BOARD_ROW_COUNT = 12;
        public const int COLOR_ATTR = 1;

        // TODO veg - turn into Enum with Descriptions.
        public enum Direction
        {
            HORIZONTAL = 1,
            VERTICAL = 2
        }

        public const string ADAPTER_ID = "Game difficulty - Player skill";

        /// <summary>
        /// Identifier for the game.
        /// </summary>
        public const string GAME_ID = "DropEm";

        // TODO veg - turn into Enum with Descriptions.
        public const string HUMAN_PLAYER = "Human";
        public const int LAST_PLAYER_REWARD = 6;

        public const int MAX_PLAYER_TILE_COUNT = 6;
        public const int MAX_SEQ_SCORE = 6;

        /// <summary>
        /// There can be three tiles of the same color:shape combination.
        /// </summary>
        public const int MAX_TILE_ID = 3;

        /// <summary>
        /// The the numeric id for color and shape features ranges from 0 to 5.
        /// </summary>
        public const int MAX_VAL_INDEX = 6;

        public const string EASY_AI = "Easy AI";
        public const string HARD_AI = "Hard AI";
        public const string MEDIUM_COLOR_AI = "Medium Color AI";
        public const string MEDIUM_SHAPE_AI = "Medium Shape AI";
        public const string VERY_EASY_AI = "Very Easy AI";
        public const string VERY_HARD_AI = "Very Hard AI";

        public const int NONE = -1;
        public const int SHAPE_ATTR = 2;
        public const int START_TILE_COUNT = 3;

        public const int GAME_REWARD = MAX_SEQ_SCORE * 2;

        public const string TILE_IMAGE_SUFFIX = "_15_50.jpg";

        public const long TURN_DURATION = 10000;

        /// <summary>
        /// The messages.
        /// </summary>
        public static List<string> Messages = new List<string>();

        /// <summary>
        /// The random number generator.
        /// </summary>
        private static Random rng = new Random();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Clears the messages.
        /// </summary>
        public static void clearMsgs()
        {
            Messages.Clear();
        }

        /// <summary>
        /// Create a shallow clone of the 2D array.
        /// </summary>
        ///
        /// <param name="tileArray"> Array of tiles. </param>
        ///
        /// <returns>
        /// The new board copy.
        /// </returns>
        public static DropEmTile[,] createBoardCopy(DropEmTile[,] tileArray)
        {
            if (tileArray == null) return null;

            int rowCount = tileArray.GetLength(0);
            int colCount = tileArray.GetLength(1);

            DropEmTile[,] newTileArray = new DropEmTile[rowCount, colCount];

            for (int currRowIndex = 0; currRowIndex < rowCount; currRowIndex++)
            {
                for (int currColIndex = 0; currColIndex < colCount; currColIndex++)
                {
                    newTileArray[currRowIndex, currColIndex] = tileArray[currRowIndex, currColIndex];
                }
            }

            return newTileArray;
        }

        /// <summary>
        /// An IList&lt;T&gt; extension method that gets random element.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="list"> The list to act on. </param>
        ///
        /// <returns>
        /// The random element.
        /// </returns>
        public static T getRandomElement<T>(this IList<T> list)
        {
            int n = list.Count;
            if (n == 1) return list[0];
            else if (n > 1) return list[rng.Next(n)];
            else return default(T);
        }

        /// <summary>
        /// Gets tile feature identifier.
        /// </summary>
        ///
        /// <param name="colorIndex"> Zero-based index of the color. </param>
        /// <param name="shapeIndex"> Zero-based index of the shape. </param>
        ///
        /// <returns>
        /// The tile feature identifier.
        /// </returns>
        public static string getTileFeatureID(int colorIndex, int shapeIndex)
        {
            return string.Format("{0}{1}", colorIndex, shapeIndex);
        }

        /// <summary>
        /// An IList&lt;T&gt; extension method that lists shallow clone.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="list"> The list to act on. </param>
        ///
        /// <returns>
        /// A List&lt;T&gt;
        /// </returns>
        public static List<T> listShallowClone<T>(this IList<T> list)
        {
            List<T> cloneList = new List<T>();
            foreach (T listItem in list)
            {
                cloneList.Add(listItem);
            }
            return cloneList;
        }

        /// <summary>
        /// An IList&lt;T&gt; extension method that removes and returns the top-of-
        /// stack object.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="list">  The list to act on. </param>
        /// <param name="index"> Zero-based index of the. </param>
        ///
        /// <returns>
        /// The previous top-of-stack object.
        /// </returns>
        public static T Pop<T>(this IList<T> list, int index)
        {
            if (list.Count > index)
            {
                T elem = list[index];
                list.RemoveAt(index);
                return elem;
            }
            return default(T);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        ///
        /// <param name="msg"> The message. </param>
        public static void showMsg(string msg)
        {
            ConsoleDialog.Write("{0} - {1}\r\n", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"), msg);

            Messages.Insert(0, msg);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        ///
        /// <param name="format"> Describes the format to use. </param>
        /// <param name="arg">      A variable-length parameters list containing
        ///                         argument. </param>
        public static void showMsg(string format, params object[] arg)
        {
            Cfg.showMsg(String.Format(format, arg));
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="list"> The list to act on. </param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion Methods
    }
}