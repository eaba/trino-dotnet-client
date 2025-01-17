﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace TrinoClient.Internal
{
    public sealed class KerberosUtil
    {
        private const string FilePrefix = "FILE:";

        private KerberosUtil()
        {
        }

        public static string DefaultCredentialCachePath()
        {
            var value = NullToEmpty(Environment.GetEnvironmentVariable("KRB5CCNAME"));
            if (value.StartsWith(FilePrefix, StringComparison.Ordinal))
            {
                value = value.Substring(FilePrefix.Length);
            }
            return value;
        }

        private static string NullToEmpty(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            return path;
        }
    }
}