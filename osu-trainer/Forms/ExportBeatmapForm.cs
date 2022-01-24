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
            SHAREX,
            TRANSFERSH
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

        struct SettingVars
        {
            public ExportMode exportMode;
            public UploadMode uploadMode;
            public string rawExportPath;
            public string rawSharexPath;
            public int sharexTickInterval;
            public bool sharexSkipUploadDetection;
            public int sharexBailoutTicks;
            public string sharexArguments;
            public int transfershMaxDownloads;
            public int transfershMaxDays;

            private bool isNullString(string s) => (s == null) || (s == "");

            public string fullExportPath
            {
                get
                {
                    return (isNullString(rawExportPath)) ? DefaultExportFolder() : rawExportPath;
                }
            }

            public string fullSharexPath
            {
                get
                {
                    return (isNullString(rawSharexPath)) ? "sharex.exe" : rawSharexPath;
                }
            }
        }

        public static string DefaultExportFolder() => Path.GetFullPath("."); // Path.GetFullPath(Path.Combine(Properties.Settings.Default.SongsFolder, "../Exports"));

        private List<ExportModeStruct> exportModeList;
        private List<UploadModeStruct> uploadModeList;
        private BeatmapEditor editor;
        private OsuButtonActivator exportBeatmapButtonActivator;
        private OsuButtonActivator uploadBeatmapButtonActivator;
        private SettingVars settings;
        private Uploaders.TransferShFactory transfersh;

        public ExportBeatmapForm(BeatmapEditor edit)
        {
            InitializeComponent();
            this.editor = edit;
            this.exportBeatmapButtonActivator = new OsuButtonActivator(exportBeatmapButton);
            this.uploadBeatmapButtonActivator = new OsuButtonActivator(uploadBeatmapButton);
            this.transfersh = new Uploaders.TransferShFactory();
        }

        private void ExportBeatmapForm_Load(object sender, EventArgs e)
        {
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
                new UploadModeStruct("ShareX", UploadMode.SHAREX),
                new UploadModeStruct("transfer.sh", UploadMode.TRANSFERSH)
            };
            var bindingSource2 = new BindingSource();
            bindingSource2.DataSource = uploadModeList;
            uploadMode.DisplayMember = "Display";
            uploadMode.ValueMember = "Value";
            uploadMode.DataSource = bindingSource2.DataSource;
            uploadMode.SelectedIndex = 0;

            displayText.Text = "";

            LoadPropertiesIntoSettings();
            LoadSettingsIntoUI();
            showUploadSettingsFor(((UploadModeStruct)uploadMode.SelectedItem).Value);
        }

        private void showUploadSettingsFor(UploadMode mode)
        {
            Control controlToShow;
            switch (mode)
            {
                case UploadMode.SHAREX:
                    controlToShow = sharexSettings;
                    break;
                case UploadMode.TRANSFERSH:
                    controlToShow = transfershSettings;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // hide all previous
            foreach(Control control in uploadAdditionalSettings.Controls)
            {
                control.Visible = false;
            }
            controlToShow.Visible = true;
            var oldHeight = uploadAdditionalSettings.Height;
            uploadAdditionalSettings.Height = controlToShow.Height;
            var newHeight = uploadAdditionalSettings.Height;
            this.Height += (-oldHeight) + newHeight;
        }

        // conv argument is a hack because i don't understand C# generics with enums
        private U saveenum<T,U>(List<T> list, int index, Func<T, U> conv)
        {
            return conv(list[index]);
        }

        private int loadenum<T,U>(List<T> list, U val, Func<T, U> conv)
        {
            int index = list.FindIndex(i => EqualityComparer<U>.Default.Equals(conv(i), val));
            if (index == -1)
                index = 0;
            return index;
        }

        private void LoadPropertiesIntoSettings()
        {
            // ugly validation but I guess we'll do this for now
            // should probably move all the validation into the setting struct

            // call as unsigned(nameof([setting here]))
            // Returns setting value if >= 0, otherwise returns default setting value
            Func<string, int> unsigned = (string attrname) =>
            {
                int val = (int)Properties.ExportBeatmap.Default.GetType().GetProperty(attrname).GetValue(Properties.ExportBeatmap.Default);
                string defa = (string) Properties.ExportBeatmap.Default.Properties[attrname].DefaultValue;
                int def = int.Parse(defa);
                return (val >= 0) ? val : def;
            };

            var newsettings = new SettingVars();
            newsettings.exportMode = (ExportMode) Properties.ExportBeatmap.Default.ExportMode;
            newsettings.uploadMode = (UploadMode) Properties.ExportBeatmap.Default.UploadMode;
            newsettings.rawExportPath = Properties.ExportBeatmap.Default.ExportFolderPath;
            newsettings.rawSharexPath = Properties.ExportBeatmap.Default.SharexPath;
            newsettings.sharexBailoutTicks = unsigned(nameof(Properties.ExportBeatmap.Default.SharexBailoutTicks));
            newsettings.sharexSkipUploadDetection = Properties.ExportBeatmap.Default.SharexSkipUploadDetection;
            newsettings.sharexTickInterval = unsigned(nameof(Properties.ExportBeatmap.Default.SharexTickInterval));
            newsettings.sharexArguments = Properties.ExportBeatmap.Default.SharexArguments;
            newsettings.transfershMaxDays = unsigned(nameof(Properties.ExportBeatmap.Default.TransfershMaxDays));
            newsettings.transfershMaxDownloads = unsigned(nameof(Properties.ExportBeatmap.Default.TransfershMaxDownloads));
            this.settings = newsettings;
        }

        private void WritePropertiesFromSettings()
        {
            Properties.ExportBeatmap.Default.ExportMode = (int)settings.exportMode;
            Properties.ExportBeatmap.Default.UploadMode = (int)settings.uploadMode;
            Properties.ExportBeatmap.Default.ExportFolderPath = settings.rawExportPath;
            Properties.ExportBeatmap.Default.SharexPath = settings.rawSharexPath;
            Properties.ExportBeatmap.Default.SharexBailoutTicks = settings.sharexBailoutTicks;
            Properties.ExportBeatmap.Default.SharexSkipUploadDetection = settings.sharexSkipUploadDetection;
            Properties.ExportBeatmap.Default.SharexTickInterval = settings.sharexTickInterval;
            Properties.ExportBeatmap.Default.SharexArguments = settings.sharexArguments;
            Properties.ExportBeatmap.Default.TransfershMaxDays = settings.transfershMaxDays;
            Properties.ExportBeatmap.Default.TransfershMaxDownloads = settings.transfershMaxDownloads;
            Properties.ExportBeatmap.Default.Save();
        }

        private void LoadSettingsIntoUI()
        {
            exportMode.SelectedIndex = loadenum(exportModeList, settings.exportMode, i => i.Value);
            uploadMode.SelectedIndex = loadenum(uploadModeList, settings.uploadMode, i => i.Value);

            exportFolderTextBox.Text = settings.fullExportPath;
            sharexPathTextBox.Text = settings.rawSharexPath;
            sharexArgsTextBox.Text = settings.sharexArguments;
        }

        private void WriteSettingsFromUI()
        {
            Func<string, string, bool> PathsEqual = (path1, path2) => string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
            settings.exportMode = saveenum(exportModeList, exportMode.SelectedIndex, i => i.Value);
            settings.uploadMode = saveenum(uploadModeList, uploadMode.SelectedIndex, i => i.Value);
            string exportText = exportFolderTextBox.Text;
            try
            {
                if (PathsEqual(exportText, DefaultExportFolder()))
                {
                    exportText = "";
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Path invalid, going with default: {e}");
                exportText = "";
            }
            
            settings.rawExportPath = exportText;
            settings.rawSharexPath = sharexPathTextBox.Text;
            settings.sharexArguments = sharexArgsTextBox.Text;
        }

        // todo: name this
        private void SetWorkingButtons(bool state)
        {
            this.exportBeatmapButtonActivator.setActivated(state);
            this.uploadBeatmapButtonActivator.setActivated(state);
        }

        private string ExportBeatmap(string exportPath, ExportMode selectedMode, Beatmap beatmap)
        {
            Func<string> makeFull = () => {
                string songsFolder = Path.GetDirectoryName(beatmap.Filename);
                return this.editor.makeOszOfFolder(exportPath, songsFolder);
            };

            Func<bool, bool, string> makeOsu = (bool withBg, bool withMp3) => {
                return this.editor.makeOszOfBeatmap(exportPath, beatmap, withBg, withMp3);
            };

            switch (selectedMode)
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

        private (bool, string) UploadWithTransferSh(string path)
        {
            // pass settings to uploader class
            transfersh.MaxDays = settings.transfershMaxDays;
            transfersh.MaxDownloads = settings.transfershMaxDownloads;

            // perform uploader
            var obj = transfersh.Upload(path).Result;
            var downloadLink = obj.DownloadLink;

            // store url in clipboard
            this.Invoke(new Action(() => Clipboard.SetText(downloadLink)));
            Console.WriteLine(obj.DownloadLink);
            Console.WriteLine(obj.DeletionLink);
            return (true, downloadLink);
        }

        private (bool, string) UploadWithShareX(string path)
        {
            Process sharex = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = settings.fullSharexPath,
                    Arguments = $"\"{path}\" {settings.sharexArguments}",
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
                throw new Exception($"ShareX executable ({settings.rawSharexPath}) couldn't be found -- please check your system PATH or provided custom path", e);
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

            int UNLOCKED_TICKS = settings.sharexBailoutTicks;
            int TICK_INTERVAL = settings.sharexTickInterval;
            bool SKIP_UPLOAD_DETECTION = settings.sharexSkipUploadDetection;

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

        private (bool, string) UploadBeatmap(string oszPath, UploadMode uploadingMode)
        {
            Console.WriteLine(oszPath);
            Console.WriteLine(uploadingMode);
            switch (uploadingMode)
            {
                case UploadMode.SHAREX:
                    return this.UploadWithShareX(oszPath);
                case UploadMode.TRANSFERSH:
                    return this.UploadWithTransferSh(oszPath);
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

            // update setting variables with current UI values
            this.Invoke(new Action(() => WriteSettingsFromUI()));
            var selectedMode = settings.exportMode;
            string exportPath = settings.fullExportPath;
            var uploadingMode = settings.uploadMode;

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

        private void ExportBeatmapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteSettingsFromUI();
            WritePropertiesFromSettings();
        }

        private void sharexPathBrowseButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                sharexPathTextBox.Text = dialog.FileName;
            }
        }

        private void uploadMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            showUploadSettingsFor(((UploadModeStruct)comboBox.SelectedItem).Value);
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
