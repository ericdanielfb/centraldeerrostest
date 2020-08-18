﻿using System;
using System.Collections.Generic;

namespace CentralDeErros.Model.Models
{
    public class Environment
    {
        public Environment()
        {
            this.Errors = new HashSet<Error>();
        }

        public int Id { get; set; }
        public string Phase { get; set; }

        public ICollection<Error> Errors { get; set; }
    }
}
