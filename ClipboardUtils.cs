using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;

namespace ClipboardUtils
{
    [SupportedOSPlatform("windows")]
    public static class ClipboardHelper
    {
        private static readonly object ClipboardLockObject = new object();
        
        /// <summary>
        /// The SetDataObject will lock/try/catch clipboard operations making it save and not show exceptions.
        /// The bool "retainAfterExit" is used to decided if the information stays on the clipboard after exit.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="retainAfterExit"></param>
        private static void SetDataObject(IDataObject data, bool retainAfterExit)
        {
            lock (ClipboardLockObject)
            {
                // Clear first, this seems to solve some issues
                try
                {
                    Clipboard.Clear();
                }
                catch (Exception clearException)
                {
                    MessageBox.Show(clearException.Message);
                }

                try
                {
                    Clipboard.SetDataObject(data, retainAfterExit, 15, 200);
                }
                catch (Exception clipboardSetException)
                {
                    MessageBox.Show(clipboardSetException.Message);
                }
            }
        }

        /// <summary>
        /// The GetDataObject will lock/try/catch clipboard operations making it save and not show exceptions.
        /// </summary>
        public static IDataObject GetDataObject()
        {
            lock (ClipboardLockObject)
            {
                int retryCount = 2;
                while (retryCount >= 0)
                {
                    try
                    {
                        return Clipboard.GetDataObject();
                    }
                    catch (Exception ee)
                    {
                        if (retryCount == 0)
                        {
                            MessageBox.Show(
                                "There was an unknown error pasting from clipboard.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    finally
                    {
                        --retryCount;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Test if the IDataObject contains Text
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public static bool ContainsText(IDataObject dataObject)
        {
            if (dataObject != null)
            {
                if (dataObject.GetDataPresent(DataFormats.Text) || dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    return true;
                }
            }

            return false;
        }

        public static void CopyTextToClipboard(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            DataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, true, content);

            SetDataObject(dataObject, true);
        }

        public static string GetTextFromClipboard()
        {
            IDataObject iData = GetDataObject();
            if (ContainsText(iData))
            {
                return (string)iData.GetData(DataFormats.Text);
            }

            return "";
        }
    }
}