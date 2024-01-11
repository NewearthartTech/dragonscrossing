using DragonsCrossing.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class TileSeed
    {
        /// <summary>
        /// The tiles attached to each zone. Currently the api only cares about combat tiles.
        /// All other tiles like the healer, adventuring guild, etc... the api doesn't use logic for so no need to be in the db.
        /// NOTE: We don't need to create any tiles for Aedos (zone 1) because the api doesn't do anything (have any logic) for Aedos tiles. 
        /// </summary>
        /// <param name="builder"></param>
        public TileSeed(ModelBuilder builder)
        {            
            builder.Entity<Tile>(b =>
            {
                b.HasData(
                    new Tile()
                    {
                        Id = 1,
                        Name = "Enchanted Fields",
                        Type = TileType.RegularCombat,
                        Order = 1,
                        ZoneId = 2,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 1,
                        Min = 1,
                        Max = 4,
                    });
            });
            builder.Entity<Tile>(b =>
            {
                b.HasData(
                    new Tile()
                    {
                        Id = 2,
                        Name = "Sylvan Woodlands",
                        Type = TileType.RegularCombat,
                        Order = 1,
                        ZoneId = 3,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 2,
                        Min = 3,
                        Max = 7,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 3,
                        Name = "Pilgrims' Clearing",
                        Type = TileType.DailyCombat,
                        Order = 2,
                        ZoneId = 3,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 3,
                        Min = 5,
                        Max = 8,
                    });
            });            
            builder.Entity<Tile>(b =>
            {
                b.HasData(
                    new Tile()
                    {
                        Id = 4,
                        Name = "Odorous Bog",
                        Type = TileType.RegularCombat,
                        Order = 1,
                        ZoneId = 4,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 4,
                        Min = 6,
                        Max = 11,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 5,
                        Name = "Ancient Battlefield",
                        Type = TileType.DailyCombat,
                        Order = 2,
                        ZoneId = 4,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 5,
                        Min = 7,
                        Max = 12,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 6,
                        Name = "Terrorswamp",
                        Type = TileType.Boss,
                        Order = 3,
                        ZoneId = 4,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 6,
                        Min = 13,
                        Max = 13,
                    });
            });            
            builder.Entity<Tile>(b =>
            {
                b.HasData(
                    new Tile()
                    {
                        Id = 7,
                        Name = "Mountain Fortress",
                        Type = TileType.RegularCombat,
                        Order = 1,
                        ZoneId = 5,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 7,
                        Min = 10,
                        Max = 15,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 8,
                        Name = "Griffon's Nest",
                        Type = TileType.DailyCombat,
                        Order = 2,
                        ZoneId = 5,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 8,
                        Min = 11,
                        Max = 16,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 9,
                        Name = "Summoner's Summit",
                        Type = TileType.Boss,
                        Order = 3,
                        ZoneId = 5,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 9,
                        Min = 16,
                        Max = 16,
                    });
            });            
            builder.Entity<Tile>(b =>
            {
                b.HasData(
                    new Tile()
                    {
                        Id = 10,
                        Name = "Labyrinthian Dungeon",
                        Type = TileType.RegularCombat,
                        Order = 1,
                        ZoneId = 6,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 10,
                        Min = 16,
                        Max = 20,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 11,
                        Name = "Slaver Row",
                        Type = TileType.DailyCombat,
                        Order = 2,
                        ZoneId = 6,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 11,
                        Min = 17,
                        Max = 20,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 12,
                        Name = "Wing of the Jailer",
                        Type = TileType.Boss,
                        Order = 3,
                        ZoneId = 6,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 12,
                        Min = 20,
                        Max = 20,
                    });
                b.HasData(
                    new Tile()
                    {
                        Id = 13,
                        Name = "Laboratory of the Archmagus",
                        Type = TileType.FinalBoss,
                        Order = 4,
                        ZoneId = 6,
                    });
                b.OwnsOne(e => e.HeroLevelRequired).HasData(
                    new
                    {
                        TileId = 13,
                        Min = 20,
                        Max = 20,
                    });
            });            
        }
    }
}
