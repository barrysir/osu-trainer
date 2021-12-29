
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace osu_trainer.Forms
{
    partial class ExportBeatmapForm
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
            this.minimizeButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.exportButtonPanel = new System.Windows.Forms.Panel();
            this.panelForMargin = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.exportBeatmapButton = new osu_trainer.Controls.OsuButton();
            this.uploadBeatmapButton = new osu_trainer.Controls.OsuButton();
            this.exportButtonPanel.SuspendLayout();
            this.panelForMargin.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // minimizeButton
            // 
            this.minimizeButton.Location = new System.Drawing.Point(0, 0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(75, 23);
            this.minimizeButton.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(0, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 100);
            this.panel1.TabIndex = 15;
            // 
            // exportButtonPanel
            // 
            this.exportButtonPanel.Controls.Add(this.panelForMargin);
            this.exportButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.exportButtonPanel.Location = new System.Drawing.Point(0, 326);
            this.exportButtonPanel.Name = "exportButtonPanel";
            this.exportButtonPanel.Size = new System.Drawing.Size(340, 35);
            this.exportButtonPanel.TabIndex = 0;
            // 
            // panelForMargin
            // 
            this.panelForMargin.Controls.Add(this.tableLayoutPanel1);
            this.panelForMargin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForMargin.Location = new System.Drawing.Point(0, 0);
            this.panelForMargin.Name = "panelForMargin";
            this.panelForMargin.Size = new System.Drawing.Size(340, 35);
            this.panelForMargin.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.64706F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.35294F));
            this.tableLayoutPanel1.Controls.Add(this.exportBeatmapButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.uploadBeatmapButton, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(340, 35);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // exportBeatmapButton
            // 
            this.exportBeatmapButton.AutoSize = true;
            this.exportBeatmapButton.BrightnessRange = 0.01F;
            this.exportBeatmapButton.Color = System.Drawing.Color.CornflowerBlue;
            this.exportBeatmapButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportBeatmapButton.ForeColor = System.Drawing.Color.White;
            this.exportBeatmapButton.Location = new System.Drawing.Point(3, 3);
            this.exportBeatmapButton.Margin = new System.Windows.Forms.Padding(3, 3, 2, 3);
            this.exportBeatmapButton.Name = "exportBeatmapButton";
            this.exportBeatmapButton.Progress = 0F;
            this.exportBeatmapButton.ProgressColor = System.Drawing.Color.Transparent;
            this.exportBeatmapButton.Size = new System.Drawing.Size(208, 29);
            this.exportBeatmapButton.Subtext = "";
            this.exportBeatmapButton.SubtextColor = System.Drawing.Color.Empty;
            this.exportBeatmapButton.TabIndex = 0;
            this.exportBeatmapButton.Text = "Export Beatmap";
            this.exportBeatmapButton.TextYOffset = 0;
            this.exportBeatmapButton.TriangleCount = 30;
            this.exportBeatmapButton.UseVisualStyleBackColor = true;
            // 
            // uploadBeatmapButton
            // 
            this.uploadBeatmapButton.BrightnessRange = 0.01F;
            this.uploadBeatmapButton.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.uploadBeatmapButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uploadBeatmapButton.ForeColor = System.Drawing.Color.White;
            this.uploadBeatmapButton.Location = new System.Drawing.Point(214, 3);
            this.uploadBeatmapButton.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.uploadBeatmapButton.Name = "uploadBeatmapButton";
            this.uploadBeatmapButton.Progress = 0F;
            this.uploadBeatmapButton.ProgressColor = System.Drawing.Color.Transparent;
            this.uploadBeatmapButton.Size = new System.Drawing.Size(123, 29);
            this.uploadBeatmapButton.Subtext = "";
            this.uploadBeatmapButton.SubtextColor = System.Drawing.Color.Empty;
            this.uploadBeatmapButton.TabIndex = 1;
            this.uploadBeatmapButton.Text = "Upload Beatmap";
            this.uploadBeatmapButton.TextYOffset = 0;
            this.uploadBeatmapButton.TriangleCount = 30;
            this.uploadBeatmapButton.UseVisualStyleBackColor = true;
            // 
            // ExportBeatmapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 361);
            this.Controls.Add(this.exportButtonPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ExportBeatmapForm";
            this.Text = "Export Beatmap";
            this.Load += new System.EventHandler(this.ExportBeatmapForm_Load);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.exportButtonPanel, 0);
            this.exportButtonPanel.ResumeLayout(false);
            this.panelForMargin.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button minimizeButton;
        private Button closeButton;
        private Panel panel1;
        private Panel exportButtonPanel;
        private Panel panelForMargin;
        private TableLayoutPanel tableLayoutPanel1;
        private Controls.OsuButton exportBeatmapButton;
        private Controls.OsuButton uploadBeatmapButton;
    }
}