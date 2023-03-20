using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace Goblin
{
    public partial class GoblinForm : Form
    {
        private Panel[,] _panels;
        private char[,] _maze;
        private Point _krustyKrab;
        private List<string> _route;
        private string _filename;

        // Label
        private Label pathLabel;
        private Label nodeCount;
        private Label stepCount;
        private Label executionTime;

        public Color checkColor(char type)
        {
            if (type == 'X')
            {
                return Color.Black;
            }
            else if (type == 'T')
            {
                return Color.Gold;
            }
            else if (type == 'K')
            { 
                return Color.Red;
            }
            return Color.White;
        }

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
                    createNewPanel(i, j, row[j]);
                    _maze[i, j] = row[j];
                }
            }
        }

        private void createNewPanel(int i, int j, char type)
        {
            // construct panel
            Panel panel = new Panel();

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

            // calculate panel size based on the number of rows and columns in _maze
            int parentWidth = panelMazeContainer.ClientSize.Width;
            int parentHeight = panelMazeContainer.ClientSize.Height;
            int panelSize = Math.Min((int)(parentWidth / _maze.GetLength(1)), (int)(parentHeight / _maze.GetLength(0)));

            // set panel location and size
            int deltaJ = (int)((panelMazeContainer.Width - _panels.GetLength(1) * panelSize)/2);
            int deltaI = (int)((panelMazeContainer.Height - _panels.GetLength(0) * panelSize)/2);
            panel.Location = new Point((j * panelSize ) + deltaJ, (i * panelSize) + deltaI);
            panel.Size = new Size(panelSize, panelSize);

            // check Coloring
            panel.BackColor = checkColor(type);
            // panel.BackColor = Color.White;
            // if (type == 'X')
            // {
            //     panel.BackColor = Color.Black;
            // }
            // else if (type == 'T')
            // {
            //     panel.BackColor = Color.Gold;
            // }
            // else if (type == 'K')
            // {
            //     _krustyKrab = new Point(i, j);
            //     panel.BackColor = Color.Red;
            // }

            if (type == 'K'){
                _krustyKrab = new Point(i,j);
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
        }

        private async Task updateColorFromRoute(List<string> routes)
        {
            await Task.Delay(200); // wait for one second
            Point toChange = _krustyKrab;

            // change the krusty krab color
            _panels[toChange.Y, toChange.X].BackColor = Color.Green;

            foreach (string route in routes)
            {
                await Task.Delay(200); // wait for one second
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
            // panel_input properties
            int panelWidth = panel_input.Width;
            int panelHeight = panel_input.Height;
            int gap = (int)(panelHeight * 0.05); 

            // Input Panel
            Panel inputPanel = new Panel();
            inputPanel.BackColor = System.Drawing.Color.Red;
            inputPanel.BorderStyle = BorderStyle.FixedSingle;
            inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputPanel.Height = (int)(panelHeight * 0.10);
            inputPanel.Width = panelWidth;
            inputPanel.Top = gap;

            // File Panel
            Panel filePanel = new Panel();
            filePanel.BackColor = System.Drawing.Color.LightCyan;
            filePanel.BorderStyle = BorderStyle.FixedSingle;
            filePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filePanel.Height = (int)(panelHeight * 0.20);
            filePanel.Width = panelWidth;
            filePanel.Top = inputPanel.Bottom + gap; // add gap

            // Algorithm Panel
            Panel algorithmPanel = new Panel();
            algorithmPanel.BackColor = System.Drawing.Color.LightGray;
            algorithmPanel.BorderStyle = BorderStyle.FixedSingle;
            algorithmPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            algorithmPanel.Height = (int)(panelHeight * 0.30);
            algorithmPanel.Width = panelWidth;
            algorithmPanel.Top = filePanel.Bottom + gap; // add gap

            // Run Panel
            Panel runPanel = new Panel();
            runPanel.BackColor = System.Drawing.Color.LightGreen;
            runPanel.BorderStyle = BorderStyle.FixedSingle;
            runPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            runPanel.Height = (int)(panelHeight * 0.15);
            runPanel.Width = panelWidth;
            runPanel.Top = algorithmPanel.Bottom + gap; // add gap

            // Label "Input"
            Label inputLabel = new Label();
            inputLabel.Text = "Input";
            inputLabel.AutoSize = true;
            float inputFontSize = inputPanel.Height * 0.25f;
            inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
            inputLabel.Location = new Point(0, 0);
            inputPanel.Controls.Add(inputLabel);

            // Label "Filename"
            Label filenameLabel = new Label();
            filenameLabel.Text = "Filename : ";
            filenameLabel.AutoSize = true;
            float fileFontSize = inputPanel.Height * 0.15f;
            filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
            filenameLabel.Location = new Point(0, 0);
            filePanel.Controls.Add(filenameLabel);

            // Open File Button
            Button openFileBtn = new Button();
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
                }
            });
            filePanel.Controls.Add(openFileBtn);

            // Label "Algoritma"
            Label algorithmLabel = new Label();
            algorithmLabel.Text = "Algoritma : ";
            float algoFontSize = inputPanel.Height * 0.15f;
            algorithmLabel.Font = new Font(algorithmLabel.Font.FontFamily, algoFontSize, FontStyle.Bold);
            algorithmLabel.AutoSize = true;
            algorithmLabel.Location = new Point(0, 0);
            algorithmPanel.Controls.Add(algorithmLabel);

            // Group box + radio checker
            GroupBox groupBox1 = new GroupBox();
            groupBox1.Location = new Point(0, algorithmLabel.Bottom);
            groupBox1.Size = new Size(algorithmPanel.Width, (int)(0.8 * algorithmPanel.Height));
            groupBox1.BackColor = Color.Red;

            RadioButton choiceBFS = new RadioButton();
            choiceBFS.Text = "BFS";
            choiceBFS.ForeColor = Color.Black;
            choiceBFS.Location = new Point(0, (int)(0.2 * groupBox1.Height));
            choiceBFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            RadioButton choiceDFS = new RadioButton();
            choiceDFS.Text = "DFS";
            choiceDFS.ForeColor = Color.Black;
            choiceDFS.Location = new Point(0, (int)(0.6 * groupBox1.Height));
            choiceDFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));

            groupBox1.Controls.Add(choiceBFS);
            groupBox1.Controls.Add(choiceDFS);

            algorithmPanel.Controls.Add(groupBox1);

            // Run Button
            bool runned = false;
            Button runBtn = new Button();
            runBtn.Text = "Run";
            runBtn.Location = new Point(0, 0);
            runBtn.Size = new Size((int) (0.8 * runPanel.Width),(int)(0.5 * runPanel.Height));
            runBtn.Click += new EventHandler((sender, e) =>
            {
                if (runned){
                    resetAllAttribute();
                    readFile(_filename);
                    runned = false;
                }

                if (choiceBFS.Checked)
                {
                    Goblin goblin = new Goblin(2, _maze);

                    var watch = new System.Diagnostics.Stopwatch();
                   
                    watch.Start();  
                    goblin.SolveWithBFS();
                    watch.Stop();   

                    runned = true;

                    _route = goblin.GetRoute();

                    updateColorFromRoute(_route);

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
                    executionTime.Text = (watch.ElapsedMilliseconds).ToString();  
                }

                if (choiceDFS.Checked) { 
                    Goblin goblin = new Goblin(2, _maze);

                    var watch = new System.Diagnostics.Stopwatch();
                   
                    watch.Start();  
                    goblin.SolveWithDFS();
                    watch.Stop();   

                    runned = true;

                    _route = goblin.GetRoute();

                    updateColorFromRoute(_route);

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
                    executionTime.Text = (watch.ElapsedMilliseconds).ToString();
                }
            });
            runPanel.Controls.Add(runBtn);

            panel_input.Controls.Add(inputPanel);
            panel_input.Controls.Add(filePanel);
            panel_input.Controls.Add(algorithmPanel);
            panel_input.Controls.Add(runPanel);

            // responsiveness
            panel_input.Resize += (sender, e) => {
                int panelWidth = panel_input.Width;
                int panelHeight = panel_input.Height;
                int gap = (int)(panelHeight * 0.05);

                // Update Input Panel
                inputPanel.Width = panelWidth;
                inputPanel.Height = (int)(panelHeight * 0.1);
                inputPanel.Top = gap;

                // Update File Panel
                filePanel.Width = panelWidth;
                filePanel.Height = (int)(panelHeight * 0.2);
                filePanel.Top = inputPanel.Bottom + gap;

                // Update Algorithm Panel
                algorithmPanel.Width = panelWidth;
                algorithmPanel.Height = (int)(panelHeight * 0.3);
                algorithmPanel.Top = filePanel.Bottom + gap;

                // Update Run Panel
                runPanel.Width = panelWidth;
                runPanel.Height = (int)(panelHeight * 0.15);
                runPanel.Top = algorithmPanel.Bottom + gap;

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