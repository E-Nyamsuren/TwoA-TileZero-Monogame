#region Header

/* 
Copyright 2015 Enkhbold Nyamsuren (http://www.bcogs.net , http://www.bcogs.info/)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Namespace: MonoGame1
Filename: Bridge.cs
Description:
    Defines a class that implements polymorphism.
*/
#endregion Header

namespace MonoGame1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AssetPackage;

    /// <summary>
    /// A bridge.
    /// </summary>
    class Bridge : IBridge, IDataStorage, IDefaultSettings
    {
        /// <summary>
        /// The storage dir for IDataStorage use. The folder will be located in the Unity Assets Folder.
        /// </summary>
        private static String StorageDir;

        /// <summary>
        /// The resource dir for IDefaulSettings use.
        /// </summary>
        ///
        /// <remarks>       This directory could be used to create and save for instance &lt;class&gt;
        ///                 AppSettings.xml Setting files at edit time but NOT at run-time.</remarks>
        /// <remarks>       Reading of files saved in this directory can be done with Unity's
        ///                 Resources.Load() methods, where the name passed is the filename relative to
        ///                 ResourceDir without file extension.</remarks>
        private static String ResourceDir;

        /// <summary>
        /// Initializes static members of the MonoGame1.Bridge class.
        /// </summary>
        static Bridge()
        {
            Debug.WriteLine("Static Bridge Constructor");

            StorageDir = "./DataStorage";

            ResourceDir = "./Resources";
        }

        /// <summary>
        /// Initializes a new instance of the asset_proof_of_concept_demo_CSharp.Bridge class.
        /// </summary>
        public Bridge()
        {
            Debug.WriteLine("Bridge Constructor");

            if (!Directory.Exists(StorageDir))
            {
                Directory.CreateDirectory(StorageDir);
            }

            if (!Directory.Exists(ResourceDir))
            {
                Directory.CreateDirectory(ResourceDir);
            }
        }

        #region IDataStorage Members

        /// <summary>
        /// Exists the given file.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to delete. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Exists(string fileId)
        {
            return File.Exists(Path.Combine(StorageDir, fileId));
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        ///
        /// <returns>
        /// A List&lt;String&gt;
        /// </returns>
        public List<String> Files()
        {
            return Directory.GetFiles(StorageDir).ToList().ConvertAll(
    new Converter<String, String>(p => p.Replace(StorageDir + @"\", ""))).ToList();

            //! EnumerateFiles not supported in Unity3D.
            // 
            //return Directory.EnumerateFiles(StorageDir).ToList().ConvertAll(
            //    new Converter<String, String>(p => p.Replace(StorageDir + @"\", ""))).ToList();
        }

        /// <summary>
        /// Saves the given file.
        /// </summary>
        ///
        /// <param name="fileId">   The file identifier to delete. </param>
        /// <param name="fileData"> Information describing the file. </param>
        public void Save(string fileId, string fileData)
        {
            File.WriteAllText(Path.Combine(StorageDir, fileId), fileData);
        }

        /// <summary>
        /// Loads the given file.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to delete. </param>
        ///
        /// <returns>
        /// A String.
        /// </returns>
        public string Load(string fileId)
        {
            return File.ReadAllText(Path.Combine(StorageDir, fileId));
        }

        /// <summary>
        /// Deletes the given fileId.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to delete. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Delete(string fileId)
        {
            if (Exists(fileId))
            {
                File.Delete(Path.Combine(StorageDir, fileId));

                return true;
            }

            return false;
        }

        #endregion

        #region IDefaultSettings Members

        /// <summary>
        /// Derive asset name.
        /// </summary>
        ///
        /// <param name="Class"> The class. </param>
        /// <param name="Id">    The identifier. </param>
        ///
        /// <returns>
        /// A String.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "Id")]
        private String DeriveAssetName(String Class, String Id)
        {
            String ds = String.Format("{0}AppSettings", Class);

            // Debug.Print("{0}->{1}", Id, Path.GetFullPath(Path.Combine(ResourceDir, ds)) + ".xml");

            return ds;
        }

        /// <summary>
        /// Query if a 'Class' with Id has default settings.
        /// </summary>
        ///
        /// <param name="Class"> The class. </param>
        /// <param name="Id">    The identifier. </param>
        ///
        /// <returns>
        /// true if default settings, false if not.
        /// </returns>
        public bool HasDefaultSettings(string Class, string Id)
        {
            return File.Exists(Path.Combine(ResourceDir, DeriveAssetName(Class, Id)) + ".xml");
        }

        /// <summary>
        /// Loads default settings for a 'Class' with Id.
        /// </summary>
        ///
        /// <param name="Class"> The class. </param>
        /// <param name="Id">    The identifier. </param>
        ///
        /// <returns>
        /// The default settings.
        /// </returns>
        public string LoadDefaultSettings(string Class, string Id)
        {
            if (HasDefaultSettings(Class, Id))
            {
                return File.ReadAllText(Path.Combine(ResourceDir, DeriveAssetName(Class, Id)) + ".xml");
            }

            return null;
        }

        /// <summary>
        /// Saves a default settings for a 'Class' with Id.
        /// </summary>
        ///
        /// <param name="Class">    The class. </param>
        /// <param name="Id">       The identifier. </param>
        /// <param name="fileData"> The File Data. </param>
        public void SaveDefaultSettings(string Class, string Id, string fileData)
        {
            //if (Application.isEditor)
            //{
            Debug.Print(Path.GetFullPath(Path.Combine(ResourceDir, DeriveAssetName(Class, Id)) + ".xml"));

            File.WriteAllText(Path.Combine(ResourceDir, DeriveAssetName(Class, Id)) + ".xml", fileData);
            //}
            //else
            //{
            //             Debug.Print(Path.GetFullPath(Path.Combine(ResourceDir, ds)));
            //            Debug.WriteLine("Warning: Cannot save resources at runtime!");

            //}
        }

        #endregion

        #region ILogger Properties

        //! veg Only for ILogger
        // 
        ///// <summary>
        ///// The prefix.
        ///// </summary>
        //public String Prefix
        //{
        //    get;
        //    set;
        //}

        #endregion
    }
}
