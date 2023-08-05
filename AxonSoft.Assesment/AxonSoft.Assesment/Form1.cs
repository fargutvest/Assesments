using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxonSoft.Assesment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string absPathToInputData = Path.Combine(Directory.GetCurrentDirectory(), Properties.Resources.RelativePathToInputData);
            ReadInputData(absPathToInputData);
        }

        private void ReadInputData(string pathToInputData)
        {
            string[] lines = File.ReadAllLines(pathToInputData);
        }
    }
}
