namespace MonoGame1
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The microsoft. xna. framework. content.
    /// </summary>
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// A texture content.
    /// </summary>
    public static class TextureContent
    {
        /// <summary>
        /// A ContentManager extension method that loads list content.
        /// 
        /// See http://stackoverflow.com/questions/12914002/how-to-load-all-files-in-a-folder-with-xna
        /// 
        /// This work had no explicit license specified.
        /// </summary>
        ///
        /// <exception cref="DirectoryNotFoundException">   Thrown when the requested
        ///                                                 directory is not present. </exception>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="contentManager"> The contentManager to act on. </param>
        /// <param name="contentFolder">  Pathname of the content folder. </param>
        /// <param name="wildcard">       The wildcard. </param>
        ///
        /// <returns>
        /// The list content.
        /// </returns>
        public static Dictionary<string, T> LoadListContent<T>(
            this ContentManager contentManager,
            string contentFolder,
            string wildcard = "*.*")
        {
            DirectoryInfo dir = String.IsNullOrEmpty(contentFolder) ?
                new DirectoryInfo(contentManager.RootDirectory) :
                new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            Dictionary<String, T> result = new Dictionary<String, T>();

            FileInfo[] files = dir.GetFiles(wildcard);

            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                result[key] = contentManager.Load<T>(
                    String.IsNullOrEmpty(contentFolder) ?
                    key :
                    contentFolder + "/" + key);
            }

            return result;
        }
    }
}
