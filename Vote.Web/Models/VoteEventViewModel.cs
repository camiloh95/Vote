﻿namespace Vote.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Data.Entities;
    using Microsoft.AspNetCore.Http;

    public class VoteEventViewModel : VoteEvent
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}