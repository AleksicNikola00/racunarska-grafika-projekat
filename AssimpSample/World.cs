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

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

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
        private float m_sceneDistance = 7000.0f;

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
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Viewport(0, 0, m_width, m_height);

            //draw floor
            DrawFloor(gl);

            //draw goal
            DrawGoal(gl);

            //draw rugby ball
            gl.PushMatrix();

            gl.Translate(0.0f, -2.0f, -20f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            m_scene.Draw();

            //ispisi text
            DrawText(gl);

            gl.PopMatrix();

            gl.Flush();
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

            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.1f, 0.9f, 0.1f);
            gl.Vertex(-15f, -5f, -15f);
            gl.Vertex(15f, -5f, -15f);
            gl.Vertex(15f, 0f, -70f);
            gl.Vertex(-15f, 0f, -70f);
            gl.End();
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
