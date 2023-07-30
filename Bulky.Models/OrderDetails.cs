﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class OrderDetails
    {
        public int id { get; set; }

        [Required]
        public int OrderHeaderId{ get; set; }
        [ForeignKey(nameof(OrderHeaderId))]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product Product {get; set; }
        public int count { get; set; }
        public double Price { get; set; }
    }
}
