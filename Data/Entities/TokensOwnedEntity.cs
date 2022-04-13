using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Fractions;

namespace BscTokenSniper.Data.Entities
{
    public class TokensOwnedEntity : BaseEntity
    {
        [Required]
        public Guid? SessionId { get; set; }
        public Session Session { get; set; }
        public string Address { get; set; }
        public string AmountText { get; set; }
        public string BnbAmountText { get; set; }
        public string SinglePriceText { get; set; }
        public int TokenIdx { get; set; }
        public string PairAddress { get; set; }
        public int Decimals { get; set; }
        public bool HoneypotCheck { get; set; }
        public bool FailedSell { get; set; }
        [NotMapped]
        public BigInteger Amount 
        { 
            get => BigInteger.Parse(AmountText);
            set => AmountText = value.ToString();
        }
        [NotMapped]
        public BigInteger BnbAmount
        { 
            get => BigInteger.Parse(BnbAmountText);
            set => BnbAmountText = value.ToString();
        }
        [NotMapped]
        public Fraction SinglePrice 
        { 
            get => Fraction.FromString(SinglePriceText);
            set => SinglePriceText = value.ToString();
        }
    }
}