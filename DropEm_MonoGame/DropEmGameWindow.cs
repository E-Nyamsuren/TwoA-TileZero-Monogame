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

Namespace: MonoGame1
Filename: DropEmGameWindow.cs
Description:
    Defines the main MonoGame Window.
*/
#endregion Header

namespace MonoGame1
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using C3.XNA;

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using DropEm.Game;
    using Swiss;

    using AssetManagerPackage;
    using AssetPackage;

    using HAT;

    /*
        // See http://www.foxdemon.com/monogame-windows-opengl-hello-world-application-with-visualstudio-2012-and-xamarin-studio/
        // 
        // See http://rbwhitaker.wikidot.com/monogame-tutorials
        // See http://rbwhitaker.wikidot.com/monogame-managing-content
        // See http://rbwhitaker.wikidot.com/monogame-drawing-text-with-spritefonts
        //
        // See http://www.monogame.net/documentation/?page=Using_The_Pipeline_Tool
        // 
        // See https://bitbucket.org/C3/2d-xna-primitives/downloads
        // 
        // See http://stackoverflow.com/questions/12914002/how-to-load-all-files-in-a-folder-with-xna
    */

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DropEmGameWindow : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        /// <summary>
        /// The frame counter.
        /// </summary>
        private FrameCounter frameCounter = new FrameCounter();

        /// <summary>
        /// State of the previous.
        /// </summary>
        private ButtonState previousState = ButtonState.Released;

        private Texture2D cursor;

        private Texture2D cell;

        private DropEmGame dropemGame;
        private VirtualDropEmBoard board;

        // TODO veg bind some of following properties to Game/Player (ie selCol, selRow and selTile)(?

        /// <summary>
        /// The Board width in cells.
        /// </summary>
        const int cols = Cfg.BOARD_COL_COUNT;

        /// <summary>
        /// The Board height in cells.
        /// </summary>
        const int rows = Cfg.BOARD_ROW_COUNT;

        /// <summary>
        /// The board offset from the left.
        /// </summary>
        const int marginLeft = 10;

        /// <summary>
        /// The board offset from the top.
        /// </summary>
        const int marginTop = 10;

        /// <summary>
        /// The width of a cell.
        /// </summary>
        const int cellw = 40;

        /// <summary>
        /// The height of a cell.
        /// </summary>
        const int cellh = 40;

        /// <summary>
        /// The X position of the mouse cursor.
        /// </summary>
        private int mx = 0;

        /// <summary>
        /// The Y position of the mouse cursor.
        /// </summary>
        int my = 0;

        /// <summary>
        /// The column number under the cursor.
        /// </summary>
        private int col = -1;

        /// <summary>
        /// The row number under the cursor.
        /// </summary>
        private int row = -1;

        /// <summary>
        /// The last clicked/selected column number.
        /// </summary>
        private int selCol = -1;

        /// <summary>
        /// The last clicked/selected row number.
        /// </summary>
        private int selRow = -1;

        /// <summary>
        /// The selected tile.
        /// </summary>
        private int selTile;

        /// <summary>
        /// The tile.
        /// </summary>
        private int tile;

        /// <summary>
        /// The tile suffix.
        /// </summary>
        string tilesuffix = Path.GetFileNameWithoutExtension(Cfg.TILE_IMAGE_SUFFIX);

        /// <summary>
        /// The glyphs.
        /// </summary>
        private Dictionary<string, Texture2D> glyphs;

        private HATAsset hatAsset;

        private string adaptID = Cfg.ADAPTER_ID;
        private string gameID = Cfg.GAME_ID;
        private string playerID = "Noob";

        private double wins = 0.0;

        /// <summary>
        /// Initializes a new instance of the MonoGame1.Game1 class.
        /// </summary>
        public DropEmGameWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = String.Format("{0} - A RAGE Game", Cfg.GAME_ID);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //? TODO: Add your initialization logic here
            // 
            graphics.IsFullScreen = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            ConsoleDialog.Show();
            ConsoleDialog.Caption = String.Format("{0} - Console", Cfg.GAME_ID);

            previousState = ButtonState.Released;


            //AssetManager.Instance.Bridge = new Bridge();
            //hatAsset = new HATAsset();

            hatAsset = new HATAsset(new Bridge());

            ConsoleDialog.WriteLine(String.Empty);
            ConsoleDialog.Write(AssetManager.Instance.VersionAndDependenciesReport);
            ConsoleDialog.WriteLine(String.Empty);

            dropemGame = new DropEmGame(hatAsset);

            NewGame();

            Cfg.showMsg("Board Size: {0}x{1}", board.colCount, board.rowCount);
            Cfg.showMsg("Available tiles: {0}", dropemGame.humanPlayer.PlayerTileCount);
            for (int i = 0; i < dropemGame.PlayerCount; i++)
            {
                Cfg.showMsg("Player {0}={1}", i, dropemGame.getPlayerNameByIndex(i));
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load all of
        /// your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //? TODO: use this.Content to load your game content here
            // 
            cell = Content.Load<Texture2D>("Cell");
            cursor = Content.Load<Texture2D>("Cursor");

            font = Content.Load<SpriteFont>("Score");

            glyphs = TextureContent.LoadListContent<Texture2D>(
                Content,
                string.Empty,
                string.Format("*{0}.xnb", tilesuffix));

            dropemGame.ActivePlayerIndex = dropemGame.humanPlayer.PlayerIndex;

            //btn_1 = new Button(this.GraphicsDevice, font, "CELL", new Vector2(24, 24), colour);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            //? TODO: Unload any non ContentManager content here

            // TODO veg - what to unload, look for examples.
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world, checking for
        /// collisions, gathering input, and playing audio.
        /// </summary>
        ///
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //? TODO: Add your update logic here
            // 
            MouseState mouseState = Mouse.GetState();

            mx = mouseState.Position.X;
            my = mouseState.Position.Y;

            //! 1 Cell Column Coordinate under the cursor.
            // 
            if (mx < marginLeft || mx > marginLeft + (cols * cellw))
            {
                col = -1;
            }
            else
            {
                col = (mx - marginLeft) / cellw;
            }

            //! 2 Cell Row Coordinate under the cursor.
            // 
            if (my < marginTop || my > marginTop + (rows * cellh))
            {
                row = -1;
            }
            else
            {
                row = (my - marginTop) / cellh;
            }

            //! 3 Tile under cursor
            // 
            tile = -1;
            if (mx >= marginLeft + (cols + 2) * cellw && mx <= marginLeft + (cols + 2) * cellw + cellw)
            {
                if (my >= marginTop && my <= marginTop + (dropemGame.humanPlayer.PlayerTileCount * cellh))
                {
                    tile = (my - marginTop) / cellh;

                    if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        selTile = tile;
                    }
                }
            }

            //! 4 Selected Cell Coordinates.
            // 
            if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed && row >= 0 && col >= 0)
            {
                selCol = col;
                selRow = row;
            }

            if (dropemGame.ActivePlayerIndex == dropemGame.humanPlayer.PlayerIndex)
            {
                //! 5 Detect 'Place Tile' Button and invoke game engine accordingly.
                // 
                if (mx >= marginLeft + (cols + 2) * cellw && mx <= marginLeft + (cols + 2) * cellw + 100)
                {
                    if (my >= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 1.1) * cellh && my <= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 1.1) * cellh + cellh)
                    {
                        if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            //! Perform Human Move (if cell & tile selected too).
                            // 
                            if (selTile != -1)
                            {
                                Cfg.clearMsgs();

                                DropEmTile selectedTile = dropemGame.humanPlayer.getTileAt(selTile);

                                dropemGame.setSelectedPlayerTile(selectedTile.ColorIndex, selectedTile.ShapeIndex, selectedTile.TileID);

                                if (selCol >= 0 && selRow >= 0)
                                {
                                    dropemGame.setSelectedCell(selRow, selCol, dropemGame.humanPlayer.PlayerIndex);

                                    // TODO veg - if selCol, selRow and selTile are linked to game/player resetSelected in placePlayerTileOnBoard should force the update.
                                    // 
                                    if (dropemGame.placePlayerTileOnBoard(dropemGame.humanPlayer.PlayerIndex))
                                    {
                                        selCol = -1;
                                        selRow = -1;
                                        selTile = -1;
                                    }
                                }
                            }
                            else
                            {
                                dropemGame.resetSelected();
                            }
                        }
                    }
                }

                //! 6 Detect 'Drop Tile' Button and invoke game engine accordingly.
                // 
                if (mx >= marginLeft + (cols + 2) * cellw && mx <= marginLeft + (cols + 2) * cellw + 100)
                {
                    if (my >= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 2.2) * cellh && my <= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 2.2) * cellh + cellh)
                    {
                        if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed && selTile >= 0)
                        {
                            DropEmTile selectedTile = dropemGame.humanPlayer.getTileAt(selTile);

                            dropemGame.setSelectedPlayerTile(selectedTile.ColorIndex, selectedTile.ShapeIndex, selectedTile.TileID);

                            dropemGame.dropHumanPlayerTile();

                            selCol = -1;
                            selRow = -1;
                            selTile = -1;

                            // TODO veg - Implement.

                            //Cfg.clearMsgs();
                        }
                    }
                }

                //! 7 Detect 'End Turn' Button and invoke game engine accordingly.
                // 
                if (mx >= marginLeft + (cols + 2) * cellw && mx <= marginLeft + (cols + 2) * cellw + 100)
                {
                    if (my >= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 3.3) * cellh && my <= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 3.3) * cellh + cellh)
                    {
                        if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            selCol = -1;
                            selRow = -1;
                            selTile = -1;

                            dropemGame.endHumanPlayerTurn();

                            Cfg.clearMsgs();
                        }
                    }
                }
            }

            if (dropemGame.ActivePlayerIndex == dropemGame.humanPlayer.PlayerIndex || dropemGame.EndGameFlag)
            {
                //! 8 Detect 'New Game' Button and invoke game engine accordingly.
                // 
                if (mx >= marginLeft + (cols + 2) * cellw && mx <= marginLeft + (cols + 2) * cellw + 100)
                {
                    if (my >= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 4.4) * cellh && my <= marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 4.4) * cellh + cellh)
                    {
                        if (mouseState.LeftButton != previousState && mouseState.LeftButton == ButtonState.Pressed)
                        {
                            selCol = -1;
                            selRow = -1;
                            selTile = -1;

                            NewGame();
                        }
                    }
                }
            }

            //! 9 Game mechanics (run the game).
            // 
            if (dropemGame.ActiveGameFlag)
            {
                if (dropemGame.StartTurnFlag)
                {
                    dropemGame.StartTurnFlag = false;
                    dropemGame.startTurn();
                }

                if (dropemGame.EndTurnFlag)
                {
                    dropemGame.EndTurnFlag = false;
                    dropemGame.EndGameFlag = dropemGame.endTurn();
                }

                if (dropemGame.EndGameFlag)
                {
                    EndGame();
                }
            }

            previousState = mouseState.LeftButton;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //? TODO: Add your drawing code here
            // 
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameCounter.Update(deltaTime);

            string fps = string.Format("FPS: {0:0.0}", frameCounter.AverageFramesPerSecond);

            spriteBatch.Begin();
            {

                for (int i = 0; i < Math.Min(Cfg.Messages.Count, 3); i++)
                {
                    spriteBatch.DrawString(font, Cfg.Messages[i], new Vector2(
                        graphics.PreferredBackBufferHeight / 2,
                        graphics.PreferredBackBufferHeight - (i + 1) * 20 - 20), Color.Black);
                }

                //! Draw player tiles
                // 
                for (int r = 0; r < dropemGame.humanPlayer.PlayerTileCount; r++)
                {
                    int l = marginLeft + (cols + 2) * cellw;
                    int t = marginTop + r * cellh;

                    DropEmTile tile2draw = dropemGame.humanPlayer.getTileAt(r);

                    string tileID = string.Format("{0}{1}",
                                Cfg.getTileFeatureID(
                                tile2draw.ColorIndex,
                                tile2draw.ShapeIndex),
                                tilesuffix);

                    spriteBatch.Draw(cell, new Vector2(l, t), Color.White);

                    Rectangle rc = new Rectangle(l + 3, t + 3, cellw - 6, cellh - 6);

                    spriteBatch.Draw(glyphs[tileID], rc, Color.White);

                    Rectangle ra = new Rectangle(l, t, cellw, cellh);

                    //! Draw selected Player tile.
                    // 
                    if (this.selTile == r)
                    {
                        Color selected = new Color(Color.Blue, 0.4f);

                        spriteBatch.FillRectangle(ra, selected);
                    }

                    //! Draw hover accent (if any).
                    //
                    if (this.tile == r)
                    {
                        Color hover = new Color(Color.Blue, 0.1f);

                        spriteBatch.FillRectangle(ra, hover);

                        spriteBatch.DrawString(font, tile2draw.ToString(), new Vector2(marginLeft + (cols + 2) * cellw, graphics.PreferredBackBufferHeight - 40), Color.Black);
                    }

                    //! Draw unplayable accent (if any).
                    // 
                    if (!tile2draw.Playable || dropemGame.ActivePlayerIndex != dropemGame.humanPlayer.PlayerIndex)
                    {
                        Color unplayable = new Color(Color.Gray, 0.4f);

                        spriteBatch.FillRectangle(ra, unplayable);
                    }
                }

                //! Draw board 15x15
                //! cell  40x40
                // 
                //! Draw Board.
                // 
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        int l = marginLeft + c * cellw;
                        int t = marginTop + r * cellh;

                        spriteBatch.Draw(cell, new Vector2(l, t), Color.White);

                        //! Draw a (demo) piece.
                        // 
                        if (board[r, c] != null)
                        {
                            string tileID = string.Format("{0}{1}",
                                Cfg.getTileFeatureID(
                                board[r, c].ColorIndex,
                                board[r, c].ShapeIndex),
                                tilesuffix);

                            Rectangle rec = new Rectangle(l + 3, t + 3, cellw - 6, cellh - 6);

                            spriteBatch.Draw(glyphs[tileID], rec, Color.White);
                        }
                    }
                }

                //! Draw selected cell (if any).
                // 
                if (selRow >= 0 && selCol >= 0)
                {
                    Rectangle r = new Rectangle(
                        marginLeft + selCol * cellw, marginTop + selRow * cellh,
                        cellw, cellh);
                    Color selected = new Color(Color.Blue, 0.4f);

                    spriteBatch.FillRectangle(r, selected);
                }

                //! Draw hover accent (if any).
                // 
                if (row >= 0 && col >= 0)
                {
                    Rectangle r = new Rectangle(
                        marginLeft + col * cellw, marginTop + row * cellh,
                        cellw, cellh);
                    Color hover = new Color(Color.Blue, 0.1f);

                    spriteBatch.FillRectangle(r, hover);
                }

                //! Draw selected cell coordinates.
                // 
                if (row >= 0 && col >= 0)
                {
                    string rc = string.Format("R{0}C{1}", row, col);
                    spriteBatch.DrawString(font, rc, new Vector2(marginLeft + (cols + 2) * cellw, graphics.PreferredBackBufferHeight - 20), Color.Black);
                }

                if (dropemGame.ActivePlayerIndex == dropemGame.humanPlayer.PlayerIndex)
                {

                    //! Draw 'Place Tile' button.
                    {
                        int l = marginLeft + (cols + 2) * cellw;
                        int t = (int)(marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 1) * cellh + cellh * 0.1);

                        Rectangle r = new Rectangle(l, t, 100, cellh);
                        Color hover = new Color(Color.Silver, 1f);
                        spriteBatch.FillRectangle(r, hover);

                        spriteBatch.DrawString(font, "Place Tile", new Vector2(l + (cellw / 2), t + (cellh / 3)), Color.Black);
                    }

                    //! Draw 'Drop Tile' button.
                    {
                        int l = marginLeft + (cols + 2) * cellw;
                        int t = (int)(marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 2) * cellh + cellh * 0.2);

                        Rectangle r = new Rectangle(l, t, 100, cellh);
                        Color hover = new Color(Color.Silver, 1f);
                        spriteBatch.FillRectangle(r, hover);

                        spriteBatch.DrawString(font, "Drop Tile", new Vector2(l + (cellw / 2), t + (cellh / 3)), Color.Black);
                    }

                    //! Draw 'End Turn' button.
                    {
                        int l = marginLeft + (cols + 2) * cellw;
                        int t = (int)(marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 3) * cellh + cellh * 0.3);

                        Rectangle r = new Rectangle(l, t, 100, cellh);
                        Color hover = new Color(Color.Silver, 1f);
                        spriteBatch.FillRectangle(r, hover);

                        spriteBatch.DrawString(font, "End Turn", new Vector2(l + (cellw / 2), t + (cellh / 3)), Color.Black);
                    }
                }

                if (dropemGame.ActivePlayerIndex == dropemGame.humanPlayer.PlayerIndex || dropemGame.EndGameFlag)
                {
                    //! Draw 'New Game' button.
                    {
                        int l = marginLeft + (cols + 2) * cellw;
                        int t = (int)(marginTop + (Cfg.MAX_PLAYER_TILE_COUNT + 4) * cellh + cellh * 0.4);

                        Rectangle r = new Rectangle(l, t, 100, cellh);
                        Color hover = new Color(Color.Silver, 1f);
                        spriteBatch.FillRectangle(r, hover);

                        spriteBatch.DrawString(font, "New Game", new Vector2(l + (cellw / 2), t + (cellh / 3)), Color.Black);
                    }
                }

                //! Draw Score
                // 
                spriteBatch.DrawString(font, "Score:", new Vector2(1, graphics.PreferredBackBufferHeight - (dropemGame.PlayerCount + 1) * 20 - 40), Color.Black);
                for (int i = 0; i < dropemGame.PlayerCount; i++)
                {
                    string name = dropemGame.getPlayerNameByIndex(i);
                    if (i == dropemGame.aiPlayer.PlayerIndex && (hatAsset.Settings as HATAssetSettings).Mode == HATAsset.HATMode.perTurn)
                    {
                        name = String.Format("{0} ({1})", name, dropemGame.aiPlayer.PlayerType);
                    }
                    string score = string.Format("{0}: {1}", name, dropemGame.getPlayerScoreByIndex(i));
                    spriteBatch.DrawString(font, score, new Vector2(20, graphics.PreferredBackBufferHeight - (i + 1) * 20 - 40), Color.Black);
                }

                {
                    string score = string.Format("Wins sofar: {0:0.0}%", wins);
                    spriteBatch.DrawString(font, score, new Vector2(20, graphics.PreferredBackBufferHeight - 40), Color.Black);
                }

                //! Draw fps counter.
                // 
                spriteBatch.DrawString(font, fps, new Vector2(1, graphics.PreferredBackBufferHeight - 20), Color.Black);

                //! Draw cursor last so it's always on top.
                // 
                spriteBatch.Draw(cursor, new Vector2(mx - 7, my), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Ends a game.
        /// </summary>
        private void EndGame()
        {
            dropemGame.endGame();

            Cfg.showMsg("Wins sofar: {0:0.0}%", hatAsset.CurrentAccuracy(adaptID, gameID, playerID) * 100);
        }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        private void NewGame()
        {
            dropemGame.ActiveGameFlag = false;
            dropemGame.StartTurnFlag = false;
            dropemGame.EndTurnFlag = false;
            dropemGame.EndGameFlag = false;

            string ScenarioID = hatAsset.TargetScenarioID(adaptID, gameID, playerID);

            string[] aiIDArray;
            if (string.IsNullOrEmpty(ScenarioID))
            {
                aiIDArray = new string[] { Cfg.MEDIUM_COLOR_AI };
            }
            else
            {
                aiIDArray = new string[] { ScenarioID };
            }

            // TODO veg - Debug output to see if game has initialized properly.
            // 
            dropemGame.initNewGame(adaptID, ScenarioID, playerID, aiIDArray, false, 30);

            dropemGame.startNewGame();

            board = dropemGame.getVirtualBoard();

            Cfg.clearMsgs();

            wins = hatAsset.CurrentAccuracy(adaptID, gameID, playerID) * 100;

            Cfg.showMsg("Wins sofar: {0:0.0}%", wins);

            // TODO veg - Note that if the player wins, (s)he gets a LAST_PLAYER_REWARD score.
        }
    }
}
