using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Labyrinth LB = new Labyrinth(new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1},
                {1,0,1,1,0,1,0,0,0,1},
                {1,0,0,0,0,1,0,1,0,1},
                {1,1,1,1,0,1,0,1,0,1},
                {1,0,0,0,0,1,0,1,0,1},
                {1,1,0,1,1,1,0,1,0,1},
                {1,1,0,0,0,0,0,1,0,1},
                {1,1,1,1,1,1,0,1,0,1},
                {1,0,0,0,0,0,0,1,0,1},
                {1,1,1,1,1,1,0,1,1,1}
            });
        public Form1()
        {
            InitializeComponent();
        }
 

        private void Form1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (!LB.ClickAndSetPath(e.Location)) MessageBox.Show("Нет пути");
            this.Invalidate();
        }

        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            LB.Draw(e.Graphics);
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }
    }

}
