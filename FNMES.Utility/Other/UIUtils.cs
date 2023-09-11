#if NETFRAMEWORK || WINDOWS
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FNMES.Utility.Other
{

    /// <summary>
    /// UI控件多线程赋值专用
    /// </summary>
    public static class UIUtils
    {
        public delegate void InvokeBackColorDelegate(Control control, Color color);
        public delegate void InvokeTextDelegate(Control control, string text);
        public delegate void InvokeVisibleDelegate(Control control, bool enable);
        public delegate void InvokeDataDelegate(Control control, DataTable dt);
        public delegate void InvokeLineDelegate(Control control, string[] line);
        public delegate void InvokeAppendLineDelegate(Control control, string line);
        public delegate void InvokeAppendLineMaxDelegate(Control control, string line);
        public delegate void InvokeTabIndex(UITabControl control, int index);
        public delegate void InvokMESStateDelegate(UILight light, UILightState state);
        public delegate void InvokeProgressDelegate(UIProcessBar control, int value);
        public static void InvokeProgressValue(this UIProcessBar control, int value)
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(new InvokeProgressDelegate(InvokeProgressValue), new object[] { control, value });
                }
                else
                {
                    control.Value = value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void InvokMESState(this UILight light, UILightState state)
        {
            if (light.InvokeRequired)
            {
                light.BeginInvoke(new InvokMESStateDelegate(InvokMESState), new object[] { light, state });
            }
            else
            {
                light.State = state;
            }
        }


        public static void InvokeBackColor(this Control control, Color color)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeBackColorDelegate(InvokeBackColor), new object[] { control, color });
            }
            else
            {
                control.BackColor = color;
            }
        }

        public static void InvokeVisible(this Control control, bool enable)
        {

            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeVisibleDelegate(InvokeVisible), new object[] { control, enable });
            }
            else
            {
                control.Visible = enable;
            }
        }


        public static void InvokeText(this Control control, string text)
        {

            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeTextDelegate(InvokeText), new object[] { control, text });
            }
            else
            {
                if (control is UITextBox)
                {
                    ((UITextBox)control).Text = text;
                }
                else if (control is TextBox)
                {
                    ((TextBox)control).Text = text;
                }
                else if (control is UILabel)
                {
                    ((UILabel)control).Text = text;
                }
                else if (control is Label)
                {
                    ((Label)control).Text = text;
                }
                else if (control is UIButton)
                {
                    ((UIButton)control).Text = text;
                }
                else if (control is Button)
                {
                    ((Button)control).Text = text;
                }
                else if (control is UIGroupBox)
                {
                    ((UIGroupBox)control).Text = text;
                }
            }
        }
        public static void InvokeData(this Control control, DataTable dt)
        {

            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeDataDelegate(InvokeData), new object[] { control, dt });
            }
            else
            {
                if (control is UIDataGridView)
                {
                    UIDataGridView dgv = ((UIDataGridView)control);
                    dgv.DataSource = dt;
                    dgv.ClearSelection();
                    dgv.Refresh();
                }
                else if (control is DataGridView)
                {
                    DataGridView dgv = ((DataGridView)control);
                    dgv.DataSource = dt;
                    dgv.ClearSelection();
                    dgv.Refresh();
                }
            }
        }

        public static void InvokeLine(this Control control, string[] line)
        {

            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeLineDelegate(InvokeLine), new object[] { control, line });
            }
            else
            {
                if (control is UITextBox)
                {
                    UITextBox tb = ((UITextBox)control);
                    tb.Lines = line;
                }
                else if (control is TextBox)
                {
                    TextBox tb = ((TextBox)control);
                    tb.Lines = line;
                }
            }
        }
        public static void InvokeLine(this Control control, List<string> line)
        {
            InvokeLine(control, line.ToArray());
        }

        public static void InvokeAppendLine(this Control control, string value)
        {

            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeAppendLineDelegate(InvokeAppendLine), new object[] { control, value });
            }
            else
            {
                if (control is UITextBox)
                {
                    UITextBox component = (UITextBox)control;
                    List<string> lines = component.Lines.ToList();
                    lines.Add(value);
                    component.Lines = lines.ToArray();
                    component.Focus();//获取焦点
                    component.Select(component.TextLength, 0);//光标定位到文本最后
                    component.ScrollToCaret();//滚动到光标处
                }
                else if (control is TextBox)
                {
                    TextBox component = (TextBox)control;
                    List<string> lines = component.Lines.ToList();
                    lines.Add(value);
                    component.Lines = lines.ToArray();
                    component.Focus();//获取焦点
                    component.Select(component.TextLength, 0);//光标定位到文本最后
                    component.ScrollToCaret();//滚动到光标处
                }

            }
        }

        public static void InvokeAppendLine(this Control control, string value, int maxLineNum)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeAppendLineMaxDelegate(InvokeAppendLine), new object[] { control, value, maxLineNum });
            }
            else
            {
                if (control is UITextBox)
                {
                    UITextBox component = (UITextBox)control;
                    List<string> lines = component.Lines.ToList();
                    lines.Add(value);
                    if (lines.Count > maxLineNum)
                    {
                        int cha = lines.Count - maxLineNum;
                        //lines.Reverse();
                        //差多少，删多少
                        for (int i = 0; i < cha; i++)
                        {
                            lines.RemoveAt(0);
                        }
                        //lines.Reverse();
                    }
                    component.Lines = lines.ToArray();
                    component.Focus();//获取焦点
                    component.Select(component.TextLength, 0);//光标定位到文本最后
                    component.ScrollToCaret();//滚动到光标处
                }
                else if (control is TextBox)
                {
                    TextBox component = (TextBox)control;
                    List<string> lines = component.Lines.ToList();
                    lines.Add(value);
                    if (lines.Count > maxLineNum)
                    {
                        int cha = lines.Count - maxLineNum;
                        //lines.Reverse();
                        //差多少，删多少
                        for (int i = 0; i < cha; i++)
                        {
                            lines.RemoveAt(0);
                        }
                        //lines.Reverse();
                    }
                    component.Lines = lines.ToArray();
                    component.Focus();//获取焦点
                    component.Select(component.TextLength, 0);//光标定位到文本最后
                    component.ScrollToCaret();//滚动到光标处
                }

            }

        }

        public static void InvokeIndex(this UITabControl control, int index)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(new InvokeTabIndex(InvokeIndex), new object[] { control, index });
            }
            else
            {
                control.SelectedIndex = index;
            }
        }
    }
}
#endif