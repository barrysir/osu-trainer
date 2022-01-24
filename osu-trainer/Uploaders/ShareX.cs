using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_trainer.Uploaders
{
    public class SharexException : Exception
    {
        public SharexException()
        {
        }

        public SharexException(string message)
            : base(message)
        {
        }

        public SharexException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Upload detection relies on checking the clipboard
    /// so pass in a callback which reads the current clipboard data.
    /// </summary>
    class ShareX
    { 
        // clipboard text can only be fetched on a "STA thread"
        // so I have to do this hack and get it from the main thread...
        Func<string> getClipboard;

        public string SharexArguments = "";
        public string SharexPath = "";
        public int BailoutTicks = 10;
        public int TickInterval = 500;
        public bool SkipUploadDetection = false;

        public string FullSharexPath => (SharexPath == "") ? "sharex.exe" : SharexPath;

        public ShareX(Func<string> getClipboard)
        {
            this.getClipboard = getClipboard;
        }

        public async Task<(bool, string)> Upload(string path)
        {
            Process sharex = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FullSharexPath,
                    Arguments = $"\"{path}\" {SharexArguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };


            string prevClipboard = getClipboard();
            try
            {
                sharex.Start();
            }
            catch (Win32Exception e) when (e.NativeErrorCode == 0x00000002) // display specialized error message when sharex couldn't be found
            {
                throw new Exception($"ShareX executable ({FullSharexPath}) couldn't be found -- please check your system PATH or provided custom path", e);
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

            int UNLOCKED_TICKS = BailoutTicks;
            int TICK_INTERVAL = TickInterval;
            bool SKIP_UPLOAD_DETECTION = SkipUploadDetection;

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

                if (isLocked)
                {
                    fileUnlockedFor = 0;
                }
                else
                {
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
                    throw new SharexException("File detected as not in use for too long, ShareX probably stuck. Stopping upload detection");
                }
                await Task.Delay(TICK_INTERVAL);
            }
            return (true, urlFromClipboardMaybe);
        }
    }
}
