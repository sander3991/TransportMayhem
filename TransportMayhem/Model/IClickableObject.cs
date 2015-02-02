using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransportMayhem.Model
{
    interface IClickableObject
    {
        void OnClick(MouseButtons button);
    }
}
