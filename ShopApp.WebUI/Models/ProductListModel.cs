﻿using ShopApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WebUI.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPage()
        {
            return (int)Math.Ceiling((decimal)TotalItems/ItemPerPage);
        }
    }


    public class ProductListModel
    {
        public List<Product> Products { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
