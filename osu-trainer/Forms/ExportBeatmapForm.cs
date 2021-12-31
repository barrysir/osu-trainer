using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace osu_trainer.Forms
{
    public partial class ExportBeatmapForm : BaseForms.BorderlessWindowForm
    {
        enum ExportMode
        {
            FULL,
            OSU_MP3_BG,
            OSU_MP3,
            OSU
        }

        struct ExportModeStruct
        {
            string display;
            ExportMode value;

            public ExportModeStruct(string display, ExportMode val)
            {
                this.display = display;
                this.value = val;
            }

            public string Display { get => display; set => display = value; }
            public ExportMode Value { get => value; set => this.value = value; }
        }

        private List<ExportModeStruct> exportModeList;
        private BeatmapEditor editor;

        public ExportBeatmapForm(BeatmapEditor edit)
        {
            InitializeComponent();
            this.editor = edit;
        }

        private void ExportBeatmapForm_Load(object sender, EventArgs e)
        {
            exportFolderTextBox.Text = DefaultExportFolder();

            exportModeList = new List<ExportModeStruct> {
                new ExportModeStruct("Full", ExportMode.FULL),
                new ExportModeStruct(".osu + mp3 + bg", ExportMode.OSU_MP3_BG),
                new ExportModeStruct(".osu + mp3", ExportMode.OSU_MP3),
                new ExportModeStruct(".osu", ExportMode.OSU),
            };
            var bindingSource1 = new BindingSource();
            bindingSource1.DataSource = exportModeList;

            exportMode.DisplayMember = "Display";
            exportMode.ValueMember = "Value";
            exportMode.DataSource = bindingSource1.DataSource;

            exportMode.SelectedIndex = 0;
        }

        public static string DefaultExportFolder() => Path.GetFullPath("."); // Path.GetFullPath(Path.Combine(Properties.Settings.Default.SongsFolder, "../Exports"));

        private void exportFolderBrowseButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                exportFolderTextBox.Text = dialog.FileName;
            }
        }

        private void exportBeatmapButton_Click(object sender, EventArgs e)
        {
            ExportModeStruct selected = exportModeList[exportMode.SelectedIndex];
            string path = exportFolderTextBox.Text;
            //Console.WriteLine(selected.Value);
            //Console.WriteLine(path);
            //Console.WriteLine(this.editor.NewBeatmap.Filename);
            //Console.WriteLine(this.editor.OriginalBeatmap.Filename);

            Action makeFull = () => {
                string songsFolder = Path.GetDirectoryName(this.editor.RawBeatmap.Filename);
                this.editor.makeOszOfFolder(path, songsFolder);
            };

            Action<bool, bool> makeOsu = (bool withBg, bool withMp3) => {
                this.editor.makeOszOfBeatmap(path, this.editor.RawBeatmap, withBg, withMp3);
            };

            switch (selected.Value)
            {
                case ExportMode.FULL:
                    makeFull();
                    break;
                case ExportMode.OSU_MP3_BG:
                    makeOsu(true, true);
                    break;
                case ExportMode.OSU_MP3:
                    makeOsu(false, true);
                    break;
                case ExportMode.OSU:
                    makeOsu(false, false);
                    break;
            }
        }
    }
}
