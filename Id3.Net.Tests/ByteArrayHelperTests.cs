#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2011 Jeevan James
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

using System.Collections.Generic;

using Id3.Internal;

using Xunit;
using Xunit.Extensions;

namespace Id3.Net.Tests
{
    public sealed class ByteArrayHelperTests
    {
        [Fact]
        public void AreEqualTests()
        {
            Assert.True(ByteArrayHelper.AreEqual(null, null));
            Assert.False(ByteArrayHelper.AreEqual(null, new byte[0]));
            Assert.True(ByteArrayHelper.AreEqual(new byte[0], new byte[0]));

            Assert.False(ByteArrayHelper.AreEqual(Create(1, 2, 3), Create(1, 2)));
            Assert.True(ByteArrayHelper.AreEqual(Create(1, 2, 3), Create(1, 2, 3)));
            Assert.False(ByteArrayHelper.AreEqual(Create(1, 2, 3), Create(1, 3, 2)));
        }

        [Theory, MemberData(nameof(GetBytesUptoSequenceData))]
        public void GetBytesUptoSequenceTests(byte[] source, byte[] sequence, byte[] expectedResult)
        {
            Assert.True(ByteArrayHelper.AreEqual(ByteArrayHelper.GetBytesUptoSequence(source, 0, sequence), expectedResult));
        }

        public static IEnumerable<object[]> GetBytesUptoSequenceData
        {
            get
            {
                yield return new object[] { Create(1, 2, 3, 4, 5, 6, 7), Create(4, 5), Create(1, 2, 3) };
                yield return new object[] { Create(1, 2, 3, 4, 5, 6, 7), Create(1, 2), new byte[0] };
            }
        }

        private static byte[] Create(params byte[] bytes)
        {
            return bytes;
        }
    }
}