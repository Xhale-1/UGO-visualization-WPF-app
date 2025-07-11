using e3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_interface
{
    public class SystemEvents
    {


        public static System.Windows.Point getpos()
        {
            System.Windows.Point getpos = new System.Windows.Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
            return getpos;
        }



    }


}
