namespace Goblin
{
    partial class GoblinForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            // set main background color
            this.BackColor = Color.Azure;
            
            // initialize panels and pictureBox
            panel_input = new Panel();
            panel_output = new Panel();
            panel_title = new Panel();
            hr = new PictureBox();
            SuspendLayout();

            // Form dimensions
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            // set default window to be fullscreen
            this.WindowState = FormWindowState.Maximized;

            // update the formWidth and formHeight attributes
            formWidth = ClientRectangle.Size.Width;
            formHeight = ClientRectangle.Size.Height;

            // 
            // panel_input
            // 
            panel_input.Location = new Point((int)(0.05 * formWidth), (int)(0.2 * formHeight));
            panel_input.Name = "panel_input";
            panel_input.Size = new Size((int)(0.225 * formWidth), (int)(0.75 * formHeight));
            panel_input.TabIndex = 0;
            // 
            // panel_output
            // 
            panel_output.Location = new Point((int)(0.315 * formWidth), (int)(0.2 * formHeight));
            panel_output.Name = "panel_output";
            panel_output.Size = new Size((int)(0.675 * formWidth), (int)(0.75 * formHeight));
            panel_output.TabIndex = 1;
            // 
            // panel_title
            // 
            panel_title.Location = new Point((int)(0.05 * formWidth), (int)(0.05 * formHeight));
            panel_title.Name = "panel_title";
            panel_title.Size = new Size((int)(0.9 * formWidth), (int)(0.1 * formHeight));
            panel_title.TabIndex = 2;
            panel_title.BackColor = Color.Transparent;

            // line separator
            hr.BackColor = Color.SkyBlue;
            hr.Location = new Point((int)(0.05 * formWidth), (int)(0.175 * formHeight));
            hr.Size = new Size((int)(0.9 * formWidth), (int)(0.005 * formHeight));
            this.Controls.Add(hr);
            
            // 
            // GoblinForm
            // 
            Controls.Add(panel_title);
            Controls.Add(panel_output);
            Controls.Add(panel_input);
            Margin = new Padding(2, 1, 2, 1);
            Name = "GoblinForm";
            Text = "Goblin";
            ResumeLayout(false);

            // Onchange
            this.SizeChanged += new EventHandler(GoblinForm_Resize);

            // panel_input children
            createInputPanels();

