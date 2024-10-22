﻿using SharpPulsar.Trino.Trino;

/// <summary>
/// Licensed to the Apache Software Foundation (ASF) under one
/// or more contributor license agreements.  See the NOTICE file
/// distributed with this work for additional information
/// regarding copyright ownership.  The ASF licenses this file
/// to you under the Apache License, Version 2.0 (the
/// "License"); you may not use this file except in compliance
/// with the License.  You may obtain a copy of the License at
/// 
///   http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing,
/// software distributed under the License is distributed on an
/// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
/// KIND, either express or implied.  See the License for the
/// specific language governing permissions and limitations
/// under the License.
/// </summary>
namespace TrinoClient.Naming
{

    /// <summary>
    /// Validator for metadata configuration.
    /// </summary>
    internal class Metadata
    {

        private const int MaxMetadataSize = 1024; // 1 Kb

        private Metadata()
        {
        }

        public static void ValidateMetadata(IDictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                return;
            }

            var size = 0;
            foreach (var e in metadata.SetOfKeyValuePairs())
            {
                size += e.Key.Length + e.Value.Length;
                if (size > MaxMetadataSize)
                {
                    throw new ArgumentException(ErrorMessage);
                }
            }
        }

        private static string ErrorMessage
        {
            get
            {
                return "metadata has a max Size of 1 Kb";
            }
        }
    }

}