using System.Diagnostics;
namespace Goblin {


    partial class GoblinForm {
        private List<Color> _defColors = new List<Color> { Color.Brown, Color.White, Color.Black, Color.Gold };

        public bool validateTextFile(string filename)
        {
            string allowedChars = "XTRK";

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                string line2 = line.Replace(" ","");
                
                foreach (char c in line2)
                {
                    Debug.WriteLine(c);
                    if (!allowedChars.Contains(c))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task updateColorFromStep()
        {
            Color tempColor = Color.Transparent;
            Point prevPoint = new Point(-1, -1);

            foreach (Point point in _steps)
            {
                await Task.Delay(_delay);

                if (tempColor != Color.Transparent)
                {
                    setPanelColor(ref _panels[prevPoint.Y, prevPoint.X], tempColor);
                    setPathColor(ref _panels[prevPoint.Y, prevPoint.X]);
                }

                tempColor = _panels[point.Y, point.X].BackColor;
                prevPoint = new Point(point.X, point.Y);
                setVisitColor(ref _panels[point.Y, point.X]);
            }

            setPanelColor(ref _panels[prevPoint.Y, prevPoint.X], tempColor);
            setPathColor(ref _panels[prevPoint.Y, prevPoint.X]);
        }

        private async Task updateColorFromRoute()
        {
            await Task.Delay(_delay);
            Point toChange = _krustyKrab;

            Color temp = _panels[toChange.Y, toChange.X].BackColor;

            // change the krusty krab color
            setVisitColor(ref _panels[toChange.Y, toChange.X]);

            foreach (char route in _route)
            {

                await Task.Delay(_delay);
                setPanelColor(ref _panels[toChange.Y, toChange.X], temp);
                setPathColor(ref _panels[toChange.Y, toChange.X]);

                movePoint(ref toChange, route);

                temp = _panels[toChange.Y, toChange.X].BackColor;
                setVisitColor(ref _panels[toChange.Y, toChange.X]);
            }

            setPanelColor(ref _panels[toChange.Y, toChange.X], temp);
            setPathColor(ref _panels[toChange.Y, toChange.X]);
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
                for (int j = 0; j < columns; j++)
                {
                    createMazePanel(i, j, row[j]);
                    _maze[i, j] = row[j];
                }
            }
        }

        private void createMazePanel(int i, int j, char type)
        {
            // construct panel
            Panel panel = new Panel();

            // calculate panel size based on the number of rows and columns in _maze
            int parentWidth = panelMazeContainer.ClientSize.Width;
            int parentHeight = panelMazeContainer.ClientSize.Height;
            int panelSize = Math.Min((int)(parentWidth / _maze.GetLength(1)), (int)(parentHeight / _maze.GetLength(0)));

            // set panel location and size
            int deltaJ = (int)((panelMazeContainer.Width - _panels.GetLength(1) * panelSize) / 2);
            int deltaI = (int)((panelMazeContainer.Height - _panels.GetLength(0) * panelSize) / 2);
            panel.Location = new Point((j * panelSize) + deltaJ, (i * panelSize) + deltaI);
            panel.Size = new Size(panelSize, panelSize);

            panel.BackgroundImageLayout = ImageLayout.Stretch;

            panel.BackColor = getDefaultColor(type);
            setBackgroundImage(ref panel, type);

            if (type == 'K')
            {
                _krustyKrab = new Point(j, i);
            }

            if (type == 'T')
            {
                _treasureCount++;
            }

            // panel Border
            panel.BorderStyle = BorderStyle.FixedSingle;

            // add panel to _panels
            _panels[i, j] = panel;

            // add panel to panelMazeContainer
            panelMazeContainer.Controls.Add(_panels[i, j]);

            // add event handler for panelMazeContainer's SizeChanged event
            panelMazeContainer.SizeChanged += (sender, e) =>
            {
                int parentWidth = panelMazeContainer.ClientSize.Width;
                int parentHeight = panelMazeContainer.ClientSize.Height;
                int panelSize = Math.Min((int)(parentWidth / _maze.GetLength(1)), (int)(parentHeight / _maze.GetLength(0)));

                // set panel location and size
                int deltaJ = (int)((panelMazeContainer.Width - _panels.GetLength(1) * panelSize) / 2);
                int deltaI = (int)((panelMazeContainer.Height - _panels.GetLength(0) * panelSize) / 2);
                panel.Location = new Point((j * panelSize) + deltaJ, (i * panelSize) + deltaI);
                panel.Size = new Size(panelSize, panelSize);
            };
        }


        private void setBackgroundImage(ref Panel panel, char type){
            string imgPath = "";
            if (type == 'K'){
                imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "assets", "krusty_crab.png");
            }
            if (type == 'T'){
                imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "assets", "treasure.png");
            }
            if (type == 'R'){
                imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "assets", "grass.png");
            }
            if (type == 'X'){
                imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "assets", "stone.jpg");
            }
            panel.BackgroundImage = Image.FromFile(imgPath);
            panel.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private Color getDefaultColor(char type){
            if (type == 'K')
            { 
                return _defColors[0];
            } 
            if (type == 'R')
            {
                return _defColors[1];
            }
            if (type == 'X')
            {
                return _defColors[2];
            }
            if (type == 'T')
            {
                return _defColors[3];
            }
            return Color.Transparent;
        }

        public void setPanelColor(ref Panel panel, Color color){
            panel.BackColor = color;
        }

        public void setVisitColor(ref Panel panel){
            panel.BackColor = Color.SkyBlue;
        }

        public void setPathColor(ref Panel panel){
            if (!_defColors.Contains(panel.BackColor)){
                Color darkerColor = Color.FromArgb(
                    (int)(panel.BackColor.R * 0.95 / 2),
                    (int)(panel.BackColor.G * 0.95 / 2),
                    (int)(panel.BackColor.B * 0.95 / 2)
                );
                panel.BackColor =  darkerColor;
            } else {
                panel.BackColor = Color.LightGreen;
                // panel.BackColor = Color.FromArgb	(189,168,0);
            }
        }

        public void resetPanelColor(){
            int rows = _panels.GetLength(0);
            int cols = _panels.GetLength(1);

            for (int i = 0; i < rows; i++){
                for (int j = 0; j < cols; j++){
                    _panels[i,j].BackColor = getDefaultColor(_maze[i,j]);
                }
            }
        }

        public Color setColorOpacity(Color color, int opacity)
        {
            return Color.FromArgb(opacity, color.R, color.G, color.B);
        }
    }
}