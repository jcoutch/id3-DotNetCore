#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2012 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace Id3
{
    public class Mp3File : Mp3Stream
    {
        public Mp3File(string filename, Mp3Permissions permissions = Mp3Permissions.Read)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            FileAccess fileAccess = PermissionsToFileAccessMapping[permissions];
            FileStream fileStream = File.Open(filename, FileMode.Open, fileAccess, FileShare.Read);
            SetupStream(fileStream, permissions);

            //Since we created the stream, we are responsible for disposing it when we're done
            StreamOwned = true;
        }

        public Mp3File(FileInfo fileInfo, Mp3Permissions permissions = Mp3Permissions.Read)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            FileAccess fileAccess = PermissionsToFileAccessMapping[permissions];
            FileStream fileStream = fileInfo.Open(FileMode.Open, fileAccess, FileShare.Read);
            SetupStream(fileStream, permissions);

            //Since we created the stream, we are responsible for disposing it when we're done
            StreamOwned = true;
        }

        private static readonly Dictionary<Mp3Permissions, FileAccess> PermissionsToFileAccessMapping = new Dictionary<Mp3Permissions, FileAccess>(3) {
            { Mp3Permissions.Read, FileAccess.Read },
            { Mp3Permissions.Write, FileAccess.Write },
            { Mp3Permissions.ReadWrite, FileAccess.ReadWrite },
        };
    }
}