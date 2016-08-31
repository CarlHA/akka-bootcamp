namespace ChartApp
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.DiskButton = new System.Windows.Forms.Button();
            this.MemoryButton = new System.Windows.Forms.Button();
            this.CpuButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // sysChart
            // 
            chartArea4.Name = "ChartArea1";
            this.sysChart.ChartAreas.Add(chartArea4);
            this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Name = "Legend1";
            this.sysChart.Legends.Add(legend4);
            this.sysChart.Location = new System.Drawing.Point(0, 0);
            this.sysChart.Name = "sysChart";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.sysChart.Series.Add(series4);
            this.sysChart.Size = new System.Drawing.Size(684, 446);
            this.sysChart.TabIndex = 0;
            this.sysChart.Text = "sysChart";
            // 
            // DiskButton
            // 
            this.DiskButton.Location = new System.Drawing.Point(564, 379);
            this.DiskButton.Name = "DiskButton";
            this.DiskButton.Size = new System.Drawing.Size(108, 23);
            this.DiskButton.TabIndex = 1;
            this.DiskButton.Text = "DISK (OFF)";
            this.DiskButton.UseVisualStyleBackColor = true;
            this.DiskButton.Click += new System.EventHandler(this.DiskButton_Click);
            // 
            // MemoryButton
            // 
            this.MemoryButton.Location = new System.Drawing.Point(564, 350);
            this.MemoryButton.Name = "MemoryButton";
            this.MemoryButton.Size = new System.Drawing.Size(108, 23);
            this.MemoryButton.TabIndex = 2;
            this.MemoryButton.Text = "MEMORY (OFF)";
            this.MemoryButton.UseVisualStyleBackColor = true;
            this.MemoryButton.Click += new System.EventHandler(this.MemoryButton_Click);
            // 
            // CpuButton
            // 
            this.CpuButton.Location = new System.Drawing.Point(564, 321);
            this.CpuButton.Name = "CpuButton";
            this.CpuButton.Size = new System.Drawing.Size(108, 23);
            this.CpuButton.TabIndex = 3;
            this.CpuButton.Text = "CPU (ON)";
            this.CpuButton.UseVisualStyleBackColor = true;
            this.CpuButton.Click += new System.EventHandler(this.CpuButton_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 446);
            this.Controls.Add(this.CpuButton);
            this.Controls.Add(this.MemoryButton);
            this.Controls.Add(this.DiskButton);
            this.Controls.Add(this.sysChart);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart sysChart;
        private System.Windows.Forms.Button DiskButton;
        private System.Windows.Forms.Button MemoryButton;
        private System.Windows.Forms.Button CpuButton;
    }
}

