﻿using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Models.DataModel
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int Votes { get; set; }

        public bool IsApproved { get; set; }


        public List<Love> Loved { get; set; }
     
    }
}
