﻿
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
            this.exportSettings = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.exportModeLabel = new System.Windows.Forms.Label();
            this.exportFolderLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.exportFolderTextBox = new System.Windows.Forms.TextBox();
            this.exportFolderBrowseButton = new System.Windows.Forms.Button();
            this.exportMode = new System.Windows.Forms.ComboBox();
            this.exportButtonPanel = new System.Windows.Forms.Panel();
            this.panelForMargin = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.exportBeatmapButton = new osu_trainer.Controls.OsuButton();
            this.uploadBeatmapButton = new osu_trainer.Controls.OsuButton();
            this.exportOrUploadWorker = new System.ComponentModel.BackgroundWorker();
            this.exportSettings.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            // exportSettings
            // 
            this.exportSettings.Controls.Add(this.tableLayoutPanel2);
            this.exportSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.exportSettings.Location = new System.Drawing.Point(0, 39);
            this.exportSettings.Name = "exportSettings";
            this.exportSettings.Size = new System.Drawing.Size(435, 100);
            this.exportSettings.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.exportModeLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.exportFolderLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.exportMode, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(435, 100);
            this.tableLayoutPanel2.TabIndex = 17;
            // 
            // exportModeLabel
            // 
            this.exportModeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exportModeLabel.AutoSize = true;
            this.exportModeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(154)))), ((int)(((byte)(233)))));
            this.exportModeLabel.Location = new System.Drawing.Point(10, 7);
            this.exportModeLabel.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.exportModeLabel.Name = "exportModeLabel";
            this.exportModeLabel.Size = new System.Drawing.Size(87, 17);
            this.exportModeLabel.TabIndex = 11;
            this.exportModeLabel.Text = "Export Mode";
            this.exportModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exportFolderLabel
            // 
            this.exportFolderLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exportFolderLabel.AutoSize = true;
            this.exportFolderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(154)))), ((int)(((byte)(233)))));
            this.exportFolderLabel.Location = new System.Drawing.Point(10, 40);
            this.exportFolderLabel.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.exportFolderLabel.Name = "exportFolderLabel";
            this.exportFolderLabel.Size = new System.Drawing.Size(91, 17);
            this.exportFolderLabel.TabIndex = 12;
            this.exportFolderLabel.Text = "Export Folder";
            this.exportFolderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.Controls.Add(this.exportFolderTextBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.exportFolderBrowseButton, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(115, 31);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(320, 35);
            this.tableLayoutPanel3.TabIndex = 15;
            // 
            // exportFolderTextBox
            // 
            this.exportFolderTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exportFolderTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.exportFolderTextBox.Location = new System.Drawing.Point(3, 5);
            this.exportFolderTextBox.Name = "exportFolderTextBox";
            this.exportFolderTextBox.Size = new System.Drawing.Size(234, 25);
            this.exportFolderTextBox.TabIndex = 13;
            // 
            // exportFolderBrowseButton
            // 
            this.exportFolderBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.exportFolderBrowseButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportFolderBrowseButton.Location = new System.Drawing.Point(243, 6);
            this.exportFolderBrowseButton.Name = "exportFolderBrowseButton";
            this.exportFolderBrowseButton.Size = new System.Drawing.Size(74, 23);
            this.exportFolderBrowseButton.TabIndex = 14;
            this.exportFolderBrowseButton.Text = "Browse";
            this.exportFolderBrowseButton.UseVisualStyleBackColor = true;
            this.exportFolderBrowseButton.Click += new System.EventHandler(this.exportFolderBrowseButton_Click);
            // 
            // exportMode
            // 
            this.exportMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportMode.FormattingEnabled = true;
            this.exportMode.Location = new System.Drawing.Point(118, 3);
            this.exportMode.Name = "exportMode";
            this.exportMode.Size = new System.Drawing.Size(121, 25);
            this.exportMode.TabIndex = 16;
            // 
            // exportButtonPanel
            // 
            this.exportButtonPanel.Controls.Add(this.panelForMargin);
            this.exportButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.exportButtonPanel.Location = new System.Drawing.Point(0, 202);
            this.exportButtonPanel.Name = "exportButtonPanel";
            this.exportButtonPanel.Size = new System.Drawing.Size(435, 35);
            this.exportButtonPanel.TabIndex = 0;
            // 
            // panelForMargin
            // 
            this.panelForMargin.Controls.Add(this.tableLayoutPanel1);
            this.panelForMargin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForMargin.Location = new System.Drawing.Point(0, 0);
            this.panelForMargin.Name = "panelForMargin";
            this.panelForMargin.Size = new System.Drawing.Size(435, 35);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(435, 35);
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
            this.exportBeatmapButton.Size = new System.Drawing.Size(267, 29);
            this.exportBeatmapButton.Subtext = "";
            this.exportBeatmapButton.SubtextColor = System.Drawing.Color.Empty;
            this.exportBeatmapButton.TabIndex = 0;
            this.exportBeatmapButton.Text = "Export Beatmap";
            this.exportBeatmapButton.TextYOffset = 0;
            this.exportBeatmapButton.TriangleCount = 30;
            this.exportBeatmapButton.UseVisualStyleBackColor = true;
            this.exportBeatmapButton.Click += new System.EventHandler(this.exportBeatmapButton_Click);
            // 
            // uploadBeatmapButton
            // 
            this.uploadBeatmapButton.BrightnessRange = 0.01F;
            this.uploadBeatmapButton.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.uploadBeatmapButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uploadBeatmapButton.ForeColor = System.Drawing.Color.White;
            this.uploadBeatmapButton.Location = new System.Drawing.Point(273, 3);
            this.uploadBeatmapButton.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.uploadBeatmapButton.Name = "uploadBeatmapButton";
            this.uploadBeatmapButton.Progress = 0F;
            this.uploadBeatmapButton.ProgressColor = System.Drawing.Color.Transparent;
            this.uploadBeatmapButton.Size = new System.Drawing.Size(159, 29);
            this.uploadBeatmapButton.Subtext = "";
            this.uploadBeatmapButton.SubtextColor = System.Drawing.Color.Empty;
            this.uploadBeatmapButton.TabIndex = 1;
            this.uploadBeatmapButton.Text = "Upload Beatmap";
            this.uploadBeatmapButton.TextYOffset = 0;
            this.uploadBeatmapButton.TriangleCount = 30;
            this.uploadBeatmapButton.UseVisualStyleBackColor = true;
            // 
            // exportOrUploadWorker
            // 
            this.exportOrUploadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.exportOrUploadWorker_DoWork);
            // 
            // ExportBeatmapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 237);
            this.Controls.Add(this.exportButtonPanel);
            this.Controls.Add(this.exportSettings);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ExportBeatmapForm";
            this.Text = "Export Beatmap";
            this.Load += new System.EventHandler(this.ExportBeatmapForm_Load);
            this.Controls.SetChildIndex(this.exportSettings, 0);
            this.Controls.SetChildIndex(this.exportButtonPanel, 0);
            this.exportSettings.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.exportButtonPanel.ResumeLayout(false);
            this.panelForMargin.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button minimizeButton;
        private Button closeButton;
        private Panel exportSettings;
        private Panel exportButtonPanel;
        private Panel panelForMargin;
        private TableLayoutPanel tableLayoutPanel1;
        private Controls.OsuButton exportBeatmapButton;
        private Controls.OsuButton uploadBeatmapButton;
        private TableLayoutPanel tableLayoutPanel2;
        private Label exportModeLabel;
        private Label exportFolderLabel;
        private TextBox exportFolderTextBox;
        private Button exportFolderBrowseButton;
        private TableLayoutPanel tableLayoutPanel3;
        private ComboBox exportMode;
        private BackgroundWorker exportOrUploadWorker;
    }
}