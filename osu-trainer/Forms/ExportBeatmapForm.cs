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
using FsBeatmapProcessor;
using Microsoft.WindowsAPICodePack.Dialogs;
using osu_trainer.Controls;

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

        enum UploadMode
        {
            SHAREX
        }

        struct UploadModeStruct
        {
            string display;
            UploadMode value;

            public UploadModeStruct(string display, UploadMode val)
            {
                this.display = display;
                this.value = val;
            }

            public string Display { get => display; set => display = value; }
            public UploadMode Value { get => value; set => this.value = value; }
        }

        private List<ExportModeStruct> exportModeList;
        private List<UploadModeStruct> uploadModeList;
        private BeatmapEditor editor;
        private OsuButtonActivator exportBeatmapButtonActivator;
        private OsuButtonActivator uploadBeatmapButtonActivator;

        public ExportBeatmapForm(BeatmapEditor edit)
        {
            InitializeComponent();
            this.editor = edit;
            this.exportBeatmapButtonActivator = new OsuButtonActivator(exportBeatmapButton);
            this.uploadBeatmapButtonActivator = new OsuButtonActivator(uploadBeatmapButton);
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

            uploadModeList = new List<UploadModeStruct> {
                new UploadModeStruct("ShareX", UploadMode.SHAREX)
            };
            var bindingSource2 = new BindingSource();
            bindingSource2.DataSource = uploadModeList;
            uploadMode.DisplayMember = "Display";
            uploadMode.ValueMember = "Value";
            uploadMode.DataSource = bindingSource2.DataSource;
            uploadMode.SelectedIndex = 0;
        }

        public static string DefaultExportFolder() => Path.GetFullPath("."); // Path.GetFullPath(Path.Combine(Properties.Settings.Default.SongsFolder, "../Exports"));

        // todo: name this
        private void SetWorkingButtons(bool state)
        {
            this.exportBeatmapButtonActivator.setActivated(state);
            this.uploadBeatmapButtonActivator.setActivated(state);
        }

        private string ExportBeatmap(string exportPath, ExportModeStruct selectedMode, Beatmap beatmap)
        {
            Func<string> makeFull = () => {
                string songsFolder = Path.GetDirectoryName(beatmap.Filename);
                return this.editor.makeOszOfFolder(exportPath, songsFolder);
            };

            Func<bool, bool, string> makeOsu = (bool withBg, bool withMp3) => {
                return this.editor.makeOszOfBeatmap(exportPath, beatmap, withBg, withMp3);
            };

            // rewrite this as ReportProgress()?
            switch (selectedMode.Value)
            {
                case ExportMode.FULL:
                    return makeFull();
                case ExportMode.OSU_MP3_BG:
                    return makeOsu(true, true);
                case ExportMode.OSU_MP3:
                    return makeOsu(false, true);
                case ExportMode.OSU:
                    return makeOsu(false, false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private bool UploadBeatmap(string oszPath, UploadModeStruct uploadingMode)
        {
            Console.WriteLine(oszPath);
            Console.WriteLine(uploadingMode);
            return false;
        }

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
            exportOrUploadWorker.RunWorkerAsync(false);
        }
        private void uploadBeatmapButton_Click(object sender, EventArgs e)
        {
            exportOrUploadWorker.RunWorkerAsync(true);
        }

        private void exportOrUploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool uploading = (bool) e.Argument;
            (ExportModeStruct selectedMode, string exportPath, UploadModeStruct uploadingMode) =
               ((ExportModeStruct, string, UploadModeStruct)) this.Invoke(new Func<(ExportModeStruct, string, UploadModeStruct)>(() =>
                {
                    selectedMode = exportModeList[exportMode.SelectedIndex];
                    exportPath = exportFolderTextBox.Text;
                    uploadingMode = uploadModeList[uploadMode.SelectedIndex];
                    return (selectedMode, exportPath, uploadingMode);
                }))
            ;

            //Console.WriteLine(selected.Value);
            //Console.WriteLine(path);
            //Console.WriteLine(this.editor.NewBeatmap.Filename);
            //Console.WriteLine(this.editor.OriginalBeatmap.Filename);

            // rewrite this as ReportProgress()?
            this.Invoke(new Action(() => SetWorkingButtons(false)));
            string oszPath = ExportBeatmap(exportPath, selectedMode, this.editor.RawBeatmap);
            if (uploading)
            {
                bool success = UploadBeatmap(oszPath, uploadingMode);
                if (success)
                {
                    if (File.Exists(oszPath))
                        File.Delete(oszPath);
                }
            }
            this.Invoke(new Action(() => SetWorkingButtons(true)));
        }
    }

    public class OsuButtonActivator
    {
        OsuButton button;
        Color savedForeColor;
        Color savedColor;

        private Color disabledForeColor = Colors.Disabled;
        private Color disabledColor = Colors.TextBoxBg;

        public OsuButtonActivator(OsuButton button)
        {
            this.button = button;
            this.savedForeColor = disabledForeColor;
            this.savedColor = disabledColor;
        }

        private Color switchColors(bool activate, Color buttonColor, ref Color savedColor, ref Color disabledColor)
        {
            if (activate)
            {
                // if button is disabled then change to saved color,
                // otherwise don't touch the color of the button
                if (buttonColor == disabledColor)
                {
                    return savedColor;
                }
                return buttonColor;
            }
            else
            {
                // only save the color if button is not already disabled
                if (buttonColor != disabledColor)
                {
                    savedColor = buttonColor;
                }
                return disabledColor;
            }
        }

        public bool setActivated(bool activate)
        {
            button.Enabled = activate;
            button.ForeColor = switchColors(activate, button.ForeColor, ref savedForeColor, ref disabledForeColor);
            button.Color = switchColors(activate, button.Color, ref savedColor, ref disabledColor);
            return true;
        }
    }

}
