﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain
{
    public abstract class Entity : IEquatable<Entity>
    {
        //public Entity()
        //{
        //    Id = GenerateId();
        //}

        public Entity(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public static string GenerateId() => Guid.NewGuid().ToString();

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            if(obj is not Entity entity)
            {
                return false;
            }

            return entity.Id == Id;
        }

        public bool Equals(Entity? other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return other.Id == Id;
        }

        public override int GetHashCode() =>  Id.GetHashCode();
    }
}