            // panel_output children
            createOutputPanels();
        }

        private void GoblinForm_Resize(object sender, EventArgs e)
        {
            // Update the client size to match the new size of the client area
            ClientSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            // Update form size
            formWidth = ClientRectangle.Size.Width;
            formHeight = ClientRectangle.Size.Height;

            if (formWidth > 0 && formHeight > 0){
                // update all panel
                panel_input.Location = new Point((int)(0.05 * formWidth), (int)(0.2 * formHeight));
                panel_input.Size = new Size((int)(0.225 * formWidth), (int)(0.75 * formHeight));

                panel_output.Location = new Point((int)(0.315 * formWidth), (int)(0.2 * formHeight));
                panel_output.Size = new Size((int)(0.675 * formWidth), (int)(0.75 * formHeight));

                panel_title.Location = new Point((int)(0.05 * formWidth), (int)(0.05 * formHeight));
                panel_title.Size = new Size((int)(0.9 * formWidth), (int)(0.1 * formHeight));

                hr.Location = new Point((int)(0.05 * formWidth), (int)(0.175 * formHeight));
                hr.Size = new Size((int)(0.9 * formWidth), (int)(0.005 * formHeight));
            }
        }

        private void createInputPanels(){
            // panel_input properties
            int panelWidth = panel_input.Width;
            int panelHeight = panel_input.Height;
            int gap = (int)(panelHeight * 0.03); 

            // Input Panel
            inputPanel = new Panel();
            inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputPanel.Height = (int)(panelHeight * 0.1);
            inputPanel.Width = panelWidth;
            inputPanel.BackColor = Color.Transparent;

            // File Panel
            filePanel = new Panel();
            filePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filePanel.Height = (int)(panelHeight * 0.25);
            filePanel.Width = panelWidth;
            filePanel.Top = inputPanel.Bottom + gap; // add gap
            filePanel.BackColor = Color.Transparent;

            // Algorithm Panel
            algorithmPanel = new Panel();
            algorithmPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            algorithmPanel.Height = (int)(panelHeight * 0.25);
            algorithmPanel.Width = panelWidth;
            algorithmPanel.Top = filePanel.Bottom + gap; // add gap
            algorithmPanel.BackColor = Color.Transparent;

            // Run Panel
            runPanel = new Panel();
            runPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            runPanel.Height = (int)(panelHeight * 0.31);
            runPanel.Width = panelWidth;
            runPanel.Top = algorithmPanel.Bottom + gap; // add gap
            runPanel.BackColor = Color.Transparent;

            panel_input.Controls.Add(inputPanel);
            panel_input.Controls.Add(filePanel);
            panel_input.Controls.Add(algorithmPanel);
            panel_input.Controls.Add(runPanel);

            // responsiveness
            panel_input.Resize += (sender, e) => {
                int panelWidth = panel_input.Width;
                int panelHeight = panel_input.Height;
                int gap = (int)(panelHeight * 0.03);

                // Update Input Panel
                inputPanel.Width = panelWidth;
                inputPanel.Height = (int)(panelHeight * 0.1);

                // Update File Panel
                filePanel.Width = panelWidth;
                filePanel.Height = (int)(panelHeight * 0.25);
                filePanel.Top = inputPanel.Bottom + gap;

                // Update Algorithm Panel
                algorithmPanel.Width = panelWidth;
                algorithmPanel.Height = (int)(panelHeight * 0.25);
                algorithmPanel.Top = filePanel.Bottom + gap;

                // Update Run Panel
                runPanel.Width = panelWidth;
                runPanel.Height = (int)(panelHeight * 0.31);
                runPanel.Top = algorithmPanel.Bottom + gap;
            };
        }

        private void createOutputPanels()
        {
            int parentWidth = panel_output.Width;
            int parentHeight = panel_output.Height;

            // Create Panel Output Title
            panelOutputTitle = new Panel();
            panelOutputTitle.Width = parentWidth;
            panelOutputTitle.Height = (int)(parentHeight * 0.1);
            panelOutputTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelOutputTitle.BackColor = Color.Transparent;

            // Create Panel Maze Container
            panelMazeContainer = new Panel();
            panelMazeContainer.Width = parentWidth;
            panelMazeContainer.Height = (int)(parentHeight * 0.75);
            panelMazeContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelMazeContainer.Top = panelOutputTitle.Bottom;
            panelMazeContainer.BackColor = Color.Transparent;

            // Create Panel Info Container
            panelInfoContainer = new Panel();
            panelInfoContainer.Width = parentWidth;
            panelInfoContainer.Height = (int)(parentHeight * 0.15);
            panelInfoContainer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelInfoContainer.Top = panelMazeContainer.Bottom;
            panelInfoContainer.AutoScroll = true;
            panelInfoContainer.BackColor = Color.Transparent;

            panel_output.Controls.Add(panelOutputTitle);
            panel_output.Controls.Add(panelMazeContainer);
            panel_output.Controls.Add(panelInfoContainer);

            panel_output.Resize += (sender, e) =>
            {
                int parentWidth = panel_output.Width;
                int parentHeight = panel_output.Height;

                // Resize Panel Output Title
                panelOutputTitle.Width = parentWidth;
                panelOutputTitle.Height = (int)(parentHeight * 0.1);

                // Resize Panel Maze Container
                panelMazeContainer.Width = parentWidth;
                panelMazeContainer.Height = (int)(parentHeight * 0.75);
                panelMazeContainer.Top = panelOutputTitle.Bottom;

                // Resize Panel Info Container
                panelInfoContainer.Width = parentWidth;
                panelInfoContainer.Height = (int)(parentHeight * 0.15);
                panelInfoContainer.Top = panelMazeContainer.Bottom;
            };
        }

        #endregion

        // attributes
        internal int formWidth;
        internal int formHeight;
        internal Panel panel_input;
        internal Panel panel_output;
        internal Panel panelOutputTitle;
        internal Panel panelMazeContainer;
        internal Panel panelInfoContainer;
        internal Panel inputPanel;
        internal Panel filePanel;
        internal Panel algorithmPanel;
        internal Panel runPanel;
        private Panel panel_title;
        private PictureBox hr;
    }
}