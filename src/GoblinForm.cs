using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace Goblin
{
    public partial class GoblinForm : Form
    {
        internal Panel[,] _panels;
        internal char[,] _maze;
        private Point _krustyKrab;
        private List<char> _route;
        private string _filename;
        private int _treasureCount = 0;

        // Label
        private Label pathLabel;
        private Label nodeCount;
        private Label stepCount;
        private Label executionTime;

        // public Color checkColor(char type)
        // {
        //     if (type == 'X')
        //     {
        //         return Color.Black;
        //     }
        //     else if (type == 'T')
        //     {
        //         return Color.Gold;
        //     }
        //     else if (type == 'K')
        //     { 
        //         return Color.Red;
        //     }
        //     return Color.White;
        // }

        public void resetAllAttribute(){
            // Release resources used by _panels
            if (_panels != null)
            {
                for (int i = 0; i < _panels.GetLength(0); i++)
                {
                    for (int j = 0; j < _panels.GetLength(1); j++)
                    {
                        if (_panels[i,j] != null)
                        {
                            _panels[i,j].Dispose();
                        }
                    }
                }
            }

            _panels = null;
            _maze = null;
            _krustyKrab = default(Point);
            _route = null;
            _treasureCount = 0;
        }

        public GoblinForm()
        {
            resetAllAttribute();
            InitializeComponent();
            
            // always on full screen mode
            //this.WindowState = FormWindowState.Maximized;
            // filepath will be get from filedialogbox
            handleInputPanel();
            handleOutputPanel();

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
            int deltaJ = (int)((panelMazeContainer.Width - _panels.GetLength(1) * panelSize)/2);
            int deltaI = (int)((panelMazeContainer.Height - _panels.GetLength(0) * panelSize)/2);
            panel.Location = new Point((j * panelSize ) + deltaJ, (i * panelSize) + deltaI);
            panel.Size = new Size(panelSize, panelSize);
            // panel.BackgroundImage = Image.FromFile("C:\\Users\\Jeffrey Chow\\Documents\\ITB\\4th Semester\\IF2211 Algorithm Strategies\\Tubes 2\\repository\\spongebob.png");
            // panel.BackgroundImageLayout = ImageLayout.Stretch;

            panel.BackColor = getDefaultColor(type);

            if (type == 'K'){
                _krustyKrab = new Point(i,j);
            }

            if (type == 'T'){
                _treasureCount++;
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

        private async Task updateColorFromRoute(List<char> routes)
        {
            await Task.Delay(200); // wait for 200ms
            Point toChange = _krustyKrab;

            Color temp = _panels[toChange.Y, toChange.X].BackColor;

            // change the krusty krab color
            setVisitColor(ref _panels[toChange.Y, toChange.X]);

            foreach (char route in routes)
            {

                await Task.Delay(200); // wait for 200ms
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

        private void handleInputPanel()
        {
            // Initialized Labels and Buttons
            Label inputLabel = new Label();

            Label filenameLabel = new Label();
            Button openFileBtn = new Button();

            Label algorithmLabel = new Label();
            GroupBox groupBox1 = new GroupBox();
            RadioButton choiceBFS = new RadioButton();
            RadioButton choiceDFS = new RadioButton();

            Button runBtn = new Button();
            Button showRoute = new Button();
            Button showStep = new Button();

            // Label "Input"
            inputLabel.Text = "Input";
            inputLabel.AutoSize = true;
            float inputFontSize = inputPanel.Height * 0.25f;
            inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
            inputLabel.Location = new Point(0, 0);
            inputPanel.Controls.Add(inputLabel);

            // Label "Filename"
            filenameLabel.Text = "Filename : ";
            filenameLabel.AutoSize = true;
            float fileFontSize = inputPanel.Height * 0.15f;
            filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
            filenameLabel.Location = new Point(0, 0);
            filePanel.Controls.Add(filenameLabel);

            // Open File Button
            openFileBtn.Text = "Open File";
            openFileBtn.Location = new Point(0, filenameLabel.Bottom + 10);
            openFileBtn.Size = new Size((int)(0.8 * filePanel.Width), (int)(0.4 * filePanel.Height));
            openFileBtn.Click += new EventHandler((sender, e) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Text files | *.txt";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    resetAllAttribute();
                    _filename = dialog.FileName;
                    readFile(_filename);
                    filenameLabel.Text = "Filename : " + Path.GetFileName(_filename);
                    showRoute.Enabled = false;
                    showStep.Enabled = false;
                }
            });
            filePanel.Controls.Add(openFileBtn);

            // Label "Algoritma"
            algorithmLabel.Text = "Algoritma : ";
            float algoFontSize = inputPanel.Height * 0.15f;
            algorithmLabel.Font = new Font(algorithmLabel.Font.FontFamily, algoFontSize, FontStyle.Bold);
            algorithmLabel.AutoSize = true;
            algorithmLabel.Location = new Point(0, 0);
            algorithmPanel.Controls.Add(algorithmLabel);

            // Group box + radio checker
            groupBox1.Location = new Point(0, algorithmLabel.Bottom);
            groupBox1.Size = new Size(algorithmPanel.Width, (int)(0.8 * algorithmPanel.Height));
            groupBox1.BackColor = Color.Red;

            choiceBFS.Text = "BFS";
            choiceBFS.ForeColor = Color.Black;
            choiceBFS.Location = new Point(0, (int)(0.2 * groupBox1.Height));
            choiceBFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            choiceDFS.Text = "DFS";
            choiceDFS.ForeColor = Color.Black;
            choiceDFS.Location = new Point(0, (int)(0.6 * groupBox1.Height));
            choiceDFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            groupBox1.Controls.Add(choiceBFS);
            groupBox1.Controls.Add(choiceDFS);

            algorithmPanel.Controls.Add(groupBox1);

            // Run Button
            bool runned = false;
            runBtn.Text = "Run";
            runBtn.Location = new Point((int)(0.1 * runPanel.Width), 0);
            runBtn.Size = new Size((int) (0.8 * runPanel.Width),(int)(0.45 * runPanel.Height));
            runBtn.Click += async (sender, e) =>
            {
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;
                
                if (runned){
                    resetAllAttribute();
                    readFile(_filename);
                    runned = false;
                }

                Goblin goblin = new Goblin(_treasureCount, _maze);
                if (choiceBFS.Checked)
                {
                    var watch = new System.Diagnostics.Stopwatch();
                   
                    watch.Start();  
                    goblin.SolveWithBFS();
                    watch.Stop();   

                    runned = true;

                    _route = goblin.GetRoute();

                    await updateColorFromRoute(_route);

                    // update pathLabel
                    String routePath = "";
                    foreach (char route in _route)
                    {
                        routePath += route.ToString();
                    }

                    pathLabel.Text = String.Join("-", routePath.ToCharArray());

                    // update stepCount
                    stepCount.Text = _route.Count().ToString();

                    // update nodeCount
                    nodeCount.Text = goblin.GetTotalVisitedNodes().ToString();
                    
                    // update executionTime
                    executionTime.Text = (watch.ElapsedMilliseconds).ToString() + " ms";  
                }

                if (choiceDFS.Checked) { 
                    var watch = new System.Diagnostics.Stopwatch();
                   
                    watch.Start();  
                    goblin.SolveWithDFS();
                    watch.Stop();   

                    runned = true;

                    _route = goblin.GetRoute();

                    await updateColorFromRoute(_route);

                    // update pathLabel
                    String routePath = "";
                    foreach (char route in _route)
                    {
                        routePath += route.ToString();
                    }

                    pathLabel.Text = String.Join("-", routePath.ToCharArray());

                    // update stepCount
                    stepCount.Text = _route.Count().ToString();

                    // update nodeCount
                    nodeCount.Text = goblin.GetTotalVisitedNodes().ToString();
                    
                    // update executionTime
                    executionTime.Text = (watch.ElapsedMilliseconds).ToString() + " ms";
                }

                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
            };
            
            // showRoute button
            showRoute.Text = "Show Route";
            showRoute.Location = new Point((int)(0.1 * runPanel.Width), (int)(0.5 * runPanel.Height));
            showRoute.Size = new Size((int) (0.4 * runPanel.Width),(int)(0.45 * runPanel.Height));
            showRoute.Click += async (sender, e) => {
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;

                resetPanelColor();
                await updateColorFromRoute(_route);

                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
            };
            showRoute.Enabled = false;

            // showStep button
            showStep.Text = "Show Step";
            showStep.Location = new Point((int)(0.5 * runPanel.Width), (int)(0.5 * runPanel.Height));
            showStep.Size = new Size((int) (0.4 * runPanel.Width),(int)(0.45 * runPanel.Height));
            showStep.Click += async (sender, e) => {
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;

                resetPanelColor();
                await updateColorFromRoute(_route);

                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
            };
            showStep.Enabled = false;

            // add buttons to runPanel
            runPanel.Controls.Add(showRoute);
            runPanel.Controls.Add(showStep);
            runPanel.Controls.Add(runBtn);

            panel_input.Resize += (sender, e) => {
                // Update Input Label
                float inputFontSize = inputPanel.Height * 0.25f;
                inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);

                // Update Filename label
                float fileFontSize = inputPanel.Height * 0.15f;
                filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
            };

        }

        private void handleOutputPanel()
        {
            /* HANDLE PANEL OUTPUT TITLE*/
            Label outputLabel = new Label();
            outputLabel.Text = "Output";
            outputLabel.AutoSize = true;
            outputLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            float outputFontSize = panelOutputTitle.Height * 0.25f;
            outputLabel.Font = new Font(outputLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
            panelOutputTitle.Controls.Add(outputLabel);

            panelOutputTitle.Resize += (sender, e) =>
            {
                float outputFontSize = panelOutputTitle.Height * 0.25f;
                outputLabel.Font = new Font(outputLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
            };

            /* HANDLE PANEL INFO CONTAINER*/
            int labelHeight = panelInfoContainer.ClientSize.Height / 2;

            /* ROUTE */
            Label routeLabel = new Label();
            pathLabel = new Label();

            // set the text for the labels
            routeLabel.Text = "Route : ";
            pathLabel.Text = "";

            // set the position for the labels
            pathLabel.Left = routeLabel.Right;

            // set the size and Anchor property for the labels
            routeLabel.AutoSize = true;
            routeLabel.Height = labelHeight;
            routeLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            pathLabel.AutoSize = true;
            pathLabel.Height = labelHeight;
            pathLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            // add the labels to the form
            panelInfoContainer.Controls.Add(routeLabel);
            panelInfoContainer.Controls.Add(pathLabel);

            // Second row top
            int topSecondRow = (int)(panelInfoContainer.Height/2);

            /* NODES INFO */
            Label nodeLabel = new Label();
            nodeCount = new Label();

            // set the text for the labels
            nodeLabel.Text = "Nodes : ";
            nodeCount.Text = "";

            // set the position for the labels
            nodeLabel.Top = topSecondRow;
            nodeCount.Left = nodeLabel.Right;
            nodeCount.Top = topSecondRow;

            // set the size and Anchor property for the labels
            nodeLabel.AutoSize = true;
            nodeLabel.Height = labelHeight;
            nodeLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            nodeCount.AutoSize = true;
            nodeCount.Height = labelHeight;
            nodeCount.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            // add the labels to the form
            panelInfoContainer.Controls.Add(nodeLabel);
            panelInfoContainer.Controls.Add(nodeCount);

            /* STEPS INFO */
            Label stepLabel = new Label();
            stepCount = new Label();

            // set the text for the labels
            stepLabel.Text = "Steps : ";
            stepCount.Text = "";

            // set the position for the labels
            stepLabel.Left = (int)(panelInfoContainer.Width / 3);
            stepLabel.Top = topSecondRow;
            stepCount.Left = stepLabel.Right;
            stepCount.Top = topSecondRow;

            // set the size and Anchor property for the labels
            stepLabel.AutoSize = true;
            stepLabel.Height = labelHeight;
            stepLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            stepCount.AutoSize = true;
            stepCount.Height = labelHeight;
            stepCount.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            // add the labels to the form
            panelInfoContainer.Controls.Add(stepLabel);
            panelInfoContainer.Controls.Add(stepCount);

            /* EXECUTION TIME INFO */
            Label executionLabel = new Label();
            executionTime = new Label();

            // set the text for the labels
            executionLabel.Text = "Time : ";
            executionTime.Text = "";

            // set the position for the labels
            executionLabel.Left = (int)(panelInfoContainer.Width * 2 / 3);
            executionLabel.Top = topSecondRow;
            executionTime.Left = executionLabel.Right + 10;
            executionTime.Top = topSecondRow;

            // set the size and Anchor property for the labels
            executionLabel.AutoSize = true;
            executionLabel.Height = labelHeight;
            executionLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            executionTime.AutoSize = true;
            executionTime.Height = labelHeight;
            executionTime.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            // add the labels to the form
            panelInfoContainer.Controls.Add(executionLabel);
            panelInfoContainer.Controls.Add(executionTime);
        }

        private void handleTitlePanel()
        {
            // Create title Label

        }



    }
}