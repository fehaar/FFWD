#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using PressPlay.FFWD.Components;
#endregion

namespace PressPlay.FFWD.ScreenManager
{
    public abstract class MenuSceneScreen : GameScreen
    {
        #region Fields

        List<ButtonComponent> menuEntries = new List<ButtonComponent>();
        string menuTitle;
        protected Scene scene;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<ButtonComponent> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuSceneScreen(string menuTitle)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap;

            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            scene = new Scene();
        }

        #endregion

        #region HandleInput

        public override void HandleInput(InputState input)
        {
            // we cancel the current menu screen if the user presses the back button
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // iterate the entries to see if any were tapped
                    for (int i = 0; i < menuEntries.Count; i++)
                    {
                        ButtonComponent menuEntry = menuEntries[i];

                        if (menuEntry.bounds.Contains(tapLocation))
                        {
                            // select the entry. since gestures are only available on Windows Phone,
                            // we can safely pass PlayerIndex.One to all entries since there is only
                            // one player on Windows Phone.
                            OnSelectEntry(i, PlayerIndex.One);
                        }
                    }
                }
            }
        }

        public bool IsMouseWithinBounds(GameObject go, Point pointer)
        {
            SpriteRenderer renderer = (SpriteRenderer)go.renderer;

            return (pointer.X > renderer.bounds.Left) && (pointer.X < renderer.bounds.Right) && (pointer.Y > renderer.bounds.Top) && (pointer.Y < renderer.bounds.Bottom);
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            foreach (ButtonComponent button in menuEntries)
            {
                if(button.gameObject.renderer != null){
                    ((SpriteRenderer)button.gameObject.renderer).material.color = new Color(1, 1, 1, TransitionAlpha); //Color.FromNonPremultiplied(255, 255, 255, (int)(255*(TransitionAlpha)));
                }
            }
        }

        #endregion

        protected GameObject AddButton(ButtonComponent button)
        {
            GameObject go = new GameObject("MenuEntry");
            go.AddComponent(button);
            scene.gameObjects.Add(go);
            menuEntries.Add(button);

            return go;
        }
    }
}
