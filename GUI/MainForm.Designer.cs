namespace GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tbTrainError = new System.Windows.Forms.TextBox();
            this.tbPredictError = new System.Windows.Forms.TextBox();
            this.btnPredict = new System.Windows.Forms.Button();
            this.btnTrain = new System.Windows.Forms.Button();
            this.tbRightPredictions = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Name = "chart";
            this.chart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(1066, 504);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";
            this.chart.Click += new System.EventHandler(this.chart_Click);
            // 
            // tbTrainError
            // 
            this.tbTrainError.Location = new System.Drawing.Point(56, 71);
            this.tbTrainError.MaxLength = 4;
            this.tbTrainError.Name = "tbTrainError";
            this.tbTrainError.ReadOnly = true;
            this.tbTrainError.Size = new System.Drawing.Size(171, 20);
            this.tbTrainError.TabIndex = 1;
            // 
            // tbPredictError
            // 
            this.tbPredictError.Location = new System.Drawing.Point(277, 71);
            this.tbPredictError.Name = "tbPredictError";
            this.tbPredictError.ReadOnly = true;
            this.tbPredictError.Size = new System.Drawing.Size(203, 20);
            this.tbPredictError.TabIndex = 1;
            // 
            // btnPredict
            // 
            this.btnPredict.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnPredict.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPredict.ForeColor = System.Drawing.SystemColors.Control;
            this.btnPredict.Location = new System.Drawing.Point(277, 21);
            this.btnPredict.Name = "btnPredict";
            this.btnPredict.Size = new System.Drawing.Size(203, 40);
            this.btnPredict.TabIndex = 0;
            this.btnPredict.Text = "Прогнозирование (1 прогон)";
            this.btnPredict.UseVisualStyleBackColor = false;
            this.btnPredict.Click += new System.EventHandler(this.BtnPredict_Click);
            // 
            // btnTrain
            // 
            this.btnTrain.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnTrain.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnTrain.ForeColor = System.Drawing.SystemColors.Control;
            this.btnTrain.Location = new System.Drawing.Point(24, 21);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(203, 40);
            this.btnTrain.TabIndex = 0;
            this.btnTrain.Text = "Обучение";
            this.btnTrain.UseVisualStyleBackColor = false;
            this.btnTrain.Click += new System.EventHandler(this.BtnTrain_Click);
            // 
            // tbRightPredictions
            // 
            this.tbRightPredictions.Location = new System.Drawing.Point(277, 97);
            this.tbRightPredictions.Name = "tbRightPredictions";
            this.tbRightPredictions.ReadOnly = true;
            this.tbRightPredictions.Size = new System.Drawing.Size(203, 20);
            this.tbRightPredictions.TabIndex = 1;
            this.tbRightPredictions.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(21, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "СКО";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbRightPredictions);
            this.panel1.Controls.Add(this.tbTrainError);
            this.panel1.Controls.Add(this.tbPredictError);
            this.panel1.Controls.Add(this.btnPredict);
            this.panel1.Controls.Add(this.btnTrain);
            this.panel1.Location = new System.Drawing.Point(0, 510);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1108, 133);
            this.panel1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 641);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart);
            this.Name = "MainForm";
            this.Text = "Прогнозирование курса валют / Криптовалюта Эфир";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Button btnTrain;
        private System.Windows.Forms.TextBox tbPredictError;
        private System.Windows.Forms.TextBox tbTrainError;
        private System.Windows.Forms.Button btnPredict;
        private System.Windows.Forms.TextBox tbRightPredictions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}

