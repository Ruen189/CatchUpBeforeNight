using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using WMPLib;
namespace CatchUpBeforeNight1
{
    // view
    public partial class View : Form
    {
        private GameController controller;
        private WindowsMediaPlayer player = new WindowsMediaPlayer();
        public View()
        {
            InitializeComponent();
            PlayMusic();
            SetVolume(10);
            controller = new GameController(this); // Инициализируем controller перед подпиской на события, шоб не было ошибки треклятой
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.KeyUp += new KeyEventHandler(OnKeyUp);
        }
        private void PlayMusic()
        {

            player.URL = @"C:\Users\rosti\Downloads\Astron - credits song for my death but im the final boss..mp3";
            //player.settings.setMode("loop", true);
            player.controls.play();

        }

        private void SetVolume(int volume)
        {
            // Проверка на допустимый диапазон громкости от 0 до 100
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;

            player.settings.volume = volume;
        }
        class MyPanel : Panel { public MyPanel() { this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); } }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                controller.HandleKeyDown(e);
                e.Handled = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            controller.HandleKeyUp(e);
        }

        public void UpdateCharacterPosition(Point position)
        {
            character.Location = position;
            this.Refresh();
        }
        /*
        public void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            controller.OnKeyPress(e);
        }
        */
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
        }

        private void background_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void character_Click(object sender, EventArgs e)
        {

        }
    }
    class GameModel
    {
        public Point CharacterPosition { get; set; }
        public bool LeftPressed { get; set; } // булевые флаги для отслеживания клавиш
        public bool RightPressed { get; set; }
        public bool UpPressed { get; set; }
        public bool DownPressed { get; set; }

        public GameModel()
        {
            CharacterPosition = new Point(0, 0); // Начальное положение персонажа
        }

        public void MoveCharacter(int dx, int dy)
        {
            CharacterPosition = new Point(CharacterPosition.X + dx, CharacterPosition.Y + dy);
        }
    }

    class GameController
    {
        private GameModel model;
        private View view;
        private Timer moveTimer;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool upPressed = false;
        private bool downPressed = false;

        public GameController(View view)
        {
            this.view = view;
            this.model = new GameModel();

            moveTimer = new Timer();
            moveTimer.Interval = 5; // Частота обновления
            moveTimer.Tick += (sender, e) => MoveCharacter();
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: model.LeftPressed = true; break;
                case Keys.D: model.RightPressed = true; break;
                case Keys.W: model.UpPressed = true; break;
                case Keys.S: model.DownPressed = true; break;
            }

            moveTimer.Start();
        }

        public void HandleKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: model.LeftPressed = false; break;
                case Keys.D: model.RightPressed = false; break;
                case Keys.W: model.UpPressed = false; break;
                case Keys.S: model.DownPressed = false; break;
            }

            if (!model.LeftPressed && !model.RightPressed && !model.UpPressed && !model.DownPressed)
                moveTimer.Stop();
        }

        private void MoveCharacter()
        {
            var speed = 5;
            var diagonalSpeed = speed / Math.Sqrt(2);

            var dx = 0; 
            var dy = 0;

            if (model.LeftPressed) dx -= speed;
            if (model.RightPressed) dx += speed;
            if (model.UpPressed) dy -= speed;
            if (model.DownPressed) dy += speed;

            // Движется ли персонаж по диагонали
            if ((model.LeftPressed || model.RightPressed) && (model.UpPressed || model.DownPressed))
            {
                dx = (int)(Math.Sign(dx) * diagonalSpeed);
                dy = (int)(Math.Sign(dy) * diagonalSpeed);
            }

            model.MoveCharacter(dx, dy);
            UpdateView();
        }

        private void UpdateView()
        {
            view.UpdateCharacterPosition(model.CharacterPosition);
        }
    }


    /*
    public partial class Form1 : Form, IView
    {
        private GameController controller;

        public Form1()
        {
            InitializeComponent();
            controller = new GameController(this, character, background);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            controller.HandleKeyPress(e.KeyCode);
        }

        public void UpdateCharacterPosition(Point newPosition)
        {
            character.Location = newPosition;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
        }

        private void background_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }

    public interface IView
    {
        void UpdateCharacterPosition(Point newPosition);
    }

    public class GameController
    {
        private const int CharacterSpeed = 5;
        private const int WorldSize = 1152;
        private IView view;
        private PictureBox character;
        private PictureBox background;

        public GameController(IView view, PictureBox character, PictureBox background)
        {
            this.view = view;
            this.character = character;
            this.background = background;
        }

        public void HandleKeyPress(Keys key)
        {
            int newX = character.Location.X;
            int newY = character.Location.Y;

            switch (key)
            {
                case Keys.W:
                    newY -= CharacterSpeed;
                    break;
                case Keys.S:
                    newY += CharacterSpeed;
                    break;
                case Keys.A:
                    newX -= CharacterSpeed;
                    break;
                case Keys.D:
                    newX += CharacterSpeed;
                    break;
            }

            newX = (newX + WorldSize) % WorldSize;
            newY = (newY + WorldSize) % WorldSize;

            Point newPosition = new Point(newX, newY);
            view.UpdateCharacterPosition(newPosition);
        }
    }
    */
}

/*
 * private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
        }

        private void background_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
*/