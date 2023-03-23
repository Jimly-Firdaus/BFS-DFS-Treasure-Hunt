using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.IO;

namespace Goblin
{
    public partial class GoblinForm : Form
    {
        // attributes
        internal Panel[,] _panels;
        internal char[,] _maze;
        private Point _krustyKrab;
        private List<char> _route;
        private List<Point> _steps;
        private string _filename;
        private int _treasureCount = 0;
        private int _delay = 200;

        // Labels
        private Label pathLabel;
        private Label nodeCount;
        private Label stepCount;
        private Label executionTime;

        // Buttons
        internal Button openFileBtn;
        internal GroupBox groupBox1;
        internal RadioButton choiceBFS;
        internal RadioButton choiceDFS;
        CheckBox tspCheckBox;
        internal Button runBtn;
        internal Button showStep;
        internal Button showRoute;
        internal Button resetBtn;

        public GoblinForm()
        {
            // reset all attributes
            resetAllAttribute();

            // initialize main components : panels
            InitializeComponent();

            // initialize sub components : sub-panels
            handleTitlePanel();
            handleInputPanel();
            handleOutputPanel();
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

            // reset other attributes
            _panels = null;
            _maze = null;
            _krustyKrab = default(Point);
            _route = null;
            _steps = null;
            _treasureCount = 0;
        }

        private void handleInputPanel()
        {
            // Initialized Labels and Buttons
            Label inputLabel = new Label();

            Label filenameLabel = new Label();
            Label filenameTextLabel = new Label();
            openFileBtn = new Button();

            Label algorithmLabel = new Label();
            groupBox1 = new GroupBox();
            choiceBFS = new RadioButton();
            choiceDFS = new RadioButton();
            tspCheckBox = new CheckBox();

            Label delayTimeLabel = new Label();
            ComboBox delayTime = new ComboBox();
            runBtn = new Button();
            resetBtn = new Button();
            showRoute = new Button();
            showStep = new Button();

            bool runned = false;

            // Label Input
            inputLabel.Text = "Input";
            float inputFontSize = (float)(inputPanel.Height * 0.25);
            inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
            inputLabel.TextAlign = ContentAlignment.MiddleCenter;
            inputLabel.Location = new Point(0, 0);
            inputLabel.Size = inputPanel.Size;
            inputPanel.Controls.Add(inputLabel);

            // Label File
            filenameLabel.Text = "File : ";
            filenameLabel.AutoSize = true;
            float fileFontSize = (float)(filePanel.Height * 0.075);
            filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
            filenameLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.25));
            filenameLabel.Location = new Point(0, 0);
            filePanel.Controls.Add(filenameLabel);

            // Label FilenameText
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
                    // reset attributes before assigning to the new one
                    resetAllAttribute();

                    // validate textfile
                    bool validate = validateTextFile(dialog.FileName);

                    if (validate){
                        // save _filename
                        _filename = dialog.FileName;

                        // read the file
                        readFile(_filename);

                        // show the filename
                        filenameTextLabel.Text = Path.GetFileName(_filename);

                        // disable buttons
                        showRoute.Enabled = false;
                        showStep.Enabled = false;
                        runned = false;
                    } else { 
                        // show error message
                        filenameTextLabel.Text = "File is not valid";
                    }
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
            groupBox1.Size = new Size((int)(algorithmPanel.Width * 0.5), (int)(0.7 * algorithmPanel.Height));
            groupBox1.FlatStyle = FlatStyle.Flat;
            groupBox1.BackColor = this.BackColor;

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

            // TSP checkbox
            tspCheckBox.Location = new Point((int)(algorithmPanel.Width * 0.5), (int)(algorithmPanel.Height * 0.3));
            tspCheckBox.Size = new Size((int)(algorithmPanel.Width * 0.4), (int)(algorithmPanel.Height * 0.35));
            tspCheckBox.Font = new Font(algorithmPanel.Font.FontFamily, (float)(algorithmPanel.Height * 0.055));
            tspCheckBox.Text = "TSP";

            algorithmPanel.Controls.Add(tspCheckBox);

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
                setButtonEnable(false);
                
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

                // Select algorithm choice
                if (choiceBFS.Checked && tspCheckBox.Checked){
                    watch.Start();
                    // run tspForBFS
                    goblin.TSPwithBFS();
                    watch.Stop();
                }

