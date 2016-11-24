using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PepesComing.Solvers;
using static PepesComing.Utils;

namespace PepesComing {

    public class Game : Microsoft.Xna.Framework.Game {
        // Random number generator
        public static readonly Random rnd = new Random();

        // Graphics
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        // Systems
        private Camera camera;
        private Controller controller;
        private World world;
        private Sprites sprites;
        private UiManager ui;

        // Solver
        private Solver solver;

        //Frame time calculation
        private int frames;
        private double elapsedTime;
        private int fps;

        public static readonly bool DEBUG_STEP = true;

        public Game() {
            Content.RootDirectory = "Content";

            //Setup window
            Window.AllowUserResizing = true;
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            _graphicsDeviceManager.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize() {
            base.Initialize();

            // Setup controller
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = Window.Handle;
            IsMouseVisible = true;
            controller = new Controller();

            // Create world
            world = new World();
            Viewport vp = _graphicsDeviceManager.GraphicsDevice.Viewport;
            camera = new Camera(ref vp);

            // Load sprites
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sprites = new Sprites(this);

            // Setup Ui
            ui = new UiManager(sprites);
        }

        protected override void LoadContent() {
        }

        protected override void UnloadContent() {
            spriteBatch.Dispose();
            sprites.Dispose();
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            //Update fps
            frames += 1;
            Window.Title = "FPS: " + fps.ToString();

            GraphicsDevice.Clear(Color.LightSkyBlue);

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            for (int x = camera.Viewport.X; x <= camera.Viewport.Width + camera.Viewport.X; x++) {
                for (int y = camera.Viewport.Y; y <= camera.Viewport.Height + camera.Viewport.Y; y++) {
                    if (x >= 0 & x < World.width & y >= 0 & y < World.height) {
                        if (solver != null && solver.Solution[x, y]) {
                            Rectangle drawPositon = new Rectangle(x * 16, y * 16, 16, 16);
                            spriteBatch.Draw(texture: sprites.Red, destinationRectangle: drawPositon, color: Color.White);
                        } else {
                            world.RenderTile(x, y, spriteBatch, sprites);
                        }
                    }
                }
            }

            Rectangle drawPosition;

            if (solver != null && solver.GetType().IsSubclassOf(typeof(SolverMouse))) {
                SolverMouse solverMouse = (SolverMouse)solver;
                Console.WriteLine(solverMouse.Mouse.position);
                drawPosition = new Rectangle((int)solverMouse.Mouse.position.X * 16, (int)solverMouse.Mouse.position.Y * 16, 16, 16);
                switch (solverMouse.Mouse.facing) {
                    case Compass.North:
                        spriteBatch.Draw(texture: sprites.Texture, destinationRectangle: drawPosition, sourceRectangle: Sprites.ArrowNorth, color: Color.White);
                        break;
                    case Compass.East:
                        spriteBatch.Draw(texture: sprites.Texture, destinationRectangle: drawPosition, sourceRectangle: Sprites.ArrowEast, color: Color.White);
                        break;
                    case Compass.South:
                        spriteBatch.Draw(texture: sprites.Texture, destinationRectangle: drawPosition, sourceRectangle: Sprites.ArrowSouth, color: Color.White);
                        break;
                    case Compass.West:
                        spriteBatch.Draw(texture: sprites.Texture, destinationRectangle: drawPosition, sourceRectangle: Sprites.ArrowWest, color: Color.White);
                        break;
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            ui.Render(GraphicsDevice, spriteBatch, sprites);
            spriteBatch.End();
        }

        protected override void Update(GameTime gameTime) {
            if (controller.Escape) {
                Exit();
            }

            // Generate world if it dosnt exsist
            if (!world.Generated) world.RegenerateMaze();

            // Calculate fps
            elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime >= 1000f) {
                fps = frames;
                frames = 0;
                elapsedTime = 0;
            }

            // Update systems
            controller.Update(Keyboard.GetState(), Microsoft.Xna.Framework.Input.Mouse.GetState(Window), camera);

            if (!ui.Update(controller, GraphicsDevice)) {
                camera.Update(controller, _graphicsDeviceManager.GraphicsDevice.Viewport);
            }

            // Regenerate maze
            if (ui.GenerateMaze) {
                if (solver != null) {
                    solver.Dispose();
                    solver = null;
                }
                world.RegenerateMaze();
            }

            // Step
            if (ui.Step && solver != null && DEBUG_STEP) {
                solver.Step();
            }

            // Solve maze
            if (ui.WallFollower) {
                if (solver != null) solver.Dispose();
                solver = new WallFollower(ref world);
                Console.Clear();
            } else if (ui.RandomMouser) {
                if (solver != null) solver.Dispose();
                solver = new RandomMouser(ref world);
            } else if (ui.Tremaux) {
                if (solver != null) solver.Dispose();
                solver = new Tremaux(ref world);
            } else if (ui.Recursive) {
                if (solver != null) solver.Dispose();
                solver = new Recursive(ref world);
            } 
            base.Update(gameTime);
        }
    }
}
