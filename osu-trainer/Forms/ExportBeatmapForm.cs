using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        // Specialized exception class just 
        // instead of the generic one
        public class ExceptionToDisplayMessage : Exception
        {
            public ExceptionToDisplayMessage()
            {
            }

            public ExceptionToDisplayMessage(string message)
                : base(message)
            {
            }

            public ExceptionToDisplayMessage(string message, Exception inner)
                : base(message, inner)
            {
            }
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

            displayText.Text = "";
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

        private (bool, string) UploadWithShareX(string path)
        {
            Process sharex = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sharex.exe",
                    Arguments = $"\"{path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            // clipboard text can only be fetched on a "STA thread"
            // so I have to do this hack and get it from the main thread...
            Func<string> getClipboard = () =>
            {
                return (string)this.Invoke(new Func<string>(() => Clipboard.GetText()));
            };

            string prevClipboard = getClipboard();
            try
            {
                sharex.Start();
            } catch (Win32Exception e) when (e.NativeErrorCode == 0x00000002) // display specialized error message when sharex couldn't be found
            {
                throw new Exception("ShareX executable couldn't be found -- please check your system PATH or provided custom path", e);
            }

            // can't do this, because if sharex wasn't already running then this waits forever
            //sharex.WaitForExit();

            // sharex commandline doesn't do anything -- it just returns immediately and tells
            // the running sharex instance to upload the file

            // we do some weird hacking here to try to detect when the upload finishes
            // by checking the clipboard and querying whether another process is using the file
            // we also grab the url from the clipboard as a bonus

            // maybe pause for a bit to let sharex start processing?
            //System.Threading.Thread.Sleep(1000);

            // todo: make these into global settings
            const int UNLOCKED_TICKS = 10;      
            const int TICK_INTERVAL = 500;
            const bool SKIP_UPLOAD_DETECTION = false;

            if (SKIP_UPLOAD_DETECTION)
            {
                return (false, "");
            }

            // number of ticks
            int fileUnlockedFor = 0;
            // when 0, the clipboard data was changed (sharex put an upload URL on the clipboard)
            int stateClipboard = 1;
            // when 0, the file went from locked to unlocked (sharex finished uploading or gave up)
            int stateFileUsed = 2;

            string urlFromClipboardMaybe = prevClipboard;
            FileInfo myFileInfo = new FileInfo(path);
            while (true)
            {
                urlFromClipboardMaybe = getClipboard();
                bool isLocked = JunUtils.IsFileLocked(myFileInfo);

                // ugly state updating code
                if (stateClipboard == 1 && urlFromClipboardMaybe != prevClipboard)
                    stateClipboard = 0;

                if (stateFileUsed == 2 && isLocked)
                    stateFileUsed = 1;
                else if (stateFileUsed == 1 && !isLocked)
                    stateFileUsed = 0;

                if (isLocked) {
                    fileUnlockedFor = 0;
                } else {
                    fileUnlockedFor += 1;
                }

                // figure out what to do based on state
                if (stateClipboard == 0 && stateFileUsed == 0)
                {
                    // success
                    break;
                }

                // if file hasn't been used for a while, something probably went wrong: exit out of the detection
                if (fileUnlockedFor >= UNLOCKED_TICKS)
                {
                    throw new ExceptionToDisplayMessage("File detected as not in use for too long, ShareX probably stuck. Stopping upload detection");
                }
                System.Threading.Thread.Sleep(TICK_INTERVAL);
            }
            Console.WriteLine(urlFromClipboardMaybe);
            return (true, urlFromClipboardMaybe);
        }

        private (bool, string) UploadBeatmap(string oszPath, UploadModeStruct uploadingMode)
        {
            Console.WriteLine(oszPath);
            Console.WriteLine(uploadingMode);
            switch (uploadingMode.Value)
            {
                case UploadMode.SHAREX:
                    return this.UploadWithShareX(oszPath);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            using (var buttonGuard = new JunUtils.RAIIGuard(
                () => this.Invoke(new Action(() => {
                    SetWorkingButtons(false);
                    this.displayText.Text = "";
                })),
                () => this.Invoke(new Action(() => {
                    SetWorkingButtons(true);
                }))
            ))
            {
                string textToDisplay = "";
                string oszPath = ExportBeatmap(exportPath, selectedMode, this.editor.RawBeatmap);
                if (uploading)
                {
                    (bool goodToDelete, string urloutput) = UploadBeatmap(oszPath, uploadingMode);
                    if (goodToDelete)
                    {
                        if (File.Exists(oszPath))
                            File.Delete(oszPath);
                    }
                    textToDisplay = urloutput;
                }
                else
                {
                    textToDisplay = oszPath;
                }
                this.Invoke(new Action(() =>
                {
                    this.displayText.Text = textToDisplay;
                }));
            }
        }

        private void exportOrUploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error.GetType() == typeof(ExceptionToDisplayMessage))
                {
                    MessageBox.Show(
                        $"{e.Error.Message}",
                        "Export Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                } 
                else
                {
                    MessageBox.Show(
                        $"Export failed because of the following error:\n{e.Error.ToString()}",
                        "Export Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                //GenerateMapButton.Progress = 0f;
            }
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
