using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuText
{
    class Tools
    {
        public static string ColorToHex(Color color)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;

            int ir = (int)(r * 255);
            int ig = (int)(g * 255);
            int ib = (int)(b * 255);

            string hr = string.Format("{0:x}", ir);
            if (hr.Length == 1) hr = "0" + hr;
            string hg = string.Format("{0:x}", ig);
            if (hg.Length == 1) hg = "0" + hg;
            string hb = string.Format("{0:x}", ib);
            if (hb.Length == 1) hb = "0" + hb;

            return "<#" + hr + hg + hb + ">";
        }

    }
}
