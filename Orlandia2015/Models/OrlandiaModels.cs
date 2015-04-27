using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Orlandia2015.Models
{
    public class Player
    {
        [Key]
        public Guid uPlayerID { get; set; }

        public Guid uFactionID { get; set; }

        public string sName { get; set; }

        public int iRank { get; set; }

        public int iPoints { get; set; }

        public virtual ICollection<PlayerAchievements> Achievements { get; set; }
        public virtual Faction Faction { get; set; }
    }

    public class Faction
    {
        [Key]
        public Guid uFactionID { get; set; }

        public string sName { get; set; }

        public int iPoints { get; set; }
    }

    public class Achievement
    {
        [Key]
        public Guid uAchievementID { get; set; }

        public string sName { get; set; }

    }

    public class PlayerAchievements
    {
        [Key]
        public Guid uPlayerAchievementID { get; set; }

        [ForeignKey("Player")]
        public Guid uPlayerID { get; set; }

        [ForeignKey("Achievement")]
        public Guid uAchievementID { get; set; }

        public virtual Player Player { get; set; }
        public virtual Achievement Achievement { get; set; }

    }

    public class OrlandiaDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
    }

}