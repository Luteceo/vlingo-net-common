﻿// Copyright (c) 2012-2020 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Vlingo.Common.Serialization
{
    public static class JsonSerialization
    {
        private static readonly JsonConverter DateTimeConverter = new IsoDateTimeConverter();

        public static T Deserialized<T>(string serialization)
            => JsonConvert.DeserializeObject<T>(serialization, DateTimeConverter);
        
        public static object? Deserialized(string serialization, Type sourceType)
            => JsonConvert.DeserializeObject(serialization, sourceType, DateTimeConverter);

        public static List<T>? DeserializedList<T>(string serialization)
            => JsonConvert.DeserializeObject<List<T>>(serialization, DateTimeConverter);

        public static string Serialized(object instance)
            => JsonConvert.SerializeObject(instance, DateTimeConverter);
    }
}
