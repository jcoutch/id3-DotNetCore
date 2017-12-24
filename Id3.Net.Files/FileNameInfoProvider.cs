﻿#region --- License & Copyright Notice ---
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
using System.IO;

using Id3.Frames;
using Id3.Info;

namespace Id3
{
    public sealed class FileNameInfoProvider : InfoProvider
    {
        protected override Id3Tag[] GetTagInfo(Id3Tag tag)
        {
            string filename = Path.GetFileNameWithoutExtension(Inputs.FileName);
            if (filename == null)
                return Id3Tag.Empty;

            string[] breakup = filename.Split(new[] { " - " }, StringSplitOptions.None);
            if (breakup.Length <= 1)
                return Id3Tag.Empty;

            var result = new Id3Tag();
            result.Artists.Value.Add(breakup[0].Trim());
            result.Title.Value = breakup[1].Trim();
            return new[] { result };
        }

        protected override InfoProviderProperties GetProperties()
        {
            var properties = new InfoProviderProperties("File name", null) {
                CanOmitTag = true,
                RequiresFilename = true
            };
            properties.AvailableOutputs.Add<ArtistsFrame>();
            properties.AvailableOutputs.Add<TitleFrame>();
            return properties;
        }
    }
}