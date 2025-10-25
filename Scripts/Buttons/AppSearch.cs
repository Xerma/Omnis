using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Media.Imaging;
using Vanara.Windows.Shell;
using Vanara.PInvoke;
using System.Windows.Interop;

namespace Omnis.Scripts.Buttons
{
    public class AppSearch
    {
        private string folderPath = "";
        private string[] exeFilePaths = {};

        //methods to-do:
        //- get icons
        //- add to main window ListBox
        //--- store icon and path
        //--- need user control that has a method to run the app
        //--- needs a method to add to my fake desktop (drag and drop?)
        //--- create option to compare images to empty icon images (filter no icon exe's out)

        /// <summary>
        /// Search a specific directory for exe files.
        /// </summary>
        public void DirectorySearch()
        {
            OpenFolderDialog folderDialog = new();
            folderDialog.Multiselect = false;
            folderDialog.Title = "Select a folder to search for applications";
            folderDialog.ShowDialog();
            folderPath = folderDialog.FolderName;

            exeFilePaths = GetAppStrings(folderPath);
            foreach (string path in exeFilePaths)
            {
                string iconNameType = Path.GetFileName(path);
                string iconName = iconNameType.Substring(0, iconNameType.Length - 4);
                string tempSavePath = @"Icons\Loaded\" + iconName + ".png";
                string savePath = Path.Combine(GetProjectRoot(), tempSavePath);

                BitmapSource? bitmap = GetLargeIcon(path);

                if (bitmap != null)
                    SaveIconAsPng(bitmap, savePath);
            }
        }

        /// <summary>
        /// Internal search method: Finds all exe files within a given directory
        /// </summary>
        /// <param name="path">Path to the directory.</param>
        private static string[] GetAppStrings(string path)
        {
            var exePaths = new List<string>();

            try
            {
                foreach (var file in Directory.GetFiles(path, "*.exe"))
                {
                    exePaths.Add(file);
                }

                foreach (var dir in Directory.GetDirectories(path))
                {
                    exePaths.AddRange(GetAppStrings(dir));
                }
                //Process.Start(exeFiles[0]);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("UnauthorizedAccessException at: " + path);
            }

            return exePaths.ToArray();
        }

        public static string GetProjectRoot()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;

            while (!string.IsNullOrEmpty(dir) && !File.Exists(Path.Combine(dir, "App Hub.csproj")))
            {
                dir = Directory.GetParent(dir)?.FullName!;
            }

            return dir;
        }

        public static BitmapSource? GetLargeIcon(string filePath, int size = 256)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                using ShellItem shellItem = new(filePath);
                Vanara.PInvoke.SIZE iconSize = new(size, size);

                Gdi32.SafeHBITMAP? safeHBITMAP = shellItem.GetImage(iconSize, ShellItemGetImageOptions.ResizeToFit);
                if (safeHBITMAP == null)
                    return null;

                IntPtr hBitmap = safeHBITMAP.DangerousGetHandle();

                BitmapSource? largeIcon = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

                Gdi32.DeleteObject(hBitmap);

                return largeIcon;
            }
            catch (Exception ex)
            {
                // change to log file
                Debug.WriteLine($"ShellItem failed for {filePath}: {ex.Message}");
                return null;
            } 
        }

        public static void SaveIconAsPng(BitmapSource icon, string savePath)
        {
            if (icon != null)
            {
                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(icon));

                using var fs = new FileStream(savePath, FileMode.Create);
                encoder.Save(fs);
            }

            Debug.WriteLine($"Bitmap {icon} is null");
        }
    }
}