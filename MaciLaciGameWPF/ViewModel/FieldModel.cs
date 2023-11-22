using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MaciLaciGameWPF
{
  public class FieldModel : ViewModelBase 
  {
        private Color color;
        public FieldModel() { }

        public int X { get; set; }
        public int Y { get; set; }

        public Color Color { get { return color; } set { color = value; OnpropertyChange(); } }
  }
}
