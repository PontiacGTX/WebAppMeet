using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Models;

namespace WebAppMeet.Data.Entities
{
    public class SKeyValues: IEntity
    {
        public SKeyValues()
        {

        }
        public SKeyValues(SaveDbKeyModel saveDbKey)
        {
            this.GenerateBy = saveDbKey.GenerateBy;
            this.Key = saveDbKey.Key;
            this.GeneratedAt = saveDbKey.GeneratedAt;
            this.IsValid = saveDbKey.IsValid;
        }
        [Key]
        public virtual int SKeyValueId { get; set; }
        public string Key { get; set; }
        public DateTime GeneratedAt { get; set; }
        public bool IsValid { get; set; }
        public string GenerateBy { get; set; }
    }
}
