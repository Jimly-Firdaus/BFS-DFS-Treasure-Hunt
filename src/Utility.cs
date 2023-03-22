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