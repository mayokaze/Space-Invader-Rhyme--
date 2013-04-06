using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Newtonsoft.Json;
using System.IO;


namespace sir
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    #region fields
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        SpriteBatch sb;
        MenuWindow menuMain;
        ArrayList songs = new ArrayList();
        ArrayList mws = new ArrayList();
        Model skydomeModel;
        Vector3 skydomeRotation;
        Texture2D skydomeTexture;
        QuakeCamera fpsCam;
        //SoundEffect shipSound;
        SoundEffect monsterSound;
        SoundEffect beatSound;
        string popup;
        string songName;
        long point = 0;
        int hp = 20;
        int popupRemain;
        int combo = 0;
        int maxCombo = 0;
       
        //    AudioEngine audioEngine;
        //   SoundBank soundBank;
        //  WaveBank waveBank;
 
        List<GameObject> enemyShips = new List<GameObject>();
        // GameObject[] enemyShips;
        GameObject gun;
        Song cs;
        Song intro;
        bool playingIntro = false;
        bool test1 = true;
        bool test2 = true;
        bool test3 = true;
        bool test4 = true;
        int holding = 0;
        List<Item> beats;
        Vector3 offset = new Vector3(0f, -1f, -3f);
        Vector3 shipMinPosition = new Vector3(-3.0f, 1.0f, -5.0f);
        Vector3 shipMaxPosition = new Vector3(3.0f, 4.0f, -5.0f);
        const float shipMinVelocity = 0.03f;
        const float shipMaxVelocity = 0.05f;
        SpriteFont spriteFont;
        SpriteFont sf;
        SpriteFont menuFont;
        List<MenuWindow> menuList;
        MenuWindow activeMenu;
        KeyboardState lastKeybState;
        GamePadState lastGamePadState;

        MenuWindow startGameEasy;
        MenuWindow startGameNormal;
        MenuWindow startGameHard;
        bool menusRunning;

        Matrix[] modelTransforms;
        Matrix[] modelTransforms2;
        int lastTime = -1;
    #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 20);   
            // TODO: Add your initialization logic here
            fpsCam = new QuakeCamera(GraphicsDevice.Viewport, new Vector3(0, 3, 10), 0, 0);
            lastKeybState = Keyboard.GetState();
            lastGamePadState = GamePad.GetState(PlayerIndex.One);
            menuList = new List<MenuWindow>();
            menusRunning = true;
            this.IsMouseVisible = true;
            base.Initialize();
            
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        #region load
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(device);
            sb = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            //     audioEngine = new AudioEngine("Content\\Audio\\TutorialAudio.xgs");
            //    waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            //   soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            skydomeModel = Content.Load<Model>("Models\\Skydome");
            skydomeTexture = Content.Load<Texture2D>("Models\\SkydomeTex");
            skydomeRotation = Vector3.Zero;
            intro = Content.Load<Song>("Audio\\intro");
            beatSound = Content.Load<SoundEffect>("Audio\\beat");
           monsterSound = Content.Load<SoundEffect>("Audio\\beat2");
            var sns = Directory.EnumerateFiles(@"Content", "*.wma");
            foreach (string currentFile in sns )
            {
                Console.WriteLine(currentFile);
                cs = Song.FromUri(currentFile, new Uri(currentFile, UriKind.Relative));
                songs.Add(cs);
                songName = cs.Name;
                Console.WriteLine(songName);
            }
