using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace Goblin
{
    public partial class GoblinForm : Form
    {
        private Panel[,] _panels;
        private char[,] _maze;
        private Point krustyKrab;

        public GoblinForm()
        {
            InitializeComponent();
            // filepath will be get from filedialogbox
            readFile("");
            handleInputPanel();
            handleOutputPanel();

            char[] routes = { 'R', 'D', 'D', 'R', 'R', 'U' };
            updateColorFromRoute(routes);
        }

        private void readFile(string filename)
        {
            // read file from text file
            string[] lines = File.ReadAllLines(filename);

            // replace " " to "" for each line in lines
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace(" ", "");
            }
            
            // count rows and columns
            int rows = lines.Length;
            int columns = lines[0].Length;
           
            // initialize _panels and _maze
            _panels = new Panel[rows, columns];
            _maze = new char[rows, columns];

            // fill the mazePanel
            for (int i = 0; i < rows; i++)
            {
                char[] row = lines[i].ToCharArray();
                for (int j = 0; j < columns; j++) {
                    createNewPanel(i, j, row[j]);
                    _maze[i, j] = row[j];   
                }
            }
        }

        private void createNewPanel(int i, int j, char type)
        {
            // construct panel
            Panel panel = new Panel();

            // add event handler for panel_output's SizeChanged event
            panel_output.SizeChanged += (sender, e) =>
            {
                int parentWidth = panel_output.ClientSize.Width;
                int parentHeight = panel_output.ClientSize.Height;
                int panelSize = Math.Min((int)(parentWidth / _maze.GetLength(1)), (int)(parentHeight / _maze.GetLength(0)));

                // set panel location and size
                panel.Location = new Point(j * panelSize, i * panelSize);
                panel.Size = new Size(panelSize, panelSize);
            };

            // calculate panel size based on the number of rows and columns in _maze
            int parentWidth = panel_output.ClientSize.Width;
            int parentHeight = panel_output.ClientSize.Height;
            int panelSize = Math.Min((int)(parentWidth / _maze.GetLength(1)), (int)(parentHeight / _maze.GetLength(0)));

            // set panel location and size
            panel.Location = new Point(j * panelSize, i * panelSize);
            panel.Size = new Size(panelSize, panelSize);

            // check Coloring
            panel.BackColor = Color.White;
            if (type == 'X')
            {
                panel.BackColor = Color.Black;
            }
            else if (type == 'T')
            {
                panel.BackColor = Color.Gold;
            } else if (type == 'K')
            {
                krustyKrab = new Point(i, j);
                panel.BackColor = Color.Red;
            }

            // create label to display type
            Label label = new Label();
            label.Text = type.ToString();
            label.AutoSize = true;
            label.Location = new Point(5, 5);
            label.BackColor = Color.Transparent;

            // add label to panel
            panel.Controls.Add(label);

            // panel Border
            panel.BorderStyle = BorderStyle.FixedSingle;

            // add panel to _panels
            _panels[i, j] = panel;

            // add panel to panel_output
            panel_output.Controls.Add(_panels[i, j]);
        }

        private async void updateColorFromRoute(List<char> routes)
        {
            await Task.Delay(1000); // wait for one second
            Point toChange = krustyKrab;

            // change the krusty krab color
            _panels[toChange.Y, toChange.X].BackColor = Color.Green;

            foreach (char route in routes)
            {
                await Task.Delay(1000); // wait for one second
                movePoint(ref toChange, route);
                _panels[toChange.Y, toChange.X].BackColor = Color.Green;
            }
        }

        private void movePoint(ref Point point, char direction)
        {
            switch (direction)
            {
                case 'L':
                    point.X -= 1; // move left
                    break;
                case 'R':
                    point.X += 1; // move right
                    break;
                case 'U':
                    point.Y -= 1; // move up
                    break;
                case 'D':
                    point.Y += 1; // move down
                    break;
                default:
                    // invalid direction, do nothing
                    break;
            }
        }

        private void handleInputPanel()
        {
            // Create the panel and set its properties
            Panel myPanel = new Panel();
            myPanel.BackColor = System.Drawing.Color.LightBlue;
            myPanel.BorderStyle = BorderStyle.FixedSingle;
            myPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            myPanel.Dock = DockStyle.Fill;

            // Add the panel to the form
            panel_input.Controls.Add(myPanel);

        }

        


        private void handleOutputPanel()
        {
        }

        private void handleTitlePanel()
        {
            // Create title Label

        }

    }
}