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
        private List<string> _route;
        private List<Point> _steps;
        private string _filename;
        private int _treasureCount = 0;
        private int _delay = 200;
        // Label
        private Label pathLabel;
        private Label nodeCount;
        private Label stepCount;
        private Label executionTime;

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
            _steps = null;
            _treasureCount = 0;
        }

        public GoblinForm()
        {
            resetAllAttribute();
            InitializeComponent();
            
            // always on full screen mode
            
            // filepath will be get from filedialogbox
            handleTitlePanel();
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
            // string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "krusty_crab.png");
            // panel.BackgroundImage = Image.FromFile(imagePath);
            //  panel.BackgroundImage = Image.FromFile("assets/krusty_crab.png");
    
            panel.BackgroundImageLayout = ImageLayout.Stretch;

            panel.BackColor = getDefaultColor(type);
            setBackgroundImage(ref panel,type);

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

        private async Task updateColorFromStep(){
            Color tempColor = Color.Transparent;
            Point prevPoint = new Point(-1,-1);

            foreach(Point point in _steps){
                await Task.Delay(_delay);

                if (tempColor != Color.Transparent){
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

            foreach (string route in _route)
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

        private void movePoint(ref Point point, string direction)
        {
            switch (direction)
            {
                case "L":
                    point.X -= 1; // move left
                    break;
                case "R":
                    point.X += 1; // move right
                    break;
                case "U":
                    point.Y -= 1; // move up
                    break;
                case "D":
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
            Label filenameTextLabel = new Label();
            Button openFileBtn = new Button();

            Label algorithmLabel = new Label();
            GroupBox groupBox1 = new GroupBox();
            RadioButton choiceBFS = new RadioButton();
            RadioButton choiceDFS = new RadioButton();

            Label delayTimeLabel = new Label();
            ComboBox delayTime = new ComboBox();
            Button runBtn = new Button();
            Button resetBtn = new Button();
            Button showRoute = new Button();
            Button showStep = new Button();

            bool runned = false;

            // Label "Input"
            inputLabel.Text = "Input";
            float inputFontSize = (float)(inputPanel.Height * 0.25);
            inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
            inputLabel.TextAlign = ContentAlignment.MiddleCenter;
            inputLabel.Location = new Point(0, 0);
            inputLabel.Size = inputPanel.Size;
            inputPanel.Controls.Add(inputLabel);

            // Label "Filename"
            filenameLabel.Text = "File : ";
            filenameLabel.AutoSize = true;
            float fileFontSize = (float)(filePanel.Height * 0.075);
            filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
            filenameLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.25));
            filenameLabel.Location = new Point(0, 0);
            filePanel.Controls.Add(filenameLabel);

            // Label "FilenameText"
            filenameTextLabel.Text = "No files chosen";
            float filenameTextFontSize = (float)(filePanel.Height * 0.07);
            filenameTextLabel.Font = new Font(filenameTextLabel.Font.FontFamily, filenameTextFontSize);
            filenameTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            filenameTextLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.4));
            filenameTextLabel.Location = new Point(0, (int)(filePanel.Height * 0.25));
            filePanel.Controls.Add(filenameTextLabel);

            // Open File Button
            openFileBtn.Text = "Open File";
            openFileBtn.Location = new Point(0, (int)(filePanel.Height * 0.65));
            openFileBtn.Size = new Size((int)(0.95 * filePanel.Width), (int)(0.35 * filePanel.Height));
            openFileBtn.FlatStyle = FlatStyle.Standard;
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
                    filenameTextLabel.Text = Path.GetFileName(_filename);
                    showRoute.Enabled = false;
                    showStep.Enabled = false;
                    runned = false;
                }
            });
            filePanel.Controls.Add(openFileBtn);

            // Label "Algoritma"
            algorithmLabel.Text = "Algoritma : ";
            float algoFontSize = (float)(algorithmPanel.Height * 0.075);
            algorithmLabel.Font = new Font(algorithmLabel.Font.FontFamily, algoFontSize, FontStyle.Bold);
            algorithmLabel.AutoSize = true;
            algorithmLabel.Location = new Point(0, 0);
            algorithmLabel.Size = new Size(algorithmPanel.Width, (int)(algorithmPanel.Height * 0.2));
            algorithmPanel.Controls.Add(algorithmLabel);

            // Group box + radio checker
            groupBox1.Location = new Point(0, (int)(algorithmPanel.Height * 0.3));
            groupBox1.Size = new Size(algorithmPanel.Width, (int)(0.7 * algorithmPanel.Height));
            groupBox1.FlatStyle = FlatStyle.Flat;

            choiceBFS.Text = "BFS";
            choiceBFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
            choiceBFS.Location = new Point(0, (int)(groupBox1.Height * 0.05));
            choiceBFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            choiceDFS.Text = "DFS";
            choiceDFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
            choiceDFS.Location = new Point(0, (int)(groupBox1.Height * 0.5));
            choiceDFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            groupBox1.Controls.Add(choiceBFS);
            groupBox1.Controls.Add(choiceDFS);

            algorithmPanel.Controls.Add(groupBox1);

            // delayTime Label
            delayTimeLabel.Text = "Delay (ms) : ";
            delayTimeLabel.AutoSize = true;
            float delayLabelFontSize = (float)(runPanel.Height * 0.05);
            delayTimeLabel.Font = new Font(delayTimeLabel.Font.FontFamily, delayLabelFontSize, FontStyle.Bold);
            delayTimeLabel.Size = new Size(runPanel.Width, (int)(runPanel.Height * 0.2));
            delayTimeLabel.Location = new Point(0, 0);

            runPanel.Controls.Add(delayTimeLabel);

            // Delay time dropdown
            delayTime.Height = (int)(runPanel.Height * 0.5);
            delayTime.AutoSize = true;
            delayTime.DropDownStyle = ComboBoxStyle.DropDownList;
            delayTime.Items.Add("200");
            delayTime.Items.Add("400");
            delayTime.Items.Add("600");
            delayTime.Items.Add("800");
            delayTime.Items.Add("1000");
            delayTime.SelectedIndex = 0;
            delayTime.Left = delayTimeLabel.Right;
            delayTime.Top = (delayTimeLabel.Top + delayTimeLabel.Bottom - delayTime.Height) / 2;
            delayTime.SelectedIndexChanged += (sender, e) => {
                _delay = Convert.ToInt32(delayTime.SelectedItem.ToString());
            };

            runPanel.Controls.Add(delayTime);

            // Enable runBtn only when Algorithm selected and File selected
            EventHandler runBtnEnabledCheck = (sender, e) => {
                if ((choiceBFS.Checked || choiceDFS.Checked) && !string.IsNullOrEmpty(_filename))
                    runBtn.Enabled = true;
                else
                    runBtn.Enabled = false;
            };

            choiceBFS.CheckedChanged += runBtnEnabledCheck;
            choiceDFS.CheckedChanged += runBtnEnabledCheck;
            openFileBtn.Click +=  runBtnEnabledCheck;

            // Run Button
            
            runBtn.Text = "Run";
            runBtn.Location = new Point(0, (int)(runPanel.Height * 0.25));
            runBtn.Size = new Size((int) (0.6 * runPanel.Width),(int)(0.35 * runPanel.Height));
            runBtn.Enabled = false;
            runBtn.Click += async (sender, e) =>
            {
                // disable other features
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;
                resetBtn.Enabled = false;
                
                // reset if runned
                if (runned){
                    resetAllAttribute();
                    readFile(_filename);
                    runned = false;
                }

                // initialize stopwatch
                var watch = new System.Diagnostics.Stopwatch();

                // instantiate goblin object for dfs and bfs
                Goblin goblin = new Goblin(_treasureCount, _maze);

                if (choiceBFS.Checked)
                {
                    watch.Start();  
                    goblin.SolveWithBFS();
                    watch.Stop();                         
                }

                if (choiceDFS.Checked) { 
                    watch.Start();  
                    goblin.SolveWithDFS();
                    watch.Stop();   
                }

                // set runned to true
                runned = true;

                // set route and steps
                _route = goblin.GetRoute();
                _steps = goblin.GetMoveHistory();

                // color the route
                await updateColorFromRoute();

                // update pathLabel
                String routePath = "";
                foreach (string route in _route)
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

                // enabled other features
                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
                resetBtn.Enabled = true;
            };
            
            // reset Button
            resetBtn.Text = "Reset";
            resetBtn.Location = new Point((int)(runPanel.Width * 0.65), (int)(runPanel.Height * 0.25));
            resetBtn.Size = new Size((int) (0.3 * runPanel.Width),(int)(0.35 * runPanel.Height));
            resetBtn.Enabled = false;
            resetBtn.Click += (sender, e) => {
                resetPanelColor();
            };

            runPanel.Controls.Add(resetBtn);
            
            // showRoute button
            showRoute.Text = "Show Route";
            showRoute.Location = new Point(0, (int)(0.65 * runPanel.Height));
            showRoute.Size = new Size((int) (0.45 * runPanel.Width),(int)(0.35 * runPanel.Height));
            showRoute.Click += async (sender, e) => {
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;
                resetBtn.Enabled = false;

                resetPanelColor();
                await updateColorFromRoute();

                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
                resetBtn.Enabled = true;
            };
            showRoute.Enabled = false;

            // showStep button
            showStep.Text = "Show Step";
            showStep.Location = new Point((int)(0.5 * runPanel.Width), (int)(0.65 * runPanel.Height));
            showStep.Size = new Size((int) (0.45 * runPanel.Width),(int)(0.35 * runPanel.Height));
            showStep.Click += async (sender, e) => {
                openFileBtn.Enabled = false;
                groupBox1.Enabled = false;
                choiceBFS.Enabled = false;
                choiceDFS.Enabled = false;
                runBtn.Enabled = false;
                showStep.Enabled = false;
                showRoute.Enabled = false;
                resetBtn.Enabled = false;

                resetPanelColor();
                await updateColorFromStep();

                openFileBtn.Enabled = true;
                groupBox1.Enabled = true;
                choiceBFS.Enabled = true;
                choiceDFS.Enabled = true;
                runBtn.Enabled = true;
                showStep.Enabled = true;
                showRoute.Enabled = true;
                resetBtn.Enabled = true;
            };
            showStep.Enabled = false;

            // add buttons to runPanel
            runPanel.Controls.Add(showRoute);
            runPanel.Controls.Add(showStep);
            runPanel.Controls.Add(runBtn);

            inputPanel.SizeChanged += (sender, e) =>
            {
                inputFontSize = (float)(inputPanel.Height * 0.25);
                inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
                inputLabel.Size = inputPanel.Size;
            };

            filePanel.SizeChanged += (sender, e) =>
            {
                fileFontSize = (float)(filePanel.Height * 0.075);
                filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
                filenameLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.25));
                filenameTextFontSize = (float)(filePanel.Height * 0.07);
                filenameTextLabel.Font = new Font(filenameTextLabel.Font.FontFamily, filenameTextFontSize);
                filenameTextLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.4));
                filenameTextLabel.Location = new Point(0, (int)(filePanel.Height * 0.25));

                openFileBtn.Location = new Point(0, (int)(filePanel.Height * 0.65));
                openFileBtn.Size = new Size((int)(0.95 * filePanel.Width), (int)(0.35 * filePanel.Height));
            };

            algorithmPanel.SizeChanged += (sender, e) => {
                float algoFontSize = (float)(algorithmPanel.Height * 0.075);
                algorithmLabel.Font = new Font(algorithmLabel.Font.FontFamily, algoFontSize, FontStyle.Bold);
                algorithmLabel.Size = new Size(algorithmPanel.Width, (int)(algorithmPanel.Height * 0.2));

                groupBox1.Location = new Point(0, (int)(algorithmPanel.Height * 0.3));
                groupBox1.Size = new Size(algorithmPanel.Width, (int)(0.7 * algorithmPanel.Height));

                choiceBFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
                choiceBFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));
                choiceBFS.Location = new Point(0, (int)(groupBox1.Height * 0.05));

                choiceDFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
                choiceDFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));
                choiceDFS.Location = new Point(0, (int)(groupBox1.Height * 0.5));
            };

            runPanel.SizeChanged += (sender, e) => {
                // Update delayTimeLabel size and location
                delayTimeLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.2));
                delayTimeLabel.Location = new Point(0, 0);
                delayLabelFontSize = (float)(runPanel.Height * 0.05);
                delayTimeLabel.Font = new Font(delayTimeLabel.Font.FontFamily, delayLabelFontSize, FontStyle.Bold);

                // Update delayTime dropdown location
                delayTime.Left = delayTimeLabel.Right;
                delayTime.Location = new Point((int)(runPanel.Width * 0.6),0);
                delayTime.Left = delayTimeLabel.Right;
                delayTime.Top = (delayTimeLabel.Top + delayTimeLabel.Bottom - delayTime.Height) / 2;

                // Update run button location and size
                runBtn.Location = new Point(0, (int)(runPanel.Height * 0.25));
                runBtn.Size = new Size((int) (0.6 * runPanel.Width),(int)(0.35 * runPanel.Height));

                // Update reset button location and size
                resetBtn.Location = new Point((int)(runPanel.Width * 0.65), (int)(runPanel.Height * 0.25));
                resetBtn.Size = new Size((int) (0.3 * runPanel.Width),(int)(0.35 * runPanel.Height));

                // Update showRoute button location and size
                showRoute.Location = new Point(0, (int)(0.65 * runPanel.Height));
                showRoute.Size = new Size((int) (0.45 * runPanel.Width),(int)(0.35 * runPanel.Height));

                // Update showStep button location and size
                showStep.Location = new Point((int)(0.5 * runPanel.Width), (int)(0.65 * runPanel.Height));
                showStep.Size = new Size((int) (0.45 * runPanel.Width),(int)(0.35 * runPanel.Height));
            };

        }

        private void handleOutputPanel()
        {
            /* HANDLE PANEL OUTPUT TITLE*/
            Label outputLabel = new Label();
            outputLabel.Text = "Output";
            float outputFontSize = (float)(panelOutputTitle.Height * 0.25);
            outputLabel.Font = new Font(outputLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
            outputLabel.TextAlign = ContentAlignment.MiddleCenter;
            outputLabel.Size = panelOutputTitle.Size;
            outputLabel.Location = new Point(0,0);
            panelOutputTitle.Controls.Add(outputLabel);

            panelOutputTitle.Resize += (sender, e) =>
            {
                float outputFontSize = (float)(panelOutputTitle.Height * 0.25);
                outputLabel.Font = new Font(outputLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
                outputLabel.Size = panelOutputTitle.Size;
            };

            /* HANDLE PANEL INFO CONTAINER*/
            int labelHeight = panelInfoContainer.ClientSize.Height / 2;
            float infoFontSize = (float)(panelInfoContainer.Height * 0.1);

            /* NODES INFO */
            Label nodeLabel = new Label();
            nodeCount = new Label();

            // set the text for the labels
            nodeLabel.Text = "Nodes : ";
            nodeLabel.Font = new Font(nodeLabel.Font.FontFamily, infoFontSize);
            nodeLabel.BackColor = Color.Transparent;

            nodeCount.Text = "";
            nodeCount.Font = new Font(nodeCount.Font.FontFamily, infoFontSize, FontStyle.Bold);

            // set the position for the labels
            nodeCount.Left = nodeLabel.Right;

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
            stepLabel.Font = new Font(stepLabel.Font.FontFamily, infoFontSize);
            stepLabel.BackColor = Color.Transparent;

            stepCount.Left = stepLabel.Right;
            stepCount.Font = new Font(stepCount.Font.FontFamily, infoFontSize, FontStyle.Bold);

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
            executionLabel.Font = new Font(executionLabel.Font.FontFamily, infoFontSize);
            executionLabel.BackColor = Color.Transparent;

            executionTime.Left = executionLabel.Right;
            executionTime.Font = new Font(executionTime.Font.FontFamily, infoFontSize, FontStyle.Bold);

            // set the size and Anchor property for the labels
            executionLabel.AutoSize = true;
            executionLabel.Height = labelHeight;
            executionLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            executionTime.AutoSize = false;
            executionTime.Height = labelHeight;
            executionTime.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            // add the labels to the form
            panelInfoContainer.Controls.Add(executionLabel);
            panelInfoContainer.Controls.Add(executionTime);

            // Second row top
            int topSecondRow = (int)(panelInfoContainer.Height/2);

             /* ROUTE */
            Label routeLabel = new Label();
            pathLabel = new Label();

            // set the text for the labels
            routeLabel.Text = "Route : ";
            routeLabel.Font = new Font(routeLabel.Font.FontFamily, infoFontSize);
            routeLabel.BackColor = Color.Transparent;

            pathLabel.Text = "";
            pathLabel.Font = new Font(pathLabel.Font.FontFamily, infoFontSize, FontStyle.Bold);
            pathLabel.AutoSize = false;
            pathLabel.MaximumSize = new Size((int)(panelInfoContainer.Width * 0.8), 0);

            // set the position for the labels
            pathLabel.Left = routeLabel.Right;

            // set the size and Anchor property for the labels
            routeLabel.AutoSize = true;
            routeLabel.Height = labelHeight;
            routeLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            routeLabel.Top = topSecondRow;

            pathLabel.AutoSize = true;
            pathLabel.Height = labelHeight;
            pathLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            pathLabel.Top = topSecondRow;
    
            // add the labels to the form
            panelInfoContainer.Controls.Add(routeLabel);
            panelInfoContainer.Controls.Add(pathLabel);

            // Responsive
            panelInfoContainer.Resize += (sender, e) =>
            {
                int labelHeight = panelInfoContainer.ClientSize.Height / 2;
                float infoFontSize = (float)(panelInfoContainer.Height * 0.1);

                // adjust the font size of the labels
                nodeLabel.Font = new Font(nodeLabel.Font.FontFamily, infoFontSize);
                stepLabel.Font = new Font(stepLabel.Font.FontFamily, infoFontSize);
                executionLabel.Font = new Font(executionLabel.Font.FontFamily, infoFontSize);
                routeLabel.Font = new Font(routeLabel.Font.FontFamily, infoFontSize);
                nodeCount.Font = new Font(nodeCount.Font.FontFamily, infoFontSize, FontStyle.Bold);
                stepCount.Font = new Font(stepCount.Font.FontFamily, infoFontSize, FontStyle.Bold);
                executionTime.Font = new Font(executionTime.Font.FontFamily, infoFontSize, FontStyle.Bold);
                pathLabel.Font = new Font(pathLabel.Font.FontFamily, infoFontSize, FontStyle.Bold);

                // adjust the position of the labels
                nodeCount.Left = nodeLabel.Right;
                stepLabel.Left = (int)(panelInfoContainer.Width / 3);
                stepCount.Left = stepLabel.Right;
                executionLabel.Left = (int)(panelInfoContainer.Width * 2 / 3);
                executionTime.Left = executionLabel.Right;
                pathLabel.Left = routeLabel.Right;

                // adjust the size of the labels
                nodeLabel.Height = labelHeight;
                nodeCount.Height = labelHeight;
                stepLabel.Height = labelHeight;
                stepCount.Height = labelHeight;
                executionLabel.Height = labelHeight;
                executionTime.Height = labelHeight;
                routeLabel.Height = labelHeight;
                pathLabel.Height = labelHeight;

                // adjust the maximum size of the pathLabel
                pathLabel.MaximumSize = new Size((int)(panelInfoContainer.Width * 0.8), 0);

                // adjust the top position of the labels in the second row
                int topSecondRow = (int)(panelInfoContainer.Height / 2);
                routeLabel.Top = topSecondRow;
                pathLabel.Top = topSecondRow;
            };
        }

        private void handleTitlePanel()
        {
            // Create title Label
            Label titleLabel = new Label();
            titleLabel.Text = "Goblin Crab Hunting Treasure";
            float outputFontSize = (float)(panel_title.Height * 0.25);
            titleLabel.Font = new Font(titleLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = panel_title.Size;
            titleLabel.Location = new Point(0,0);
            panel_title.Controls.Add(titleLabel);

            panel_title.Resize += (sender, e) =>
            {
                float outputFontSize = (float)(panel_title.Height * 0.25);
                titleLabel.Font = new Font(titleLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
                titleLabel.Size = panel_title.Size;
            };

            panel_title.Controls.Add(titleLabel);
        }



    }
}