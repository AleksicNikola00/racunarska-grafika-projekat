
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();
            block = false;
            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\RugbyBall"), "model.dae", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        private bool block;

        public bool Block { get { return block; } set { block = value; } }

        private float _rotationSpeed;
        public float RotationSpeed
        {
            get { return _rotationSpeed; }
            set{
                if (m_world != null)
                    m_world.RotationSpeed = value;
                _rotationSpeed = value;
            }
        }
        private float _ballScale;
        public float BallScale
        {
            get { return _ballScale; }
            set
            {
                _ballScale = value;
                if (m_world != null)
                    m_world.BallScale = value;
            }
        }
        private float _goalHeight;
        public float GoalHeight {
            get {  return _goalHeight; }
            set
            {
                _goalHeight = value;
                if(m_world!=null)
                    m_world.GoalHeight = value;
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
  
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:  this.Close(); break;
                case Key.E:
                    if (block) return;
                    if (m_world.RotationX > -20.0f)
                        m_world.RotationX -= 5.0f;
                    
                    break;
                case Key.D:
                    if (block) return;
                    if (m_world.RotationX < 90.0f) 
                        m_world.RotationX += 5.0f;
                    
                    break;
                case Key.S: if (block) return; m_world.RotationY -= 5.0f; break;
                case Key.F: if (block) return; m_world.RotationY += 5.0f; break;
                case Key.Add: if (block) return; m_world.SceneDistance += 2.0f; break;
                case Key.Subtract: if (block) return; m_world.SceneDistance -= 2.0f; break;
                case Key.V:
                    slider1.IsEnabled = block;
                    slider2.IsEnabled = block;
                    slider3.IsEnabled = block;
                    block = !block;
                    m_world.ShotAnimation();
                    break;
            }
        }
    }
}
