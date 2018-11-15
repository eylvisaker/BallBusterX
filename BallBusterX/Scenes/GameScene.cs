using AgateLib.Input;
using AgateLib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace BallBusterX.Scenes
{
    public class GameScene : Scene
    {
        private readonly CImage img;
        private readonly CSound snd;
        private readonly BBXConfig config;
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;

        private readonly KeyboardEvents keyboard;
        private readonly IMouseEvents mouse;
        private readonly GamePadEvents gamePad;

        private GameState gameState;
        private Song song;

        private Point mousePos;
        private Point mouseCenterPos = new Point(400, 300);

        public GameScene(GraphicsDevice graphicsDevice,
                         GameState gameState,
                         IMouseEvents mouse,
                         CImage img,
                         CSound snd,
                         BBXConfig config)
        {
            this.gameState = gameState;
            this.graphicsDevice = graphicsDevice;
            this.mouse = mouse;
            this.img = img;
            this.snd = snd;
            this.config = config;
            this.spriteBatch = new SpriteBatch(graphicsDevice);

            gamePad = new GamePadEvents(PlayerIndex.One);
            gamePad.ButtonPressed += GamePad_ButtonPressed;

            mouse.MouseMove += Mouse_MouseMove;
            mouse.MouseDown += Mouse_MouseDown;

            keyboard = new KeyboardEvents();
            keyboard.KeyUp += Keyboard_KeyUp;

            InitializeLevel(true);
        }

        public event Action Pause;
        public event Action StageComplete;

        private void Keyboard_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.D:
                    gameState.SelfDestruct();
                    break;

                case Keys.Escape:
                case Keys.End:
                    gameState.Lives = 0;
                    gameState.SelfDestruct();
                    break;

                case Keys.P:
                case Keys.Pause:
                    OnPause();
                    break;
            }
        }

        private void OnPause()
        {
            if (!gameState.transitionout)
            {
                Pause?.Invoke();
            }
        }

        private void GamePad_ButtonPressed(object sender, GamepadButtonEventArgs e)
        {
            if (e.Button == Buttons.Start)
            {
                OnPause();
            }

            if (e.Button == Buttons.A)
            {
                gameState.MouseClick();
            }
        }

        private void Mouse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gameState.MouseClick();
        }

        public void InitializeLevel(bool resetPowerups)
        {
            gameState.initLevel(resetPowerups);

            song = PlayRandomSong();
        }

        private Song PlayRandomSong()
        {
            if (snd.music.Count == 0)
                return null;

            int randsong = gameState.random.Next(snd.music.Count);
            Song selectedSong = snd.music[randsong];

            if (config.PlayMusic)
            {
                MediaPlayer.Play(selectedSong);
                MediaPlayer.IsRepeating = true;
            }

            return selectedSong;
        }

        private void Mouse_MouseMove(object sender, MouseEventArgs e)
        {
            Vector2 delta = e.MousePosition.ToVector2() - mouseCenterPos.ToVector2();

            mousePos.X += (int)(delta.X + 0.5f);
            mousePos.Y += (int)(delta.Y + 0.5f);

            gameState.MouseMove(mousePos);

            ResetMousePosition();
        }

        private void ResetMousePosition()
        {
            mousePos.X = (int)(gameState.paddle.x + 0.5f);
            mousePos.Y = 400;

            Mouse.SetPosition(mouseCenterPos.X, mouseCenterPos.Y);
        }

        protected override void OnUpdateInput(IInputState input)
        {
            mouse.Update(input);
            keyboard.Update(input);
            gamePad.Update(input);

            if (Math.Abs(gamePad.LeftStick.X) > 0.1f)
            {
                // Right trigger acts as an accelerator, left trigger acts as a brake.
                float gpScale = 500 * (1 + 2 * gamePad.RightTrigger) * (1 - 0.8f * gamePad.LeftTrigger);

                float amount = gamePad.LeftStick.X * gpScale * (float)input.GameTime.ElapsedGameTime.TotalSeconds;

                mousePos.X += (int)(amount);

                gameState.MouseMove(mousePos);

                ResetMousePosition();
            }
        }

        protected override void OnUpdate(GameTime time)
        {
            base.OnUpdate(time);

            gameState.UpdateLevel(time);

            IsFinished = gameState.gameover;

            if (gameState.transitionout)
            {
                if (song != null)
                    MediaPlayer.Stop();
            }
            if (gameState.stageComplete)
            {
                StageComplete?.Invoke();
            }
        }

        protected override void DrawScene(GameTime time)
        {
            spriteBatch.Begin();

            gameState.DrawLevel(spriteBatch);

            spriteBatch.End();
        }
    }
}
