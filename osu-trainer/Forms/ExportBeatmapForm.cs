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

        private List<ExportModeStruct> exportModeList;
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
        }

        public static string DefaultExportFolder() => Path.GetFullPath("."); // Path.GetFullPath(Path.Combine(Properties.Settings.Default.SongsFolder, "../Exports"));

        // todo: name this
        private void SetWorkingButtons(bool state)
        {
            this.exportBeatmapButtonActivator.setActivated(state);
            this.uploadBeatmapButtonActivator.setActivated(state);
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
            exportOrUploadWorker.RunWorkerAsync();
        }

        private void exportOrUploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (ExportModeStruct selectedMode, string exportPath) =
               ((ExportModeStruct, string)) this.Invoke(new Func<(ExportModeStruct, string)>(() =>
                {
                    selectedMode = exportModeList[exportMode.SelectedIndex];
                    exportPath = exportFolderTextBox.Text;
                    return (selectedMode, exportPath);
                }))
            ;

            //Console.WriteLine(selected.Value);
            //Console.WriteLine(path);
            //Console.WriteLine(this.editor.NewBeatmap.Filename);
            //Console.WriteLine(this.editor.OriginalBeatmap.Filename);

            // maybe move this to a function exportBeatmap() or something
            Action makeFull = () => {
                string songsFolder = Path.GetDirectoryName(this.editor.RawBeatmap.Filename);
                this.editor.makeOszOfFolder(exportPath, songsFolder);
            };

            Action<bool, bool> makeOsu = (bool withBg, bool withMp3) => {
                this.editor.makeOszOfBeatmap(exportPath, this.editor.RawBeatmap, withBg, withMp3);
            };

            // rewrite this as ReportProgress()?
            this.Invoke(new Action(() => SetWorkingButtons(false)));
            switch (selectedMode.Value)
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
