namespace 测试
{
    partial class Form1
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
            start = new Button();
            stop = new Button();
            delay = new NumericUpDown();
            label1 = new Label();
            asyn = new NumericUpDown();
            label2 = new Label();
            time = new NumericUpDown();
            label3 = new Label();
            status = new TextBox();
            success = new TextBox();
            fail = new TextBox();
            richTextBox1 = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)delay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)asyn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)time).BeginInit();
            SuspendLayout();
            // 
            // start
            // 
            start.Location = new Point(399, 274);
            start.Margin = new Padding(2, 3, 2, 3);
            start.Name = "start";
            start.Size = new Size(73, 25);
            start.TabIndex = 0;
            start.Text = "执行";
            start.UseVisualStyleBackColor = true;
            start.Click += start_Click;
            // 
            // stop
            // 
            stop.Location = new Point(506, 274);
            stop.Margin = new Padding(2, 3, 2, 3);
            stop.Name = "stop";
            stop.Size = new Size(73, 25);
            stop.TabIndex = 0;
            stop.Text = "停止";
            stop.UseVisualStyleBackColor = true;
            stop.Click += stop_Click;
            // 
            // delay
            // 
            delay.Location = new Point(203, 123);
            delay.Margin = new Padding(2, 3, 2, 3);
            delay.Name = "delay";
            delay.Size = new Size(117, 23);
            delay.TabIndex = 1;
            delay.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(202, 104);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 2;
            label1.Text = "延时";
            // 
            // asyn
            // 
            asyn.Location = new Point(203, 182);
            asyn.Margin = new Padding(2, 3, 2, 3);
            asyn.Name = "asyn";
            asyn.Size = new Size(117, 23);
            asyn.TabIndex = 1;
            asyn.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(202, 162);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 2;
            label2.Text = "并发";
            // 
            // time
            // 
            time.Location = new Point(202, 243);
            time.Margin = new Padding(2, 3, 2, 3);
            time.Name = "time";
            time.Size = new Size(117, 23);
            time.TabIndex = 1;
            time.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(201, 224);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(32, 17);
            label3.TabIndex = 2;
            label3.Text = "时长";
            // 
            // status
            // 
            status.Location = new Point(3, 357);
            status.Name = "status";
            status.Size = new Size(197, 23);
            status.TabIndex = 3;
            // 
            // success
            // 
            success.Location = new Point(201, 357);
            success.Name = "success";
            success.Size = new Size(197, 23);
            success.TabIndex = 3;
            // 
            // fail
            // 
            fail.Location = new Point(399, 357);
            fail.Name = "fail";
            fail.Size = new Size(197, 23);
            fail.TabIndex = 3;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(365, 22);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(214, 183);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 382);
            Controls.Add(richTextBox1);
            Controls.Add(fail);
            Controls.Add(success);
            Controls.Add(status);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(time);
            Controls.Add(asyn);
            Controls.Add(delay);
            Controls.Add(stop);
            Controls.Add(start);
            Margin = new Padding(2, 3, 2, 3);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)delay).EndInit();
            ((System.ComponentModel.ISupportInitialize)asyn).EndInit();
            ((System.ComponentModel.ISupportInitialize)time).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private Button start;
        private Button stop;
        private NumericUpDown delay;
        private Label label1;
        private NumericUpDown asyn;
        private Label label2;
        private NumericUpDown time;
        private Label label3;
        private TextBox status;
        private TextBox success;
        private TextBox fail;
        private RichTextBox richTextBox1;
    }
}