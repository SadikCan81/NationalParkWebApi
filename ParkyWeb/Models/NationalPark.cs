﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Models
{
    public class NationalPark
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string State { get; set; }

        public byte[] Image { get; set; }

        public DateTime EstablishedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
