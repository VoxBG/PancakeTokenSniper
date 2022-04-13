using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BscTokenSniper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BscTokenSniper.Data.Entities
{
    public class Configuration : BaseEntity
    {
        [Required]
        public Guid? SessionId { get; set; }

        public Session Session { get; set; }

        [Required]
        public string SniperConfigurationJson { get; set; }

        [NotMapped]
        public SniperConfiguration SniperConfiguration
        {
            get
            {
                return JsonConvert.DeserializeObject<SniperConfiguration>(string.IsNullOrWhiteSpace(SniperConfigurationJson) ? "{}" : SniperConfigurationJson);
            }
            set
            {
                SniperConfigurationJson = JsonConvert.SerializeObject(value);
            }
        }
    }
}