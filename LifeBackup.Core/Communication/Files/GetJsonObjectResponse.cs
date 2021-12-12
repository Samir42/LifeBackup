﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeBackup.Core.Communication.Files
{
    public class GetJsonObjectResponse
    {
        public int Id { get; set; }

        public DateTime DateSent { get; set; }

        public string Data { get; set; }
    }
}