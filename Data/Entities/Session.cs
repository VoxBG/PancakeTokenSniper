using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BscTokenSniper.Data.Entities
{
    public class Session : BaseEntity {
        public bool Active { get; set; }
        public List<Configuration> Configurations { get; set;}
        public List<TokensOwnedEntity> TokensOwnedEntities { get; set;}
    }
}
