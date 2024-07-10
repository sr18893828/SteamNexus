﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SteamNexus_Server.Models;

public partial class TrackedGame
{
    [Key]
    public int TrackedGameId { get; set; }

    public int UserId { get; set; }

    public int GamesId { get; set; }

    [MaxLength(500)]
    public string Title { get; set; }

    public virtual ICollection<Game> Game { get; set; }

    public virtual ICollection<User> Users { get; set; }


}