                else if (choiceDFS.Checked && tspCheckBox.Checked){
                    watch.Start();
                    // run tspForDFS
                    goblin.SolveWithDFS("TSP");
                    watch.Stop();
                }

                else if (choiceBFS.Checked)
                {
                    watch.Start();  
                    goblin.SolveWithBFS();
                    watch.Stop();                         
                }

                else if (choiceDFS.Checked) { 
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

                // enabled other features
                setButtonEnable(true);
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
                setButtonEnable(false);

                resetPanelColor();
                await updateColorFromRoute();

                setButtonEnable(true);
            };
            showRoute.Enabled = false;

            // showStep button
            showStep.Text = "Show Step";
            showStep.Location = new Point((int)(0.5 * runPanel.Width), (int)(0.65 * runPanel.Height));
            showStep.Size = new Size((int) (0.45 * runPanel.Width),(int)(0.35 * runPanel.Height));
            showStep.Click += async (sender, e) => {
                setButtonEnable(false);

                resetPanelColor();
                await updateColorFromStep();

                setButtonEnable(true);
            };
            showStep.Enabled = false;

            // add buttons to runPanel
            runPanel.Controls.Add(showRoute);
            runPanel.Controls.Add(showStep);
            runPanel.Controls.Add(runBtn);

            inputPanel.SizeChanged += (sender, e) =>
            {
                // update font size and label size
                inputFontSize = (float)(inputPanel.Height * 0.25);
                inputLabel.Font = new Font(inputLabel.Font.FontFamily, inputFontSize, FontStyle.Bold);
                inputLabel.Size = inputPanel.Size;
            };

            filePanel.SizeChanged += (sender, e) =>
            {
                // update font size and label size and position
                fileFontSize = (float)(filePanel.Height * 0.075);
                filenameLabel.Font = new Font(filenameLabel.Font.FontFamily, fileFontSize, FontStyle.Bold);
                filenameLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.25));
                filenameTextFontSize = (float)(filePanel.Height * 0.07);
                filenameTextLabel.Font = new Font(filenameTextLabel.Font.FontFamily, filenameTextFontSize);
                filenameTextLabel.Size = new Size(filePanel.Width, (int)(filePanel.Height * 0.4));
                filenameTextLabel.Location = new Point(0, (int)(filePanel.Height * 0.25));

                // update openFileButton location and size
                openFileBtn.Location = new Point(0, (int)(filePanel.Height * 0.65));
                openFileBtn.Size = new Size((int)(0.95 * filePanel.Width), (int)(0.35 * filePanel.Height));
            };

            algorithmPanel.SizeChanged += (sender, e) => {
                // update algorithmLabel fontwsize
                float algoFontSize = (float)(algorithmPanel.Height * 0.075);
                algorithmLabel.Font = new Font(algorithmLabel.Font.FontFamily, algoFontSize, FontStyle.Bold);
                algorithmLabel.Size = new Size(algorithmPanel.Width, (int)(algorithmPanel.Height * 0.2));

                // update groupBox location and size
                groupBox1.Location = new Point(0, (int)(algorithmPanel.Height * 0.3));
                groupBox1.Size = new Size((int)(algorithmPanel.Width * 0.5), (int)(0.7 * algorithmPanel.Height));

                // update choices font size and location
                choiceBFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
                choiceBFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));
                choiceBFS.Location = new Point(0, (int)(groupBox1.Height * 0.05));

                choiceDFS.Font = new Font(groupBox1.Font.FontFamily, (float)(groupBox1.Height * 0.08));
                choiceDFS.Size = new Size(groupBox1.Width, (int)(0.4 * groupBox1.Height));
                choiceDFS.Location = new Point(0, (int)(groupBox1.Height * 0.5));

                // update checkbox location and font size
                tspCheckBox.Location = new Point((int)(algorithmPanel.Width * 0.5), (int)(algorithmPanel.Height * 0.3));
                tspCheckBox.Size = new Size((int)(algorithmPanel.Width * 0.4), (int)(algorithmPanel.Height * 0.35));
                tspCheckBox.Font = new Font(algorithmPanel.Font.FontFamily, (float)(algorithmPanel.Height * 0.055));
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
            panelOutputTitle.Resize += (sender, e) =>
            {
                // update font size
                float outputFontSize = (float)(panelOutputTitle.Height * 0.25);
                outputLabel.Font = new Font(outputLabel.Font.FontFamily, outputFontSize, FontStyle.Bold);
                outputLabel.Size = panelOutputTitle.Size;
            };

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