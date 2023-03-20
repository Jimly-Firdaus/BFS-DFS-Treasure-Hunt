using System.Diagnostics;

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

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel_input = new Panel();
            panel_output = new Panel();
            panel_title = new Panel();
            SuspendLayout();

            // Form dimensions
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 414);

            formWidth = ClientRectangle.Size.Width;
            formHeight = ClientRectangle.Size.Height;
            Debug.WriteLine(formWidth);
            Debug.WriteLine(formHeight);

            // 
            // panel_input
            // 
            panel_input.BackColor = Color.LightPink;
            panel_input.Location = new Point((int)(0.05 * formWidth), (int)(0.2 * formHeight));
            panel_input.Margin = new Padding(2, 1, 2, 1);
            panel_input.Name = "panel_input";
            panel_input.Size = new Size((int)(0.225 * formWidth), (int)(0.75 * formHeight));
            panel_input.TabIndex = 0;
            // 
            // panel_output
            // 
            panel_output.BackColor = Color.Yellow;
            panel_output.Location = new Point((int)(0.275 * formWidth), (int)(0.2 * formHeight));
            panel_output.Margin = new Padding(2, 1, 2, 1);
            panel_output.Name = "panel_output";
            panel_output.Size = new Size((int)(0.675 * formWidth), (int)(0.75 * formHeight));
            panel_output.TabIndex = 1;
            // 
            // panel_title
            // 
            panel_title.BackColor = Color.Yellow;
            panel_title.Location = new Point((int)(0.05 * formWidth), (int)(0.05 * formHeight));
            panel_title.Name = "panel_title";
            panel_title.Size = new Size((int)(0.9 * formWidth), (int)(0.1 * formHeight));
            panel_title.TabIndex = 2;
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

            // output_panel children
            createOutputPanel();
        }

        private void GoblinForm_Resize(object sender, EventArgs e)
        {
            // Update the client size to match the new size of the client area
            ClientSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            // Update form size
            formWidth = ClientRectangle.Size.Width;
            formHeight = ClientRectangle.Size.Height;

            // update all panel
            panel_input.Location = new Point((int)(0.05 * formWidth), (int)(0.2 * formHeight));
            panel_input.Size = new Size((int)(0.225 * formWidth), (int)(0.75 * formHeight));

            panel_output.Location = new Point((int)(0.275 * formWidth), (int)(0.2 * formHeight));
            panel_output.Size = new Size((int)(0.675 * formWidth), (int)(0.75 * formHeight));

            panel_title.Location = new Point((int)(0.05 * formWidth), (int)(0.05 * formHeight));
            panel_title.Size = new Size((int)(0.9 * formWidth), (int)(0.1 * formHeight));
        }

        private void createOutputPanel()
        {
            int parentWidth = panel_output.Width;
            int parentHeight = panel_output.Height;

            // Create Panel Output Title
            panelOutputTitle = new Panel();
            panelOutputTitle.Width = parentWidth;
            panelOutputTitle.Height = (int)(parentHeight * 0.1);
            panelOutputTitle.BackColor = Color.LightGreen;
            panelOutputTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Create Panel Maze Container
            panelMazeContainer = new Panel();
            panelMazeContainer.Width = parentWidth;
            panelMazeContainer.Height = (int)(parentHeight * 0.75);
            panelMazeContainer.BackColor = Color.LightBlue;
            panelMazeContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelMazeContainer.Top = panelOutputTitle.Bottom;

            // Create Panel Info Container
            panelInfoContainer = new Panel();
            panelInfoContainer.Width = parentWidth;
            panelInfoContainer.Height = (int)(parentHeight * 0.15);
            panelInfoContainer.BackColor = Color.LightGreen;
            panelInfoContainer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelInfoContainer.Top = panelMazeContainer.Bottom;

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

        internal int formWidth;
        internal int formHeight;
        internal Panel panel_input;
        internal Panel panel_output;
        internal Panel panelOutputTitle;
        internal Panel panelMazeContainer;
        internal Panel panelInfoContainer;
        private Panel panel_title;
    }
}