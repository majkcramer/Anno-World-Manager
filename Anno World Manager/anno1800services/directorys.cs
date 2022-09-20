using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Anno_World_Manager.anno1800services
{
    internal class directorys
    {
        /// <summary>
        /// All Directorys are valid?
        /// </summary>
        public bool is_valid { get; set; } = false;


        public bool is_path_install_valid { get; set; } = false;
        public string? path_install = String.Empty;
        
        public bool is_path_data_valid { get; set; } = false;
        public string? path_data = String.Empty;

        //  TODO: implementieren
        public bool is_path_mods_valid { get; set; } = false;
        public string? path_mods = String.Empty;

        public void Initialize()
        {
            #region Initialize: Anno 1800 Installation Path
            //  Read from Directory
            path_install = GetInstallDirFromRegistry();
            //  Check - Is Path valid
            is_path_install_valid = IsPathInstallValid();
            #endregion

            #region Initialize: Anno 1800 Data Folder
            if (is_path_install_valid)
            {
                if (!is_path_data_valid && File.Exists(Path.Combine(path_install, "maindata/data0.rda")))
                {
                    path_data = Path.Combine(path_install, "maindata/");
                    is_path_data_valid = true;
                }

                if (!is_path_data_valid && File.Exists(Path.Combine(path_install, "data0.rda")))
                { 
                    path_data = Path.GetDirectoryName(path_install);
                    is_path_data_valid = true;
                }

                if (!is_path_data_valid && Directory.Exists(Path.Combine(path_install, "data/dlc01")))
                { 
                    path_data = path_install;
                    is_path_data_valid = true;
                }
                if (!is_path_data_valid && Directory.Exists(Path.Combine(path_install, "dlc01")))
                { 
                    path_data = Path.GetDirectoryName(path_install);
                    is_path_data_valid = true;
                }
            }
            #endregion

            //  Initialize: Anno 1800 Mod Folder

            #region Summarize: Every Directories exists and are usable
            if (is_path_install_valid && is_path_data_valid
                // TODO && is_mod
                )
            {
                is_valid = true;
            }


            #endregion
        }


        /// <summary>
        /// Get Anno 1800 Install Directory
        /// </summary>
        /// <returns>directory or null</returns>
        private string? GetInstallDirFromRegistry()
        {
            string installDirKey = @"SOFTWARE\WOW6432Node\Ubisoft\Anno 1800";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(installDirKey);
            return key?.GetValue("InstallDir") as string;
        }

        private bool IsPathInstallValid()
        {
            if (string.IsNullOrEmpty(path_install)) { return false; }
            return IsValidPath(path_install);
        }


        private bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
