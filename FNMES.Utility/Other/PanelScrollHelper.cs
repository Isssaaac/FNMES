#if NETFRAMEWORK || WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FNMES.Utility.Other
{
    public class PanelScrollHelper
    {
        /// <summary>
        /// 初始化panel
        /// </summary>
        /// <param name="panel"></param>
        public static void InitializePanelScroll(Panel panel)
        {
            panel.Click += (obj, arg) => { panel.Select(); };
            InitializePanelScroll(panel, panel);
        }

        /// <summary>
        /// 递归初始化panel的内部个容器和控件
        /// </summary>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        private static void InitializePanelScroll(Control container, Control panelRoot)
        {
            foreach (Control control in container.Controls)
            {
                if (control is Panel || control is GroupBox || control is SplitContainer || control is TabControl || control is UserControl)
                {
                    control.Click += (obj, arg) => { panelRoot.Select(); };
                    InitializePanelScroll(control, panelRoot);
                }
                else if (control is Label)
                {
                    control.Click += (obj, arg) => { panelRoot.Select(); };
                }
            }
        }
    }
}
#endif