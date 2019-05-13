using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace CitirocUI
{
    public partial class maskCheckBox : CheckBox
    {
        public maskCheckBox()
        {
            InitializeComponent();
        }

        static Color WeerocLightBlue = Color.FromArgb(255, 0, 121, 144);

        private Color _checkedBackColor = WeerocLightBlue;
        [DefaultValue(typeof(Color), "0x007990")]
        public Color checkedBackColor
        {
            get { return _checkedBackColor; }
            set { _checkedBackColor = value; }
        }

        private Color _checkedForeColor = Color.White;
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        public Color checkedForeColor
        {
            get { return _checkedForeColor; }
            set { _checkedForeColor = value; }
        }

        private Color _uncheckedBackColor = Color.White;
        [DefaultValue(typeof(Color), "0xFFFFFF")]
        public Color uncheckedBackColor
        {
            get { return _uncheckedBackColor; }
            set { _uncheckedBackColor = value; }
        }

        private Color _uncheckedForeColor = Color.Black;
        [DefaultValue(typeof(Color), "0x000000")]
        public Color uncheckedForeColor
        {
            get { return _uncheckedForeColor; }
            set { _uncheckedForeColor = value; }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Call base class’ OnPaint
            base.OnPaint(pevent);
            // padding of the standard CheckBox
            int offset = 1;
            // both faces of check box square are checkBoxWidth
            int checkBoxWidth = (int)(this.Size.Height * 0.9);
            Graphics graphics = pevent.Graphics;
            graphics.Clear(BackColor);
            // get Text measure according to selected Font
            SizeF stringMeasure = graphics.MeasureString(Text, Font);
            int stringWidth = Convert.ToInt32(stringMeasure.Width);
            int stringHeight = Convert.ToInt32(stringMeasure.Height);
            // change textcolor
            Color _foreColor = Color.Black;
            // calculate offsets
            int leftOffset = offset + Padding.Left;
            int topOffset = 1;//(int)(ClientRectangle.Height - (stringMeasure.Height / 2));
            if (topOffset < 0) { topOffset = offset + Padding.Top; } else { topOffset += Padding.Top; }
            if (Checked)
            {
                // Fill CheckBox's rectangle 
                graphics.FillRectangle(new SolidBrush(checkedBackColor), leftOffset + 2, topOffset + 2, checkBoxWidth - 3, checkBoxWidth - 3);
                // Draw Checkox's rectangle 
                graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                _foreColor = checkedForeColor;
            }
            else
            {
                // Fill CheckBox's rectangle 
                graphics.FillRectangle(new SolidBrush(uncheckedBackColor), leftOffset + 2, topOffset + 2, checkBoxWidth - 3, checkBoxWidth - 3);
                // Draw Checkox's rectangle 
                graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                _foreColor = uncheckedForeColor;
            }
            graphics.DrawString(Text, Font, new SolidBrush(_foreColor), new Point(leftOffset + checkBoxWidth / 2 - stringWidth / 2, topOffset + checkBoxWidth / 2 - stringHeight / 2 + 1));
        }
    }

    public partial class checkBox : CheckBox
    {
        public checkBox()
        {
            InitializeComponent();
        }

        static Color WeerocLightBlue = Color.FromArgb(255, 0, 121, 144);
        static Color WeerocDisableLightBlue = Color.FromArgb(255, 72, 132, 144);

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Call base class’ OnPaint
            base.OnPaint(pevent);
            // padding of the standard CheckBox
            int offset = 1;
            // distance betwen CheckBox area and included label
            int distance = 5;
            // both faces of check box square
            int checkBoxWidth = this.Size.Height / 2;
            Graphics graphics = pevent.Graphics;
            graphics.Clear(BackColor);
            // get Text measure according to selected Font
            SizeF stringMeasure = graphics.MeasureString(Text, Font);
            FontFamily ff = Font.FontFamily;
            float lineSpace = ff.GetLineSpacing(Font.Style);
            float ascent = ff.GetCellAscent(Font.Style);
            float baseline = Font.GetHeight(graphics) * ascent / lineSpace;
            // calculate offsets
            int leftOffset = offset + Padding.Left;
            int topOffset = (int)(baseline - checkBoxWidth);
            if (topOffset < 0) { topOffset = offset + Padding.Top; } else { topOffset += Padding.Top; }
            if (Checked)
            {
                if (Enabled)
                {
                    // Fill CheckBox's rectangle 
                    graphics.FillRectangle(new SolidBrush(WeerocLightBlue), leftOffset + 2, topOffset + 2, checkBoxWidth - 3, checkBoxWidth - 3);
                    // Draw Checkox's rectangle 
                    graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                }
                else
                {
                    // Fill CheckBox's rectangle 
                    graphics.FillRectangle(new SolidBrush(Color.LightGray), leftOffset + 2, topOffset + 2, checkBoxWidth - 3, checkBoxWidth - 3);
                    // Draw Checkox's rectangle 
                    graphics.DrawRectangle(new Pen(Color.LightGray, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                }
            }
            else
            {
                // Fill CheckBox's rectangle 
                graphics.FillRectangle(new SolidBrush(Color.White), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                // Draw Checkox's rectangle 
                if (Enabled) graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
                else graphics.DrawRectangle(new Pen(Color.Gray, 1), leftOffset, topOffset, checkBoxWidth, checkBoxWidth);
            }
            if (Enabled) graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Point(leftOffset + checkBoxWidth + distance, 0));
            else graphics.DrawString(Text, Font, new SolidBrush(Color.Gray), new Point(leftOffset + checkBoxWidth + distance, 0));
        }
    }

    public partial class switchBox : CheckBox
    {
        static Color WeerocLightBlue = Color.FromArgb(255, 0, 121, 144);

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Call base class’ OnPaint
            base.OnPaint(pevent);
            // padding of the standard CheckBox
            int offset = 1;
            // distance betwen CheckBox area and included label
            int distance = 5;
            // set the size of the box
            int checkBoxWidth = (int)(this.Size.Height * 0.8);
            int checkBoxHeight = this.Size.Height / 2;
            this.Padding = new Padding(0, 0, checkBoxWidth / 2, 0);
            Graphics graphics = pevent.Graphics;
            graphics.Clear(BackColor);
            // get Text measure according to selected Font
            SizeF stringMeasure = graphics.MeasureString(Text, Font);
            FontFamily ff = Font.FontFamily;
            float lineSpace = ff.GetLineSpacing(Font.Style);
            float ascent = ff.GetCellAscent(Font.Style);
            float baseline = Font.GetHeight(graphics) * ascent / lineSpace;
            // calculate offsets
            int leftOffset = offset + Padding.Left;
            int topOffset = (int)(baseline - checkBoxHeight);
            if (topOffset < 0) { topOffset = offset + Padding.Top; } else { topOffset += Padding.Top; }
            if (Checked)
            {
                // Fill CheckBox's rectangle
                graphics.FillRectangle(new SolidBrush(WeerocLightBlue), leftOffset + checkBoxWidth / 2 + 1, topOffset, checkBoxWidth / 2 - 1, checkBoxHeight);
                // Draw Checkbox's rectangle
                graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxHeight);
                // Draw white border
                Point[] points = new Point[] { new Point(leftOffset + 1, topOffset + 1), new Point(leftOffset + 1, checkBoxHeight + topOffset - 1), new Point(checkBoxWidth, checkBoxHeight + topOffset - 1), new Point(checkBoxWidth, topOffset + 1), new Point(leftOffset + 1, topOffset + 1) };
                graphics.DrawLines(new Pen(Color.White, 1), points);
            }
            else
            {
                // Fill CheckBox's rectangle
                graphics.FillRectangle(new SolidBrush(WeerocLightBlue), leftOffset, topOffset, checkBoxWidth / 2, checkBoxHeight);
                // Draw Checkbox's rectangle
                graphics.DrawRectangle(new Pen(Color.Black, 1), leftOffset, topOffset, checkBoxWidth, checkBoxHeight);
                // Draw white border
                Point[] points = new Point[] { new Point(leftOffset + 1, topOffset + 1), new Point(leftOffset + 1, checkBoxHeight + topOffset - 1), new Point(checkBoxWidth, checkBoxHeight + topOffset - 1), new Point(checkBoxWidth, topOffset + 1), new Point(leftOffset + 1, topOffset + 1) };
                graphics.DrawLines(new Pen(Color.White, 1), points);
            }
            graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new Point(leftOffset + checkBoxWidth + distance, 0));
        }
    }

    public partial class roundButton : Button
    {
        public roundButton()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath grPath = new GraphicsPath();
            grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            Region = new Region(grPath);
            base.OnPaint(e);
        }
    }

    public class intTextBox : TextBox
    {
        private bool _enableNegative;

        public bool enableNegative
        {
            get { return _enableNegative; }
            set { _enableNegative = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            System.Globalization.NumberFormatInfo fi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            string c = e.KeyChar.ToString();
            if (char.IsDigit(c, 0))
                return;

            if ((SelectionStart == 0) && (c.Equals(fi.NegativeSign) && _enableNegative))
                return;

            // copy/paste
            if ((((int)e.KeyChar == 22) || ((int)e.KeyChar == 3))
                && ((ModifierKeys & Keys.Control) == Keys.Control))
                return;

            if (e.KeyChar == '\b')
                return;

            e.Handled = true;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_PASTE = 0x0302;
            if (m.Msg == WM_PASTE)
            {
                string text = Clipboard.GetText();
                if (string.IsNullOrEmpty(text))
                    return;

                if ((text.IndexOf('+') >= 0) && (SelectionStart != 0))
                    return;

                int i;
                if (!int.TryParse(text, out i)) // change this for other integer types
                    return;

                if ((i < 0) && (SelectionStart != 0))
                    return;
            }
            base.WndProc(ref m);
        }
    }

    public class doubleTextBox : TextBox
    {
        private bool _enableNegative;

        public bool enableNegative
        {
            get { return _enableNegative; }
            set { _enableNegative = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            System.Globalization.NumberFormatInfo fi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            char decimalSeparator = (fi.NumberDecimalSeparator.ToCharArray())[0];

            string c = e.KeyChar.ToString();
            if (char.IsDigit(c, 0))
                return;

            if ((SelectionStart == 0) && (c.Equals(fi.NegativeSign) && _enableNegative))
                return;

            // copy/paste
            if ((((int)e.KeyChar == 22) || ((int)e.KeyChar == 3))
                && ((ModifierKeys & Keys.Control) == Keys.Control))
                return;

            if (e.KeyChar == '\b')
                return;

            if (e.KeyChar == decimalSeparator && (SelectionStart != 0) && Text.IndexOf(decimalSeparator) == -1)
                return;

            e.Handled = true;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_PASTE = 0x0302;
            if (m.Msg == WM_PASTE)
            {
                string text = Clipboard.GetText();
                if (string.IsNullOrEmpty(text))
                    return;

                if ((text.IndexOf('+') >= 0) && (SelectionStart != 0))
                    return;

                double i;
                if (!double.TryParse(text, out i))
                    return;

                if ((i < 0) && (SelectionStart != 0))
                    return;
            }
            base.WndProc(ref m);
        }
    }
}
