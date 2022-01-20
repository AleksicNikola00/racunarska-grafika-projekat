// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi
        //ZA 2KT
        private enum TextureObjects { Grass=0,WhitePlastic };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;

        private string[] m_textureFiles = {"..//..//Images//grass-texture.jpg", "..//..//Images//white-plastic.jpg" };


        //animacije
        private bool ballGoingUp;
        private float ballHeight;
        private float ballRotation;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            
            // Model sencenja 
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            //inicijalni enable-ovi
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            //color
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);


            //pokreni scenu
            m_scene.LoadScene();
            m_scene.Initialize();
            SetupLightning(gl);
            SetupTextures(gl);

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(10);
            timer1.Tick += new EventHandler(UpdateAnimation1);
            timer1.Start();
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(3f);
            timer2.Tick += new EventHandler(UpdateAnimation2);
            timer2.Start();

            ballHeight = 0f;
            ballRotation = 0f;
            ballGoingUp = true;

        }

        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (ballGoingUp)
                ballHeight += 0.02f;
            else
                ballHeight -= 0.02f;

            ballRotation += 1f;
        }

        /// <summary>
        /// Obrće smer pomeranja kocki
        /// </summary>
        private void UpdateAnimation2(object sender, EventArgs e)
        {
            if (!ballGoingUp)
            {
                ballHeight = 0f;
            }
            ballGoingUp = !ballGoingUp;
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {  
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Viewport(0, 0, m_width, m_height);

            //podesi kameru
            gl.LookAt(0f, 0f, -5f, 0f, 0f, -100f, 0.0f, 1.0f, 0.0f);

            //namesti rotacije i zumiranje
            gl.PushMatrix();
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Translate(0.0f, 0.0f, m_sceneDistance);

            //draw floor
            DrawFloor(gl);

            //draw goal
            DrawGoal(gl);

            //draw rugby ball
            gl.Translate(0.0f, -2.0f, -20f);
            gl.Translate(0f, Clamp(ballHeight,0f,2f), 0f);
            gl.Rotate(ballRotation, 1.0f, 0.0f, 0.0f);
            m_scene.Draw();
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { 0f, 20f, -2f });

            //ispisi text
            DrawText(gl);

            gl.PopMatrix();

            gl.Flush();
        }



        private void SetupTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            //gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            //dodato
            

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);		// Nearest Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);		// Nearest Filtering


                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void SetupLightning(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); //TACKASTI IZVOR


            float[] pos = { 15f, 10f, -45f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pos);

            //reflektor
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
            float[] light1diffuse = new float[] { 1f, 0.0f, 0.5f, 1f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 35.0f);
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { 0f,5f,-20f});


            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        private void DrawText(OpenGL gl)
        {
            gl.Viewport(m_width / 2, m_height / 2, m_width / 2, m_height / 2);     //donji desni ugao
            gl.DrawText3D("Tahoma", 14, 0, 0, "");
            gl.DrawText(m_width - 280, m_height - 20 , 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Predmet: Racunarska grafika");
            gl.DrawText(m_width - 280, m_height - 22, 1.0f, 1.0f, 0.0f, "Tahoma", 10,  "_________________________");
            gl.DrawText(m_width - 280, m_height - 50, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Sk.god: 2021/22.");
            gl.DrawText(m_width - 280, m_height - 52, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "_____________");
            gl.DrawText(m_width - 280, m_height - 80, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Ime: Nikola");
            gl.DrawText(m_width - 280, m_height - 82, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "_________");
            gl.DrawText(m_width - 280, m_height - 110, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Prezime: Aleksic");
            gl.DrawText(m_width - 280, m_height - 112, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "_____________");
            gl.DrawText(m_width - 280, m_height - 140, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Sifra zad: 7.1");
            gl.DrawText(m_width - 280, m_height - 142, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "___________");
        }

        private void DrawGoal(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhitePlastic]);
            gl.PushMatrix();
            Cylinder cylinder = new Cylinder();
            cylinder.BaseRadius = 0.1;
            cylinder.TopRadius = 0.1;
            cylinder.Height = 10;

            gl.Translate(0.0f, 8f, -45f);
            gl.Rotate(90f, 1f, 0f, 0f);
            gl.Color(1f, 1f, 1f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.Translate(-5f, 0f, 0f);
            gl.Rotate(90f, 0f, 1f, 0f);
            //gl.Color(1f, 1f, 0f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.Translate(10f, 0f, 0f);
            gl.Rotate(90f, 0f, -1f, 0f);
            //gl.Color(1f, 0f, 1f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.Translate(10f, 0f, 0f);
            //gl.Color(1f, 0f, 1f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            gl.PopMatrix();
        }

        private void DrawFloor(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.PushMatrix();
            gl.Scale(10f, 10f, 10f);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1, 0);
            gl.Color(0.1f, 0.9f, 0.1f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-15f, -5f, -15f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(15f, -5f, -15f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(15f, 0f, -70f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-15f, 0f, -70f);
            gl.End();

            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