//.Select(file => Song.FromUri(file, new Uri(file)))
//.ToList();

   //         cs = Content.Load<Song>("Audio\\sample");
            //string directory = Environment.GetFolderPath(Environment.SpecialFolder.);
            spriteFont = Content.Load<SpriteFont>("Arial");
            sf = Content.Load<SpriteFont>("SpriteFont");

            //  JsonTextReader reader = new JsonTextReader(re);
        //    MediaPlayer.IsRepeating = false;
         //   MediaPlayer.Play(cs);
            gun = new GameObject();
            gun.model = Content.Load<Model>("Models\\Ship");
            gun.scale = 0.0005f;
            gun.type = GameObject.GUN;

            
             menuFont = Content.Load<SpriteFont>("ourfont");
            Texture2D instruction = Content.Load<Texture2D>("instruction");
            Texture2D bg = Content.Load<Texture2D>("bg2");
            Texture2D bg3 = Content.Load<Texture2D>("bg3");
            


            //dummy menus
            startGameEasy = new MenuWindow(null, null, null);
            startGameNormal = new MenuWindow(null, null, null);
            startGameHard = new MenuWindow(null, null, null);
            List<MenuWindow> songItems = new List<MenuWindow>();

             menuMain = new MenuWindow(menuFont, "Main Menu", bg3);
            MenuWindow menuNewGame = new MenuWindow(menuFont, "Start a New Game", bg);
            MenuWindow menuSelect = new MenuWindow(menuFont, "Select Music", bg);
            MenuWindow menuInstruction = new MenuWindow(menuFont, "Instruction", instruction);
            foreach (Song s in songs)
            {
                MenuWindow mw = new MenuWindow(null, null, null);
                mws.Add(mw);
                menuSelect.AddMenuItem(s.Name,mw);
            }
            menuList.Add(menuMain);
            menuList.Add(menuNewGame);
            menuList.Add(menuSelect);
            menuList.Add(menuInstruction);

            menuMain.AddMenuItem("New Game", menuNewGame);
            menuMain.AddMenuItem("Select Music", menuSelect);
            menuMain.AddMenuItem("Instruction", menuInstruction);
            menuMain.AddMenuItem("Exit Game", null);
            menuNewGame.AddMenuItem("Easy", startGameEasy);
            menuNewGame.AddMenuItem("Normal", startGameNormal);
            menuNewGame.AddMenuItem("Hard", startGameHard);
            menuNewGame.AddMenuItem("Back to Main menu", menuMain);
            menuInstruction.AddMenuItem("Back to Main menu", menuMain);
            activeMenu = menuMain;
            menuMain.WakeUp();
        }


        private void MenuInput(KeyboardState currentKeybState, GamePadState currentGamePadState)
        {
            MenuWindow newActive = activeMenu.ProcessInput(lastKeybState, currentKeybState, lastGamePadState, currentGamePadState);

            for (int i = 0; i < mws.Count; i++) {
                if (newActive == mws[i]) {
                    cs = (Song)songs[i];
                    songName = cs.Name;
                    activeMenu = menuMain;
                    menuMain.WakeUp();
                    return;
                }
                
            }
                if (newActive == startGameEasy)
                {
                    //set level to easy
                    menusRunning = false;
                    StreamReader re = new StreamReader(songName + ".easy.json");
                    String json = re.ReadToEnd();
                    Console.WriteLine(json);
                    beats = JsonConvert.DeserializeObject<List<Item>>(json);
                    MediaPlayer.IsRepeating = false;
                    // MediaPlayer.;
                    MediaPlayer.Play(cs);
                }
                else if (newActive == startGameNormal)
                {
                    //set level to normal
                    menusRunning = false;
                    StreamReader re = new StreamReader(songName + ".normal.json");
                    String json = re.ReadToEnd();
                    Console.WriteLine(json);
                    beats = JsonConvert.DeserializeObject<List<Item>>(json);
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(cs);
                }
                else if (newActive == startGameHard)
                {
                    //set level to hard
                    menusRunning = false;
                    StreamReader re = new StreamReader(songName + ".normal.json");
                    String json = re.ReadToEnd();
                    Console.WriteLine(json);
                    beats = JsonConvert.DeserializeObject<List<Item>>(json);
                    MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(cs);
                }

                else if (newActive == null)
                    this.Exit();
                else if (newActive != activeMenu)
                    newActive.WakeUp();

            activeMenu = newActive;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion
        #region update
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
        

            MouseState mouseState = Mouse.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keybState = Keyboard.GetState();
            
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                this.Exit();
       
            if (menusRunning)
            {
                if (!playingIntro)
                {
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(intro);
                    playingIntro = true;
                }
                foreach (MenuWindow currentMenu in menuList)
                    currentMenu.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                MenuInput(keybState, gamePadState);
            }
            else
            {
                if (playingIntro)
                {
                    MediaPlayer.IsRepeating = false;
                  //  MediaPlayer.Stop();
                    playingIntro = false;
                }
              
                    // fpsCam.Update(mouseState, Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
                    // audioEngine.Update();
                    UpdateEnemyShips();

                    CheckMousClick();
                    
                    //  Console.WriteLine(gameTime.TotalGameTime.TotalMilliseconds);

                    int time = (int)MediaPlayer.PlayPosition.TotalMilliseconds / 100;

                    if (time - lastTime >= 1)
                    {
                        UpdateNewEnemies(time);
                        lastTime = time;
                    }
                    skydomeRotation += new Vector3(0, (float)gameTime.ElapsedGameTime.TotalSeconds * 0.05f, 0);
                    if (holding > 0)
                    {
                        //Console.WriteLine(holding);
                        if (holding % 10 == 0) { 
                                 Ray ray = GetRay();
                foreach (GameObject ship in enemyShips)
                {
                    if (ship.alive)
                    {
                        BoundingSphere shipSphere = ship.model.Meshes[0].BoundingSphere;
                        shipSphere.Center = ship.position;
                        shipSphere.Radius *= ship.scale;
                        float? intersection = ray.Intersects(shipSphere);
                        if (intersection == null){
                          ship.onFire = false;
                        }
                        }
                       
                       
                    }
            }
                        holding--;
        }
            }
            lastKeybState = keybState;
            lastGamePadState = gamePadState;
            base.Update(gameTime);
            }

        void UpdateNewEnemies(int gameTime)
        {
            //  Console.WriteLine(gameTime);
            foreach (Item beat in beats)
            {
                if (beat.time == gameTime)
                {
                    if (beat.type == GameObject.SHIP)
                    {
                        GameObject enemyShip = new GameObject();
                        enemyShip.model = this.LoadModelWithBoundingSphere(ref modelTransforms, "Models\\enemy", Content); //Content.Load<Model>("Models\\enemy");
                        enemyShip.scale = 0.0003f;
                        enemyShip.rotation = new Vector3(0.0f, MathHelper.Pi, 0.0f);
                        enemyShip.type = GameObject.SHIP;

                        enemyShip.alive = true;
                        enemyShip.position = new Vector3(beat.position[0], beat.position[1], beat.position[2]);
                        enemyShip.velocity = new Vector3(0.0f, 0.0f, beat.velocity);
                        enemyShips.Add(enemyShip);
                    }
                    else if (beat.type == GameObject.BOAT) {
                        GameObject enemyBoat = new GameObject();
                        enemyBoat.model = this.LoadModelWithBoundingSphere(ref modelTransforms2, "Models\\boat", Content); //Content.Load<Model>("Models\\enemy");
                        enemyBoat.scale = 0.01f;
                       enemyBoat.rotation = new Vector3(0.0f, 2*MathHelper.Pi, 0.0f);
                        enemyBoat.type = GameObject.BOAT;

                        enemyBoat.alive = true;
                        enemyBoat.position = new Vector3(beat.position[0], beat.position[1], beat.position[2]);
                        enemyBoat.velocity = new Vector3(0.0f, 0.0f, beat.velocity);
                        enemyShips.Add(enemyBoat);
                    }
                }

            }

        }


        void UpdateEnemyShips()
        {
            foreach (GameObject ship in enemyShips)
            {
                if (ship.alive)
                {
                    ship.position += ship.velocity;

                }
                /*    else
                    {
                        ship.alive = true;
                        ship.position = new Vector3(
                        MathHelper.Lerp(shipMinPosition.X, shipMaxPosition.X, (float)r.NextDouble()),
                        MathHelper.Lerp(shipMinPosition.Y, shipMaxPosition.Y, (float)r.NextDouble()),
                        MathHelper.Lerp(shipMinPosition.Z, shipMaxPosition.Z, (float)r.NextDouble()));
                        ship.velocity = new Vector3(0.0f, 0.0f,
                        MathHelper.Lerp(shipMinVelocity, shipMaxVelocity, (float)r.NextDouble()));
                    }*/
            }
        }
        #endregion


        #region draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            if (menusRunning)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                foreach (MenuWindow currentMenu in menuList)
                    currentMenu.Draw(spriteBatch);
                spriteBatch.End();
                Window.Title = "Menu running ...";
            }
            else
            {
      

                Window.Title = "Game running ...";

                graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


                if (MediaPlayer.State == MediaState.Stopped)
                {
                    point += combo * (combo / 10);
                    if (combo > maxCombo)
                        maxCombo = combo;
                    combo = 0;
                    
                   
                  //  double max = (double)beats.Count + (beats.Count * ((double)beats.Count / 10));
                   
                    string text = "Result: " + point + " pts     Max combo: " + maxCombo +  "   Perfectness: " + ((double)point / (beats.Count + ((double)(beats.Count * (beats.Count / 10)))/100)) + "%";
                    spriteBatch.Begin();
                    spriteBatch.DrawString(menuFont, text, new Vector2(150, 150), Color.Green);
      
                    spriteBatch.End();
                }
                else if (hp == 0) {
                   
                    spriteBatch.Begin();
                    spriteBatch.DrawString(menuFont, "GAME OVER!", new Vector2(200, 150), Color.Green);
                    MediaPlayer.Stop();
                    spriteBatch.End();                
                }

                else
                {
                    //draw the skydome
                    device.DepthStencilState = DepthStencilState.None;


                    foreach (ModelMesh mesh in skydomeModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.Texture = skydomeTexture;
                            effect.TextureEnabled = true;
                            effect.World = effect.World = Matrix.CreateRotationY(skydomeRotation.Y) *
                                Matrix.CreateTranslation(fpsCam.Position + new Vector3(0, -0.7f, 0));
                            effect.View = fpsCam.ViewMatrix;
                            effect.Projection = fpsCam.ProjectionMatrix;
                        }
                        mesh.Draw();
                    }

                    //Globals.spriteBatch.DrawString();
                    /*   sb.Begin();
                       sb.DrawString(sf, myText, new Vector2(20, 20), Color.Green);
                       if (popupRemain > 0)
                       {
                           sb.DrawString(spriteFont, popup, new Vector2(90, 90), Color.Blue);
                           popupRemain--;
                       }

                       sb.End();*/
                    device.DepthStencilState = DepthStencilState.Default;
                    // Matrix camaraRotation = Matrix.CreateFromYawPitchRoll(fpsCam.LeftRightRot, fpsCam.UpDownRot, 0);
                    //   offset = Vector3.Transform(offset, camaraRotation);
                    gun.rotation.Y = fpsCam.LeftRightRot + MathHelper.Pi;

                    gun.rotation.X = fpsCam.UpDownRot;
                    gun.position = fpsCam.Position + offset;
                    DrawGameObject(gun);
                    foreach (GameObject ship in enemyShips)
                    {
                        if (ship.alive)
                        {
                            DrawGameObject(ship);
                        }
                    }
                    //   TimeSpan toGo = MediaPlayer.PlayPosition;
                    string text = "Point: " + point + " HP: " + hp + " Combo: "+combo+" Time: " + MediaPlayer.PlayPosition.ToString();


                    spriteBatch.Begin();
                    spriteBatch.DrawString(sf, text, new Vector2(20, 20), Color.Green);
                    if (popupRemain > 0)
                    {
                        spriteBatch.DrawString(spriteFont, popup, new Vector2(90, 90), Color.Blue);
                        popupRemain--;
                    }
                    spriteBatch.End();



                    device.BlendState = BlendState.Opaque;
                    device.DepthStencilState = DepthStencilState.Default;
                    device.SamplerStates[0] = SamplerState.LinearWrap;
                }
                
            }
            base.Draw(gameTime);
        }
        void DrawGameObject(GameObject gameObject)
        {
            foreach (ModelMesh mesh in gameObject.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    //  effect.EnableDefaultLighting();
                    // effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.CreateFromYawPitchRoll(
                    gameObject.rotation.Y, gameObject.rotation.X, gameObject.rotation.Z) *
                    Matrix.CreateScale(gameObject.scale) *
                    Matrix.CreateTranslation(gameObject.position);
                    effect.Projection = fpsCam.ProjectionMatrix;
                    effect.View = fpsCam.ViewMatrix;
                    if (gameObject.position.Z > 5 && gameObject.type != GameObject.GUN&&gameObject.alive)
                    {
                        //    Console.WriteLine("MISS");
                        gameObject.alive = false;
                        popup = "MISS";
                        if (gameObject.type == GameObject.SHIP)
                            hp--;
                        else
                            hp -= 3;
                        
                        popupRemain = 100;
                        if (combo > maxCombo)
                            maxCombo = combo;
                        point += combo * (combo/10);
                        combo = 0;

                        if (test1)
                        {
                            test1 = false;
                            Console.WriteLine("MISS " + MediaPlayer.PlayPosition.TotalMilliseconds);
                        }

                    }

                    else if (gameObject.position.Z > 3 && gameObject.type == GameObject.SHIP)
                    {
                        effect.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);
                        if (test2)
                        {
                            test2 = false;
                            Console.WriteLine("PERFECT " + MediaPlayer.PlayPosition.TotalMilliseconds);
                        }
                    }
                    else if (gameObject.position.Z > 2 && gameObject.position.Z < 3 && gameObject.type == GameObject.SHIP)
                    {
                        effect.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f);
                        if (test3)
                        {
                            test3 = false;
                            Console.WriteLine("GOOD" + MediaPlayer.PlayPosition.TotalMilliseconds);
                        }
                    }
                    else if (gameObject.position.Z > 1 && gameObject.position.Z < 2 && gameObject.type == GameObject.SHIP)
                    {
                        effect.DiffuseColor = new Vector3(0.0f, 0.0f, 1.0f);
                        if (test4)
                        {
                            test4 = false;
                            Console.WriteLine("OK" + MediaPlayer.PlayPosition.TotalMilliseconds);
                        }
                    }
                    else if ( gameObject.type == GameObject.BOAT && gameObject.onFire)
                    {
                        effect.DiffuseColor = new Vector3(1.0f, 0.0f, 0.0f);
                        
                        //    Console.WriteLine("spider");
                        
                    }

                    else
                    {
                        effect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                    }
                }
                mesh.Draw();
            }
        }
        #endregion

        #region collision
        private Ray GetRay()
        {
            MouseState ms = Mouse.GetState();
            Vector3 neerSource = new Vector3(ms.X, ms.Y, 0);
            Vector3 farSource = new Vector3(ms.X, ms.Y, 1);

            Vector3 neerPosi = GraphicsDevice.Viewport.Unproject(neerSource, fpsCam.ProjectionMatrix, fpsCam.ViewMatrix, Matrix.Identity);
            Vector3 farPosi = GraphicsDevice.Viewport.Unproject(farSource, fpsCam.ProjectionMatrix, fpsCam.ViewMatrix, Matrix.Identity);
            Vector3 direction = farPosi - neerPosi;
            direction.Normalize();
            return new Ray(neerPosi, direction);
        }


        private void CheckMousClick()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Ray ray = GetRay();
                foreach (GameObject ship in enemyShips)
                {
                    if (ship.alive)
                    {
                        BoundingSphere shipSphere = ship.model.Meshes[0].BoundingSphere;
                        shipSphere.Center = ship.position;
                        shipSphere.Radius *= ship.scale;
                        float? intersection = ray.Intersects(shipSphere);
                        if (intersection != null)
                        {
                         //   Console.WriteLine("type  "+ship.type);

                            if (ship.type == GameObject.SHIP){
                                if (ship.position.Z > 1)
                                {
                                    if (ship.position.Z < 2)
                                    {
                                        popup = "OK";
                                        popupRemain = 100;
                                        point += 50;
                                       

                                    }
                                    else if (ship.position.Z < 3)
                                    {
                                        popup = "GOOD";
                                        popupRemain = 100;
                                        point += 80;
                                        
                                    }
                                    else if (ship.position.Z < 4)
                                    {
                                        popup = "PERFECT";
                                        popupRemain = 100;
                                        point += 100;
                                        
                                    }
                                    else if (ship.position.Z < 5)
                                    {
                                        popup = "GOOD";
                                        popupRemain = 100;
                                        point += 80;
                                       
                                    }
                                    beatSound.Play();
                                    Console.WriteLine("combo++");
                                    combo++;
                                    ship.alive = false;
                                }

                                Console.WriteLine(point);
                            }
                            else if (ship.type == GameObject.BOAT)
                            {
                                Console.WriteLine(ship.onFire + " "  +holding);
                                if (holding <= 0 && ship.onFire)
                                   {
                                       if (ship.position.Z < 5 && ship.position.Z > 3.5) {
                                           popup = "PERFECT";
                                           popupRemain = 100;
                                           point += 100;
                                          
                                       }
                                       else{ 
                                            popup = "GOOD";
                                           popupRemain = 100;
                                           point += 80;
                                           
                                       }
                                       combo++;
                                       ship.alive = false;

                                   }
                                else if (!ship.onFire)
                                    {
                                       
                                        ship.onFire = true;
                                        holding = 100;
                                        monsterSound.Play();

                                    }
                                
                            }

                        }
                    }
                }
            }
        }




        private Model LoadModelWithBoundingSphere(ref Matrix[] modelTransforms, string asset, ContentManager content)
        {
            Model newModel = content.Load<Model>(asset);

            modelTransforms = new Matrix[newModel.Bones.Count];
            newModel.CopyAbsoluteBoneTransformsTo(modelTransforms);

            BoundingSphere completeBoundingSphere = new BoundingSphere();
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                BoundingSphere origMeshSphere = mesh.BoundingSphere;
                BoundingSphere transMeshSphere = XNAUtils.TransformBoundingSphere(origMeshSphere, modelTransforms[mesh.ParentBone.Index]);
                completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
            }
            newModel.Tag = completeBoundingSphere;

            return newModel;
        }

    }
   }
  
#endregion